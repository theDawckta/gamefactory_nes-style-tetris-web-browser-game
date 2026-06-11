using NUnit.Framework;
using UnityEngine;
using Tetris.UI;

namespace Tetris.Tests.UI
{
    public class GameScreenTests
    {
        private GameObject _screenGameObject;
        private GameScreen _gameScreen;

        [SetUp]
        public void SetUp()
        {
            _screenGameObject = new GameObject("TestGameScreen");
            _gameScreen = _screenGameObject.AddComponent<GameScreen>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_screenGameObject);
        }

        [Test]
        public void GameScreenInheritsFromBaseScreen()
        {
            Assert.IsTrue(_gameScreen is BaseScreen);
        }

        [Test]
        public void GameScreenShowEnablesGameObject()
        {
            _screenGameObject.SetActive(false);
            _gameScreen.Show();
            Assert.IsTrue(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void GameScreenHideDisablesGameObject()
        {
            _screenGameObject.SetActive(true);
            _gameScreen.Hide();
            Assert.IsFalse(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void GameScreenUpdateScoreDoesNotThrow()
        {
            Assert.DoesNotThrow(() => _gameScreen.UpdateScore(9999));
        }
    }
}
