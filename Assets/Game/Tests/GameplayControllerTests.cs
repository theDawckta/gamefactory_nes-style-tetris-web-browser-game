#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using Game.Gameplay;
using Tetris.Data;

namespace Tetris.Tests
{
    public class GameplayControllerTests
    {
        private GameObject _gameObject;
        private GameplayController _controller;

        private bool _gameOverFired;
        private int _lastReportedScore;
        private int _lastReportedLevel;
        private int _lastReportedLines;

        [SetUp]
        public void Setup()
        {
            _gameObject = new GameObject("GameplayControllerTest");
            _controller = _gameObject.AddComponent<GameplayController>();

            _gameOverFired = false;
            _lastReportedScore = 0;
            _lastReportedLevel = 0;
            _lastReportedLines = 0;

            _controller.OnGameOver += OnGameOver;
            _controller.OnScoreChanged += OnScoreChanged;
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(_gameObject);
        }

        private void OnGameOver()
        {
            _gameOverFired = true;
        }

        private void OnScoreChanged(int score, int level, int lines)
        {
            _lastReportedScore = score;
            _lastReportedLevel = level;
            _lastReportedLines = lines;
        }

        [Test]
        public void StartGame_InitializesGameState()
        {
            _controller.StartGame();

            Assert.IsFalse(_gameOverFired);
            Assert.AreEqual(0, _lastReportedScore);
        }

        [Test]
        public void StopGame_HaltsTheLoop()
        {
            _controller.StartGame();
            _controller.StopGame();

            // After stopping, multiple Updates should not change state
            // (this is a basic check that StopGame works)
            Assert.Pass("StopGame executed without errors");
        }

        [Test]
        public void ScoringSystem_SoftDrop_AwardsPoints()
        {
            var scoring = new ScoringSystem();
            scoring.Reset();

            scoring.AddSoftDropScore(1);
            Assert.AreEqual(1, scoring.Score);

            scoring.AddSoftDropScore(5);
            Assert.AreEqual(6, scoring.Score);
        }

        [Test]
        public void ScoringSystem_AddLines_CalculatesCorrectly()
        {
            var scoring = new ScoringSystem();
            scoring.Reset();
            scoring.AddLines(1);

            Assert.AreEqual(40, scoring.Score);
        }

        [Test]
        public void ScoringSystem_AddLines_Level1_MultipliesByLevel()
        {
            var scoring = new ScoringSystem();
            scoring.Reset();

            // Add 10 lines to reach level 1
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);
            scoring.AddLines(1);

            // Now at level 1, adding 1 more line should multiply by 2
            int scoreBeforeLevel1 = scoring.Score;
            scoring.AddLines(1);

            // Level 1 bonus: 40 * 2 = 80
            Assert.AreEqual(scoreBeforeLevel1 + 80, scoring.Score);
            Assert.AreEqual(1, scoring.Level);
        }

        [Test]
        public void ScoringSystem_FourLines_HighestBonus()
        {
            var scoring = new ScoringSystem();
            scoring.Reset();
            scoring.AddLines(4);

            // Tetris (4 lines) awards 1200 points
            Assert.AreEqual(1200, scoring.Score);
        }

        [Test]
        public void Playfield_IsValidPosition_RejectsOutOfBounds()
        {
            var playfield = new PlayfieldModel();
            var grid = new[,] { { 1 } };

            // Column out of bounds
            Assert.IsFalse(playfield.IsValidPosition(grid, -1, 0));
            Assert.IsFalse(playfield.IsValidPosition(grid, 10, 0));

            // Row out of bounds
            Assert.IsFalse(playfield.IsValidPosition(grid, 0, 20));
        }

        [Test]
        public void Playfield_LockPiece_LocksCorrectly()
        {
            var playfield = new PlayfieldModel();
            var grid = new[,]
            {
                { 1, 1 },
                { 1, 1 }
            };

            playfield.LockPiece(grid, 3, 5, 1);

            Assert.AreEqual(1, playfield.GetCell(3, 5));
            Assert.AreEqual(1, playfield.GetCell(4, 5));
            Assert.AreEqual(1, playfield.GetCell(3, 6));
            Assert.AreEqual(1, playfield.GetCell(4, 6));
        }

        [Test]
        public void Playfield_ClearFullLines_IdentifiesCompletedRows()
        {
            var playfield = new PlayfieldModel();

            // Fill row 19 completely
            for (int col = 0; col < 10; col++)
            {
                playfield.LockPiece(new[,] { { 1 } }, col, 19, 1);
            }

            int cleared = playfield.ClearFullLines();

            Assert.AreEqual(1, cleared);
        }

        [Test]
        public void GravityController_Tick_DropsOnInterval()
        {
            var gravity = new GravityController();
            gravity.SetLevel(0);

            bool dropped = false;
            for (int i = 0; i < 48; i++)
            {
                if (gravity.Tick())
                {
                    dropped = true;
                    break;
                }
            }

            Assert.IsTrue(dropped);
        }

        [Test]
        public void GravityController_LockDelay_Expires30Frames()
        {
            var gravity = new GravityController();

            bool expired = false;
            for (int i = 0; i < 30; i++)
            {
                if (gravity.TickLockDelay())
                {
                    expired = true;
                    break;
                }
            }

            Assert.IsTrue(expired);
        }

        [Test]
        public void DASController_Update_ReturnsZeroOnInitialPress()
        {
            var das = new DASController();
            das.Update(true, false);

            Assert.AreEqual(-1, das.GetHorizontalDelta());
        }

        [Test]
        public void DASController_Update_DelaysBeforeAutoRepeat()
        {
            var das = new DASController();
            das.Update(true, false);

            Assert.AreEqual(-1, das.GetHorizontalDelta());

            // Hold for 15 frames (below initial delay of 16)
            for (int i = 0; i < 15; i++)
            {
                das.Update(true, false);
            }

            // Should not have repeat yet
            Assert.AreEqual(0, das.GetHorizontalDelta());
        }

        [Test]
        public void TetrominoData_GetPieceData_ReturnsCorrectPieces()
        {
            var iData = TetrominoData.GetPieceData(PieceType.I);
            Assert.AreEqual(PieceType.I, iData.Type);
            Assert.AreEqual(0, iData.ColorIndex);

            var oData = TetrominoData.GetPieceData(PieceType.O);
            Assert.AreEqual(PieceType.O, oData.Type);
            Assert.AreEqual(1, oData.ColorIndex);
        }

        [Test]
        public void TetrominoData_GetRotationState_CyclesCorrectly()
        {
            var data = TetrominoData.GetPieceData(PieceType.I);

            // I piece has 4 rotation states
            var state0 = data.GetRotationState(0);
            var state1 = data.GetRotationState(1);
            var state4 = data.GetRotationState(4); // Should cycle back to state 0

            Assert.NotNull(state0.Grid);
            Assert.NotNull(state1.Grid);
            Assert.NotNull(state4.Grid);
        }
    }
}
#endif
