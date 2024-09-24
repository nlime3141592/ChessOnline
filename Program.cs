using System;
using System.Collections.Generic;
using nl.ChessOnline3;

namespace nl.ChessOnline3
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ChessManager manager = new ChessManager();
            GameState gameState = GameState.Ready;

            manager.Initialize();

            // TODO: Draw
            manager.Draw();

            while((gameState = manager.UpdateGameState()) == GameState.Running)
            {
                Console.WriteLine("현재 게임 상태 == {0}", gameState.ToString());

                // TODO: Input
                Console.Write("위치 입력 x=[0, 7] y=[0, 7]: ");
                string[] position = Console.ReadLine().Split();
                int px = int.Parse(position[0]);
                int py = int.Parse(position[1]);
                CellData cellData = manager.chessBoard[px, py];
                List<PieceActionList> nextActions = cellData.shownPiece.nextActions;
                for(int i = 0; i < nextActions.Count; ++i)
                {
                    Console.WriteLine(nextActions[i].ToString());
                }
                Console.WriteLine("이동할 인덱스 입력 i=[0, _]:");
                int index = int.Parse(Console.ReadLine());

                // TODO: Logic
                manager.Move(px, py, index);

                // TODO: Draw
                manager.Draw();
            }

            Console.WriteLine("최종 게임 상태 == {0}", gameState.ToString());
        }
    }
}