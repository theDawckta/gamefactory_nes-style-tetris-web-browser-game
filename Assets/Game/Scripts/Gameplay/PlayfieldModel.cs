namespace Game.Gameplay
{
    public class PlayfieldModel
    {
        private const int Width = 10;
        private const int Height = 20;

        private int[,] _grid;

        public PlayfieldModel()
        {
            _grid = new int[Width, Height];
            Reset();
        }

        public bool IsValidPosition(int[,] rotationState, int pivotCol, int pivotRow)
        {
            for (int row = 0; row < rotationState.GetLength(0); row++)
            {
                for (int col = 0; col < rotationState.GetLength(1); col++)
                {
                    if (rotationState[row, col] == 0)
                        continue;

                    int gridCol = pivotCol + col;
                    int gridRow = pivotRow + row;

                    // Check bounds
                    if (gridCol < 0 || gridCol >= Width || gridRow < 0 || gridRow >= Height)
                        return false;

                    // Check collision with locked cells
                    if (_grid[gridCol, gridRow] != 0)
                        return false;
                }
            }

            return true;
        }

        public void LockPiece(int[,] rotationState, int pivotCol, int pivotRow, int colorIndex)
        {
            for (int row = 0; row < rotationState.GetLength(0); row++)
            {
                for (int col = 0; col < rotationState.GetLength(1); col++)
                {
                    if (rotationState[row, col] == 0)
                        continue;

                    int gridCol = pivotCol + col;
                    int gridRow = pivotRow + row;

                    _grid[gridCol, gridRow] = colorIndex;
                }
            }
        }

        public int ClearFullLines()
        {
            int linesCleared = 0;
            int writeRow = Height - 1;

            for (int readRow = Height - 1; readRow >= 0; readRow--)
            {
                if (IsRowFull(readRow))
                {
                    linesCleared++;
                }
                else
                {
                    if (readRow != writeRow)
                    {
                        CopyRow(readRow, writeRow);
                    }
                    writeRow--;
                }
            }

            // Clear remaining rows at the top
            for (int row = writeRow; row >= 0; row--)
            {
                ClearRow(row);
            }

            return linesCleared;
        }

        public int GetCell(int col, int row)
        {
            if (col < 0 || col >= Width || row < 0 || row >= Height)
                return 0;

            return _grid[col, row];
        }

        public void Reset()
        {
            for (int col = 0; col < Width; col++)
            {
                for (int row = 0; row < Height; row++)
                {
                    _grid[col, row] = 0;
                }
            }
        }

        private bool IsRowFull(int row)
        {
            for (int col = 0; col < Width; col++)
            {
                if (_grid[col, row] == 0)
                    return false;
            }
            return true;
        }

        private void CopyRow(int fromRow, int toRow)
        {
            for (int col = 0; col < Width; col++)
            {
                _grid[col, toRow] = _grid[col, fromRow];
            }
        }

        private void ClearRow(int row)
        {
            for (int col = 0; col < Width; col++)
            {
                _grid[col, row] = 0;
            }
        }
    }
}
