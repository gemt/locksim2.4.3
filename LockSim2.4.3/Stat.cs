using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public class Stat
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
        
        public static int Intel = 0;
        public static int Stam = 1;
        public static int Sp = 2;
        public static int ShadSP = 3;
        public static int Hit = 4;
        public static int Crit = 5;
        public static int Haste = 6;
        public static int Spirit = 7;
        public static int Mp5 = 8;
        public static int MAX_STAT = 9;
        public Stat(int type, int value) {
            Type = type;
            Value = value;
        }
        public int Type { get; }
        public int Value { get; set; }
    }
}
