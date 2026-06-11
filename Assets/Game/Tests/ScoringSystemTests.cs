#if UNITY_INCLUDE_TESTS

using NUnit.Framework;
using Game.Gameplay;

namespace Tetris.Tests
{
    public class ScoringSystemTests
    {
        private ScoringSystem _scoring;

        [SetUp]
        public void Setup()
        {
            _scoring = new ScoringSystem();
        }

        [Test]
        public void Constructor_InitializesValuesToZero()
        {
            Assert.AreEqual(0, _scoring.Score);
            Assert.AreEqual(0, _scoring.Level);
            Assert.AreEqual(0, _scoring.TotalLines);
        }

        [Test]
        public void AddLines_Single_Level0_Awards40Points()
        {
            _scoring.AddLines(1);
            Assert.AreEqual(40, _scoring.Score);
            Assert.AreEqual(1, _scoring.TotalLines);
            Assert.AreEqual(0, _scoring.Level);
        }

        [Test]
        public void AddLines_Double_Level0_Awards100Points()
        {
            _scoring.AddLines(2);
            Assert.AreEqual(100, _scoring.Score);
            Assert.AreEqual(2, _scoring.TotalLines);
            Assert.AreEqual(0, _scoring.Level);
        }

        [Test]
        public void AddLines_Triple_Level0_Awards300Points()
        {
            _scoring.AddLines(3);
            Assert.AreEqual(300, _scoring.Score);
            Assert.AreEqual(3, _scoring.TotalLines);
            Assert.AreEqual(0, _scoring.Level);
        }

        [Test]
        public void AddLines_Tetris_Level0_Awards1200Points()
        {
            _scoring.AddLines(4);
            Assert.AreEqual(1200, _scoring.Score);
            Assert.AreEqual(4, _scoring.TotalLines);
            Assert.AreEqual(0, _scoring.Level);
        }

        [Test]
        public void AddLines_Single_Level1_Awards80Points()
        {
            // Add 10 lines to reach level 1
            _scoring.AddLines(4);
            _scoring.AddLines(3);
            _scoring.AddLines(2);
            _scoring.AddLines(1);
            Assert.AreEqual(1, _scoring.Level);

            int scoreBeforeSingle = _scoring.Score;
            _scoring.AddLines(1);
            Assert.AreEqual(scoreBeforeSingle + 80, _scoring.Score);
        }

        [Test]
        public void AddLines_Double_Level1_Awards200Points()
        {
            _scoring.AddLines(4);
            _scoring.AddLines(3);
            _scoring.AddLines(2);
            _scoring.AddLines(1);
            Assert.AreEqual(1, _scoring.Level);

            int scoreBeforeDouble = _scoring.Score;
            _scoring.AddLines(2);
            Assert.AreEqual(scoreBeforeDouble + 200, _scoring.Score);
        }

        [Test]
        public void AddLines_Triple_Level1_Awards600Points()
        {
            _scoring.AddLines(4);
            _scoring.AddLines(3);
            _scoring.AddLines(2);
            _scoring.AddLines(1);
            Assert.AreEqual(1, _scoring.Level);

            int scoreBeforeTriple = _scoring.Score;
            _scoring.AddLines(3);
            Assert.AreEqual(scoreBeforeTriple + 600, _scoring.Score);
        }

        [Test]
        public void AddLines_Tetris_Level1_Awards2400Points()
        {
            _scoring.AddLines(4);
            _scoring.AddLines(3);
            _scoring.AddLines(2);
            _scoring.AddLines(1);
            Assert.AreEqual(1, _scoring.Level);

            int scoreBeforeTetris = _scoring.Score;
            _scoring.AddLines(4);
            Assert.AreEqual(scoreBeforeTetris + 2400, _scoring.Score);
        }

        [Test]
        public void AddLines_LevelAdvancement_IncreasesEvery10Lines()
        {
            // Level 0: 0-9 lines
            _scoring.AddLines(4);
            Assert.AreEqual(0, _scoring.Level);
            _scoring.AddLines(3);
            Assert.AreEqual(0, _scoring.Level);
            _scoring.AddLines(2);
            Assert.AreEqual(0, _scoring.Level);

            // Adding 1 more line brings us to 10 total -> level 1
            _scoring.AddLines(1);
            Assert.AreEqual(1, _scoring.Level);
            Assert.AreEqual(10, _scoring.TotalLines);

            // Continue to level 2
            _scoring.AddLines(4);
            _scoring.AddLines(3);
            _scoring.AddLines(2);
            _scoring.AddLines(1);
            Assert.AreEqual(2, _scoring.Level);
            Assert.AreEqual(20, _scoring.TotalLines);
        }

        [Test]
        public void AddLines_Level2_ScoresCorrectly()
        {
            // Get to level 2 (20 lines)
            for (int i = 0; i < 5; i++)
            {
                _scoring.AddLines(4);
            }
            Assert.AreEqual(2, _scoring.Level);

            // Single at level 2 should award 40 * 3 = 120
            int scoreBeforeSingle = _scoring.Score;
            _scoring.AddLines(1);
            Assert.AreEqual(scoreBeforeSingle + 120, _scoring.Score);
        }

        [Test]
        public void AddLines_InvalidInput_DoesNothing()
        {
            _scoring.AddLines(0);
            Assert.AreEqual(0, _scoring.Score);
            Assert.AreEqual(0, _scoring.TotalLines);

            _scoring.AddLines(5);
            Assert.AreEqual(0, _scoring.Score);
            Assert.AreEqual(0, _scoring.TotalLines);

            _scoring.AddLines(-1);
            Assert.AreEqual(0, _scoring.Score);
            Assert.AreEqual(0, _scoring.TotalLines);
        }

        [Test]
        public void Reset_ClearsAllValues()
        {
            _scoring.AddLines(4);
            _scoring.AddLines(3);
            _scoring.AddLines(4);

            Assert.AreNotEqual(0, _scoring.Score);
            Assert.AreNotEqual(0, _scoring.Level);
            Assert.AreNotEqual(0, _scoring.TotalLines);

            _scoring.Reset();

            Assert.AreEqual(0, _scoring.Score);
            Assert.AreEqual(0, _scoring.Level);
            Assert.AreEqual(0, _scoring.TotalLines);
        }

        [Test]
        public void AddLines_AccumulatesScore()
        {
            _scoring.AddLines(1);
            int score1 = _scoring.Score;

            _scoring.AddLines(1);
            int score2 = _scoring.Score;

            Assert.AreEqual(score1 + 40, score2);
        }

        [Test]
        public void AddLines_TotalLinesAccumulates()
        {
            _scoring.AddLines(1);
            Assert.AreEqual(1, _scoring.TotalLines);

            _scoring.AddLines(2);
            Assert.AreEqual(3, _scoring.TotalLines);

            _scoring.AddLines(4);
            Assert.AreEqual(7, _scoring.TotalLines);
        }
    }
}

#endif
