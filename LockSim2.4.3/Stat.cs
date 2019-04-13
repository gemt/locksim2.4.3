using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public struct Stat
    {
        
        public enum eStat
        {
            Intel,
            Stam,
            Sp,
            ShadSP,
            Hit,
            Crit,
            Haste,
            Spirit,
            Mp5,
            MAX_STAT
        }
        
        public static byte Intel = 0;
        public static byte Stam = 1;
        public static byte Sp = 2;
        public static byte ShadSP = 3;
        public static byte Hit = 4;
        public static byte Crit = 5;
        public static byte Haste = 6;
        public static byte Spirit = 7;
        public static byte Mp5 = 8;
        public static byte MAX_STAT = 9;
        public Stat(byte type, short value) {
            Type = type;
            Value = value;
        }
        public byte Type { get; }
        public short Value { get; set; }
    }
}
