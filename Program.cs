using System;

using nl.ChessOnline3;

namespace nl.ChessOnline
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("hello world");

            nl.ChessOnline3.Piece p = null;

            ChessManager manager = new ChessManager();
            manager.Initialize();

            // manager.gameBoard = new Piece[8, 8];
            Piece whiteQ = manager.pieces[3];
            Piece blackK = manager.pieces[28];
            // manager.gameBoard[4, 4] = whiteQ;
            // manager.gameBoard[3, 5] = blackK;

            bool isChecked = manager.IsChecked(Color.White, Color.Black);

            for(int y = 7; y >= 0; --y)
            {
                for(int x = 0; x < 8; ++x)
                {
                    if(manager.gameBoard[x, y] == blackK)
                        Console.Write("K");
                    else
                        Console.Write("{0}", manager.checkMap[x, y] ? "O" : "*");
                }

                Console.WriteLine();
            }

            if(isChecked)
                Console.WriteLine("Checked");
            else
                Console.WriteLine("Not Checked");
        }
    }
}