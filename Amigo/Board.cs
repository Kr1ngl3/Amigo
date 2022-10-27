using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
                Vector pos = new(random.Next(x), random.Next(y - 3) + 3);
                if (this.ContainsKey(pos))
                {
                    i--;
                    continue;
                }
                Virus virus = new(random.Next(Enum.GetNames(typeof(Color)).Length));
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

        public void Rotate(Pill pill, bool direction)
        {
            Vector orientation = this.GetPos(pill.onePiece) - this.GetPos(pill.twoPiece);

            int temp = (int)(orientation.X * 10 + orientation.Y);

            switch (temp)
            {
                case -10: // onePice left, twoPiece right
                    if (direction) // puts twoPiece down, onePiece up
                    {
                        Vector pos = this.GetPos(pill.onePiece);
                        Vector pos1 = pos + new Vector(0, -1);
                        this.Remove(this.GetPos(pill.twoPiece));
                        this.Remove(this.GetPos(pill.onePiece));
                        this.Add(pos, pill.twoPiece);
                        this.Add(pos1, pill.onePiece);
                        pill.onePiece.rotation = Rotation.Rotate90;
                        pill.twoPiece.rotation = Rotation.Rotate90;
                        break;
                    }
                    else // puts twoPiece up, onePiece down
                    {
                        Vector pos = this.GetPos(pill.twoPiece);
                        pos += new Vector(-1, -1);
                        this.Remove(this.GetPos(pill.twoPiece));
                        this.Add(pos, pill.twoPiece);
                        pill.onePiece.rotation = Rotation.Rotate270;
                        pill.twoPiece.rotation = Rotation.Rotate270;
                        break;
                    }
                case 10: // onePiece right, twoPiece left
                    if (direction) // puts onePiece down, twoPiece up
                    {
                        Vector pos = this.GetPos(pill.twoPiece);
                        Vector pos1 = pos + new Vector(0, -1);
                        this.Remove(this.GetPos(pill.onePiece));
                        this.Remove(this.GetPos(pill.twoPiece));
                        this.Add(pos, pill.onePiece);
                        this.Add(pos1, pill.twoPiece);
                        pill.twoPiece.rotation = Rotation.Rotate270;
                        pill.onePiece.rotation = Rotation.Rotate270;
                        break;
                    }
                    else // puts onePiece up, twoPiece down
                    {
                        Vector pos = this.GetPos(pill.onePiece);
                        pos += new Vector(-1, -1);
                        this.Remove(this.GetPos(pill.onePiece));
                        this.Add(pos, pill.onePiece);
                        pill.twoPiece.rotation = Rotation.Rotate90;
                        pill.onePiece.rotation = Rotation.Rotate90;
                        break;
                    }
                case -1: // onepiece up, twoPiece down
                    if (direction)
                    {
                        Vector pos = this.GetPos(pill.onePiece);
                        Vector pos1 = pos + new Vector(1, 1);
                        this.Remove(this.GetPos(pill.onePiece));
                        this.Add(pos1, pill.onePiece);
                        pill.onePiece.rotation = Rotation.Rotate180;
                        pill.twoPiece.rotation = Rotation.Rotate0;
                        break;
                    }
                    else
                    {
                        Vector pos = this.GetPos(pill.twoPiece);
                        Vector pos1 = pos + new Vector(1, 0);
                        this.Remove(this.GetPos(pill.twoPiece));
                        this.Remove(this.GetPos(pill.onePiece));
                        this.Add(pos, pill.onePiece);
                        this.Add(pos1, pill.twoPiece);
                        pill.onePiece.rotation = Rotation.Rotate0;
                        pill.twoPiece.rotation = Rotation.Rotate180;
                        break;
                    }
                case 1: // onepiece down, twoPiece up
                    if (direction)
                    {
                        Vector pos = this.GetPos(pill.twoPiece);
                        Vector pos1 = pos + new Vector(1, 1);
                        this.Remove(this.GetPos(pill.twoPiece));
                        this.Add(pos1, pill.twoPiece);
                        pill.twoPiece.rotation = Rotation.Rotate180;
                        pill.onePiece.rotation = Rotation.Rotate0;
                        break;
                    }
                    else
                    {
                        Vector pos = this.GetPos(pill.onePiece);
                        Vector pos1 = pos + new Vector(1, 0);
                        this.Remove(this.GetPos(pill.onePiece));
                        this.Remove(this.GetPos(pill.twoPiece));
                        this.Add(pos, pill.twoPiece);
                        this.Add(pos1, pill.onePiece);
                        pill.twoPiece.rotation = Rotation.Rotate0;
                        pill.onePiece.rotation = Rotation.Rotate180;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
