using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public class Character
    {

        // setbonuses
        public Character(Item[] items, Item ring1, Item ring2, Item trinket1, Item trinket2, Stat gloveEnchant) {
            Items = new Item[12];
            Array.Copy(items, Items, 12);
            Ring1 = ring1;
            Ring2 = ring2;
            Trinket1 = trinket1;
            Trinket2 = trinket2;

            GloveEnchant = gloveEnchant;

            Stats = new Stat[9];
            Stats[0] = new Stat(0, 0);
            Stats[1] = new Stat(1, 0);
            Stats[2] = new Stat(2, 0);
            Stats[3] = new Stat(3, 0);
            Stats[4] = new Stat(4, 0);
            Stats[5] = new Stat(5, 0);
            Stats[6] = new Stat(6, 0);
            Stats[7] = new Stat(7, 0);
            Stats[8] = new Stat(8, 0);
            
            Stats[gloveEnchant.Type].Value += gloveEnchant.Value;
            AddStats(Items[Item.Head]);
            AddStats(Items[Item.Neck]);
            AddStats(Items[Item.Shoulder]);
            AddStats(Items[Item.Back]);
            AddStats(Items[Item.Chest]);
            AddStats(Items[Item.Wrist]);
            AddStats(Items[Item.Hand]);
            AddStats(Items[Item.Waist]);
            AddStats(Items[Item.Leg]);
            AddStats(Items[Item.Feet]);
            AddStats(Items[Item.Wand]);
            AddStats(Items[Item.Wep]);
            AddStats(ring1);
            AddStats(ring2);
            AddStats(trinket1);
            AddStats(trinket2);
            Stats[Stat.ShadSP].Value += (int)T4_2Bonus;
            Stats[Stat.Sp].Value += (int)SPellstrikeBonus;
            Stats[Stat.Spirit].Value += 20 + 19 + 50; // buffs
            Stats[Stat.Spirit].Value += 134; // orc race
            Stats[Stat.Intel].Value += 40 + 19 + 126; // arcane int, imp gotw, orc race
            Stats[Stat.Crit].Value += 66; //  totem of wrath
            Stats[Stat.Hit].Value += 38; // Totem of wrath
            Stats[Stat.ShadSP].Value += 80; // pure death
            Stats[Stat.Sp].Value += 42 + 130 + 101 + 23; // wizard oil, demonic aegis, wrath of air, foodbuff
            // Kings
            Stats[Stat.Spirit].Value += (int)Math.Round(Stats[Stat.Spirit].Value * 0.1, MidpointRounding.ToEven);
            Stats[Stat.Intel].Value += (int)Math.Round(Stats[Stat.Intel].Value * 0.1, MidpointRounding.ToEven);

            Stats[Stat.Spirit].Value -= (int)Math.Round(Stats[Stat.Spirit].Value * 0.05, MidpointRounding.ToEven); // demonic embrace reduction

            Stats[Stat.Sp].Value += (int)Math.Round(Stats[Stat.Spirit].Value * 0.1, MidpointRounding.ToEven); // imp divine spirit
            dps = (ShadowboltDamage / SBCastFrequency) * SbCastRatio;



            /*
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
            */
        }
        public string Hash { get; }
        void AddStats(Item itm) {
            foreach(var stat in itm.Stats) {
                Stats[stat.Type].Value += stat.Value;
            }
        }
        //double charVal = 0;
        public Stat[] Stats { get; }
        public Item[] Items { get; }

        public Item Ring1 { get; set; }
        public Item Ring2 { get; set; }
        public Item Trinket1 { get; set; }
        public Item Trinket2 { get; set; }

        public Stat GloveEnchant { get; set; }

        int ShadowDmg {
            get {
                return Stats[Stat.Sp].Value + Stats[Stat.ShadSP].Value;
            }
        }

        double Haste { get {
                return Stats[Stat.Haste].Value / 15.77;
            } }

        double Hit {
            get {
                return Math.Min(16.0, Stats[Stat.Hit].Value / 12.6);//3=shaman totem
            }
        }
        double Crit {
            get {
                return 
                    + 1.701+ Intellect / 82.0
                    + Stats[Stat.Crit].Value / 22.08
                    + 3 + 5; //talents
            }
        }
        double CritBonus {
            get {
                if (Items[Item.Head].Sockets.Any(x => x.EquippedGem == Gem.MetaGem))
                    return 1.09;
                return 1.0;
            }
        }
        double Misery { get { return 1.05; } }
        double Sw { get { return 1.1; } }
        double Sm { get { return 0.0; } }
        double ImpSB { get { return 5.0; } }
        double IsbUptime { get { return 0.75; } }
        double Cos { get { return 1.1; } } // no malediction in raid
        double Ruin { get { return 1.0; } }
        double Sf { get { return 5.0; } }
        double T4_2Bonus {
            get {
                return Items.Count(x => x.Set == Item.ESet.T4) >= 2
                ? (1.0 - Math.Pow((1.0 - 0.05),(10.0 * LandedSpellsPerSec))) * 135
                : 0;
            }
        }
        double SPellstrikeBonus { get {
                return Items.Count(x => x.Set == Item.ESet.Spellstrike) >= 2
                ? (1.0 - Math.Pow((1.0 - 0.05), (15.0 * LandedSpellsPerSec))) * 92
                : 0;
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
        double SbCastRatio { get {
                return 0.93; // XXX: should take LT frequency into account, does not yet....
            } }
        /*
        double LtMana { get {
                return Mana * 0.2;
            } }
        int Mana { get {
                return 1; // XXX: todo
            } }
        */
        int Intellect { get { return Stats[Stat.Intel].Value; } }
        public double ShadowboltDamage{
            get {
                double v = (572.0 + ShadowDmg * (0.856 + Sf * 0.04));
                v*= (1.0 - Math.Max(1.0, 17.0 - Hit) / 100.0);
                v *= (1.0 + Crit / 100.0 * (0.5 + Ruin * 0.5) * CritBonus);
                v *= 1.15;
                v *= Sw * Misery * (1 + Sm / 50) * (1 + ImpSB * 0.04 * IsbUptime) * Cos;
                return v;
            }
        }

        readonly double dps;
        public double DPS {
            get {
                return dps;
            } }

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
                    ret += $"Bonus: {itm.SocketBonus.Value}{((Stat.eStat)itm.SocketBonus.Type).ToString()}";

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
