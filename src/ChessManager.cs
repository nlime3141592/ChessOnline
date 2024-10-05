using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace nl.ChessOnline3
{
    public class ChessManager
    {
        public CellData[,] chessBoard;
        public int currentTurnNumber;

        public Piece[] pieces;

        public List<PieceAction> actionHistory;

        public Stack<Piece> catchedPieces;
        public Piece latestSpecialStartedPawnOrNull;

        public ChessManager()
        {
            chessBoard = new CellData[8, 8];
            currentTurnNumber = 0;

            pieces = new Piece[32];

            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                chessBoard[x, y].checkablePieces = new List<Piece>(16);
            }

            for(int i = 0; i < 32; ++i)
                pieces[i] = new Piece();

            actionHistory = new List<PieceAction>(256);
            catchedPieces = new Stack<Piece>(32);
        }

        public void Initialize()
        {
            for(int i = 0; i < 16; ++i)
            {
                pieces[i].color = Color.White;
                pieces[i + 16].color = Color.Black;
            }

            for(int i = 0; i < 2; ++i)
            {
                pieces[16 * i + 0].pieceType = PieceType.King;
                pieces[16 * i + 1].pieceType = PieceType.Pawn;
                pieces[16 * i + 2].pieceType = PieceType.Pawn;
                pieces[16 * i + 3].pieceType = PieceType.Pawn;
                pieces[16 * i + 4].pieceType = PieceType.Pawn;
                pieces[16 * i + 5].pieceType = PieceType.Pawn;
                pieces[16 * i + 6].pieceType = PieceType.Pawn;
                pieces[16 * i + 7].pieceType = PieceType.Pawn;
                pieces[16 * i + 8].pieceType = PieceType.Pawn;
                pieces[16 * i + 9].pieceType = PieceType.Bishop;
                pieces[16 * i + 10].pieceType = PieceType.Bishop;
                pieces[16 * i + 11].pieceType = PieceType.Knight;
                pieces[16 * i + 12].pieceType = PieceType.Knight;
                pieces[16 * i + 13].pieceType = PieceType.Rook;
                pieces[16 * i + 14].pieceType = PieceType.Rook;
                pieces[16 * i + 15].pieceType = PieceType.Queen;
            }

            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                chessBoard[x, y].piece = null;

                chessBoard[x, y].position.x = x;
                chessBoard[x, y].position.y = y;

                chessBoard[x, y].checkablePieces.Clear();
            }

            for(int i = 0; i < 8; ++i)
            {
                chessBoard[i, 1].piece = pieces[i + 1];
                chessBoard[i, 6].piece = pieces[i + 17];
            }

            chessBoard[0, 0].piece = pieces[13];
            chessBoard[1, 0].piece = pieces[12];
            chessBoard[2, 0].piece = pieces[9];
            chessBoard[3, 0].piece = pieces[15];
            chessBoard[4, 0].piece = pieces[0];
            chessBoard[5, 0].piece = pieces[10];
            chessBoard[6, 0].piece = pieces[12];
            chessBoard[7, 0].piece = pieces[14];

            chessBoard[0, 7].piece = pieces[29];
            chessBoard[1, 7].piece = pieces[28];
            chessBoard[2, 7].piece = pieces[25];
            chessBoard[3, 7].piece = pieces[31];
            chessBoard[4, 7].piece = pieces[16];
            chessBoard[5, 7].piece = pieces[26];
            chessBoard[6, 7].piece = pieces[28];
            chessBoard[7, 7].piece = pieces[30];
        }

        private PieceAction PeekHistory()
        {
            if (actionHistory.Count == 0)
                return null;

            return actionHistory[actionHistory.Count - 1];
        }

        public GameState UpdateGameState()
        {
            int attackerTeamColorInt = currentTurnNumber % 2;
            bool hasAnyLegalAction = false;

            Color attackerTeamColor = (Color)attackerTeamColorInt;
            Color defencerTeamColor = (Color)(1 - attackerTeamColorInt);

            PieceAction latestActionOrNull = PeekHistory();

            for (int y = 0; y < 8; ++y)
            for (int x = 0; x < 8; ++x)
            {
                if (chessBoard[x, y].piece == null || chessBoard[x, y].piece.color != attackerTeamColor)
                    continue;

                CellData cellData = chessBoard[x, y];
                Piece attackerPiece = cellData.piece;

                attackerPiece.nextActions.Clear();
                m_GenerateAgentAction(cellData);

                List<PieceAction> actions = attackerPiece.nextActions;

                for(int j = actions.Count - 1; j >= 0; --j)
                {
                    m_DoPieceAction(latestActionOrNull, actions[j]);
                    DrawCheckableMap(attackerTeamColor);
                    m_UndoPieceAction(latestActionOrNull, actions[j]);

                    if(IsChecked(attackerTeamColor))
                    {
                        // NOTE: attacker의 불법적인 수
                        actions.RemoveAt(j);
                    }
                    else
                    {
                        // NOTE: attacker의 합법적인 수
                        hasAnyLegalAction = true;
                    }
                }
            }

            DrawCheckableMap(attackerTeamColor);

            if(hasAnyLegalAction)
            {
                // NOTE: attacker는 움직일 수 있습니다.
                return GameState.Running;
            }
            else if(IsChecked(attackerTeamColor))
            {
                // NOTE: 체크메이트 상태, attackerTeam이 패배했습니다. 게임을 종료합니다.
                return GameState.Checkmate;
            }
            else
            {
                // NOTE: 스테일메이트 상태, 게임을 무승부 처리합니다.
                return GameState.Stalemate;
            }
        }

#region Piece Action Generating
        private void DrawCheckableMap(Color _attackerTeamColor)
        {
            Queue<CellData> attackerCells = new Queue<CellData>();

            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                Piece attackerPiece = chessBoard[x, y].piece;

                if(attackerPiece != null && attackerPiece.color == _attackerTeamColor)
                    attackerCells.Enqueue(chessBoard[x, y]);

                chessBoard[x, y].checkablePieces.Clear();
            }

            while(attackerCells.Count > 0)
            {
                CellData cellData = attackerCells.Dequeue();

                switch(cellData.piece.pieceType)
                {
                    case PieceType.Pawn:
                        int axis = _attackerTeamColor == Color.White ? 1 : -1;
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(axis, axis), true);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-axis, axis), true);
                        break;
                    case PieceType.Bishop:
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(1, 1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(1, -1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(-1, 1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(-1, -1));
                        break;
                    case PieceType.Knight:
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(2, 1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(1, 2), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-2, 1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-1, 2), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(2, -1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(1, -2), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-2, -1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-1, -2), false);
                        break;
                    case PieceType.Rook:
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(1, 0));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(0, 1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(-1, 0));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(0, -1));
                        break;
                    case PieceType.Queen:
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(1, 1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(1, -1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(-1, 1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(-1, -1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(1, 0));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(0, 1));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(-1, 0));
                        m_DrawCheckableMapAsLinear(cellData, new IntVector2(0, -1));
                        break;
                    case PieceType.King:
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(0, 1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-1, 1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-1, 0), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(-1, -1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(0, -1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(1, -1), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(1, 0), false);
                        m_DrawCheckableMapAsCell(cellData, new IntVector2(1, 1), false);
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

        private void m_DrawCheckableMapAsCell(CellData _originCellData, IntVector2 _deltaPosition, bool _onlyCatch)
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

            chessBoard[dstPosition.x, dstPosition.y].checkablePieces.Add(_originCellData.piece);
        }

        private void m_DrawCheckableMapAsLinear(CellData _originCellData, IntVector2 _deltaPosition)
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

                chessBoard[dstPosition.x, dstPosition.y].checkablePieces.Add(_originCellData.piece);
            }
        }

        private void m_GenerateAgentAction(CellData _cellData)
        {
            switch(_cellData.piece.pieceType)
            {
                case PieceType.Pawn:
                    m_GeneratePawnAction(_cellData);
                    break;
                case PieceType.Bishop:
                    m_GenerateLinearAction(_cellData, new IntVector2(1, 1));
                    m_GenerateLinearAction(_cellData, new IntVector2(1, -1));
                    m_GenerateLinearAction(_cellData, new IntVector2(-1, 1));
                    m_GenerateLinearAction(_cellData, new IntVector2(-1, -1));
                    break;
                case PieceType.Knight:
                    m_GenerateCellAction(_cellData, new IntVector2(2, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 2));
                    m_GenerateCellAction(_cellData, new IntVector2(-2, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(-1, 2));
                    m_GenerateCellAction(_cellData, new IntVector2(2, -1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, -2));
                    m_GenerateCellAction(_cellData, new IntVector2(-2, -1));
                    m_GenerateCellAction(_cellData, new IntVector2(-1, -2));
                    break;
                case PieceType.Rook:
                    m_GenerateLinearAction(_cellData, new IntVector2(1, 0));
                    m_GenerateLinearAction(_cellData, new IntVector2(0, 1));
                    m_GenerateLinearAction(_cellData, new IntVector2(-1, 0));
                    m_GenerateLinearAction(_cellData, new IntVector2(0, -1));
                    break;
                case PieceType.Queen:
                    m_GenerateLinearAction(_cellData, new IntVector2(1, 1));
                    m_GenerateLinearAction(_cellData, new IntVector2(1, -1));
                    m_GenerateLinearAction(_cellData, new IntVector2(-1, 1));
                    m_GenerateLinearAction(_cellData, new IntVector2(-1, -1));
                    m_GenerateLinearAction(_cellData, new IntVector2(1, 0));
                    m_GenerateLinearAction(_cellData, new IntVector2(0, 1));
                    m_GenerateLinearAction(_cellData, new IntVector2(-1, 0));
                    m_GenerateLinearAction(_cellData, new IntVector2(0, -1));
                    break;
                case PieceType.King:
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 0));
                    m_GenerateCellAction(_cellData, new IntVector2(1, -1));
                    m_GenerateCellAction(_cellData, new IntVector2(0, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(0, -1));
                    m_GenerateCellAction(_cellData, new IntVector2(-1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(-1, 0));
                    m_GenerateCellAction(_cellData, new IntVector2(-1, -1));
                    m_GenerateCastlingAction(_cellData, new IntVector2(1, 0));
                    m_GenerateCastlingAction(_cellData, new IntVector2(-1, 0));
                    break;
                default:
                    break;
            }
        }

        private void m_GeneratePawnAction(CellData _cellData)
        {
            Piece piece = _cellData.piece;
            List<PieceAction> nextActions = piece.nextActions;
            int sx = _cellData.position.x;
            int sy = _cellData.position.y;
            int axis = piece.color == Color.White ? 1 : -1;

            IntVector2 vOrigin = _cellData.position;
            IntVector2 vFront1 = new IntVector2(sx, sy + axis);
            IntVector2 vFront2 = new IntVector2(sx, sy + 2 * axis);
            IntVector2 vDiag1 = new IntVector2(sx + axis, vFront1.y);
            IntVector2 vDiag2 = new IntVector2(sx - axis, vFront1.y);
            IntVector2 vSide1 = new IntVector2(vDiag1.x, sy);
            IntVector2 vSide2 = new IntVector2(vDiag2.x, sy);

            if (this.IsValidPosition(vFront1) && this.IsEmptyCell(vFront1))
            {
                nextActions.Add(PieceAction.GetMoveAction(sx, sy, vFront1.x, vFront1.y));

                if (piece.movedCount == 0 && this.IsEmptyCell(vFront2))
                {
                    PieceAction actFront2 = PieceAction.GetMoveAction(sx, sy, vFront2.x, vFront2.y);
                    actFront2.isSpecialStartedPawn = true;
                    nextActions.Add(actFront2);
                }
            }

            if (this.IsValidPosition(vDiag1) && this.IsEnemyPiece(vOrigin, vDiag1))
                nextActions.Add(PieceAction.GetCatchAction(sx, sy, vDiag1.x, vDiag1.y));

            if (this.IsValidPosition(vDiag2) && this.IsEnemyPiece(vOrigin, vDiag2))
                nextActions.Add(PieceAction.GetCatchAction(sx, sy, vDiag2.x, vDiag2.y));

            if (this.IsValidPosition(vSide1) &&
                !this.IsEmptyCell(vSide1) &&
                chessBoard[vSide1.x, vSide1.y].piece == latestSpecialStartedPawnOrNull
            )
            {
                nextActions.Add(PieceAction.GetEnPassantAction(sx, sy, vSide1.x, vSide1.y, axis));
            }

            if (this.IsValidPosition(vSide2) &&
                !this.IsEmptyCell(vSide2) &&
                chessBoard[vSide2.x, vSide2.y].piece == latestSpecialStartedPawnOrNull
            )
            {
                nextActions.Add(PieceAction.GetEnPassantAction(sx, sy, vSide2.x, vSide2.y, axis));
            }
        }

        private void m_GenerateLinearAction(CellData _cellData, IntVector2 _deltaPosition)
        {
            Debug.Assert(Math.Abs(_deltaPosition.x) <= 1 && Math.Abs(_deltaPosition.y) <= 1);

            IntVector2 vNext = _cellData.position;

            do
            {
                vNext.x += _deltaPosition.x;
                vNext.y += _deltaPosition.y;
            }
            while (m_GenerateCellAction(_cellData, vNext) == 0);
        }

        private int m_GenerateCellAction(CellData _cellData, IntVector2 _deltaPosition)
        {
            Piece piece = _cellData.piece;
            List<PieceAction> nextActions = piece.nextActions;
            IntVector2 vOrigin = _cellData.position;
            IntVector2 vNext = new IntVector2(vOrigin.x + _deltaPosition.x, vOrigin.y + _deltaPosition.y);

            if(!this.IsValidPosition(vNext) || this.IsAllyPiece(vOrigin, vNext))
            {
                return -1;
            }
            else if(this.IsEnemyPiece(_cellData.position, vNext))
            {
                nextActions.Add(PieceAction.GetCatchAction(vOrigin.x, vOrigin.y, vNext.x, vNext.y));
                return 1;
            }
            else
            {
                nextActions.Add(PieceAction.GetMoveAction(vOrigin.x, vOrigin.y, vNext.x, vNext.y));
                return 0;
            }
        }

        // TODO: Implement whenever.
        private void m_GenerateCastlingAction(CellData _cellData, IntVector2 _deltaPosition)
        {
            // throw new NotImplementedException();

            /*
            Debug.Assert(Math.Abs(_deltaPosition.x) == 1 && _deltaPosition.y == 0);

            Piece piece = _cellData.hiddenPiece;
            IntVector2 nextPosition = _cellData.position;

            if(piece.movedCount > 0)
                return;

            while(true)
            {
                nextPosition.x += _deltaPosition.x;

                if (IsEmptyCell(nextPosition))
                    continue;
                else if(ComparePieceType(nextPosition, PieceType.Rook) && chessBoard[nextPosition.x, nextPosition.y].hiddenPiece.movedCount == 0)
                {
                    // TODO: Castling
                    return;
                }
                else // NOTE: No Castling.
                    return;
            }
            */
        }
#endregion

#region Piece Action Handling
        private void m_DoPieceAction(PieceAction _prevAction, PieceAction _currentAction)
        {
            switch (_currentAction.pieceActionType)
            {
                case PieceActionType.Move:
                    m_DoPieceMove(_currentAction);
                    break;
                case PieceActionType.Catch:
                    m_DoPieceCatch(_currentAction);
                    break;
                case PieceActionType.Castling:
                    m_DoPieceCastling(_currentAction);
                    break;
                case PieceActionType.EnPassant:
                    m_DoPieceEnPassant(_currentAction);
                    break;
                default:
                    break;
            }

            m_SetPawnSpecialStarts(_currentAction);
        }

        private void m_UndoPieceAction(PieceAction _lastActionOrNull, PieceAction _undidAction)
        {
            switch (_undidAction.pieceActionType)
            {
                case PieceActionType.Move:
                    m_UndoPieceMove(_undidAction);
                    break;
                case PieceActionType.Catch:
                    m_UndoPieceCatch(_undidAction);
                    break;
                case PieceActionType.Castling:
                    m_UndoPieceCastling(_undidAction);
                    break;
                case PieceActionType.EnPassant:
                    m_UndoPieceEnPassant(_undidAction);
                    break;
                default:
                    break;
            }

            if(_lastActionOrNull != null)
                m_SetPawnSpecialStarts(_lastActionOrNull);
        }

        private void m_SetPawnSpecialStarts(PieceAction _pieceAction)
        {
            if(!_pieceAction.isSpecialStartedPawn)
            {
                latestSpecialStartedPawnOrNull = null;
                return;
            }

            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            latestSpecialStartedPawnOrNull = chessBoard[dx, dy].piece;
        }

        private void m_DoPieceMove(PieceAction _pieceAction)
        {
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            Debug.Assert(chessBoard[sx, sy].piece != null);
            Debug.Assert(chessBoard[dx, dy].piece == null);

            chessBoard[dx, dy].piece = chessBoard[sx, sy].piece;
            chessBoard[sx, sy].piece = null;

            ++chessBoard[dx, dy].piece.movedCount;
        }

        private void m_UndoPieceMove(PieceAction _pieceAction)
        {
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            --chessBoard[dx, dy].piece.movedCount;

            chessBoard[sx, sy].piece = chessBoard[dx, dy].piece;
            chessBoard[dx, dy].piece = null;
        }

        private void m_DoPieceCatch(PieceAction _pieceAction)
        {
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            Debug.Assert(chessBoard[sx, sy].piece != null);
            Debug.Assert(chessBoard[dx, dy].piece != null);
            Debug.Assert(chessBoard[sx, sy].piece != chessBoard[dx, dy].piece);

            catchedPieces.Push(chessBoard[dx, dy].piece);
            chessBoard[dx, dy].piece.isCatched = true;
            chessBoard[dx, dy].piece = chessBoard[sx, sy].piece;
            chessBoard[sx, sy].piece = null;

            ++chessBoard[dx, dy].piece.movedCount;
        }

        private void m_UndoPieceCatch(PieceAction _pieceAction)
        {
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            --chessBoard[dx, dy].piece.movedCount;

            chessBoard[sx, sy].piece = chessBoard[dx, dy].piece;
            chessBoard[dx, dy].piece = catchedPieces.Pop();
            chessBoard[dx, dy].piece.isCatched = false;
        }

        private void m_DoPieceCastling(PieceAction _pieceAction)
        {
            int rx = _pieceAction.posRepresentative.x;
            int ry = _pieceAction.posRepresentative.y;
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;
            int dirKingToRook = dx - sx < 0 ? -1 : 1;

            // NOTE: Source Constraints
            Debug.Assert(chessBoard[sx, sy].piece != null);
            Debug.Assert(chessBoard[sx, sy].piece.pieceType == PieceType.King);
            Debug.Assert(chessBoard[sx, sy].piece.movedCount == 0);

            // NOTE: Destination Constraints
            Debug.Assert(chessBoard[dx, dy].piece != null);
            Debug.Assert(chessBoard[dx, dy].piece.pieceType == PieceType.Rook);
            Debug.Assert(chessBoard[dx, dy].piece.movedCount == 0);

            chessBoard[rx, ry].piece = chessBoard[sx, sy].piece;
            ++chessBoard[sx, sy].piece.movedCount;
            chessBoard[sx, sy].piece = null;
            chessBoard[rx - dirKingToRook, ry].piece = chessBoard[dx, dy].piece;
            ++chessBoard[dx, dy].piece.movedCount;
            chessBoard[dx, dy].piece = null;
        }

        private void m_UndoPieceCastling(PieceAction _pieceAction)
        {
            int rx = _pieceAction.posRepresentative.x;
            int ry = _pieceAction.posRepresentative.y;
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;
            int dirKingToRook = dx - sx < 0 ? -1 : 1;

            --chessBoard[rx, ry].piece.movedCount;
            chessBoard[sx, sy].piece = chessBoard[rx, ry].piece;
            chessBoard[rx, ry].piece = null;
            --chessBoard[rx - dirKingToRook, ry].piece.movedCount;
            chessBoard[dx, dy].piece = chessBoard[rx - dirKingToRook, ry].piece;
            chessBoard[rx - dirKingToRook, ry].piece = null;
        }

        private void m_DoPieceEnPassant(PieceAction _pieceAction)
        {
            int rx = _pieceAction.posRepresentative.x;
            int ry = _pieceAction.posRepresentative.y;
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            Debug.Assert(chessBoard[rx, ry].piece == null);
            Debug.Assert(chessBoard[sx, sy].piece != null);
            Debug.Assert(chessBoard[dx, dy].piece != null);

            catchedPieces.Push(chessBoard[dx, dy].piece);
            chessBoard[dx, dy].piece.isCatched = true;
            chessBoard[dx, dy].piece = null;
            chessBoard[rx, ry].piece = chessBoard[sx, sy].piece;
            chessBoard[sx, sy].piece = null;

            ++chessBoard[rx, ry].piece.movedCount;
        }

        private void m_UndoPieceEnPassant(PieceAction _pieceAction)
        {
            int rx = _pieceAction.posRepresentative.x;
            int ry = _pieceAction.posRepresentative.y;
            int sx = _pieceAction.posSource.x;
            int sy = _pieceAction.posSource.y;
            int dx = _pieceAction.posDestination.x;
            int dy = _pieceAction.posDestination.y;

            --chessBoard[rx, ry].piece.movedCount;

            chessBoard[sx, sy].piece = chessBoard[rx, ry].piece;
            chessBoard[rx, ry].piece = null;
            chessBoard[dx, dy].piece = catchedPieces.Pop();
            chessBoard[dx, dy].piece.isCatched = false;
        }
#endregion

        private bool IsChecked(Color _defencerTeamColor)
        {
            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                if (chessBoard[x, y].piece != null &&
                    chessBoard[x, y].piece.pieceType == PieceType.King &&
                    chessBoard[x, y].piece.color == _defencerTeamColor
                )
                {
                    return chessBoard[x, y].checkablePieces.Count > 0;
                }
            }

            return false;
        }

#region Conditions
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
            if(IsEmptyCell(_srcPositionX, _srcPositionY) || IsEmptyCell(_dstPositionX, _dstPositionY))
                return false;

            return chessBoard[_srcPositionX, _srcPositionY].piece.color != chessBoard[_dstPositionX, _dstPositionY].piece.color;
        }

        public bool IsAllyPiece(IntVector2 _srcPosition, IntVector2 _dstPosition)
        {
            return this.IsAllyPiece(_srcPosition.x, _srcPosition.y, _dstPosition.x, _dstPosition.y);
        }

        public bool IsAllyPiece(int _srcPositionX, int _srcPositionY, int _dstPositionX, int _dstPositionY)
        {
            if(IsEmptyCell(_srcPositionX, _srcPositionY) || IsEmptyCell(_dstPositionX, _dstPositionY))
                return false;

            return chessBoard[_srcPositionX, _srcPositionY].piece.color == chessBoard[_dstPositionX, _dstPositionY].piece.color;
        }

        public bool IsEmptyCell(IntVector2 _position)
        {
            return this.IsEmptyCell(_position.x, _position.y);
        }

        public bool IsEmptyCell(int _positionX, int _positionY)
        {
            return chessBoard[_positionX, _positionY].piece == null;
        }

        public bool ComparePieceType(IntVector2 _position, PieceType _pieceType)
        {
            return this.ComparePieceType(_position.x, _position.y, _pieceType);
        }

        public bool ComparePieceType(int _positionX, int _positionY, PieceType _pieceType)
        {
            return chessBoard[_positionX, _positionY].piece.pieceType == _pieceType;
        }
#endregion

        public void Draw()
        {
            string charset = "PBNRQK";

            for (int y = 7; y >= 0; --y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    if (IsEmptyCell(x, y))
                        Console.Write("-");
                    else if(chessBoard[x, y].piece.color != Color.White)
                        Console.Write("{0}", (char)((int)charset[(int)chessBoard[x, y].piece.pieceType] + 0x20));
                    else
                        Console.Write("{0}", charset[(int)chessBoard[x, y].piece.pieceType]);
                }

                Console.WriteLine();
            }
        }

        public void Move(int _positionX, int _positionY, int _idxNextAction)
        {
            CellData cellData = chessBoard[_positionX, _positionY];

            PieceAction latestActionOrNull = PeekHistory();
            PieceAction action = cellData.piece.nextActions[_idxNextAction];

            m_DoPieceAction(latestActionOrNull, action);
            actionHistory.Add(action);
            ++currentTurnNumber;
        }
    }
}