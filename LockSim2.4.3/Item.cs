using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public class Item {
        public enum SlotEnum {
            Head = 0,
            Neck,
            Shoulder,
            Back,
            Chest,
            Wrist,
            Hand,
            Waist,
            Leg,
            Feet,
            Wand,
            Wep,

            Finger,
            Trinket,
            MAX_ESLOT = 14
        };
        public static int Head = 0;
        public static int Neck = 1;
        public static int Shoulder = 2;
        public static int Back = 3;
        public static int Chest = 4;
        public static int Wrist = 5;
        public static int Hand = 6;
        public static int Waist = 7;
        public static int Leg = 8;
        public static int Feet = 9;
        public static int Wand = 10;
        public static int Wep = 11;
        public static int Finger = 12;
        public static int Trinket = 13;
        public static int MAX_ESLOT = 14;
        public enum ESet {
            T4,
            Spellstrike
        }

        public Item(int slot, string name, Stat[] stats, Socket[] sockets, Stat? socketBonus, ESet? set, int id) {
            Slot = slot;
            Name = name;
            Stats = new short[16];
            for(int i = 0; i < stats.Length; i++) {
                Stats[i] = stats[i].Value;
            }
            Sockets = sockets;
            SocketBonus = socketBonus;
            Set = set;
            Id = id;
            // Hardcoded "enchants"
            if (slot == Head) {
                Stats[Stat.Sp] += 22;
                Stats[Stat.Hit] += 14;
            }
            if(slot == Shoulder) {
                Stats[Stat.Crit] += 15;
                Stats[Stat.Sp] += 12;
            }
            if(slot == Leg) {
                Stats[Stat.Sp] += 35;
                Stats[Stat.Stam] += 20;
            }
            if(slot == Chest) {
                Stats[Stat.Intel] += 6;
                Stats[Stat.Spirit] += 6;
                Stats[Stat.Stam] += 6;
            }
            if (slot == Wrist) {
                Stats[Stat.Sp] += 15;
            }
            if(slot == Finger) {
                Stats[Stat.Sp] += 12;
            }
            if(slot == Wep) {
                Stats[Stat.ShadSP] += 54;
            }

            // applying gem stats to stats
            if (Sockets != null && Sockets.Length != 0) {
                bool anyMissingColor = false;
                foreach (var sock in Sockets) {
                    foreach (var st in sock.EquippedGem.Stats) {
                        Stats[st.Type] += st.Value;
                    }
                    if (!sock.EquippedGem.Colors.Contains(sock.Color)) {
                        anyMissingColor = true;
                    }
                }
                if (!anyMissingColor) {
                    Stats[SocketBonus.Value.Type] += SocketBonus.Value.Value;
                    HasSocketBonus = true;
                }
            }
            HasMetaGem = Sockets.Any(x => x.EquippedGem == Gem.MetaGem);
        }

        readonly public int Slot;
        readonly public string Name;

        public short[] Stats;
        public Socket[] Sockets;
        public Stat? SocketBonus;
        readonly public bool HasSocketBonus = false;
        readonly public bool HasMetaGem = false;
        public ESet? Set { get; }

        public int Id { get; }

        public override string ToString() {
            var ret = $"{Slot.ToString()} {Name}\n";
            ret += "\t";

            for(int i = 0; i < Stat.MAX_STAT; i++) {
                ret += $"{((Stat.eStat)i).ToString()}: {Stats[i]}, ";
            }

            foreach(var s in Sockets) {
                ret += "\n\t";
                ret += $"{s.Color.ToString()}: {s.EquippedGem.Name}";
            }
            if(SocketBonus != null) {
                if (HasSocketBonus) {
                    Stat.eStat es = (Stat.eStat)SocketBonus.Value.Type;
                    ret += $"\n\tBonus: {es.ToString()} {SocketBonus.Value.Value}";
                }
                else
                    ret += $"\n\tBonus: 0";
            }
            if (Set.HasValue)
                ret += $"\n\tSet: {Set.Value.ToString()}";
            return ret;
        }
    }


}
