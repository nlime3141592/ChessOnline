using UnityEngine;
using nl.ChessOnline;

namespace nl.ChessOnline.Unity
{
    public class GameManager : Singleton<GameManager>
    {
        public ChessManager chessManager { get; private set; }
        public GameState gameState;

        private void Awake()
        {
            chessManager = new ChessManager();
            this.InitGame();
            this.StartGame();
        }

        public void InitGame()
        {
            chessManager.Initialize();
            gameState = GameState.Ready;
        }

        public void StartGame()
        {
            gameState = chessManager.UpdateGameState();
        }

        public void UpdateGame()
        {
            gameState = chessManager.UpdateGameState();
        }
    }
}