using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal class Board : Dictionary<Vector, Tile>
    {
        public Board(int gameNumber, int x, int y)
        {
            int difficulty = gameNumber * 4;

            Random random = new Random();

            for (int i = 0; i < difficulty; i++)
            {
                Vector pos = new(random.NextInt64(x), random.NextInt64(y - 3) + 3);
                if (this.ContainsKey(pos))
                {
                    i--;
                    continue;
                }
                Virus virus = new((int)random.NextInt64(Enum.GetNames(typeof(Color)).Length));
                this.Add(pos, virus);
            }
        }

        public Vector GetPos(Tile t)
        {
            return this.FirstOrDefault(x => x.Value == t).Key;
        }


        // not implemented
        public bool Move(Tile t, Vector dir)
        {
            Vector pos = this.GetPos(t);
            if (this.ContainsKey(pos + dir))
                return false;
            return true;
        }

        public void TestForConnections()
        {
            List<Vector> allConnectedTiles = new List<Vector>();
            foreach(Vector vec in this.Keys)
            {   
                
                int searchLength = 0;
                while (this.ContainsKey(new Vector(vec.X + searchLength, vec.Y)))
                {
                    searchLength++;
                }
                if (searchLength <= 4)
                {
                    for(int i = 0; i < searchLength; i++)
                    {
                        allConnectedTiles.Add(new Vector(vec.X + i, vec.Y));
                    }
                }
                searchLength = 0;
                while (this.ContainsKey(new Vector(vec.X, vec.Y + searchLength)))
                {
                    searchLength++;
                }
                if (searchLength <= 4)
                {
                    for (int i = 0; i < searchLength; i++)
                    {
                        allConnectedTiles.Add(new Vector(vec.X, vec.Y + i));
                    }
                }

            }
            foreach(Vector vec in allConnectedTiles)
            {
                if (this.ContainsKey(vec))
                {
                    this.Remove(vec);
                }
            }
        }
    }
}
