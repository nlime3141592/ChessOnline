using nl.ChessOnline;
using System.Collections.Generic;

namespace nl.ChessOnline
{
    public struct CellData
    {
        public Piece piece;

        public IntVector2 position;

        public List<Piece> checkablePieces;
    }
}