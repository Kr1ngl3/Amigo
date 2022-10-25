using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal abstract class Tile
    {
        protected Color color;
        protected State state;
        protected Vector2Int? pos;
    }

    public class Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(long x, long y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }
    }

    public enum Color
    { 
        red,
        yellow,
        blue
    }

    public enum State
    { 
        empty,
        pill,
        virus
    }
}
