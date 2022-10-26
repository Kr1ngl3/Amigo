using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal class Board
    {
        public Tile[,] board;

        public Board(int gameNumber, int x, int y)
        {
            int difficulty = gameNumber * 4;

            board = new Tile[x,y];
            Random random = new Random();

            for (int i = 0; i < difficulty; i++)
            {
                Vector2Int pos = new(random.NextInt64(8), random.NextInt64(10) + 3);

                Virus virus = new((int)random.NextInt64(3));
                virus.pos = pos;
                board[pos.x, pos.y] = virus;

            }


        }
    }
}
