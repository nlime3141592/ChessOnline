using System.Collections.Generic;

namespace nl.ChessOnline
{
    public class Piece
    {
        public List<Movement> AvailableMovements { get; private set; }

        public Color color;
        public PieceType pieceType;
        public int movedCount;
        public bool isCatched;

        public Piece()
        {
            color = Color.None;
            pieceType = PieceType.None;
            movedCount = 0;
            AvailableMovements = new List<Movement>(28);
        }

        public void Initialize(Color _color, PieceType _pieceType)
        {
            color = _color;
            pieceType = _pieceType;
            movedCount = 0;
            isCatched = false;
            AvailableMovements.Clear();
        }

        public void RecognizeEnvironment(ChessManager _manager)
        {
            AvailableMovements.Clear();
/*
            switch(pieceType)
            {
                case PieceType.Pawn:
                    m_RecognizeAsPawn(_manager);
                    break;
                case PieceType.Bishop:
                    m_RecognizeAsLinear(_manager, 1, 1, 7);
                    m_RecognizeAsLinear(_manager, -1, 1, 7);
                    m_RecognizeAsLinear(_manager, 1, -1, 7);
                    m_RecognizeAsLinear(_manager, -1, -1, 7);
                    break;
                case PieceType.Knight:
                    m_RecognizeAsKnight(_manager, 1, 2);
                    m_RecognizeAsKnight(_manager, 2, 1);
                    m_RecognizeAsKnight(_manager, -1, 2);
                    m_RecognizeAsKnight(_manager, -2, 1);
                    m_RecognizeAsKnight(_manager, 1, -2);
                    m_RecognizeAsKnight(_manager, 2, -1);
                    m_RecognizeAsKnight(_manager, -1, -2);
                    m_RecognizeAsKnight(_manager, -2, -1);
                    break;
                case PieceType.Rook:
                    m_RecognizeAsLinear(_manager, 1, 0, 7);
                    m_RecognizeAsLinear(_manager, 0, 1, 7);
                    m_RecognizeAsLinear(_manager, -1, 0, 7);
                    m_RecognizeAsLinear(_manager, 0, -1, 7);
                    break;
                case PieceType.Queen:
                    m_RecognizeAsLinear(_manager, 1, 1, 7);
                    m_RecognizeAsLinear(_manager, -1, 1, 7);
                    m_RecognizeAsLinear(_manager, 1, -1, 7);
                    m_RecognizeAsLinear(_manager, -1, -1, 7);
                    m_RecognizeAsLinear(_manager, 1, 0, 7);
                    m_RecognizeAsLinear(_manager, 0, 1, 7);
                    m_RecognizeAsLinear(_manager, -1, 0, 7);
                    m_RecognizeAsLinear(_manager, 0, -1, 7);
                    break;
                case PieceType.King:
                    break;
                default:
                    break;
            }
*/
        }

        public void Move(ChessManager _manager, int _idxMovement)
        {

        }
/*
        private void m_RecognizeAsPawn(ChessManager _manager)
        {
            int axis = this.color == Color.White ? 1 : -1;
            MovementType promotableType = ((position.y + axis) % 7 == 0) ? MovementType.Promotion : MovementType.Normal;

            if(_manager.IsEmptyCell(position.x, position.y + axis))
                AvailableMovements.Add(new Movement(idxPiece, position.x, position.y + 1 * axis, promotableType | MovementType.Normal));
            if (movedCount == 0 && _manager.IsEmptyCell(position.x, position.y + 2 * axis))
                AvailableMovements.Add(new Movement(idxPiece, position.x, position.y + 2 * axis, MovementType.Normal));
            if(_manager.IsEnemyPiece(position.x, position.y, position.x - axis, position.y + axis))
                AvailableMovements.Add(new Movement(idxPiece, position.x - axis, position.y + axis, promotableType | MovementType.Catch));
            if(_manager.IsEnemyPiece(position.x, position.y, position.x + axis, position.y + axis))
                AvailableMovements.Add(new Movement(idxPiece, position.x + axis, position.y + axis, promotableType | MovementType.Catch));

            // TODO: 이 곳에 앙파상 규칙을 작성합니다.
            // if (_manager.IsEnemyPiece(position.x, position.y, position.x + axis, position.y))
        }

        private void m_RecognizeAsKnight(ChessManager _manager, int _dx, int _dy)
        {
            _dx += position.x;
            _dy += position.y;

            if(_manager.IsEmptyCell(_dx, _dy))
                AvailableMovements.Add(new Movement(idxPiece, _dx, _dy, MovementType.Normal));
            else if(_manager.IsEnemyPiece(position.x, position.y, _dx, _dy))
                AvailableMovements.Add(new Movement(idxPiece, _dx, _dy, MovementType.Catch));
            else
                return;
        }

        private void m_RecognizeAsLinear(ChessManager _manager, int _dx, int _dy, int _maxDistance)
        {
            for(int i = 0; i < _maxDistance; ++i)
            {
                int x = position.x + _dx * (i + 1);
                int y = position.y + _dy * (i + 1);

                if(_manager.IsEmptyCell(x, y))
                    AvailableMovements.Add(new Movement(idxPiece, x, y, MovementType.Normal));
                else if(_manager.IsEnemyPiece(position.x, position.y, x, y))
                    AvailableMovements.Add(new Movement(idxPiece, x, y, MovementType.Catch));
                else
                    break;
            }
        }

        private void m_RecognizeAsKing(ChessManager _manager, int _dx, int _dy)
        {
            
        }

        private void m_RecognizeCastlingK(ChessManager _manager)
        {

        }

        private void m_RecognizeCastlingQ(ChessManager _manager)
        {

        }
*/
    }
}