using System.Collections.Generic;

namespace nl.ChessOnline3
{
    public struct CellData
    {
        public Piece shownPiece;
        public Piece hiddenPiece;

        public IntVector2 position;
        
        public List<Piece> checkablePieces;
    }
}