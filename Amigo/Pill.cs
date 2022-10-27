using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Amigo
{
    internal class Pill
    {
        public PillPiece onePiece;
        public PillPiece twoPiece;
        public Pill(int color1, int color2)
        {
            onePiece = new(color1, false);
            onePiece.rotation = Rotation.Rotate0;
            twoPiece = new(color2, true);
            twoPiece.rotation = Rotation.Rotate180;
        }
    }
}
