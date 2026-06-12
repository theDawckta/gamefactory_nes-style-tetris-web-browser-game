using UnityEngine;

namespace Game.Gameplay
{
    public class PlayfieldRenderer : MonoBehaviour
    {
        private const int GridWidth = 10;
        private const int GridHeight = 20;
        private const float CellSize = 1f;

        [SerializeField]
        private Vector3 _gridOrigin = Vector3.zero;

        [SerializeField]
        private Sprite[] _cellSprites = new Sprite[7];

        [SerializeField]
        private Sprite _emptyCellSprite;

        [SerializeField]
        private Sprite _borderSprite;

        [SerializeField]
        private Color[] _cellColors = new Color[7]
        {
            new Color(0, 1, 1, 1),       // 0: Cyan (I)
            new Color(1, 1, 0, 1),       // 1: Yellow (O)
            new Color(1, 0, 1, 1),       // 2: Purple (T)
            new Color(0, 1, 0, 1),       // 3: Green (S)
            new Color(1, 0, 0, 1),       // 4: Red (Z)
            new Color(0, 0, 1, 1),       // 5: Blue (J)
            new Color(1, 0.5f, 0, 1)     // 6: Orange (L)
        };

        private SpriteRenderer[,] _gridRenderers = new SpriteRenderer[GridWidth, GridHeight];
        private SpriteRenderer[,] _activePieceRenderers = new SpriteRenderer[GridWidth, GridHeight];
        private SpriteRenderer _borderRenderer;

        private Sprite _fallbackSprite;
        private bool _initialized;

        private void OnEnable()
        {
            EnsureInitialized();
        }

        private Sprite GetFallbackSprite()
        {
            if (_fallbackSprite != null)
                return _fallbackSprite;

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _fallbackSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            return _fallbackSprite;
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            InitializeGrid();
            _initialized = true;
        }

        private void InitializeGrid()
        {
            // Create grid cell renderers
            Transform gridParent = new GameObject("Grid").transform;
            gridParent.SetParent(transform);
            gridParent.localPosition = Vector3.zero;

            for (int row = 0; row < GridHeight; row++)
            {
                for (int col = 0; col < GridWidth; col++)
                {
                    GameObject cellObj = new GameObject($"Cell_{col}_{row}");
                    cellObj.transform.SetParent(gridParent);
                    cellObj.transform.localPosition = _gridOrigin + new Vector3(col * CellSize, (GridHeight - 1 - row) * CellSize, 0);

                    SpriteRenderer spriteRenderer = cellObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _emptyCellSprite;
                    spriteRenderer.color = Color.white;
                    spriteRenderer.sortingOrder = 0;

                    _gridRenderers[col, row] = spriteRenderer;
                }
            }

            // Create active piece renderers
            Transform activePieceParent = new GameObject("ActivePiece").transform;
            activePieceParent.SetParent(transform);
            activePieceParent.localPosition = Vector3.zero;

            for (int row = 0; row < GridHeight; row++)
            {
                for (int col = 0; col < GridWidth; col++)
                {
                    GameObject cellObj = new GameObject($"ActiveCell_{col}_{row}");
                    cellObj.transform.SetParent(activePieceParent);
                    cellObj.transform.localPosition = _gridOrigin + new Vector3(col * CellSize, (GridHeight - 1 - row) * CellSize, 0);

                    SpriteRenderer spriteRenderer = cellObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = null;
                    spriteRenderer.color = Color.white;
                    spriteRenderer.sortingOrder = 1;

                    _activePieceRenderers[col, row] = spriteRenderer;
                }
            }

            // Create border renderer
            GameObject borderObj = new GameObject("Border");
            borderObj.transform.SetParent(transform);
            borderObj.transform.localPosition = Vector3.zero;

            _borderRenderer = borderObj.AddComponent<SpriteRenderer>();
            _borderRenderer.sprite = _borderSprite;
            _borderRenderer.color = Color.white;
            _borderRenderer.sortingOrder = -1;

            // Position border at center of grid
            float gridCenterX = _gridOrigin.x + (GridWidth * CellSize) / 2f - CellSize / 2f;
            float gridCenterY = _gridOrigin.y + (GridHeight * CellSize) / 2f - CellSize / 2f;
            borderObj.transform.localPosition = new Vector3(gridCenterX, gridCenterY, 0.1f);
        }

        public void RenderGrid(PlayfieldModel model)
        {
            EnsureInitialized();

            for (int row = 0; row < GridHeight; row++)
            {
                for (int col = 0; col < GridWidth; col++)
                {
                    int colorIndex = model.GetCell(col, row);
                    SpriteRenderer renderer = _gridRenderers[col, row];

                    if (colorIndex == 0)
                    {
                        renderer.sprite = _emptyCellSprite;
                        renderer.color = Color.white;
                    }
                    else if (colorIndex > 0 && colorIndex <= 7)
                    {
                        int spriteIndex = colorIndex - 1;
                        Sprite cellSprite = (spriteIndex < _cellSprites.Length && _cellSprites[spriteIndex] != null)
                            ? _cellSprites[spriteIndex]
                            : GetFallbackSprite();
                        renderer.sprite = cellSprite;
                        renderer.color = _cellColors[spriteIndex];
                    }
                }
            }
        }

        public void RenderActivePiece(int[,] rotationState, int pivotCol, int pivotRow, int colorIndex)
        {
            EnsureInitialized();

            // Clear all active piece renderers first
            for (int row = 0; row < GridHeight; row++)
            {
                for (int col = 0; col < GridWidth; col++)
                {
                    _activePieceRenderers[col, row].sprite = null;
                }
            }

            // Render the active piece
            if (colorIndex > 0 && colorIndex <= 7)
            {
                int spriteIndex = colorIndex - 1;
                for (int row = 0; row < rotationState.GetLength(0); row++)
                {
                    for (int col = 0; col < rotationState.GetLength(1); col++)
                    {
                        if (rotationState[row, col] == 0)
                            continue;

                        int gridCol = pivotCol + col;
                        int gridRow = pivotRow + row;

                        if (gridCol >= 0 && gridCol < GridWidth && gridRow >= 0 && gridRow < GridHeight)
                        {
                            SpriteRenderer renderer = _activePieceRenderers[gridCol, gridRow];
                            Sprite cellSprite = (spriteIndex < _cellSprites.Length && _cellSprites[spriteIndex] != null)
                                ? _cellSprites[spriteIndex]
                                : GetFallbackSprite();
                            renderer.sprite = cellSprite;
                            renderer.color = _cellColors[spriteIndex];
                        }
                    }
                }
            }
        }

        public void SetRowColor(int row, Color color)
        {
            EnsureInitialized();

            if (row >= 0 && row < GridHeight)
            {
                for (int col = 0; col < GridWidth; col++)
                {
                    _gridRenderers[col, row].color = color;
                }
            }
        }

        public void ClearActivePiece()
        {
            EnsureInitialized();

            for (int row = 0; row < GridHeight; row++)
            {
                for (int col = 0; col < GridWidth; col++)
                {
                    _activePieceRenderers[col, row].sprite = null;
                }
            }
        }
    }
}
