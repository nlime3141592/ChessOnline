namespace nl.ChessOnline
{
    public class Movement
    {
        public int idxMovingPiece;
        public Cell nextPosition;
        public MovementType movementType;

        public Movement(int _idxMovingPiece, int _positionX, int _positionY, MovementType _movementType)
        {
            this.idxMovingPiece = _idxMovingPiece;
            this.nextPosition.x = _positionX;
            this.nextPosition.y = _positionY;
            this.movementType = _movementType;
        }
/*
        public void Move(ChessManager _manager, int _idxOriginPiece)
        {
            Cell pOrigin = _manager.pieces[_idxOriginPiece].position;
            Cell pTarget = nextPosition;

            int idxTargetPiece = _manager.gameBoard[pTarget.x, pTarget.y];

            if((movementType | MovementType.Catch) != 0)
            {
                _manager.Catch(pTarget);
                _manager.Swap(pOrigin, pTarget);
            }
            if((movementType | MovementType.EnPassant) != 0)
            {
                _manager.Catch(new Cell(pTarget.x, pOrigin.y));
                _manager.Swap(pOrigin, nextPosition);
            }

            if((movementType | MovementType.Promotion) != 0)
            {

            }

            if((movementType | MovementType.CastlingK) != 0)
            {
                // 4 and 7
                // 28 and 31
            }
            if((movementType | MovementType.CastlingQ) != 0)
            {
                // 4 and 0
                // 28 and 24
            }
        }
*/
    }
}