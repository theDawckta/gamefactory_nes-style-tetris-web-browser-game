using UnityEngine;
using UnityEngine.UIElements;
using Tetris.Data;

namespace Tetris.UI
{
    public class NextPieceWidget
    {
        private readonly VisualElement[,] _cells = new VisualElement[4, 4];

        private static readonly Color[] PieceColors =
        {
            new Color(0f,    1f,    1f,    1f), // 1: Cyan (I)
            new Color(1f,    1f,    0f,    1f), // 2: Yellow (O)
            new Color(1f,    0f,    1f,    1f), // 3: Purple (T)
            new Color(0f,    1f,    0f,    1f), // 4: Green (S)
            new Color(1f,    0f,    0f,    1f), // 5: Red (Z)
            new Color(0f,    0f,    1f,    1f), // 6: Blue (J)
            new Color(1f,    0.5f,  0f,    1f), // 7: Orange (L)
        };

        public NextPieceWidget(VisualElement rootElement)
        {
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    _cells[row, col] = rootElement?.Q<VisualElement>($"np-{row}-{col}");
        }

        public void UpdatePiece(PieceType type)
        {
            PieceData pieceData = TetrominoData.GetPieceData(type);
            bool[,] grid = pieceData.GetRotationState(0).Grid;
            int colorIndex = pieceData.ColorIndex;
            Color fill = colorIndex >= 1 && colorIndex <= 7 ? PieceColors[colorIndex - 1] : Color.clear;

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (_cells[row, col] == null) continue;
                    _cells[row, col].style.backgroundColor = new StyleColor(grid[row, col] ? fill : Color.clear);
                }
            }
        }
    }
}
