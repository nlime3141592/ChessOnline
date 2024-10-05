using System.Collections.Generic;

namespace nl.ChessOnline3
{
    public struct CellData
    {
        public Piece piece;

        public IntVector2 position;
        
        public List<Piece> checkablePieces;
    }
}