using nl.ChessOnline;

namespace nl.ChessOnline
{
    /// <summary>
    /// Piece가 수행할 수 있는 마이크로 액션을 대표하는 클래스
    /// </summary>
    public class PieceActionData
    {
        public readonly PieceActionType pieceActionType;
        public readonly IntVector2 posRepresentative;
        public readonly IntVector2 posSource;
        public readonly IntVector2 posDestination;

        public bool isSpecialStartedPawn = false;

        private PieceActionData(
            PieceActionType _pieceActionType,
            int _rx, int _ry,
            int _sx, int _sy,
            int _dx, int _dy
        )
        {
            this.pieceActionType = _pieceActionType;
            this.posRepresentative = new IntVector2(_rx, _ry);
            this.posSource = new IntVector2(_sx, _sy);
            this.posDestination = new IntVector2(_dx, _dy);
        }

        public static PieceActionData GetMoveAction(int _sx, int _sy, int _dx, int _dy)
        {
            PieceActionData action = new PieceActionData(
                PieceActionType.Move,
                _dx, _dy,
                _sx, _sy,
                _dx, _dy
            );

            return action;
        }

        public static PieceActionData GetCatchAction(int _sx, int _sy, int _dx, int _dy)
        {
            PieceActionData action = new PieceActionData(
                PieceActionType.Catch,
                _dx, _dy,
                _sx, _sy,
                _dx, _dy
            );

            return action;
        }

        public static PieceActionData GetCastlingAction(int _sx, int _sy, int _dx, int _dy)
        {
            int dirKingToRook = _dx - _sx < 0 ? -2 : 2;

            PieceActionData action = new PieceActionData(
                PieceActionType.Castling,
                _sx + dirKingToRook, _sy,
                _sx, _sy,
                _dx, _dy
            );

            return action;
        }

        public static PieceActionData GetEnPassantAction(int _sx, int _sy, int _dx, int _dy, int _axis)
        {
            PieceActionData action = new PieceActionData(
                PieceActionType.EnPassant,
                _dx, _dy + _axis,
                _sx, _sy,
                _dx, _dy
            );

            return action;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", posRepresentative.x, posRepresentative.y);
        }
    }
}