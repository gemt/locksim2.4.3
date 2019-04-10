using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    public class Socket
    {
        public Socket(eColor color, Gem equippedGem) {
            Color = color;
            EquippedGem = equippedGem;
        }
        public eColor Color { get; }
        public Gem EquippedGem { get; set; }
    }
}
