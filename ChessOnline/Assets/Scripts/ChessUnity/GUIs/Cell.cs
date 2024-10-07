using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nl.ChessOnline.Unity
{
    public class Cell : MonoBehaviour
    {
        public nl.ChessOnline.Color teamColor;
        public IntVector2 position { get; set; }
        public Text txtPosition { get; private set; }
        public Image imgTile { get; private set; }
        public Image imgPiece { get; private set; }

        public event Action<Cell> onCellClicked;

        private Button m_button;

        private void Awake()
        {
            m_button = GetComponent<Button>();
            imgTile = GetComponent<Image>();
            txtPosition = transform.Find("Position").GetComponent<Text>();
            imgPiece = transform.Find("PieceImage").GetComponent<Image>();

            m_button.onClick.AddListener(m_OnButtonClicked);
        }

        private void Update()
        {
            if (imgPiece.sprite == null)
                imgPiece.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.0f);
            else
                imgPiece.color = UnityEngine.Color.white;
        }

        private void m_OnButtonClicked()
        {
            onCellClicked?.Invoke(this);
        }
    }
}