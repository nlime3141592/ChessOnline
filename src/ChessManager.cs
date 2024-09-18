using System;
using System.Collections;
using System.Diagnostics;

namespace nl.ChessOnline
{
    public class ChessManager
    {
        private const int c_BOARD_SIZE_X = 8;
        private const int c_BOARD_SIZE_Y = 8;
        private const int c_PIECE_COUNT = 32;

        public Piece[,] gameBoard;
        public Piece[] pieces;
        public bool[,] checkMap;
        public int turnNumber;
        public bool isGameStarted;

#region Input Register
        public int inputSelectedPositionX = -1;
        public int inputSelectedPositionY = -1;

        public int inputMovingPositionX = -1;
        public int inputMovingPositionY = -1;

        public PieceType inputPromoted = PieceType.None;

        public Piece inputPawnSpecialStarted = null;
#endregion

        public ChessManager()
        {
            gameBoard = new Piece[c_BOARD_SIZE_X, c_BOARD_SIZE_Y];
            pieces = new Piece[c_PIECE_COUNT];
            checkMap = new bool[c_BOARD_SIZE_X, c_BOARD_SIZE_Y];

            for(int i = 0; i < c_PIECE_COUNT; ++i)
                pieces[i] = new Piece();

            this.Initialize();
        }

        public void Initialize()
        {
            Color tW = Color.White;
            Color tB = Color.Black;

            PieceType pP = PieceType.Pawn;
            PieceType pB = PieceType.Bishop;
            PieceType pN = PieceType.Knight;
            PieceType pR = PieceType.Rook;
            PieceType pQ = PieceType.Queen;
            PieceType pK = PieceType.King;

            turnNumber = 0;
            isGameStarted = false;

            inputSelectedPositionX = -1;
            inputSelectedPositionY = -1;

            inputMovingPositionX = -1;
            inputMovingPositionY = -1;

            inputPromoted = PieceType.None;

            for(int i = 0; i < 8; ++i)
            {
                gameBoard[i, 0] = pieces[i];
                gameBoard[i, 1] = pieces[i + 8];
                gameBoard[i, 2] = null;
                gameBoard[i, 3] = null;
                gameBoard[i, 4] = null;
                gameBoard[i, 5] = null;
                gameBoard[i, 6] = pieces[i + 16];
                gameBoard[i, 7] = pieces[i + 24];
            }

            // NOTE: White Team Pieces, 0 to 15.
            pieces[0].Initialize(tW, pR);
            pieces[1].Initialize(tW, pN);
            pieces[2].Initialize(tW, pB);
            pieces[3].Initialize(tW, pQ);
            pieces[4].Initialize(tW, pK);
            pieces[5].Initialize(tW, pB);
            pieces[6].Initialize(tW, pN);
            pieces[7].Initialize(tW, pR);
            pieces[8].Initialize(tW, pP);
            pieces[9].Initialize(tW, pP);
            pieces[10].Initialize(tW, pP);
            pieces[11].Initialize(tW, pP);
            pieces[12].Initialize(tW, pP);
            pieces[13].Initialize(tW, pP);
            pieces[14].Initialize(tW, pP);
            pieces[15].Initialize(tW, pP);

            // NOTE: Black Team Pieces, 16 to 31.
            pieces[16].Initialize(tB, pP);
            pieces[17].Initialize(tB, pP);
            pieces[18].Initialize(tB, pP);
            pieces[19].Initialize(tB, pP);
            pieces[20].Initialize(tB, pP);
            pieces[21].Initialize(tB, pP);
            pieces[22].Initialize(tB, pP);
            pieces[23].Initialize(tB, pP);
            pieces[24].Initialize(tB, pR);
            pieces[25].Initialize(tB, pN);
            pieces[26].Initialize(tB, pB);
            pieces[27].Initialize(tB, pQ);
            pieces[28].Initialize(tB, pK);
            pieces[29].Initialize(tB, pB);
            pieces[30].Initialize(tB, pN);
            pieces[31].Initialize(tB, pR);
        }

#region Input Signal
        public void SignalSelect(int _posX, int _posY)
        {
            Debug.Assert(_posX >= 0 && _posX < c_BOARD_SIZE_X);
            Debug.Assert(_posY >= 0 && _posY < c_BOARD_SIZE_Y);

            if(inputSelectedPositionX == _posX && inputSelectedPositionY == _posY)
            {
                inputSelectedPositionX = -1;
                inputSelectedPositionY = -1;
            }
            else
            {
                inputSelectedPositionX = _posX;
                inputSelectedPositionY = _posY;
            }
        }

        public void SignalMove(int _posX, int _posY)
        {
            Debug.Assert(_posX >= 0 && _posX < c_BOARD_SIZE_X);
            Debug.Assert(_posY >= 0 && _posY < c_BOARD_SIZE_Y);
            Debug.Assert(inputSelectedPositionX >= 0 && inputSelectedPositionX < c_BOARD_SIZE_X);
            Debug.Assert(inputSelectedPositionY >= 0 && inputSelectedPositionY < c_BOARD_SIZE_Y);
            Debug.Assert(gameBoard[inputSelectedPositionX, inputSelectedPositionY] != null);

            inputMovingPositionX = _posX;
            inputMovingPositionY = _posY;
        }

        public void SignalPromote(PieceType _pieceType)
        {
            inputPromoted = _pieceType;
        }
#endregion

#region Draw Check Map
        public void ClearCheckMap()
        {
            for(int i = 0; i < c_BOARD_SIZE_X; ++i)
            for(int j = 0; j < c_BOARD_SIZE_Y; ++j)
            {
                checkMap[i, j] = false;
            }
        }

        public void DrawCheckMapWhite()
        {
            this.ClearCheckMap();
            this.m_DrawCheckMap(Color.White);
        }

        public void DrawCheckMapBlack()
        {
            this.ClearCheckMap();
            this.m_DrawCheckMap(Color.Black);
        }

        private void m_DrawCheckMap(Color _color)
        {
            for(int i = 0; i < c_BOARD_SIZE_X; ++i)
            {
                for(int j = 0; j < c_BOARD_SIZE_Y; ++j)
                {
                    if(gameBoard[i, j] != null && gameBoard[i, j].color == _color)
                    {
                        m_DrawCheckMap(i, j);
                    }
                }
            }
        }

        private void m_DrawCheckMap(int _posX, int _posY)
        {
            Debug.Assert(gameBoard[_posX, _posY] != null);

            switch(gameBoard[_posX, _posY].pieceType)
            {
                case PieceType.Pawn:
                    m_DrawAsPawn(_posX, _posY);
                    break;
                case PieceType.Bishop:
                    m_DrawAsBishop(_posX, _posY);
                    break;
                case PieceType.Knight:
                    m_DrawAsKnight(_posX, _posY);
                    break;
                case PieceType.Rook:
                    m_DrawAsRook(_posX, _posY);
                    break;
                case PieceType.Queen:
                    m_DrawAsQueen(_posX, _posY);
                    break;
                case PieceType.King:
                    m_DrawAsKing(_posX, _posY);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private void m_DrawAsPawn(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            Piece piece = gameBoard[_posX, _posY];

            Debug.Assert(piece != null && piece.pieceType == PieceType.Pawn, string.Format("({0}, {1})에 폰이 존재하지 않음", _posX, _posY));

            int axis = piece.color == Color.White ? 1 : -1;

            // NOTE: catch
            if (IsValidPosition(_posX + axis, _posY + axis) &&
                !IsEmptyCell(_posX + axis, _posY + axis) &&
                IsEnemyPiece(_posX, _posY, _posX + axis, _posY + axis)
            )
            {
                checkMap[_posX + axis, _posY + axis] = true;
            }
            if (IsValidPosition(_posX - axis, _posY + axis) &&
                !IsEmptyCell(_posX - axis, _posY + axis) &&
                IsEnemyPiece(_posX, _posY, _posX - axis, _posY + axis)
            )
            {
                checkMap[_posX - axis, _posY + axis] = true;
            }

            // NOTE: en passant
            if (IsValidPosition(_posX + axis, _posY) &&
                !IsEmptyCell(_posX + axis, _posY) &&
                gameBoard[_posX + axis, _posY] == inputPawnSpecialStarted
            )
            {
                checkMap[_posX + axis, _posY] = true;
            }
            if (IsValidPosition(_posX - axis, _posY) &&
                !IsEmptyCell(_posX - axis, _posY) &&
                gameBoard[_posX - axis, _posY] == inputPawnSpecialStarted
            )
            {
                checkMap[_posX - axis, _posY] = true;
            }
        }

        private void m_DrawAsBishop(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            Piece piece = gameBoard[_posX, _posY];

            Debug.Assert(piece != null && piece.pieceType == PieceType.Bishop, string.Format("({0}, {1})에 비숍이 존재하지 않음", _posX, _posY));

            m_DrawAsLinear(_posX, _posY, 1, 1);
            m_DrawAsLinear(_posX, _posY, 1, -1);
            m_DrawAsLinear(_posX, _posY, -1, 1);
            m_DrawAsLinear(_posX, _posY, -1, -1);
        }

        private void m_DrawAsKnight(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            Piece piece = gameBoard[_posX, _posY];

            Debug.Assert(piece != null && piece.pieceType == PieceType.Knight, string.Format("({0}, {1})에 나이트가 존재하지 않음", _posX, _posY));

            m_DrawAsCell(_posX + 2, _posY + 1);
            m_DrawAsCell(_posX + 1, _posY + 2);
            m_DrawAsCell(_posX - 1, _posY + 2);
            m_DrawAsCell(_posX - 2, _posY + 1);
            m_DrawAsCell(_posX + 1, _posY - 2);
            m_DrawAsCell(_posX + 2, _posY - 1);
            m_DrawAsCell(_posX - 1, _posY - 2);
            m_DrawAsCell(_posX - 2, _posY - 1);
        }

        private void m_DrawAsRook(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            Piece piece = gameBoard[_posX, _posY];

            Debug.Assert(piece != null && piece.pieceType == PieceType.Rook, string.Format("({0}, {1})에 룩이 존재하지 않음", _posX, _posY));

            m_DrawAsLinear(_posX, _posY, 1, 0);
            m_DrawAsLinear(_posX, _posY, 0, 1);
            m_DrawAsLinear(_posX, _posY, -1, 0);
            m_DrawAsLinear(_posX, _posY, 0, -1);
        }

        private void m_DrawAsQueen(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            Piece piece = gameBoard[_posX, _posY];

            Debug.Assert(piece != null && piece.pieceType == PieceType.Queen, string.Format("({0}, {1})에 퀸이 존재하지 않음", _posX, _posY));

            m_DrawAsLinear(_posX, _posY, 1, 1);
            m_DrawAsLinear(_posX, _posY, 1, -1);
            m_DrawAsLinear(_posX, _posY, -1, 1);
            m_DrawAsLinear(_posX, _posY, -1, -1);
            m_DrawAsLinear(_posX, _posY, 1, 0);
            m_DrawAsLinear(_posX, _posY, 0, 1);
            m_DrawAsLinear(_posX, _posY, -1, 0);
            m_DrawAsLinear(_posX, _posY, 0, -1);
        }

        private void m_DrawAsKing(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            Piece piece = gameBoard[_posX, _posY];

            Debug.Assert(piece != null && piece.pieceType == PieceType.King, string.Format("({0}, {1})에 킹이 존재하지 않음", _posX, _posY));

            m_DrawAsCell(_posX + 1, _posY + 1);
            m_DrawAsCell(_posX + 1, _posY);
            m_DrawAsCell(_posX + 1, _posY - 1);
            m_DrawAsCell(_posX, _posY + 1);
            m_DrawAsCell(_posX, _posY - 1);
            m_DrawAsCell(_posX - 1, _posY + 1);
            m_DrawAsCell(_posX - 1, _posY);
            m_DrawAsCell(_posX - 1, _posY - 1);
        }

        private void m_DrawAsLinear(int _srcPosX, int _srcPosY, int _deltaX, int _deltaY)
        {
            Debug.Assert(Math.Abs(_deltaX) <= 1 && Math.Abs(_deltaY) <= 1);

            int px = _srcPosX + _deltaX;
            int py = _srcPosY + _deltaY;

            while(IsValidPosition(px, py))
            {
                checkMap[px, py] = true;
                px += _deltaX;
                py += _deltaY;
            }
        }

        private void m_DrawAsCell(int _posX, int _posY)
        {
            if(!IsValidPosition(_posX, _posY))
                return;

            checkMap[_posX, _posY] = true;
        }
#endregion

#region Piece Actions
        private void m_Move(int _srcPosX, int _srcPosY, int _dstPosX, int _dstPosY)
        {
            Debug.Assert(gameBoard[_srcPosX, _srcPosY] != null);
            Debug.Assert(gameBoard[_dstPosX, _dstPosY] == null);

            gameBoard[_dstPosX, _dstPosY] = gameBoard[_srcPosX, _srcPosY];
            gameBoard[_srcPosX, _srcPosY] = null;
        }

        private void m_Catch(int _posX, int _posY)
        {
            Debug.Assert(gameBoard[_posX, _posY] != null);

            gameBoard[_posX, _posY].isCatched = true;
            gameBoard[_posX, _posY] = null;
        }

#endregion
/*
        public void Update()
        {
            inputIdxSelectedPiece = -1;
            inputIdxMovement = -1;
            inputPromotedPieceType = -1;

            for(int i = 0; i < pieces.Length; ++i)
            {
                if(i == 4 || i == 28)
                    continue;

                pieces[i].RecognizeEnvironment(this);
            }

            // NOTE: K의 자충수 유무를 판단하기 위해서는 다른 기물의 움직임 계산이 선행되어야 합니다.
            pieces[4].RecognizeEnvironment(this);
            pieces[28].RecognizeEnvironment(this);

            // TODO: Check whether gameover state or not.
            // gameover state == checkmate, stalemate, special draw cases.
            // special draw cases == K vs K, K+N vs K, K+B vs K, K+N vs K+N, K+N vs K+B, K+B vs K+B (6가지 경우)

            // TODO: Player's input here.
            // while(inputIdxSelectedPiece < 0 || inputIdxMovement < 0);

            pieces[inputIdxSelectedPiece].Move(this, inputIdxMovement);
        }
*/
/*
        private void Move(int _idxPiece, int _idxMovement)
        {
            Movement movement = pieces[_idxPiece].AvailableMovements[_idxMovement];

            int idxTemp = gameBoard[movement.nextPosition.x, movement.nextPosition.y];
            gameBoard[movement.nextPosition.x, movement.nextPosition.y] = gameBoard[pieces[_idxPiece].position.x, pieces[_idxPiece].position.y];
            gameBoard[pieces[_idxPiece].position.x, pieces[_idxPiece].position.y] = idxTemp;

            if((movement.movementType | MovementType.Catch) != 0)
            {
                gameBoard[pieces[_idxPiece].position.x, pieces[_idxPiece].position.y] = -1;
            }
            if((movement.movementType | MovementType.Promotion) != 0)
            {
                
            }
            if((movement.movementType | MovementType.EnPassant) != 0)
            {
                
            }
            if((movement.movementType | MovementType.CastlingK) != 0)
            {
                
            }
            if((movement.movementType | MovementType.CastlingQ) != 0)
            {
                
            }

            pieces[_idxPiece].position.x = movement.nextPosition.x;
            pieces[_idxPiece].position.y = movement.nextPosition.y;
        }
*/
#region Conditions

        public bool IsValidPosition(int _posX, int _posY)
        {
            return !(_posX < 0 || _posX >= c_BOARD_SIZE_X || _posY < 0 || _posY >= c_BOARD_SIZE_Y);
        }

        public bool IsEnemyPiece(int _srcPosX, int _srcPosY, int _dstPosX, int _dstPosY)
        {
            Debug.Assert(_srcPosX != _dstPosX || _srcPosY != _dstPosY, "같은 위치는 비교할 수 없음");
            Debug.Assert(IsValidPosition(_srcPosX, _srcPosY), "소스 좌표가 올바르지 않음");
            Debug.Assert(IsValidPosition(_dstPosX, _dstPosY), "목적지 좌표가 올바르지 않음");
            Debug.Assert(gameBoard[_srcPosX, _srcPosY] != null, "빈 셀에 대해 비교할 수 없음");
            Debug.Assert(gameBoard[_dstPosX, _dstPosY] != null, "빈 셀에 대해 비교할 수 없음");

            return gameBoard[_srcPosX, _srcPosY].color != gameBoard[_dstPosX, _dstPosY].color;
        }

        public bool IsAllyPiece(int _srcPosX, int _srcPosY, int _dstPosX, int _dstPosY)
        {
            Debug.Assert(_srcPosX != _dstPosX || _srcPosY != _dstPosY, "같은 위치는 비교할 수 없음");
            Debug.Assert(IsValidPosition(_srcPosX, _srcPosY), "소스 좌표가 올바르지 않음");
            Debug.Assert(IsValidPosition(_dstPosX, _dstPosY), "목적지 좌표가 올바르지 않음");
            Debug.Assert(gameBoard[_srcPosX, _srcPosY] != null, "빈 셀에 대해 비교할 수 없음");
            Debug.Assert(gameBoard[_dstPosX, _dstPosY] != null, "빈 셀에 대해 비교할 수 없음");

            return gameBoard[_srcPosX, _srcPosY].color == gameBoard[_dstPosX, _dstPosY].color;
        }

        public bool IsEmptyCell(int _posX, int _posY)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");

            return gameBoard[_posX, _posY] == null;
        }

        public bool IsPawnCell(int _posX, int _posY)
        {
            return IsPieceType(_posX, _posY, PieceType.Pawn);
        }

        public bool IsBishopCell(int _posX, int _posY)
        {
            return IsPieceType(_posX, _posY, PieceType.Bishop);
        }

        public bool IsKnightCell(int _posX, int _posY)
        {
            return IsPieceType(_posX, _posY, PieceType.Knight);
        }

        public bool IsRookCell(int _posX, int _posY)
        {
            return IsPieceType(_posX, _posY, PieceType.Rook);
        }

        public bool IsQueenCell(int _posX, int _posY)
        {
            return IsPieceType(_posX, _posY, PieceType.Queen);
        }

        public bool IsKingCell(int _posX, int _posY)
        {
            return IsPieceType(_posX, _posY, PieceType.King);
        }

        public bool IsPieceType(int _posX, int _posY, PieceType _pieceType)
        {
            Debug.Assert(IsValidPosition(_posX, _posY), "좌표가 올바르지 않음");
            Debug.Assert(_pieceType != PieceType.None, "정의되지 않은 기물 유형");
            Debug.Assert(!IsEmptyCell(_posX, _posY), string.Format("({0}, {1})에 기물이 존재하지 않음", _posX, _posY));

            return gameBoard[_posX, _posY].pieceType == _pieceType;
        }
#endregion
/*
        public void Catch(Cell _targetPosition)
        {
            Debug.Assert(gameBoard[_targetPosition.x, _targetPosition.y] >= 0);

            int idxTagetPiece = gameBoard[_targetPosition.x, _targetPosition.y];

            gameBoard[_targetPosition.x, _targetPosition.y] = -1;
            pieces[idxTagetPiece].isCatched = true;
        }

        public void Swap(Cell _position0, Cell _position1)
        {
            int tmp = gameBoard[_position0.x, _position0.y];
            gameBoard[_position0.x, _position0.y] = gameBoard[_position1.x, _position1.y];
            gameBoard[_position1.x, _position1.y] = tmp;
        }

        public void Promote(int _idxPiece, PieceType _pieceType)
        {
            Debug.Assert(_pieceType != PieceType.Pawn);
            Debug.Assert(_pieceType != PieceType.King);

            pieces[_idxPiece].pieceType = _pieceType;
        }

        public void CastlingK(int _idxKingPiece, int _idxRookPiece)
        {
            Cell pKing = pieces[_idxKingPiece].position;
            Cell pRook = pieces[_idxRookPiece].position;

            Swap(pKing, new Cell(pKing.x + 2, pKing.y));
            Swap(pRook, new Cell(pKing.x + 1, pKing.y));
        }

        public void CastlingQ(int _idxKingPiece, int _idxRookPiece)
        {
            Cell pKing = pieces[_idxKingPiece].position;
            Cell pRook = pieces[_idxRookPiece].position;

            Swap(pKing, new Cell(pKing.x + 2, pKing.y));
            Swap(pRook, new Cell(pKing.x + 1, pKing.y));
        }

        public void EnPassant(int _idxAllyPawnPiece, int _idxEnemyPawnPiece)
        {
            int axis = pieces[_idxAllyPawnPiece].team == Color.White ? 1 : -1;

            Cell pAlly = pieces[_idxAllyPawnPiece].position;
            Cell pEnemy = pieces[_idxEnemyPawnPiece].position;

            Catch(pEnemy);
            Swap(pAlly, new Cell(pEnemy.x, pAlly.y + axis));
        }
*/
    }
}