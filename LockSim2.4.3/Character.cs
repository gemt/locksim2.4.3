using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public class Character
    {

        // setbonuses
        public Character(Item[] items, Item ring1, Item ring2, Item trinket1, Item trinket2, Stat gloveEnchant) {
            Items = items;// new Item[12];
            //Array.Copy(items, Items, 12);
            Ring1 = ring1;
            Ring2 = ring2;
            Trinket1 = trinket1;
            Trinket2 = trinket2;

            GloveEnchant = gloveEnchant;

            Stats = new short[16];
            Stats[gloveEnchant.Type] += gloveEnchant.Value;
            SetupStatsSimd();
            AddConstants();
            Multiplications();
            DPS = (ShadowboltDamage / SBCastFrequency) * SbCastRatio;



#if false
            string names = "";
            foreach(var itm in Items) {
                names += itm.Name;
                foreach(var gem in itm.Sockets) {
                    names += gem.EquippedGem.Name;
                }
            }
            names += ring1.Name;
            names += ring2.Name;
            names += trinket1.Name;
            names += trinket2.Name;
            names += GloveEnchant.Type.ToString();
            using (System.Security.Cryptography.MD5 md5Algorithm = System.Security.Cryptography.MD5.Create()) {
                var hash = md5Algorithm.ComputeHash(Encoding.ASCII.GetBytes(names));
                Hash = BitConverter.ToString(hash).Replace("-", "");
            }
#endif
        }
        public string Hash { get; }
        void SetupStatsSimd() {
            AddStatsSimd(Items[Item.Head]);
            AddStatsSimd(Items[Item.Neck]);
            AddStatsSimd(Items[Item.Shoulder]);
            AddStatsSimd(Items[Item.Back]);
            AddStatsSimd(Items[Item.Chest]);
            AddStatsSimd(Items[Item.Wrist]);
            AddStatsSimd(Items[Item.Hand]);
            AddStatsSimd(Items[Item.Waist]);
            AddStatsSimd(Items[Item.Leg]);
            AddStatsSimd(Items[Item.Feet]);
            AddStatsSimd(Items[Item.Wand]);
            AddStatsSimd(Items[Item.Wep]);
            AddStatsSimd(Ring1);
            AddStatsSimd(Ring2);
            AddStatsSimd(Trinket1);
            AddStatsSimd(Trinket2);
        }
        void AddConstants() {
            Stats[Stat.Intel] += 40 + 19 + 126; // arcane int, imp gotw, orc race
            Stats[Stat.Sp] += (short)SPellstrikeBonus;
            Stats[Stat.ShadSP] += (short)T4_2Bonus;

            Stats[Stat.Hit] += 38; // Totem of wrath
            Stats[Stat.Crit] += 66; //  totem of wrath
            Stats[Stat.Spirit] += 20 + 19 + 50 + 134;// buffs
            Stats[Stat.ShadSP] += 80; // pure death
            Stats[Stat.Sp] += 42 + 130 + 101 + 23; // wizard oil, demonic aegis, wrath of air, foodbuff

        }

        void Multiplications() {
            var spirit = Stats[Stat.Spirit];
            Stats[Stat.Spirit] += (short)(spirit * 0.1); // kings
            Stats[Stat.Spirit] -= (short)(spirit * 0.05); // demonic embrace reduction
            Stats[Stat.Sp] += (short)(spirit * 0.1); // imp divine spirit

            Stats[Stat.Intel] += (short)(Stats[Stat.Intel] * 0.1); // kings


        }
        void AddStatsSimd(Item itm) {
            var a = new Vector<short>(Stats);
            var b = new Vector<short>(itm.Stats);
            (a + b).CopyTo(Stats);
        }
        
        public short[] Stats;
        public Item[] Items;

        public readonly Item Ring1;
        public readonly Item Ring2;
        public readonly Item Trinket1;
        public readonly Item Trinket2;

        public Stat GloveEnchant { get; set; }

        int ShadowDmg {
            get {
                return Stats[Stat.Sp] + Stats[Stat.ShadSP];
            }
        }

        double Haste { get {
                return Stats[Stat.Haste] / 15.77;
            } }

        double Hit {
            get {
                return Math.Min(16.0, Stats[Stat.Hit] / 12.6);//3=shaman totem
            }
        }
        double Crit {
            get {
                return 
                    + 1.701+ Stats[Stat.Intel] / 82.0
                    + Stats[Stat.Crit] / 22.08
                    + 3 + 5; //talents
            }
        }
        double CritBonus {
            get {
                if(Items[Item.Head].HasMetaGem)
                    return 1.09;
                return 1.0;
            }
        }

        const double Misery = 1.05;
        const double Sw = 1.1;
        const double Sm = 0.0;
        const double ImpSB = 5.0;
        const double IsbUptime = 0.75;
        const double Cos = 1.1;
        const double Ruin = 1.0;
        const double Sf = 5.0;
        const double SbCastRatio = 0.93;

        double T4_2Bonus {
            get {
                int t4count = 0;
                t4count += Items[Item.Head].Set == Item.ESet.T4 ? 1 : 0;
                t4count += Items[Item.Shoulder].Set == Item.ESet.T4 ? 1 : 0;
                t4count += Items[Item.Chest].Set == Item.ESet.T4 ? 1 : 0;
                t4count += Items[Item.Hand].Set == Item.ESet.T4 ? 1 : 0;
                t4count += Items[Item.Leg].Set == Item.ESet.T4 ? 1 : 0;
                if (t4count >= 2)
                    return (1.0 - Math.Pow((1.0 - 0.05), (10.0 * LandedSpellsPerSec))) * 135;
                return 0;
            }
        }
        double SPellstrikeBonus { get {
                if (Items[Item.Head].Set == Item.ESet.Spellstrike && Items[Item.Leg].Set == Item.ESet.Spellstrike)
                    return (1.0 - Math.Pow((1.0 - 0.05), (15.0 * LandedSpellsPerSec))) * 92;
                return 0;
            } }
        double LandedSpellsPerSec {
            get {
                return (1.0 - Math.Max(1.0, 17.0 - Hit) / 100.0) * (1.0/SBCastFrequency);
            }
        }
        double SBCastFrequency { get {
                return 2.55 * (1.0 - (Haste / 100.0));
            }
        }
        public double ShadowboltDamage{
            get {
                double v = (572.0 + ShadowDmg * (0.856 + Sf * 0.04));
                v*= (1.0 - Math.Max(1.0, 17.0 - Hit) / 100.0);
                v *= 1.0 + Crit / 100.0 * (0.5 + Ruin * 0.5) * CritBonus;
                v *= 1.15;
                v *= Sw * Misery * (1 + Sm / 50) * (1 + ImpSB * 0.04 * IsbUptime) * Cos;
                return v;
            }
        }

        public readonly double DPS;
        
        public override string ToString() {
            var ret = String.Format("DPS: {0,6:0.00}   SBdmg: {1,6:0.00}   SP: {2,6:0.00}   Hit: {3,6:0.00}%   Crit: {4,4:0.00}   Haste: {5,4:0.00}% GloveEnchant: {6}{7}\n", 
                DPS, 
                ShadowboltDamage,
                ShadowDmg,
                Hit,
                Crit,
                Haste,
                GloveEnchant.Value,
                ((Stat.eStat)GloveEnchant.Type).ToString());

            for(int i = 0; i < Item.Wep+1; i++) {
                var slot = i;
                var itm = Items.First(x => x.Slot == slot);
                ret += String.Format("\t{0,9}: {1,-35}", ((Item.SlotEnum)slot).ToString(), itm.Name);
                for(int g = 0; g < itm.Sockets.Length; g++) {
                    var sock = itm.Sockets[g];
                    sock.Color.ToString();
                    var gemStatStr = "";
                    foreach (var gs in sock.EquippedGem.Stats) {
                        if (gemStatStr.Length > 0)
                            gemStatStr += ", ";
                    }
                    ret += String.Format("{0,-6}: {1,-22}",
                        sock.Color.ToString(),
                        sock.EquippedGem.Name);
                }
                if (itm.HasSocketBonus) {
                    Stat.eStat es = (Stat.eStat)itm.SocketBonus.Value.Type;
                    ret += $"Bonus: {itm.SocketBonus.Value.Value}{es.ToString()}";

                }
                ret += "\n";

            }
            ret += String.Format("\t{0,9}: {1,-35}\n", "Ring1", Ring1.Name);
            ret += String.Format("\t{0,9}: {1,-35}\n", "Ring2", Ring2.Name);
            ret += String.Format("\t{0,9}: {1,-35}\n", "Trinket1", Trinket1.Name);
            ret += String.Format("\t{0,9}: {1,-35}\n", "Trinket2", Trinket2.Name);
            return ret;
            //return $"DPS: {DPS}, SBdmg: {ShadowboltDamage}, SP: {ShadowDmg}, Hit: {Hit}";
        }
    }
}
