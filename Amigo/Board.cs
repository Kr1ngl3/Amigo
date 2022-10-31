using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Amigo
{

    internal class Board : Dictionary<Vector, Tile>
    {
        int x, y;
        int difficulty = 1;
        MainWindow mw;
        public Board(int gameNumber, int x, int y, MainWindow mw)
        {
            this.mw = mw;
            this.y = y;
            this.x = x;
            
            int virusCount = gameNumber * 4;

            Random random = new Random();

            for (int i = 0; i < virusCount; i++)
            {
                Vector pos = new(random.Next(x), random.Next(y - 3) + 3);
                if (ContainsKey(pos))
                {
                    i--;
                    continue;
                }
                Virus virus = new(random.Next(Enum.GetNames(typeof(Color)).Length));
                Add(pos, virus);
            }
        }

        public Vector GetPos(Tile t)
        {
            return this.FirstOrDefault(x => x.Value == t).Key;
        }


        // not implemented
        public bool Move(Tile t, Vector dir)
        {
            Vector pos = GetPos(t) + dir;
            if (ContainsKey(pos))
                return false;
            if (pos.X >= x || pos.Y >= y || pos.X < 0 || pos.Y < 0)
                return false;
            Remove(GetPos(t));
            Add(pos, t);
            return true;
        }
        public bool PillFall(Pill pill)
        {
            Vector dir = new(0, 1);
            Vector orientation = GetPos(pill.onePiece) - GetPos(pill.twoPiece);
            int temp = (int)(orientation.X * 10 + orientation.Y);

            if (Math.Abs(temp) == 10) // when pill is flat
            {
                Vector pos1 = GetPos(pill.onePiece);
                Vector pos2 = GetPos(pill.twoPiece);
                if (!Move(pill.onePiece, dir) || !Move(pill.twoPiece, dir))
                {
                    Remove(GetPos(pill.onePiece));
                    Remove(GetPos(pill.twoPiece));
                    Add(pos1, pill.onePiece);
                    Add(pos2, pill.twoPiece);
                    return false;
                }

            }
            else if (temp < 0) // vertical twoPiece down
            {
                if (!Move(pill.twoPiece, dir))
                    return false;
                Move(pill.onePiece, dir);
            }
            else // vertical onePiece down
            {
                if (!Move(pill.onePiece, dir))
                    return false;
                Move(pill.twoPiece, dir);
            }
            return true;
        }

        public bool PillMove(Pill pill, Vector dir)
        {
            Vector orientation = GetPos(pill.onePiece) - GetPos(pill.twoPiece);
            int temp = (int)(orientation.X * 10 + orientation.Y);

            if (Math.Abs(temp) == 1) // when pill is vertical
            {
                Vector pos1 = GetPos(pill.onePiece);
                Vector pos2 = GetPos(pill.twoPiece);
                if (!Move(pill.onePiece, dir) || !Move(pill.twoPiece, dir))
                {
                    Remove(GetPos(pill.onePiece));
                    Remove(GetPos(pill.twoPiece));
                    Add(pos1, pill.onePiece);
                    Add(pos2, pill.twoPiece);
                    return false;
                }

            }
            else if (temp < 0 && dir.X == -1) // horizontal onePiece left and move left
            {
                if (!Move(pill.onePiece, dir))
                    return false;
                Move(pill.twoPiece, dir);
            }
            else if (temp > 0 && dir.X == 1) // horizontal onePiece right and move right
            {
                if (!Move(pill.onePiece, dir))
                    return false;
                Move(pill.twoPiece, dir);
            }
            else if (temp < 0 && dir.X == 1) // horizontal twoPiece right and move right
            {
                if (!Move(pill.twoPiece, dir))
                    return false;
                Move(pill.onePiece, dir);
            }
            else if (temp > 0 && dir.X == -1) // horizontal twoPiece left and move left
            {
                if (!Move(pill.twoPiece, dir))
                    return false;
                Move(pill.onePiece, dir);
            }
            return true;
        }

        bool allDone = false;
        public void Gravity(Object source, System.Timers.ElapsedEventArgs e)
        {
            bool test = false;
            List<PillPiece> list = new();
            foreach (Tile t in Values)
            {
                if (t.state == State.pill)
                    list.Add((PillPiece)t);
            }
            foreach (PillPiece p in list)
            {
                if (p.pill == null)
                {
                    if (Move(p, new Vector(0, 1)))
                        test = true;
                }
                else
                {
                    if (PillFall(p.pill))
                        test = true;
                }
            }
            mw.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal,
            new Action(() =>
            {
                mw.Update();
            }));
            allDone = !test;
            if (allDone)
            {
                mw.fallLoopTimer.Start();
                gravityLoopTimer.Enabled = false;
                TestForConnections();
            }
        }


        public void TestForConnections()
        {
            bool tileRemoved = false;
            int destroyedVirus = 0;
            List<Vector> allConnectedTiles = new List<Vector>();
            foreach (Vector vec in Keys)
            {

                Tile keyTile = this[vec];

                int searchLength = 0;
                
                while (ContainsKey(new Vector(vec.X + searchLength, vec.Y)))
                {
                    Tile tempTile = this[new Vector(vec.X + searchLength, vec.Y)];
                    if(tempTile.color != keyTile.color) break;
                    searchLength++;
                }
                if (searchLength >= 4)
                {
                    for (int i = 0; i < searchLength; i++)
                    {
                        allConnectedTiles.Add(new Vector(vec.X + i, vec.Y));
                    }
                }
                searchLength = 0;
                while (ContainsKey(new Vector(vec.X, vec.Y + searchLength)))
                {
                    Tile tempTile = this[new Vector(vec.X, vec.Y + searchLength)];
                    if (tempTile.color != keyTile.color) break;
                    searchLength++;
                }
                if (searchLength >= 4)
                {
                    for (int i = 0; i < searchLength; i++)
                    {
                        allConnectedTiles.Add(new Vector(vec.X, vec.Y + i));
                    }
                }

            }
            foreach (Vector vec in allConnectedTiles)
            {
                if (ContainsKey(vec))
                {
                    Tile tempTile = this[vec];
                    if (tempTile.state == State.pill)
                    {
                        PillPiece tempPillPiece = (PillPiece)tempTile;
                        if (tempPillPiece.pill != null)
                        {
                            if (tempPillPiece.isTwoPiece)
                                tempPillPiece.pill.onePiece.pill = null;
                            else
                                tempPillPiece.pill.twoPiece.pill = null;
                        }
                    }
                    if (tempTile.state == State.virus) 
                        destroyedVirus++;
                    tileRemoved = Remove(vec);
                    
                }
            }

            if (tileRemoved)
            {
                SoundPlayer player = new SoundPlayer(Directory.GetCurrentDirectory() + @"\Boing.wav");
                player.Load();
                player.Play();
                double pointsToAdd = difficulty * 0.5 * Math.Pow(2, destroyedVirus) * 100;
                mw.points += pointsToAdd;
                StartGravityLoop();
            }
        }

        System.Timers.Timer gravityLoopTimer;
        int fallTime = 5; // per second
        public void StartGravityLoop()
        {
            //make timer
            mw.fallLoopTimer.Stop();

            gravityLoopTimer = new System.Timers.Timer(1000 / fallTime);
            gravityLoopTimer.Enabled = true;
            gravityLoopTimer.Elapsed += Gravity;
        }
        public void Rotate(Pill pill, bool direction)
        {
            Vector orientation = GetPos(pill.onePiece) - GetPos(pill.twoPiece);

            int temp = (int)(orientation.X * 10 + orientation.Y);

            switch (temp)
            {
                case -10: // onePice left, twoPiece right
                    if (direction) // puts twoPiece down, onePiece up
                    {
                        if (!Move(pill.onePiece, new Vector(0, -1)))
                            break;
                        Move(pill.twoPiece, new Vector(-1, 0));
                        pill.onePiece.rotation = Rotation.Rotate90;
                        pill.twoPiece.rotation = Rotation.Rotate90;
                        break;
                    }
                    else // puts twoPiece up, onePiece down
                    {
                        if (!Move(pill.twoPiece, new Vector(-1, -1)))
                            break;
                        pill.onePiece.rotation = Rotation.Rotate270;
                        pill.twoPiece.rotation = Rotation.Rotate270;
                        break;
                    }
                case 10: // onePiece right, twoPiece left
                    if (direction) // puts onePiece down, twoPiece up
                    {
                        if (!Move(pill.twoPiece, new Vector(0, -1)))
                            break;
                        Move(pill.onePiece, new Vector(-1, 0));
                        pill.twoPiece.rotation = Rotation.Rotate270;
                        pill.onePiece.rotation = Rotation.Rotate270;
                        break;
                    }
                    else // puts onePiece up, twoPiece down
                    {
                        if (!Move(pill.onePiece, new Vector(-1, -1)))
                            break;
                        pill.twoPiece.rotation = Rotation.Rotate90;
                        pill.onePiece.rotation = Rotation.Rotate90;
                        break;
                    }
                case -1: // onepiece up, twoPiece down
                    if (direction)
                    {
                        if (!Move(pill.onePiece, new Vector(1, 1)))
                        {
                            if (!Move(pill.twoPiece, new Vector(-1, 0)))
                                break;
                            Move(pill.onePiece, new Vector(0, 1));
                            pill.onePiece.rotation = Rotation.Rotate180;
                            pill.twoPiece.rotation = Rotation.Rotate0;
                            break;
                        }
                        pill.onePiece.rotation = Rotation.Rotate180;
                        pill.twoPiece.rotation = Rotation.Rotate0;
                        break;
                    }
                    else
                    {
                        if (!Move(pill.twoPiece, new Vector(1, 0)))
                        {
                            if (!Move(pill.onePiece, new Vector(-1, 1)))
                                break;
                            pill.onePiece.rotation = Rotation.Rotate0;
                            pill.twoPiece.rotation = Rotation.Rotate180;
                            break;
                        }
                        Move(pill.onePiece, new Vector(0, 1));
                        pill.onePiece.rotation = Rotation.Rotate0;
                        pill.twoPiece.rotation = Rotation.Rotate180;
                        break;
                    }
                case 1: // onepiece down, twoPiece up
                    if (direction)
                    {
                        if (!Move(pill.twoPiece, new Vector(1, 1)))
                        {
                            if (!Move(pill.onePiece, new Vector(-1, 0)))
                                break;
                            Move(pill.twoPiece, new Vector(0, 1));
                            pill.twoPiece.rotation = Rotation.Rotate180;
                            pill.onePiece.rotation = Rotation.Rotate0;
                            break;
                        }
                        pill.twoPiece.rotation = Rotation.Rotate180;
                        pill.onePiece.rotation = Rotation.Rotate0;
                        break;
                    }
                    else
                    {
                        if (!Move(pill.onePiece, new Vector(1, 0)))
                        {
                            if (!Move(pill.twoPiece, new Vector(-1, 1)))
                                break;
                            pill.twoPiece.rotation = Rotation.Rotate0;
                            pill.onePiece.rotation = Rotation.Rotate180;
                            break;
                        }
                        Move(pill.twoPiece, new Vector(0, 1));
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
