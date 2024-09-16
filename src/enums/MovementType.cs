using System;

namespace nl.ChessOnline
{
    [Flags]
    public enum MovementType
    {
        Normal = 0,
        Catch = 1,
        Promotion = 2,
        EnPassant = 4,
        CastlingK = 8,
        CastlingQ = 16
    }
}