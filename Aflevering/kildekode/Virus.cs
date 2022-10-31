using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal class Virus : Tile
    {
        public Virus(int color)
        {
            this.state = State.virus;
            this.color = (Color)color;
        }
    }
}
