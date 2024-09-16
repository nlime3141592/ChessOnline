namespace nl.ChessOnline
{
    public class ChessManager
    {
        public int[,] gameBoard;
        public Piece[] pieces;
        public int turnNumber;
        public bool isGameStarted;

#region Input Register
        public int inputIdxSelectedPiece = -1;
        public int inputIdxMovement = -1;

        public int inputPromotedPieceType = -1;
#endregion

        public ChessManager()
        {
            gameBoard = new int[8, 8];
            pieces = new Piece[32];

            this.Initialize();
        }

        public void Initialize()
        {
            TeamType tW = TeamType.White;
            TeamType tB = TeamType.Black;

            PieceType pP = PieceType.Pawn;
            PieceType pB = PieceType.Bishop;
            PieceType pN = PieceType.Knight;
            PieceType pR = PieceType.Rook;
            PieceType pQ = PieceType.Queen;
            PieceType pK = PieceType.King;

            turnNumber = 0;
            isGameStarted = false;

            for(int i = 0; i < 8; ++i)
            {
                gameBoard[0, i] = i;
                gameBoard[1, i] = i + 8;
                gameBoard[2, i] = -1;
                gameBoard[3, i] = -1;
                gameBoard[4, i] = -1;
                gameBoard[5, i] = -1;
                gameBoard[6, i] = i + 16;
                gameBoard[7, i] = i + 24;
            }

            // NOTE: White Team Pieces, 0 to 15.
            pieces[0].Initialize(tW, pR, 0, 0);
            pieces[1].Initialize(tW, pN, 1, 0);
            pieces[2].Initialize(tW, pB, 2, 0);
            pieces[3].Initialize(tW, pQ, 3, 0);
            pieces[4].Initialize(tW, pK, 4, 0);
            pieces[5].Initialize(tW, pB, 5, 0);
            pieces[6].Initialize(tW, pN, 6, 0);
            pieces[7].Initialize(tW, pR, 7, 0);
            pieces[8].Initialize(tW, pP, 0, 1);
            pieces[9].Initialize(tW, pP, 1, 1);
            pieces[10].Initialize(tW, pP, 2, 1);
            pieces[11].Initialize(tW, pP, 3, 1);
            pieces[12].Initialize(tW, pP, 4, 1);
            pieces[13].Initialize(tW, pP, 5, 1);
            pieces[14].Initialize(tW, pP, 6, 1);
            pieces[15].Initialize(tW, pP, 7, 1);

            // NOTE: Black Team Pieces, 16 to 31.
            pieces[16].Initialize(tB, pP, 0, 6);
            pieces[17].Initialize(tB, pP, 1, 6);
            pieces[18].Initialize(tB, pP, 2, 6);
            pieces[19].Initialize(tB, pP, 3, 6);
            pieces[20].Initialize(tB, pP, 4, 6);
            pieces[21].Initialize(tB, pP, 5, 6);
            pieces[22].Initialize(tB, pP, 6, 6);
            pieces[23].Initialize(tB, pP, 7, 6);
            pieces[24].Initialize(tB, pR, 0, 7);
            pieces[25].Initialize(tB, pN, 1, 7);
            pieces[26].Initialize(tB, pB, 2, 7);
            pieces[27].Initialize(tB, pQ, 3, 7);
            pieces[28].Initialize(tB, pK, 4, 7);
            pieces[29].Initialize(tB, pB, 5, 7);
            pieces[30].Initialize(tB, pN, 6, 7);
            pieces[31].Initialize(tB, pR, 7, 7);
        }

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

        public bool IsEnemyPiece(int _originRow, int _originCol, int _targetRow, int _targetCol)
        {
            return (gameBoard[_originRow, _originCol] + gameBoard[_targetRow, _targetCol]) % 16 == 1;
        }

        public bool IsEnemyPiece(Cell _originPosition, Cell _targetPosition)
        {
            int idxOrigin = gameBoard[_originPosition.x, _originPosition.y];
            int idxTarget = gameBoard[_targetPosition.x, _targetPosition.y];

            if(idxOrigin < 0 || idxTarget < 0)
                return false;

            TeamType tOrigin = pieces[idxOrigin].team;
            TeamType tTarget = pieces[idxTarget].team;

            return tOrigin != tTarget;
        }

        public bool IsAllyPiece(Cell _originPosition, Cell _targetPosition)
        {
            int idxOrigin = gameBoard[_originPosition.x, _originPosition.y];
            int idxTarget = gameBoard[_targetPosition.x, _targetPosition.y];

            if(idxOrigin < 0 || idxTarget < 0)
                return false;

            TeamType tOrigin = pieces[idxOrigin].team;
            TeamType tTarget = pieces[idxTarget].team;

            return tOrigin == tTarget;
        }

        public bool IsEmptyCell(int _targetRow, int _targetCol)
        {
            return gameBoard[_targetRow, _targetCol] == -1;
        }

        public bool IsValidPosition(Cell _position)
        {
            return this.IsValidPosition(_position.x, _position.y);
        }

        public bool IsValidPosition(int _row, int _col)
        {
            return !(_row < 0 || _col < 0 || _row >= 8 || _col >= 8);
        }
    }
}