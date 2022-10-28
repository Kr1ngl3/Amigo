using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Amigo
{
    internal class PillPiece : Tile
    {
        public Rotation rotation;
        public Pill pill;
        public bool isTwoPiece;

        public PillPiece(int color, bool isTwoPiece, Pill pill)
        {
            this.state = State.pill;
            this.pill = pill;
            this.color = (Color)color;
            this.isTwoPiece = isTwoPiece;
        }
    }
}
