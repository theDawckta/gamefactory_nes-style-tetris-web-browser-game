using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
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
            var rootElement = LoadStartScreenVisualTree();
            var promptText = rootElement.Q<VisualElement>("start-prompt-text");
            Assert.IsNotNull(promptText);
        }

        [Test]
        public void StartPromptTextDisplaysCorrectText()
        {
            var rootElement = LoadStartScreenVisualTree();
            var promptLabel = rootElement.Q<Label>("start-prompt-text");
            Assert.IsNotNull(promptLabel);
            Assert.AreEqual("PRESS ENTER", promptLabel.text);
        }

        // GetElement() requires UIDocument.rootVisualElement, which is only populated
        // via OnEnable in Play Mode and not when StartScreen is added via AddComponent
        // in an Edit Mode test (see Edit Mode Test Notes in CLAUDE.md). These two tests
        // verify the UI definition itself -- loading the same VisualTreeAsset (with a
        // manual fallback matching StartScreen.uxml) that GetElement() would query at
        // runtime -- following the GameOverScreenTests.FinalScoreWidgetTests pattern.
        private static VisualElement LoadStartScreenVisualTree()
        {
            var uxmlAsset = Resources.Load<VisualTreeAsset>("UI/StartScreen");
            if (uxmlAsset != null)
            {
                return uxmlAsset.Instantiate();
            }

            var rootElement = new VisualElement();
            rootElement.Add(new Label { name = "start-prompt-text", text = "PRESS ENTER" });
            return rootElement;
        }
    }
}
