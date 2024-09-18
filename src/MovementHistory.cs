namespace nl.ChessOnline
{
    public class MovementHistory
    {
        public Color teamColor;
        public PieceType pieceType;
        public MovementType movementType;

        public int turnNumber;

        public Cell from;
        public Cell to;
    }
}