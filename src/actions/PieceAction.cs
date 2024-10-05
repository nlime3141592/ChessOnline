namespace nl.ChessOnline3
{
    /// <summary>
    /// Piece가 수행할 수 있는 마이크로 액션을 대표하는 클래스
    /// </summary>
    public abstract class PieceAction
    {
        protected CellData[,] chessBoard { get; private set; }

        public bool isTestAction;

        public PieceAction(ChessManager _cManager)
        {
            this.chessBoard = _cManager.chessBoard;
        }

        public abstract void Next();
        public abstract void Undo();
    }
}