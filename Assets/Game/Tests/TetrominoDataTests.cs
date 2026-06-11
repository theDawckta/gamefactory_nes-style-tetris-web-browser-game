using NUnit.Framework;
using Tetris.Data;

namespace Tetris.Tests
{
    public class TetrominoDataTests
    {
        [Test]
        public void GetPieceData_I_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.I);

            Assert.AreEqual(PieceType.I, pieceData.Type);
            Assert.AreEqual(0, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetPieceData_O_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.O);

            Assert.AreEqual(PieceType.O, pieceData.Type);
            Assert.AreEqual(1, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetPieceData_T_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.T);

            Assert.AreEqual(PieceType.T, pieceData.Type);
            Assert.AreEqual(2, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetPieceData_S_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.S);

            Assert.AreEqual(PieceType.S, pieceData.Type);
            Assert.AreEqual(3, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetPieceData_Z_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.Z);

            Assert.AreEqual(PieceType.Z, pieceData.Type);
            Assert.AreEqual(4, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetPieceData_J_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.J);

            Assert.AreEqual(PieceType.J, pieceData.Type);
            Assert.AreEqual(5, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetPieceData_L_ReturnsValidData()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.L);

            Assert.AreEqual(PieceType.L, pieceData.Type);
            Assert.AreEqual(6, pieceData.ColorIndex);
            Assert.AreEqual(3, pieceData.SpawnColumn);
            Assert.AreEqual(0, pieceData.SpawnRow);
            Assert.AreEqual(4, pieceData.RotationTable.Length);
        }

        [Test]
        public void GetRotationState_CyclesThroughFourStates()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.T);

            var state0 = pieceData.GetRotationState(0);
            var state1 = pieceData.GetRotationState(1);
            var state2 = pieceData.GetRotationState(2);
            var state3 = pieceData.GetRotationState(3);
            var state4 = pieceData.GetRotationState(4); // Should wrap to 0

            Assert.NotNull(state0.Grid);
            Assert.NotNull(state1.Grid);
            Assert.NotNull(state2.Grid);
            Assert.NotNull(state3.Grid);
            Assert.AreEqual(state0.Grid, state4.Grid);
        }

        [Test]
        public void I_RotationState0_Horizontal()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.I);
            var grid = pieceData.RotationTable[0].Grid;

            // Horizontal line should be in row 1
            Assert.IsTrue(grid[1, 0]);
            Assert.IsTrue(grid[1, 1]);
            Assert.IsTrue(grid[1, 2]);
            Assert.IsTrue(grid[1, 3]);
        }

        [Test]
        public void I_RotationState1_Vertical()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.I);
            var grid = pieceData.RotationTable[1].Grid;

            // Vertical line should be in column 2
            Assert.IsTrue(grid[0, 2]);
            Assert.IsTrue(grid[1, 2]);
            Assert.IsTrue(grid[2, 2]);
            Assert.IsTrue(grid[3, 2]);
        }

        [Test]
        public void O_AllRotationsIdentical()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.O);

            var grid0 = pieceData.RotationTable[0].Grid;
            var grid1 = pieceData.RotationTable[1].Grid;
            var grid2 = pieceData.RotationTable[2].Grid;
            var grid3 = pieceData.RotationTable[3].Grid;

            // All rotation states should be identical for O piece
            Assert.AreEqual(grid0, grid1);
            Assert.AreEqual(grid0, grid2);
            Assert.AreEqual(grid0, grid3);
        }

        [Test]
        public void T_RotationState0_BaseOrientation()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.T);
            var grid = pieceData.RotationTable[0].Grid;

            // T piece in base orientation: three across bottom, stem up from center
            Assert.IsTrue(grid[1, 0]);
            Assert.IsTrue(grid[1, 1]);
            Assert.IsTrue(grid[1, 2]);
            Assert.IsTrue(grid[2, 1]);
        }

        [Test]
        public void S_RotationState0_Shape()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.S);
            var grid = pieceData.RotationTable[0].Grid;

            // S piece: two on upper-right, two on lower-left
            Assert.IsTrue(grid[1, 1]);
            Assert.IsTrue(grid[1, 2]);
            Assert.IsTrue(grid[2, 0]);
            Assert.IsTrue(grid[2, 1]);
        }

        [Test]
        public void Z_RotationState0_Shape()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.Z);
            var grid = pieceData.RotationTable[0].Grid;

            // Z piece: two on upper-left, two on lower-right
            Assert.IsTrue(grid[1, 0]);
            Assert.IsTrue(grid[1, 1]);
            Assert.IsTrue(grid[2, 1]);
            Assert.IsTrue(grid[2, 2]);
        }

        [Test]
        public void J_RotationState0_Shape()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.J);
            var grid = pieceData.RotationTable[0].Grid;

            // J piece: three across, hook on right
            Assert.IsTrue(grid[1, 0]);
            Assert.IsTrue(grid[1, 1]);
            Assert.IsTrue(grid[1, 2]);
            Assert.IsTrue(grid[2, 2]);
        }

        [Test]
        public void L_RotationState0_Shape()
        {
            var pieceData = TetrominoData.GetPieceData(PieceType.L);
            var grid = pieceData.RotationTable[0].Grid;

            // L piece: three across, hook on left
            Assert.IsTrue(grid[1, 0]);
            Assert.IsTrue(grid[1, 1]);
            Assert.IsTrue(grid[1, 2]);
            Assert.IsTrue(grid[2, 0]);
        }

        [Test]
        public void AllPiecesHaveCorrectColorIndices()
        {
            var expectedColors = new[] { 0, 1, 2, 3, 4, 5, 6 };

            for (int i = 0; i < expectedColors.Length; i++)
            {
                var pieceType = (PieceType)i;
                var pieceData = TetrominoData.GetPieceData(pieceType);
                Assert.AreEqual(expectedColors[i], pieceData.ColorIndex);
            }
        }

        [Test]
        public void AllPiecesSpawnAtCorrectPosition()
        {
            for (int i = 0; i < 7; i++)
            {
                var pieceType = (PieceType)i;
                var pieceData = TetrominoData.GetPieceData(pieceType);

                Assert.AreEqual(3, pieceData.SpawnColumn);
                Assert.AreEqual(0, pieceData.SpawnRow);
            }
        }
    }
}
