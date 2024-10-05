namespace nl.ChessOnline3
{
    public class PieceMoveAction : PieceAction
    {
        private IntVector2 m_prevPosition;
        private IntVector2 m_nextPosition;

        public PieceMoveAction(ChessManager _cManager, IntVector2 _prevPosition, IntVector2 _nextPosition)
        : base(_cManager)
        {
            m_prevPosition = _prevPosition;
            m_nextPosition = _nextPosition;
        }

        public override void Next()
        {
            base.chessBoard[m_nextPosition.x, m_nextPosition.y].hiddenPiece = base.chessBoard[m_prevPosition.x, m_prevPosition.y].hiddenPiece;
            base.chessBoard[m_prevPosition.x, m_prevPosition.y].hiddenPiece = null;

            if(!base.isTestAction)
            {
                base.chessBoard[m_nextPosition.x, m_nextPosition.y].shownPiece = base.chessBoard[m_nextPosition.x, m_nextPosition.y].hiddenPiece;
                base.chessBoard[m_prevPosition.x, m_prevPosition.y].shownPiece = base.chessBoard[m_prevPosition.x, m_prevPosition.y].hiddenPiece;
            }
        }

        public override void Undo()
        {
            base.chessBoard[m_prevPosition.x, m_prevPosition.y].hiddenPiece = base.chessBoard[m_nextPosition.x, m_nextPosition.y].hiddenPiece;
            base.chessBoard[m_nextPosition.x, m_nextPosition.y].hiddenPiece = null;

            if(!base.isTestAction)
            {
                base.chessBoard[m_nextPosition.x, m_nextPosition.y].shownPiece = base.chessBoard[m_nextPosition.x, m_nextPosition.y].hiddenPiece;
                base.chessBoard[m_prevPosition.x, m_prevPosition.y].shownPiece = base.chessBoard[m_prevPosition.x, m_prevPosition.y].hiddenPiece;
            }
        }
    }
}