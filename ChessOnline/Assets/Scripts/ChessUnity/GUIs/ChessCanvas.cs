using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nl.ChessOnline.Unity
{
    public class ChessCanvas : MonoBehaviour
    {
        public bool bShowPosition = false;
        private RectTransform m_cellContainer;
        private RectTransform m_cellDotContainer;

        private Text m_txtGameState;

        private Cell m_selectedCell = null;
        private Dictionary<IntVector2, Cell> m_cellTable;
        private Dictionary<IntVector2, int> m_nextActionIndexTable;

        private Sprite[] m_resPieces;

        private Sprite m_resPawnBlack;
        private Sprite m_resPawnBishop;
        private Sprite m_resPawnKnight;
        private Sprite m_resPawnRook;
        private Sprite m_resPawnQueen;
        private Sprite m_resPawnKing;

        private void Awake()
        {
            m_cellContainer = transform.Find("Cells").GetComponent<RectTransform>();
            m_cellDotContainer = transform.Find("CellDots").GetComponent<RectTransform>();
            m_txtGameState = transform.Find("GameState").GetComponent<Text>();
            m_cellTable = new Dictionary<IntVector2, Cell>(64);
            m_nextActionIndexTable = new Dictionary<IntVector2, int>(28);

            m_resPieces = new Sprite[12];

            m_resPieces[0] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Pawn-White");
            m_resPieces[1] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Bishop-White");
            m_resPieces[2] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Knight-White");
            m_resPieces[3] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Rook-White");
            m_resPieces[4] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Queen-White");
            m_resPieces[5] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/King-White");
            m_resPieces[6] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Pawn-Black");
            m_resPieces[7] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Bishop-Black");
            m_resPieces[8] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Knight-Black");
            m_resPieces[9] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Rook-Black");
            m_resPieces[10] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/Queen-Black");
            m_resPieces[11] = Resources.Load<Sprite>("Sprites/ChessPiece-00001/King-Black");

            Cell resCellWhite = Resources.Load<Cell>("Prefabs/GUIs/CellWhite");
            Cell resCellBlack = Resources.Load<Cell>("Prefabs/GUIs/CellBlack");
            GameObject resCellDot = Resources.Load<GameObject>("Prefabs/GUIs/CellDot");

            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    Cell instCell = null;
                    
                    if((x + y) % 2 == 0)
                        instCell = GameObject.Instantiate<Cell>(resCellBlack);
                    else
                        instCell = GameObject.Instantiate<Cell>(resCellWhite);

                    instCell.position = new IntVector2(x, y);
                    instCell.txtPosition.text = string.Format("({0},{1})", x, y);
                    instCell.name = string.Format("Cell {0}", instCell.txtPosition.text);
                    instCell.imgPiece.sprite = null;
                    instCell.transform.SetParent(m_cellContainer, false);

                    instCell.onCellClicked += m_OnCellClicked;

                    m_cellTable.Add(instCell.position, instCell);
                }
            }

            for (int y = 0; y < 9; ++y)
            {
                for (int x = 0; x < 9; ++x)
                {
                    GameObject instCellDot = GameObject.Instantiate<GameObject>(resCellDot);
                    instCellDot.name = string.Format("CellDot ({0},{1})", x, y);
                    instCellDot.transform.SetParent(m_cellDotContainer, false);
                }
            }
        }

        private void Start()
        {
            m_UpdatePiece();
        }

        private void Update()
        {
            m_txtGameState.text = GameManager.Instance.gameState.ToString();

            foreach (Cell cell in m_cellTable.Values)
            {
                cell.txtPosition.gameObject.SetActive(bShowPosition);
            }
        }

        private void m_OnCellClicked(Cell _clickedCell)
        {
            ChessManager chessManager = GameManager.Instance.chessManager;
            int px = _clickedCell.position.x;
            int py = _clickedCell.position.y;

            if (m_selectedCell == null) // NOTE: 새로 셀을 클릭했으므로 이동 경로 보여주기
            {
                // NOTE: 올바르지 않은 셀을 선택한 경우 동작 무시
                if (chessManager.chessBoard[px, py].piece == null)
                    return;
                else if ((chessManager.currentTurnNumber % 2) != (int)chessManager.chessBoard[px, py].piece.color)
                    return;
                Debug.Log("Selected");
                m_selectedCell = _clickedCell;
                m_nextActionIndexTable.Clear();

                for(int i = 0; i < chessManager.chessBoard[px, py].piece.nextActions.Count; ++i)
                    m_nextActionIndexTable.Add(chessManager.chessBoard[px, py].piece.nextActions[i].posRepresentative, i);
            }
            else if (m_selectedCell == this) // NOTE: 같은 셀을 다시 눌렀으므로 이동 경로 지우기
            {
                Debug.Log("Deselected");
                m_selectedCell = null;
                m_nextActionIndexTable.Clear();
            }
            else // NOTE: 이동 가능한 셀이면 이동, 그렇지 않으면 아무 동작도 하지 않음
            {
                IntVector2 posClicked = _clickedCell.position;

                if (m_nextActionIndexTable.ContainsKey(_clickedCell.position))
                {
                    Debug.Log("Moved");
                    chessManager.Move(m_selectedCell.position.x, m_selectedCell.position.y, m_nextActionIndexTable[posClicked]);
                    chessManager.UpdateGameState();
                    m_UpdatePiece();
                }

                m_selectedCell = null;
                m_nextActionIndexTable.Clear();
            }
        }

        private void m_UpdatePiece()
        {
            ChessManager chessManager = GameManager.Instance.chessManager;
            IntVector2 position = new IntVector2(0, 0);

            for (position.y = 0; position.y < 8; ++position.y)
            {
                for (position.x = 0; position.x < 8; ++position.x)
                {
                    Piece piece = chessManager.chessBoard[position.x, position.y].piece;

                    if (piece == null)
                        m_cellTable[position].imgPiece.sprite = null;
                    else
                    {
                        int ptrBase = (int)piece.color;
                        int ptrOffset = (int)piece.pieceType;

                        m_cellTable[position].imgPiece.sprite = m_resPieces[ptrBase * 6 + ptrOffset];
                    }
                }
            }
        }
    }
}