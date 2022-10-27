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
        public bool isConnected;
        public bool isTwoPiece;
        public PillPiece(int color, bool isTwoPiece)
        {
            this.state = State.pill;
            this.color = (Color)color;
            isConnected = true;
            this.isTwoPiece = isTwoPiece;
        }

    }
}
