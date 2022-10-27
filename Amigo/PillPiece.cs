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
        public PillPiece(int color)
        {
            this.state = State.pill;
            this.color = (Color)color;
            isConnected = true; 
        }

    }
}
