using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal class PillPiece : Tile
    {
        public PillPiece? Connected { get; set; }
        public PillPiece(int color)
        {
            this.state = State.pill;
            this.color = (Color)color;
        }
    }
}
