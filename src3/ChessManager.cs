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

        public Piece latestSpecialStartedPawn;

        public ChessManager()
        {
            chessBoard = new CellData[8, 8];
            currentTurnNumber = 0;

            actionHistory = new List<PieceAction>(384);
        }

        public GameState UpdateGameState()
        {
            int attackerTeamColorInt = currentTurnNumber % 2;
            bool hasAnyLegalAction = false;

            Color attackerTeamColor = (Color)attackerTeamColorInt;
            Color defencerTeamColor = (Color)(1 - attackerTeamColorInt);

            for(int i = 0; i < 16; ++i)
            {
                // TODO: 이 곳에서 attacker의 기물별 움직일 수 있는 모든 수를 계산해서 리스트에 넣어줍니다.
                Piece attackerPiece = pieces[attackerTeamColorInt * 16 + i];
                attackerPiece.nextActions.Clear();
                // pieces[attackerTeamColorInt * 16 + i].nextActions

                List<PieceActionList> actions = pieces[attackerTeamColorInt * 16 + i].nextActions;

                for(int j = actions.Count - 1; j >= 0; --j)
                {
                    actions[j].isTestAction = true;
                    actions[j].Next();
                    DrawCheckableMap(attackerTeamColor);

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

                    actions[j].Next();
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

        public void DrawCheckableMap(Color _attackerTeamColor)
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
                    m_GenerateBishopAction(_cellData);
                    break;
                case PieceType.Knight:
                    m_GenerateKnightAction(_cellData);
                    break;
                case PieceType.Rook:
                    m_GenerateRookAction(_cellData);
                    break;
                case PieceType.Queen:
                    m_GenerateQueenAction(_cellData);
                    break;
                case PieceType.King:
                    m_GenerateKingAction(_cellData);
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

            if (this.IsEmptyCell(frontPosition1))
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

            if (this.IsEnemyPiece(position, sideFrontPosition1))
            {
                PieceActionList sideFront1 = new PieceActionList(sideFrontPosition1, 1, true);
                sideFront1.Add(new PieceMoveAction(this, position, sideFrontPosition1));
                piece.nextActions.Add(sideFront1);
            }

            if (this.IsEnemyPiece(position, sideFrontPosition2))
            {
                PieceActionList sideFront2 = new PieceActionList(sideFrontPosition2, 1, true);
                sideFront2.Add(new PieceMoveAction(this, position, sideFrontPosition2));
                piece.nextActions.Add(sideFront2);
            }

            if (this.IsEnemyPiece(position, sidePosition1) &&
                piece == this.latestSpecialStartedPawn &&
                this.ComparePieceType(position, PieceType.Pawn)
            )
            {
                PieceActionList side1 = new PieceActionList(sidePosition1, 2, true);
                side1.Add(new PieceCatchAction(this, sidePosition1));
                side1.Add(new PieceMoveAction(this, position, sideFrontPosition1));
                piece.nextActions.Add(side1);
            }

            if (this.IsEnemyPiece(position, sidePosition2) &&
                piece == this.latestSpecialStartedPawn &&
                this.ComparePieceType(position, PieceType.Pawn)
            )
            {
                PieceActionList side2 = new PieceActionList(sidePosition2, 2, true);
                side2.Add(new PieceCatchAction(this, sidePosition2));
                side2.Add(new PieceMoveAction(this, position, sideFrontPosition1));
                piece.nextActions.Add(side2);
            }
        }

        private void m_GenerateBishopAction(CellData _cellData)
        {

        }

        private void m_GenerateKnightAction(CellData _cellData)
        {

        }

        private void m_GenerateRookAction(CellData _cellData)
        {

        }

        private void m_GenerateQueenAction(CellData _cellData)
        {

        }

        private void m_GenerateKingAction(CellData _cellData)
        {

        }
#endregion

        private bool IsChecked(Color _defencerTeamColor)
        {
            for(int y = 0; y < 8; ++y)
            for(int x = 0; x < 8; ++x)
            {
                if (chessBoard[x, y].hiddenPiece.pieceType == PieceType.King &&
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

        public bool ComparePieceType(IntVector2 _position, PieceType _pieceType)
        {
            return this.ComparePieceType(_position.x, _position.y, _pieceType);
        }

        public bool ComparePieceType(int _positionX, int _positionY, PieceType _pieceType)
        {
            return chessBoard[_positionX, _positionY].hiddenPiece.pieceType == _pieceType;
        }
#endregion
    }
}