using System.Collections.Generic;

namespace nl.ChessOnline3
{
    public class Piece
    {
        public PieceType pieceType;
        public Color color;
        public bool isCatched;
        public int movedCount;

        public List<PieceAction> nextActions;

        public Piece()
        {
            nextActions = new List<PieceAction>(28);
        }
    }
}