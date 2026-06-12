using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Game.Gameplay;
using Tetris.UI;
using Tetris.Services;
using Tetris.Systems;

namespace Tetris.Tests.Systems
{
    public class GameStateManagerTests
    {
        private GameObject _managerGO;
        private GameStateManager _manager;
        private GameObject _controllerGO;
        private GameplayController _controller;
        private GameObject _startScreenGO;
        private StartScreen _startScreen;
        private GameObject _gameScreenGO;
        private GameScreen _gameScreen;
        private GameObject _gameOverScreenGO;
        private GameOverScreen _gameOverScreen;
        private GameObject _initialsEntryScreenGO;
        private InitialsEntryScreen _initialsEntryScreen;

        [SetUp]
        public void SetUp()
        {
            _managerGO = new GameObject("GameStateManager");
            _manager = _managerGO.AddComponent<GameStateManager>();

            _controllerGO = new GameObject("GameplayController");
            _controller = _controllerGO.AddComponent<GameplayController>();

            _startScreenGO = new GameObject("StartScreen");
            _startScreen = _startScreenGO.AddComponent<StartScreen>();

            _gameScreenGO = new GameObject("GameScreen");
            _gameScreen = _gameScreenGO.AddComponent<GameScreen>();

            _gameOverScreenGO = new GameObject("GameOverScreen");
            _gameOverScreen = _gameOverScreenGO.AddComponent<GameOverScreen>();

            _initialsEntryScreenGO = new GameObject("InitialsEntryScreen");
            _initialsEntryScreen = _initialsEntryScreenGO.AddComponent<InitialsEntryScreen>();

            SetPrivateField(_manager, "_gameplayController", _controller);
            SetPrivateField(_manager, "_startScreen", _startScreen);
            SetPrivateField(_manager, "_gameScreen", _gameScreen);
            SetPrivateField(_manager, "_gameOverScreen", _gameOverScreen);
            SetPrivateField(_manager, "_initialsEntryScreen", _initialsEntryScreen);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_managerGO);
            UnityEngine.Object.DestroyImmediate(_controllerGO);
            UnityEngine.Object.DestroyImmediate(_startScreenGO);
            UnityEngine.Object.DestroyImmediate(_gameScreenGO);
            UnityEngine.Object.DestroyImmediate(_gameOverScreenGO);
            UnityEngine.Object.DestroyImmediate(_initialsEntryScreenGO);
        }

        private static void SetPrivateField(object obj, string name, object value)
        {
            obj.GetType()
               .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
               ?.SetValue(obj, value);
        }

        private static void InvokePrivateMethod(object obj, string name)
        {
            obj.GetType()
               .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)
               ?.Invoke(obj, null);
        }

        private static void FireEventBackingField<T>(object obj, string eventName) where T : Delegate
        {
            var field = obj.GetType().GetField(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
            (field?.GetValue(obj) as T)?.DynamicInvoke();
        }

        private void InitializeManager()
        {
            InvokePrivateMethod(_manager, "Start");
        }

        [Test]
        public void GameStateManagerIsAMonoBehaviour()
        {
            Assert.IsTrue(_manager is MonoBehaviour);
        }

        [Test]
        public void GameStateManagerStartScreenShownOnStart()
        {
            InitializeManager();
            Assert.IsTrue(_startScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerGameScreenHiddenOnStart()
        {
            InitializeManager();
            Assert.IsFalse(_gameScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerGameOverScreenHiddenOnStart()
        {
            InitializeManager();
            Assert.IsFalse(_gameOverScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerInitialsEntryScreenHiddenOnStart()
        {
            InitializeManager();
            Assert.IsFalse(_initialsEntryScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerGameScreenShownWhenStartPressed()
        {
            InitializeManager();
            FireEventBackingField<Action>(_startScreen, "OnStartPressed");
            Assert.IsTrue(_gameScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerStartScreenHiddenWhenPlaying()
        {
            InitializeManager();
            FireEventBackingField<Action>(_startScreen, "OnStartPressed");
            Assert.IsFalse(_startScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerInitialsEntryShownOnGameOverWithNullLeaderboard()
        {
            // Null cached leaderboard causes GameOverScreen.Show to immediately fire
            // OnTopScoreAchieved, so the final visible state is InitialsEntry.
            InitializeManager();
            FireEventBackingField<Action>(_startScreen, "OnStartPressed");
            FireEventBackingField<GameplayController.GameOverEventHandler>(_controller, "OnGameOver");
            Assert.IsTrue(_initialsEntryScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerGameOverScreenShownWhenNotTopScore()
        {
            // Pre-seed the cached leaderboard so the test score (0) is below the threshold.
            var leaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { initials = "AAA", score = 10000 },
                new LeaderboardEntry { initials = "BBB", score = 8000 },
                new LeaderboardEntry { initials = "CCC", score = 6000 },
                new LeaderboardEntry { initials = "DDD", score = 4000 },
                new LeaderboardEntry { initials = "EEE", score = 2000 },
            };
            SetPrivateField(_manager, "_cachedLeaderboard", leaderboard);
            InitializeManager();
            FireEventBackingField<Action>(_startScreen, "OnStartPressed");
            FireEventBackingField<GameplayController.GameOverEventHandler>(_controller, "OnGameOver");
            Assert.IsTrue(_gameOverScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerStartScreenShownAfterContinuePressed()
        {
            var leaderboard = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { initials = "AAA", score = 10000 },
                new LeaderboardEntry { initials = "BBB", score = 8000 },
                new LeaderboardEntry { initials = "CCC", score = 6000 },
                new LeaderboardEntry { initials = "DDD", score = 4000 },
                new LeaderboardEntry { initials = "EEE", score = 2000 },
            };
            SetPrivateField(_manager, "_cachedLeaderboard", leaderboard);
            InitializeManager();
            FireEventBackingField<Action>(_startScreen, "OnStartPressed");
            FireEventBackingField<GameplayController.GameOverEventHandler>(_controller, "OnGameOver");
            _gameOverScreen.OnContinuePressed.Invoke();
            Assert.IsTrue(_startScreenGO.activeInHierarchy);
        }

        [Test]
        public void GameStateManagerStartScreenShownAfterInitialsConfirmed()
        {
            InitializeManager();
            FireEventBackingField<Action>(_startScreen, "OnStartPressed");
            FireEventBackingField<GameplayController.GameOverEventHandler>(_controller, "OnGameOver");
            _initialsEntryScreen.OnInitialsConfirmed.Invoke();
            Assert.IsTrue(_startScreenGO.activeInHierarchy);
        }
    }
}
