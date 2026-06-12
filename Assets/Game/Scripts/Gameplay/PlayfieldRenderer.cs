using UnityEngine;

namespace Game.Gameplay
{
    public class PlayfieldRenderer : MonoBehaviour
    {
        private const int GridWidth = 10;
        private const int GridHeight = 20;
        private const float CellSize = 1f;
        private const float CellRenderScale = 0.91f;

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
                    cellObj.transform.localScale = new Vector3(CellRenderScale, CellRenderScale, 1f);

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
                    cellObj.transform.localScale = new Vector3(CellRenderScale, CellRenderScale, 1f);

                    SpriteRenderer spriteRenderer = cellObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = null;
                    spriteRenderer.color = Color.white;
                    spriteRenderer.sortingOrder = 1;

                    _activePieceRenderers[col, row] = spriteRenderer;
                }
            }

            // Create border (4 segments around the playfield)
            const float borderThickness = 0.3f;
            float gridLeft   = _gridOrigin.x - CellSize * 0.5f;
            float gridRight  = _gridOrigin.x + GridWidth  * CellSize - CellSize * 0.5f;
            float gridBottom = _gridOrigin.y - CellSize * 0.5f;
            float gridTop    = _gridOrigin.y + GridHeight * CellSize - CellSize * 0.5f;
            Color borderColor = new Color(0.85f, 0.85f, 0.85f, 1f);

            Transform borderParent = new GameObject("Border").transform;
            borderParent.SetParent(transform);
            borderParent.localPosition = Vector3.zero;

            CreateBorderSegment(borderParent, "Left",
                new Vector3(gridLeft - borderThickness * 0.5f, (gridBottom + gridTop) * 0.5f, 0f),
                new Vector3(borderThickness, GridHeight * CellSize + borderThickness * 2f, 1f), borderColor);
            CreateBorderSegment(borderParent, "Right",
                new Vector3(gridRight + borderThickness * 0.5f, (gridBottom + gridTop) * 0.5f, 0f),
                new Vector3(borderThickness, GridHeight * CellSize + borderThickness * 2f, 1f), borderColor);
            CreateBorderSegment(borderParent, "Bottom",
                new Vector3((gridLeft + gridRight) * 0.5f, gridBottom - borderThickness * 0.5f, 0f),
                new Vector3(GridWidth * CellSize, borderThickness, 1f), borderColor);
            CreateBorderSegment(borderParent, "Top",
                new Vector3((gridLeft + gridRight) * 0.5f, gridTop + borderThickness * 0.5f, 0f),
                new Vector3(GridWidth * CellSize, borderThickness, 1f), borderColor);
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

        private void CreateBorderSegment(Transform parent, string segmentName, Vector3 localPos, Vector3 scale, Color color)
        {
            GameObject obj = new GameObject(segmentName);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = scale;
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = GetFallbackSprite();
            sr.color = color;
            sr.sortingOrder = -1;
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
