using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amigo
{
    internal class Board
    {
        Tile[,] board;

        Board(int gameNumber)
        {
            int difficulty = gameNumber * 4;

            board = new Tile[8,13];
            Random random = new Random();

            for (int i = 0; i < difficulty; i++)
            {
                Vector2Int pos = new(random.NextInt64(8), random.NextInt64(10) + 3);

                board[pos.x, pos.y] = new Virus((int)random.NextInt64(2));
            }


        }
    }
}
