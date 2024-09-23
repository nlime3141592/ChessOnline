using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace nl.ChessOnline3
{
    public class ChessManager
    {
        public CellData[,] chessBoard;
        public int currentTurnNumber;

        public List<PieceAction> actionHistory;

        public Piece latestSpecialStartedPawn;

        public ChessManager()
        {
            chessBoard = new CellData[8, 8];
            currentTurnNumber = 0;

            actionHistory = new List<PieceAction>(384);
        }

        public void DrawReachableMap(Color _attackerTeamColor)
        {
            Queue<CellData> attackerCells = new Queue<CellData>();

            for(int x = 0; x < 8; ++x)
            for(int y = 0; y < 8; ++y)
            {
                Piece attackerPiece = chessBoard[x, y].hiddenPiece;

                if(attackerPiece != null && attackerPiece.color == _attackerTeamColor)
                    attackerCells.Enqueue(chessBoard[x, y]);

                chessBoard[x, y].checkablePieces.Clear();
            }

            while(attackerCells.Count > 0)
            {
                CellData cellData = attackerCells.Dequeue();

                switch(cellData.hiddenPiece.pieceType)
                {
                    case PieceType.Pawn:
                        int axis = _attackerTeamColor == Color.White ? 1 : -1;
                        m_DrawReachableMapAsCell(cellData, new IntVector2(axis, axis), true);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-axis, axis), true);
                        break;
                    case PieceType.Bishop:
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(1, 1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(1, -1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(-1, 1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(-1, -1));
                        break;
                    case PieceType.Knight:
                        m_DrawReachableMapAsCell(cellData, new IntVector2(2, 1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(1, 2), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-2, 1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-1, 2), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(2, -1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(1, -2), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-2, -1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-1, -2), false);
                        break;
                    case PieceType.Rook:
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(1, 0));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(0, 1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(-1, 0));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(0, -1));
                        break;
                    case PieceType.Queen:
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(1, 1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(1, -1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(-1, 1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(-1, -1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(1, 0));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(0, 1));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(-1, 0));
                        m_DrawReachableMapAsLinear(cellData, new IntVector2(0, -1));
                        break;
                    case PieceType.King:
                        m_DrawReachableMapAsCell(cellData, new IntVector2(0, 1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-1, 1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-1, 0), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(-1, -1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(0, -1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(1, -1), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(1, 0), false);
                        m_DrawReachableMapAsCell(cellData, new IntVector2(1, 1), false);
                        break;
                    default:
                        break;
                }
            }
/*
            PieceActionList actionList = null;

            actionList.isTestAction = true;
            actionList.Next();
            // if (legal movement)
            // else (illegal movement)
            actionList.Undo();
*/
        }

        private void m_DrawReachableMapAsCell(CellData _originCellData, IntVector2 _deltaPosition, bool _onlyCatch)
        {
            IntVector2 dstPosition = _originCellData.position;
            dstPosition.x += _deltaPosition.x;
            dstPosition.y += _deltaPosition.y;

            if (!IsValidPosition(dstPosition) ||
                (IsEmptyCell(dstPosition) && !_onlyCatch) ||
                !IsEnemyPiece(_originCellData.position, dstPosition)
            )
            {
                return;
            }

            chessBoard[dstPosition.x, dstPosition.y].checkablePieces.Add(_originCellData.hiddenPiece);
        }

        private void m_DrawReachableMapAsLinear(CellData _originCellData, IntVector2 _deltaPosition)
        {
            Debug.Assert(Math.Abs(_deltaPosition.x) <= 1 && Math.Abs(_deltaPosition.y) <= 1);

            IntVector2 dstPosition = _originCellData.position;

            while(true)
            {
                dstPosition.x += _deltaPosition.x;
                dstPosition.y += _deltaPosition.y;

                if (!IsValidPosition(dstPosition) ||
                    IsAllyPiece(_originCellData.position, dstPosition)
                )
                {
                    break;
                }

                chessBoard[dstPosition.x, dstPosition.y].checkablePieces.Add(_originCellData.hiddenPiece);
            }
        }

        public bool IsValidPosition(IntVector2 _position)
        {
            return this.IsValidPosition(_position.x, _position.y);
        }

        public bool IsValidPosition(int _positionX, int _positionY)
        {
            return !(_positionX < 0 || _positionX >= 8 || _positionY < 0 || _positionY >= 8);
        }

        public bool IsEnemyPiece(IntVector2 _srcPosition, IntVector2 _dstPosition)
        {
            return this.IsEnemyPiece(_srcPosition.x, _srcPosition.y, _dstPosition.x, _dstPosition.y);
        }

        public bool IsEnemyPiece(int _srcPositionX, int _srcPositionY, int _dstPositionX, int _dstPositionY)
        {
            return chessBoard[_srcPositionX, _srcPositionY].hiddenPiece.color != chessBoard[_dstPositionX, _dstPositionY].hiddenPiece.color;
        }

        public bool IsAllyPiece(IntVector2 _srcPosition, IntVector2 _dstPosition)
        {
            return this.IsAllyPiece(_srcPosition.x, _srcPosition.y, _dstPosition.x, _dstPosition.y);
        }

        public bool IsAllyPiece(int _srcPositionX, int _srcPositionY, int _dstPositionX, int _dstPositionY)
        {
            return chessBoard[_srcPositionX, _srcPositionY].hiddenPiece.color == chessBoard[_dstPositionX, _dstPositionY].hiddenPiece.color;
        }

        public bool IsEmptyCell(IntVector2 _position)
        {
            return this.IsEmptyCell(_position.x, _position.y);
        }

        public bool IsEmptyCell(int _positionX, int _positionY)
        {
            return chessBoard[_positionX, _positionY].hiddenPiece == null;
        }
    }
}