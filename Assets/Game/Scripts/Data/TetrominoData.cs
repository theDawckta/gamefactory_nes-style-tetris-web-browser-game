using System.Collections.Generic;

namespace Tetris.Data
{
    public enum PieceType
    {
        I = 0,
        O = 1,
        T = 2,
        S = 3,
        Z = 4,
        J = 5,
        L = 6
    }

    public struct RotationState
    {
        public bool[,] Grid { get; private set; }

        public RotationState(bool[,] grid)
        {
            Grid = grid;
        }
    }

    public struct PieceData
    {
        public PieceType Type { get; private set; }
        public RotationState[] RotationTable { get; private set; }
        public int ColorIndex { get; private set; }
        public int SpawnColumn { get; private set; }
        public int SpawnRow { get; private set; }

        public PieceData(PieceType type, RotationState[] rotationTable, int colorIndex)
        {
            Type = type;
            RotationTable = rotationTable;
            ColorIndex = colorIndex;
            SpawnColumn = 3;
            SpawnRow = 0;
        }

        public RotationState GetRotationState(int rotationIndex)
        {
            return RotationTable[rotationIndex % RotationTable.Length];
        }
    }

    public static class TetrominoData
    {
        private static readonly Dictionary<PieceType, PieceData> PieceDataMap = new();

        static TetrominoData()
        {
            InitializePieces();
        }

        private static void InitializePieces()
        {
            // I Piece (Cyan) - Color Index 1
            PieceDataMap[PieceType.I] = new PieceData(
                PieceType.I,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  true,  true  },
                        { false, false, false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, true,  false },
                        { false, false, true,  false },
                        { false, false, true,  false },
                        { false, false, true,  false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  true,  true  },
                        { false, false, false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, true,  false },
                        { false, false, true,  false },
                        { false, false, true,  false },
                        { false, false, true,  false }
                    })
                },
                1
            );

            // O Piece (Yellow) - Color Index 2
            PieceDataMap[PieceType.O] = new PieceData(
                PieceType.O,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { false, true,  true,  false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { false, true,  true,  false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { false, true,  true,  false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { false, true,  true,  false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    })
                },
                2
            );

            // T Piece (Purple) - Color Index 3
            PieceDataMap[PieceType.T] = new PieceData(
                PieceType.T,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  true,  false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { true,  true,  false, false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { true,  true,  true,  false },
                        { false, false, false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { false, true,  true,  false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    })
                },
                3
            );

            // S Piece (Green) - Color Index 4
            PieceDataMap[PieceType.S] = new PieceData(
                PieceType.S,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { false, true,  true,  false },
                        { true,  true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { false, true,  true,  false },
                        { false, false, true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { false, true,  true,  false },
                        { true,  true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { false, true,  true,  false },
                        { false, false, true,  false },
                        { false, false, false, false }
                    })
                },
                4
            );

            // Z Piece (Red) - Color Index 5
            PieceDataMap[PieceType.Z] = new PieceData(
                PieceType.Z,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  false, false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, true,  false },
                        { false, true,  true,  false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  false, false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, true,  false },
                        { false, true,  true,  false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    })
                },
                5
            );

            // J Piece (Blue) - Color Index 6
            PieceDataMap[PieceType.J] = new PieceData(
                PieceType.J,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  true,  false },
                        { false, false, true,  false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { false, true,  false, false },
                        { true,  true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { true,  false, false, false },
                        { true,  true,  true,  false },
                        { false, false, false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  true,  false },
                        { false, true,  false, false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    })
                },
                6
            );

            // L Piece (Orange) - Color Index 7
            PieceDataMap[PieceType.L] = new PieceData(
                PieceType.L,
                new[]
                {
                    new RotationState(new[,]
                    {
                        { false, false, false, false },
                        { true,  true,  true,  false },
                        { true,  false, false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { true,  true,  false, false },
                        { false, true,  false, false },
                        { false, true,  false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, false, true,  false },
                        { true,  true,  true,  false },
                        { false, false, false, false },
                        { false, false, false, false }
                    }),
                    new RotationState(new[,]
                    {
                        { false, true,  false, false },
                        { false, true,  false, false },
                        { false, true,  true,  false },
                        { false, false, false, false }
                    })
                },
                7
            );
        }

        public static PieceData GetPieceData(PieceType type)
        {
            return PieceDataMap[type];
        }
    }
}
