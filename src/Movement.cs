namespace nl.ChessOnline
{
    public class Movement
    {
        public Cell nextPosition;
        public MovementType movementType;

        public Movement(int _positionX, int _positionY, MovementType _movementType)
        {
            this.nextPosition.x = _positionX;
            this.nextPosition.y = _positionY;
            this.movementType = _movementType;
        }
    }
}