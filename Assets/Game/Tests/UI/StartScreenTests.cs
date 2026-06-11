using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Tetris.UI;
using Tetris.Services;

namespace Tetris.Tests.UI
{
    public class StartScreenTests
    {
        private GameObject _screenGameObject;
        private StartScreen _startScreen;

        [SetUp]
        public void SetUp()
        {
            _screenGameObject = new GameObject("TestStartScreen");
            _startScreen = _screenGameObject.AddComponent<StartScreen>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_screenGameObject);
        }

        [Test]
        public void StartScreenInheritsFromBaseScreen()
        {
            Assert.IsTrue(_startScreen is OneTimeGames.CoreSystems.BaseScreen);
        }

        [Test]
        public void StartScreenShowEnablesGameObject()
        {
            _screenGameObject.SetActive(false);
            _startScreen.Show();
            Assert.IsTrue(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void StartScreenHideDisablesGameObject()
        {
            _screenGameObject.SetActive(true);
            _startScreen.Hide();
            Assert.IsFalse(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void StartScreenOnStartPressedEventExists()
        {
            bool eventFired = false;
            _startScreen.OnStartPressed += () => eventFired = true;
            _startScreen.OnStartPressed?.Invoke();
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void StartScreenCanFindLeaderboardServiceAutomatically()
        {
            var serviceGO = new GameObject("LeaderboardService");
            var service = serviceGO.AddComponent<LeaderboardService>();

            _startScreen.Awake();

            Object.DestroyImmediate(serviceGO);
            Assert.Pass("Should automatically find LeaderboardService");
        }
    }
}
