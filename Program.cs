using System;

namespace nl.ChessOnline
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("hello world");

            ChessManager manager = new ChessManager();
            manager.Initialize();
/*
            manager.gameBoard = new Piece[8, 8];
            manager.gameBoard[4, 4] = manager.pieces[8];
            manager.gameBoard[3, 5] = manager.pieces[16];
            manager.gameBoard[5, 5] = manager.pieces[17];
            manager.Draw(4, 4);
*/
            manager.DrawCheckMapBlack();

            for(int y = 7; y >= 0; --y)
            {
                for(int x = 0; x < 8; ++x)
                {
                    Console.Write("{0}", manager.checkMap[x, y] ? "O" : "*");
                }

                Console.WriteLine();
            }
        }
    }
}