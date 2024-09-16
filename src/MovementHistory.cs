namespace nl.ChessOnline
{
    public class MovementHistory
    {
        public TeamType teamColor;
        public PieceType pieceType;
        public MovementType movementType;

        public int turnNumber;

        public Cell from;
        public Cell to;
    }
}