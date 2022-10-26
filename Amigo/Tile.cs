using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal abstract class Tile
    {
        public Color color;
        public State state;
    }

    public enum Color
    { 
        red,
        yellow,
        blue
    }

    public enum State
    { 
        pill,
        virus
    }
}
