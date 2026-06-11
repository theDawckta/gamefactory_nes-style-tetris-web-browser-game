#if UNITY_INCLUDE_TESTS

using NUnit.Framework;
using Game.Gameplay;

namespace Tetris.Tests
{
    public class PlayfieldModelTests
    {
        private PlayfieldModel _playfield;

        [SetUp]
        public void Setup()
        {
            _playfield = new PlayfieldModel();
        }

        [Test]
        public void GetCell_EmptyCell_ReturnsZero()
        {
            Assert.AreEqual(0, _playfield.GetCell(0, 0));
            Assert.AreEqual(0, _playfield.GetCell(9, 19));
            Assert.AreEqual(0, _playfield.GetCell(5, 10));
        }

        [Test]
        public void GetCell_OutOfBounds_ReturnsZero()
        {
            Assert.AreEqual(0, _playfield.GetCell(-1, 0));
            Assert.AreEqual(0, _playfield.GetCell(10, 0));
            Assert.AreEqual(0, _playfield.GetCell(0, -1));
            Assert.AreEqual(0, _playfield.GetCell(0, 20));
        }

        [Test]
        public void Reset_ClearsEntireGrid()
        {
            // Fill a cell
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            _playfield.LockPiece(block, 0, 0, 1);
            Assert.AreEqual(1, _playfield.GetCell(0, 0));

            // Reset
            _playfield.Reset();
            Assert.AreEqual(0, _playfield.GetCell(0, 0));
            Assert.AreEqual(0, _playfield.GetCell(9, 19));
        }

        [Test]
        public void IsValidPosition_EmptyGrid_ReturnsTrueForValidPositions()
        {
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 0 } };

            Assert.IsTrue(_playfield.IsValidPosition(block, 0, 0));
            Assert.IsTrue(_playfield.IsValidPosition(block, 8, 18));
            Assert.IsTrue(_playfield.IsValidPosition(block, 5, 10));
        }

        [Test]
        public void IsValidPosition_OutOfBoundsLeft_ReturnsFalse()
        {
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            Assert.IsFalse(_playfield.IsValidPosition(block, -1, 0));
        }

        [Test]
        public void IsValidPosition_OutOfBoundsRight_ReturnsFalse()
        {
            int[,] block = new int[2, 2] { { 0, 1 }, { 0, 0 } };
            Assert.IsFalse(_playfield.IsValidPosition(block, 9, 0));
        }

        [Test]
        public void IsValidPosition_OutOfBoundsTop_ReturnsFalse()
        {
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            Assert.IsFalse(_playfield.IsValidPosition(block, 0, -1));
        }

        [Test]
        public void IsValidPosition_OutOfBoundsBottom_ReturnsFalse()
        {
            int[,] block = new int[2, 2] { { 0, 0 }, { 1, 0 } };
            Assert.IsFalse(_playfield.IsValidPosition(block, 0, 19));
        }

        [Test]
        public void IsValidPosition_OverlapWithLockedCell_ReturnsFalse()
        {
            // Lock a piece
            int[,] block1 = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            _playfield.LockPiece(block1, 0, 0, 1);

            // Try to place overlapping piece
            int[,] block2 = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            Assert.IsFalse(_playfield.IsValidPosition(block2, 0, 0));
        }

        [Test]
        public void IsValidPosition_AdjacentToLockedCell_ReturnsTrue()
        {
            // Lock a piece at (0, 0)
            int[,] block1 = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            _playfield.LockPiece(block1, 0, 0, 1);

            // Try adjacent position - should be valid
            int[,] block2 = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            Assert.IsTrue(_playfield.IsValidPosition(block2, 2, 0));
        }

        [Test]
        public void LockPiece_WritesColorToGrid()
        {
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 1 } };
            _playfield.LockPiece(block, 0, 0, 5);

            Assert.AreEqual(5, _playfield.GetCell(0, 0));
            Assert.AreEqual(0, _playfield.GetCell(1, 0));
            Assert.AreEqual(0, _playfield.GetCell(0, 1));
            Assert.AreEqual(5, _playfield.GetCell(1, 1));
        }

        [Test]
        public void LockPiece_WithOffset_WritesColorToCorrectCells()
        {
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            _playfield.LockPiece(block, 3, 5, 2);

            Assert.AreEqual(2, _playfield.GetCell(3, 5));
            Assert.AreEqual(0, _playfield.GetCell(4, 5));
            Assert.AreEqual(0, _playfield.GetCell(3, 6));
        }

        [Test]
        public void LockPiece_IgnoresZeroCells()
        {
            int[,] block = new int[3, 3] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 0, 0 } };
            _playfield.LockPiece(block, 0, 0, 3);

            Assert.AreEqual(0, _playfield.GetCell(0, 0));
            Assert.AreEqual(0, _playfield.GetCell(1, 0));
            Assert.AreEqual(3, _playfield.GetCell(1, 1));
            Assert.AreEqual(3, _playfield.GetCell(2, 1));
            Assert.AreEqual(3, _playfield.GetCell(0, 2));
        }

        [Test]
        public void ClearFullLines_NoFullLines_ReturnsZero()
        {
            int[,] block = new int[2, 2] { { 1, 0 }, { 0, 0 } };
            _playfield.LockPiece(block, 0, 0, 1);

            int cleared = _playfield.ClearFullLines();
            Assert.AreEqual(0, cleared);
        }

        [Test]
        public void ClearFullLines_OneFullLineAtBottom_ReturnsOne()
        {
            // Fill the entire bottom row
            int[,] fullRow = new int[1, 10];
            for (int i = 0; i < 10; i++)
            {
                fullRow[0, i] = 1;
            }
            _playfield.LockPiece(fullRow, 0, 19, 1);

            int cleared = _playfield.ClearFullLines();
            Assert.AreEqual(1, cleared);

            // Bottom row should be empty now
            for (int col = 0; col < 10; col++)
            {
                Assert.AreEqual(0, _playfield.GetCell(col, 19));
            }
        }

        [Test]
        public void ClearFullLines_MultipleFullLines_ReturnsCorrectCount()
        {
            // Fill rows 19 and 18
            int[,] row19 = new int[1, 10];
            int[,] row18 = new int[1, 10];
            for (int i = 0; i < 10; i++)
            {
                row19[0, i] = 1;
                row18[0, i] = 2;
            }
            _playfield.LockPiece(row19, 0, 19, 1);
            _playfield.LockPiece(row18, 0, 18, 2);

            int cleared = _playfield.ClearFullLines();
            Assert.AreEqual(2, cleared);

            // Both rows should be empty
            for (int col = 0; col < 10; col++)
            {
                Assert.AreEqual(0, _playfield.GetCell(col, 19));
                Assert.AreEqual(0, _playfield.GetCell(col, 18));
            }
        }

        [Test]
        public void ClearFullLines_ShiftsRowsDown()
        {
            // Fill row 19 (bottom)
            int[,] row19 = new int[1, 10];
            for (int i = 0; i < 10; i++)
            {
                row19[0, i] = 1;
            }
            _playfield.LockPiece(row19, 0, 19, 1);

            // Place a block at row 18
            int[,] block = new int[2, 2] { { 2, 0 }, { 0, 0 } };
            _playfield.LockPiece(block, 0, 18, 2);

            // Before clear
            Assert.AreEqual(2, _playfield.GetCell(0, 18));

            // Clear
            _playfield.ClearFullLines();

            // Row 18 should now have the block that was at row 18 (it moved to row 19)
            Assert.AreEqual(2, _playfield.GetCell(0, 19));
            Assert.AreEqual(0, _playfield.GetCell(0, 18));
        }

        [Test]
        public void ClearFullLines_SkipsPartialRows()
        {
            // Create a nearly full row with one empty cell (fill first 9 columns, leave last empty)
            int[,] almostFullRow = new int[1, 10];
            for (int i = 0; i < 9; i++)
            {
                almostFullRow[0, i] = 1;
            }
            _playfield.LockPiece(almostFullRow, 0, 19, 1);

            // Place another block elsewhere
            int[,] block = new int[2, 2] { { 2, 0 }, { 0, 0 } };
            _playfield.LockPiece(block, 8, 18, 2);

            int cleared = _playfield.ClearFullLines();
            Assert.AreEqual(0, cleared);

            // Blocks should still be there
            for (int col = 0; col < 9; col++)
            {
                Assert.AreEqual(1, _playfield.GetCell(col, 19));
            }
            Assert.AreEqual(2, _playfield.GetCell(8, 18));
        }

        [Test]
        public void ClearFullLines_FourLinesAtOnce_ReturnsFour()
        {
            // Fill rows 19, 18, 17, 16
            for (int row = 16; row <= 19; row++)
            {
                int[,] fullRow = new int[1, 10];
                for (int i = 0; i < 10; i++)
                {
                    fullRow[0, i] = 1;
                }
                _playfield.LockPiece(fullRow, 0, row, 1);
            }

            int cleared = _playfield.ClearFullLines();
            Assert.AreEqual(4, cleared);

            // All four rows should be empty
            for (int row = 16; row <= 19; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Assert.AreEqual(0, _playfield.GetCell(col, row));
                }
            }
        }

        [Test]
        public void ClearFullLines_NonConsecutiveFullRows_ClearsAndShifts()
        {
            // Fill row 19
            int[,] row19 = new int[1, 10];
            for (int i = 0; i < 10; i++)
            {
                row19[0, i] = 1;
            }
            _playfield.LockPiece(row19, 0, 19, 1);

            // Add a partial row at 18
            int[,] partialRow18 = new int[1, 5];
            for (int i = 0; i < 5; i++)
            {
                partialRow18[0, i] = 2;
            }
            _playfield.LockPiece(partialRow18, 0, 18, 2);

            // Fill row 17
            int[,] row17 = new int[1, 10];
            for (int i = 0; i < 10; i++)
            {
                row17[0, i] = 3;
            }
            _playfield.LockPiece(row17, 0, 17, 3);

            int cleared = _playfield.ClearFullLines();
            Assert.AreEqual(2, cleared);

            // Row 18 (partial) should drop to row 19
            for (int col = 0; col < 5; col++)
            {
                Assert.AreEqual(2, _playfield.GetCell(col, 19));
            }

            // Rows 17, 18 should be empty
            for (int col = 0; col < 10; col++)
            {
                Assert.AreEqual(0, _playfield.GetCell(col, 17));
                Assert.AreEqual(0, _playfield.GetCell(col, 18));
            }
        }
    }
}

#endif
