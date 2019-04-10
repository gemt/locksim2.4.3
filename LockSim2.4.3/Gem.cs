using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public class Gem {
        public static List<Gem> Gems = new List<Gem> {
            new Gem{Name = "Glowing Nightseye",  Colors = new eColor[]{ eColor.Blue, eColor.Red }, Stats = new Stat[]{new Stat(Stat.Stam,6), new Stat(Stat.Sp,5) },Id=1 },
            //new Gem{Name = "Great Dawnstone", Colors = new eColor[]{eColor.Yellow }, Stats= new Stat[]{new Stat(Stat.Hit, 8)}, Id=2 }},
            new Gem{Name = "Potent Noble Topaz", Colors = new eColor[]{eColor.Red, eColor.Yellow }, Stats= new Stat[]{new Stat(Stat.Sp, 5), new Stat(Stat.Crit, 4) }, Id=3 },
            new Gem{Name = "Runed Living Ruby", Colors = new eColor[]{eColor.Red }, Stats = new Stat[]{new Stat(Stat.Sp, 9) },Id=4}
        };

        public static Gem MetaGem = new Gem { Name = "Chaotic Skyfire Diamond", Colors = new eColor[] { eColor.Meta }, Stats = new Stat[] { new Stat(Stat.Crit, 12) }, Id=5 };

        public string Name { get; set; }
        public Stat[] Stats { get; set; }
        public eColor[] Colors { get; set; }
        public int Id { get; set; }
    }
}
