namespace nl.ChessOnline3
{
    public class PieceCatchAction : PieceAction
    {
        private IntVector2 m_position;

        private Piece m_catchedPiece;

        public PieceCatchAction(ChessManager _cManager, IntVector2 _position)
        : base(_cManager)
        {
            m_position = _position;
        }

        public override void Next()
        {
            m_catchedPiece = base.chessBoard[m_position.x, m_position.y].hiddenPiece;
            m_catchedPiece.isCatched = true;

            base.chessBoard[m_position.x, m_position.y].hiddenPiece = null;

            if(!base.isTestAction)
            {
                base.chessBoard[m_position.x, m_position.y].shownPiece = base.chessBoard[m_position.x, m_position.y].hiddenPiece;
            }
        }

        public override void Undo()
        {
            m_catchedPiece.isCatched = false;

            base.chessBoard[m_position.x, m_position.y].hiddenPiece = m_catchedPiece;

            m_catchedPiece = null;

            if(!base.isTestAction)
            {
                base.chessBoard[m_position.x, m_position.y].shownPiece = base.chessBoard[m_position.x, m_position.y].hiddenPiece;
            }
        }
    }
}