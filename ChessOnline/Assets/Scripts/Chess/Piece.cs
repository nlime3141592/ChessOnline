using System.Collections.Generic;

namespace nl.ChessOnline
{
    public class Piece
    {
        public PieceType pieceType;
        public Color color;
        public bool isCatched;
        public int movedCount;

        public List<PieceActionData> nextActions;

        public Piece()
        {
            nextActions = new List<PieceActionData>(28);
        }
    }
}