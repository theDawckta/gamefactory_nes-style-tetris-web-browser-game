using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using Tetris.UI;
using Tetris.Services;

namespace Tetris.Tests.UI
{
    public class GameOverScreenTests
    {
        private GameObject _screenGameObject;
        private GameOverScreen _gameOverScreen;

        [SetUp]
        public void SetUp()
        {
            _screenGameObject = new GameObject("TestGameOverScreen");
            _gameOverScreen = _screenGameObject.AddComponent<GameOverScreen>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_screenGameObject);
        }

        [Test]
        public void GameOverScreenInheritsFromBaseScreen()
        {
            Assert.IsTrue(_gameOverScreen is Tetris.UI.BaseScreen);
        }

        [Test]
        public void GameOverScreenHideDisablesGameObject()
        {
            _screenGameObject.SetActive(true);
            _gameOverScreen.Hide();
            Assert.IsFalse(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void GameOverScreenOnTopScoreAchievedEventExists()
        {
            bool eventSubscribed = false;
            try
            {
                _gameOverScreen.OnTopScoreAchieved.AddListener(() => { });
                eventSubscribed = true;
            }
            catch
            {
                eventSubscribed = false;
            }
            Assert.IsTrue(eventSubscribed);
        }

        [Test]
        public void GameOverScreenOnContinuePressedEventExists()
        {
            bool eventSubscribed = false;
            try
            {
                _gameOverScreen.OnContinuePressed.AddListener(() => { });
                eventSubscribed = true;
            }
            catch
            {
                eventSubscribed = false;
            }
            Assert.IsTrue(eventSubscribed);
        }

        [Test]
        public void GameOverScreenEmptyLeaderboardIsTopScore()
        {
            var leaderboard = new List<LeaderboardEntry>();
            int testScore = 1000;
            bool eventFired = false;
            _gameOverScreen.OnTopScoreAchieved.AddListener(() => { eventFired = true; });

            _gameOverScreen.Show(testScore, 1, leaderboard);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void GameOverScreenLessThanFiveEntriesIsTopScore()
        {
            var leaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 5000 }
            };
            bool eventFired = false;
            _gameOverScreen.OnTopScoreAchieved.AddListener(() => { eventFired = true; });

            _gameOverScreen.Show(3000, 3, leaderboard);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void GameOverScreenHigherScoreThanFifthPlaceIsTopScore()
        {
            var leaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 10000 },
                new LeaderboardEntry { rank = 2, initials = "BBB", score = 8000 },
                new LeaderboardEntry { rank = 3, initials = "CCC", score = 6000 },
                new LeaderboardEntry { rank = 4, initials = "DDD", score = 4000 },
                new LeaderboardEntry { rank = 5, initials = "EEE", score = 2000 }
            };
            bool eventFired = false;
            _gameOverScreen.OnTopScoreAchieved.AddListener(() => { eventFired = true; });

            _gameOverScreen.Show(3000, 2, leaderboard);

            Assert.IsTrue(eventFired);
        }

        [Test]
        public void GameOverScreenLowerScoreThanFifthPlaceIsNotTopScore()
        {
            var leaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 10000 },
                new LeaderboardEntry { rank = 2, initials = "BBB", score = 8000 },
                new LeaderboardEntry { rank = 3, initials = "CCC", score = 6000 },
                new LeaderboardEntry { rank = 4, initials = "DDD", score = 4000 },
                new LeaderboardEntry { rank = 5, initials = "EEE", score = 2000 }
            };
            bool topScoreFired = false;
            _gameOverScreen.OnTopScoreAchieved.AddListener(() => { topScoreFired = true; });

            _gameOverScreen.Show(1000, 1, leaderboard);

            Assert.IsFalse(topScoreFired);
        }

        [Test]
        public void GameOverScreenEqualScoreThanFifthPlaceIsNotTopScore()
        {
            var leaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 10000 },
                new LeaderboardEntry { rank = 2, initials = "BBB", score = 8000 },
                new LeaderboardEntry { rank = 3, initials = "CCC", score = 6000 },
                new LeaderboardEntry { rank = 4, initials = "DDD", score = 4000 },
                new LeaderboardEntry { rank = 5, initials = "EEE", score = 2000 }
            };
            bool topScoreFired = false;
            _gameOverScreen.OnTopScoreAchieved.AddListener(() => { topScoreFired = true; });

            _gameOverScreen.Show(2000, 1, leaderboard);

            Assert.IsFalse(topScoreFired);
        }

        [Test]
        public void GameOverScreenNullLeaderboardIsTopScore()
        {
            bool eventFired = false;
            _gameOverScreen.OnTopScoreAchieved.AddListener(() => { eventFired = true; });

            _gameOverScreen.Show(1000, 1, null);

            Assert.IsTrue(eventFired);
        }
    }

    public class FinalScoreWidgetTests
    {
        private GameObject _widgetGameObject;
        private VisualElement _rootElement;

        [SetUp]
        public void SetUp()
        {
            _widgetGameObject = new GameObject("TestFinalScoreWidget");
            var uiDocument = _widgetGameObject.AddComponent<UIDocument>();

            var uxmlAsset = Resources.Load<VisualTreeAsset>("UI/GameOverScreen");
            if (uxmlAsset != null)
            {
                _rootElement = uxmlAsset.Instantiate();
            }
            else
            {
                _rootElement = new VisualElement();
                var finalScoreValue = new Label { name = "final-score-value", text = "0" };
                var finalLevelValue = new Label { name = "final-level-value", text = "0" };
                _rootElement.Add(finalScoreValue);
                _rootElement.Add(finalLevelValue);
            }
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_widgetGameObject);
        }

        [Test]
        public void FinalScoreWidgetSetFinalScoreUpdatesScore()
        {
            var widget = new Tetris.UI.FinalScoreWidget(_rootElement);

            widget.SetFinalScore(9500, 5);

            var scoreLabel = _rootElement.Q<Label>("final-score-value");
            Assert.AreEqual("9500", scoreLabel.text);
        }

        [Test]
        public void FinalScoreWidgetSetFinalScoreUpdatesLevel()
        {
            var widget = new Tetris.UI.FinalScoreWidget(_rootElement);

            widget.SetFinalScore(9500, 5);

            var levelLabel = _rootElement.Q<Label>("final-level-value");
            Assert.AreEqual("5", levelLabel.text);
        }

        [Test]
        public void FinalScoreWidgetSetFinalScoreWithZeroValues()
        {
            var widget = new Tetris.UI.FinalScoreWidget(_rootElement);

            widget.SetFinalScore(0, 0);

            var scoreLabel = _rootElement.Q<Label>("final-score-value");
            var levelLabel = _rootElement.Q<Label>("final-level-value");
            Assert.AreEqual("0", scoreLabel.text);
            Assert.AreEqual("0", levelLabel.text);
        }

        [Test]
        public void FinalScoreWidgetSetFinalScoreLargeValues()
        {
            var widget = new Tetris.UI.FinalScoreWidget(_rootElement);

            widget.SetFinalScore(999999, 99);

            var scoreLabel = _rootElement.Q<Label>("final-score-value");
            var levelLabel = _rootElement.Q<Label>("final-level-value");
            Assert.AreEqual("999999", scoreLabel.text);
            Assert.AreEqual("99", levelLabel.text);
        }
    }
}
