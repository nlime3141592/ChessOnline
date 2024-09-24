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

        public List<PieceActionList> actionHistory;

        public Piece latestSpecialStartedPawn;

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

            actionHistory = new List<PieceActionList>(256);
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
                chessBoard[x, y].hiddenPiece = null;

                chessBoard[x, y].position.x = x;
                chessBoard[x, y].position.y = y;

                chessBoard[x, y].checkablePieces.Clear();
            }

            for(int i = 0; i < 8; ++i)
            {
                chessBoard[i, 1].hiddenPiece = pieces[i + 1];
                chessBoard[i, 6].hiddenPiece = pieces[i + 17];
            }

            chessBoard[0, 0].hiddenPiece = pieces[13];
            chessBoard[1, 0].hiddenPiece = pieces[12];
            chessBoard[2, 0].hiddenPiece = pieces[9];
            chessBoard[3, 0].hiddenPiece = pieces[15];
            chessBoard[4, 0].hiddenPiece = pieces[0];
            chessBoard[5, 0].hiddenPiece = pieces[10];
            chessBoard[6, 0].hiddenPiece = pieces[12];
            chessBoard[7, 0].hiddenPiece = pieces[14];

            chessBoard[0, 7].hiddenPiece = pieces[29];
            chessBoard[1, 7].hiddenPiece = pieces[28];
            chessBoard[2, 7].hiddenPiece = pieces[25];
            chessBoard[3, 7].hiddenPiece = pieces[31];
            chessBoard[4, 7].hiddenPiece = pieces[16];
            chessBoard[5, 7].hiddenPiece = pieces[26];
            chessBoard[6, 7].hiddenPiece = pieces[28];
            chessBoard[7, 7].hiddenPiece = pieces[30];

            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                chessBoard[x, y].shownPiece = chessBoard[x, y].hiddenPiece;
            }
        }

        public GameState UpdateGameState()
        {
            int attackerTeamColorInt = currentTurnNumber % 2;
            bool hasAnyLegalAction = false;

            Color attackerTeamColor = (Color)attackerTeamColorInt;
            Color defencerTeamColor = (Color)(1 - attackerTeamColorInt);

            for (int y = 0; y < 8; ++y)
            for (int x = 0; x < 8; ++x)
            {
                if (chessBoard[x, y].hiddenPiece == null || chessBoard[x, y].hiddenPiece.color != attackerTeamColor)
                    continue;

                CellData cellData = chessBoard[x, y];
                Piece attackerPiece = cellData.hiddenPiece;

                attackerPiece.nextActions.Clear();
                m_GenerateAgentAction(cellData);

                List<PieceActionList> actions = attackerPiece.nextActions;

                for(int j = actions.Count - 1; j >= 0; --j)
                {
                    actions[j].isTestAction = true;
                    actions[j].Next();
                    DrawCheckableMap(attackerTeamColor);
                    actions[j].Undo();

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

        private void DrawCheckableMap(Color _attackerTeamColor)
        {
            Queue<CellData> attackerCells = new Queue<CellData>();

            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
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

#region Action Handling
        private void m_GenerateAgentAction(CellData _cellData)
        {
            switch(_cellData.hiddenPiece.pieceType)
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
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
                    m_GenerateCellAction(_cellData, new IntVector2(1, 1));
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
            Piece piece = _cellData.hiddenPiece;

            int axis = piece.color == Color.White ? 1 : -1;
            IntVector2 position = _cellData.position;

            IntVector2 frontPosition1 = new IntVector2(position.x, position.y + axis);
            IntVector2 frontPosition2 = new IntVector2(position.x, frontPosition1.y + axis);
            IntVector2 sideFrontPosition1 = new IntVector2(position.x - axis, frontPosition1.y);
            IntVector2 sideFrontPosition2 = new IntVector2(position.x + axis, frontPosition1.y);
            IntVector2 sidePosition1 = new IntVector2(sideFrontPosition1.x, position.y);
            IntVector2 sidePosition2 = new IntVector2(sideFrontPosition2.x, position.y);

            if (this.IsValidPosition(frontPosition1) &&
                this.IsEmptyCell(frontPosition1))
            {
                PieceActionList front1 = new PieceActionList(frontPosition1, 1, true);
                front1.Add(new PieceMoveAction(this, position, frontPosition1));
                piece.nextActions.Add(front1);

                if (piece.movedCount == 0 && this.IsEmptyCell(frontPosition2))
                {
                    PieceActionList front2 = new PieceActionList(frontPosition2, 1, true);
                    front2.Add(new PieceMoveAction(this, position, frontPosition2));
                    piece.nextActions.Add(front2);
                }
            }

            if (this.IsValidPosition(sideFrontPosition1) &&
                this.IsEnemyPiece(position, sideFrontPosition1))
            {
                PieceActionList sideFront1 = new PieceActionList(sideFrontPosition1, 1, true);
                sideFront1.Add(new PieceMoveAction(this, position, sideFrontPosition1));
                piece.nextActions.Add(sideFront1);
            }

            if (this.IsValidPosition(sideFrontPosition2) &&
                this.IsEnemyPiece(position, sideFrontPosition2))
            {
                PieceActionList sideFront2 = new PieceActionList(sideFrontPosition2, 1, true);
                sideFront2.Add(new PieceMoveAction(this, position, sideFrontPosition2));
                piece.nextActions.Add(sideFront2);
            }

            if (this.IsValidPosition(sidePosition1) &&
                this.IsEnemyPiece(position, sidePosition1) &&
                piece == this.latestSpecialStartedPawn &&
                this.ComparePieceType(position, PieceType.Pawn)
            )
            {
                PieceActionList side1 = new PieceActionList(sideFrontPosition1, 2, true);
                side1.Add(new PieceCatchAction(this, sidePosition1));
                side1.Add(new PieceMoveAction(this, position, sideFrontPosition1));
                piece.nextActions.Add(side1);
            }

            if (this.IsValidPosition(sidePosition2) &&
                this.IsEnemyPiece(position, sidePosition2) &&
                piece == this.latestSpecialStartedPawn &&
                this.ComparePieceType(position, PieceType.Pawn)
            )
            {
                PieceActionList side2 = new PieceActionList(sideFrontPosition2, 2, true);
                side2.Add(new PieceCatchAction(this, sidePosition2));
                side2.Add(new PieceMoveAction(this, position, sideFrontPosition2));
                piece.nextActions.Add(side2);
            }
        }

        private void m_GenerateLinearAction(CellData _cellData, IntVector2 _deltaPosition)
        {
            Debug.Assert(Math.Abs(_deltaPosition.x) <= 1 && Math.Abs(_deltaPosition.y) <= 1);

            Piece piece = _cellData.hiddenPiece;
            IntVector2 nextPosition = _cellData.position;

            while (true)
            {
                nextPosition.x += _deltaPosition.x;
                nextPosition.y += _deltaPosition.y;

                if (!this.IsValidPosition(nextPosition) || this.IsAllyPiece(_cellData.position, nextPosition))
                {
                    break;
                }
                else if (this.IsEnemyPiece(_cellData.position, nextPosition))
                {
                    PieceActionList catchAction = new PieceActionList(nextPosition, 2, true);
                    catchAction.Add(new PieceCatchAction(this, nextPosition));
                    catchAction.Add(new PieceMoveAction(this, _cellData.position, nextPosition));
                    piece.nextActions.Add(catchAction);
                    break;
                }
                else
                {
                    PieceActionList moveAction = new PieceActionList(nextPosition, 1, true);
                    moveAction.Add(new PieceMoveAction(this, _cellData.position, nextPosition));
                    piece.nextActions.Add(moveAction);
                }
            }
        }

        private void m_GenerateCellAction(CellData _cellData, IntVector2 _deltaPosition)
        {
            Piece piece = _cellData.hiddenPiece;
            IntVector2 nextPosition = _cellData.position;
            nextPosition.x += _deltaPosition.x;
            nextPosition.y += _deltaPosition.y;

            if(!this.IsValidPosition(nextPosition) || this.IsAllyPiece(_cellData.position, nextPosition))
                return;
            else if(this.IsEnemyPiece(_cellData.position, nextPosition))
            {
                PieceActionList catchAction = new PieceActionList(nextPosition, 2, true);
                catchAction.Add(new PieceCatchAction(this, nextPosition));
                catchAction.Add(new PieceMoveAction(this, _cellData.position, nextPosition));
                piece.nextActions.Add(catchAction);
            }
            else
            {
                PieceActionList moveAction = new PieceActionList(nextPosition, 1, true);
                moveAction.Add(new PieceMoveAction(this, _cellData.position, nextPosition));
                piece.nextActions.Add(moveAction);
            }
        }

        private void m_GenerateCastlingAction(CellData _cellData, IntVector2 _deltaPosition)
        {
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
        }
#endregion

        private bool IsChecked(Color _defencerTeamColor)
        {
            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                if (chessBoard[x, y].hiddenPiece != null &&
                    chessBoard[x, y].hiddenPiece.pieceType == PieceType.King &&
                    chessBoard[x, y].hiddenPiece.color == _defencerTeamColor
                )
                {
                    return chessBoard[x, y].checkablePieces.Count > 0;
                }
            }

            return false;
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

            chessBoard[dstPosition.x, dstPosition.y].checkablePieces.Add(_originCellData.hiddenPiece);
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

                chessBoard[dstPosition.x, dstPosition.y].checkablePieces.Add(_originCellData.hiddenPiece);
            }
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

            return chessBoard[_srcPositionX, _srcPositionY].hiddenPiece.color != chessBoard[_dstPositionX, _dstPositionY].hiddenPiece.color;
        }

        public bool IsAllyPiece(IntVector2 _srcPosition, IntVector2 _dstPosition)
        {
            return this.IsAllyPiece(_srcPosition.x, _srcPosition.y, _dstPosition.x, _dstPosition.y);
        }

        public bool IsAllyPiece(int _srcPositionX, int _srcPositionY, int _dstPositionX, int _dstPositionY)
        {
            if(IsEmptyCell(_srcPositionX, _srcPositionY) || IsEmptyCell(_dstPositionX, _dstPositionY))
                return false;

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

        public bool ComparePieceType(IntVector2 _position, PieceType _pieceType)
        {
            return this.ComparePieceType(_position.x, _position.y, _pieceType);
        }

        public bool ComparePieceType(int _positionX, int _positionY, PieceType _pieceType)
        {
            return chessBoard[_positionX, _positionY].hiddenPiece.pieceType == _pieceType;
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
                    else if(chessBoard[x, y].shownPiece.color != Color.White)
                        Console.Write("{0}", (char)((int)charset[(int)chessBoard[x, y].shownPiece.pieceType] + 0x20));
                    else
                        Console.Write("{0}", charset[(int)chessBoard[x, y].shownPiece.pieceType]);
                }

                Console.WriteLine();
            }
        }

        public void Move(int _positionX, int _positionY, int _idxNextAction)
        {
            CellData cellData = chessBoard[_positionX, _positionY];

            PieceActionList actions = cellData.shownPiece.nextActions[_idxNextAction];

            ++cellData.shownPiece.movedCount;
            actions.isTestAction = false;
            actions.Next();

            actionHistory.Add(actions);

            ++currentTurnNumber;
        }
    }
}