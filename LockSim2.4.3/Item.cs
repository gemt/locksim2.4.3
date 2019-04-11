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

        public Item(int slot, string name, Stat[] stats, Socket[] sockets, Stat socketBonus, ESet? set, int id) {
            Slot = slot;
            Name = name;
            Stats = stats;
            Sockets = sockets;
            SocketBonus = socketBonus;
            Set = set;
            Id = id;
            // Hardcoded "enchants"
            if (slot == Head) {
                AddStat(new Stat(Stat.Sp, 22));
                AddStat(new Stat(Stat.Hit, 14));
            }
            if(slot == Shoulder) {
                AddStat(new Stat(Stat.Crit, 15));
                AddStat(new Stat(Stat.Sp, 12));
            }
            if(slot == Leg) {
                AddStat(new Stat(Stat.Sp, 35));
                AddStat(new Stat(Stat.Stam, 20));
            }
            if(slot == Chest) {
                AddStat(new Stat(Stat.Intel, 6));
                AddStat(new Stat(Stat.Spirit, 6));
                AddStat(new Stat(Stat.Stam, 6));
            }
            if (slot == Wrist) {
                AddStat(new Stat(Stat.Sp, 15));
            }
            if(slot == Finger) {
                AddStat(new Stat(Stat.Sp, 12));
            }
            if(slot == Wep) {
                AddStat(new Stat(Stat.ShadSP, 54));
            }

            // applying gem stats to stats
            if (Sockets != null && Sockets.Length != 0) {
                bool anyMissingColor = false;
                foreach (var sock in Sockets) {
                    foreach (var Stats in sock.EquippedGem.Stats) {
                        var s = stats.First(x => x.Type == Stats.Type);
                        s.Value += Stats.Value;
                    }
                    if (!sock.EquippedGem.Colors.Contains(sock.Color)) {
                        anyMissingColor = true;
                    }
                }
                if (!anyMissingColor) {
                    var s = stats.First(x => x.Type == SocketBonus.Type);
                    s.Value += SocketBonus.Value;
                    HasSocketBonus = true;
                }
            }
        }

        void AddStat(Stat stat) {
            var existing = Stats.Single(x => x.Type == stat.Type);
            existing.Value += stat.Value;
        }

        public int Slot { get; }
        public string Name { get; }
        
        public Stat[] Stats { get; }
        public Socket[] Sockets { get; set; }
        public Stat SocketBonus { get; set; }
        public bool HasSocketBonus { get; } = false;
        public ESet? Set { get; }

        public int Id { get; }

        public override string ToString() {
            var ret = $"{Slot.ToString()} {Name}\n";
            ret += "\t";
            foreach(var stat in Stats) {
                ret += $"{stat.Type.ToString()}: {stat.Value}, ";
            }

            foreach(var s in Sockets) {
                ret += "\n\t";
                ret += $"{s.Color.ToString()}: {s.EquippedGem.Name}";
            }
            if(SocketBonus != null) {
                if(HasSocketBonus)
                    ret += $"\n\tBonus: {SocketBonus.Type.ToString()} {SocketBonus.Value}";
                else
                    ret += $"\n\tBonus: 0";
            }
            if (Set.HasValue)
                ret += $"\n\tSet: {Set.Value.ToString()}";
            return ret;
        }
    }


}
