namespace nl.ChessOnline
{
    public class ReachableCell
    {
        public int row;
        public int col;
        public MovementType movementType;

        public ReachableCell(int _row, int _col, MovementType _movementType = MovementType.None)
        {
            this.row = _row;
            this.col = _col;
            this.movementType = _movementType;
        }
    }
}