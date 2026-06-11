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
            Assert.IsTrue(_startScreen is Tetris.UI.BaseScreen);
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
            // Event should be accessible and subscribable
            bool eventSubscribed = false;
            try
            {
                _startScreen.OnStartPressed += () => { };
                eventSubscribed = true;
            }
            catch
            {
                eventSubscribed = false;
            }
            Assert.IsTrue(eventSubscribed);
        }

        [Test]
        public void StartPromptTextElementExists()
        {
            var promptText = _startScreen.GetElement("start-prompt-text");
            Assert.IsNotNull(promptText);
        }

        [Test]
        public void StartPromptTextDisplaysCorrectText()
        {
            var promptLabel = _startScreen.GetElement("start-prompt-text") as UnityEngine.UIElements.Label;
            Assert.IsNotNull(promptLabel);
            Assert.AreEqual("PRESS ENTER", promptLabel.text);
        }

    }
}
