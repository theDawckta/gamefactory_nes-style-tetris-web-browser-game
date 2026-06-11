using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using Tetris.UI;

namespace Tetris.Tests.UI
{
    public class InitialsEntryScreenTests
    {
        private GameObject _screenGameObject;
        private InitialsEntryScreen _initialsEntryScreen;

        [SetUp]
        public void SetUp()
        {
            _screenGameObject = new GameObject("TestInitialsEntryScreen");
            _initialsEntryScreen = _screenGameObject.AddComponent<InitialsEntryScreen>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_screenGameObject);
        }

        [Test]
        public void InitialsEntryScreenInheritsFromBaseScreen()
        {
            Assert.IsTrue(_initialsEntryScreen is Tetris.UI.BaseScreen);
        }

        [Test]
        public void InitialsEntryScreenShowEnablesGameObject()
        {
            _screenGameObject.SetActive(false);
            _initialsEntryScreen.Show(1000);
            Assert.IsTrue(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void InitialsEntryScreenHideDisablesGameObject()
        {
            _screenGameObject.SetActive(true);
            _initialsEntryScreen.Hide();
            Assert.IsFalse(_screenGameObject.activeInHierarchy);
        }

        [Test]
        public void InitialsEntryScreenOnInitialsConfirmedEventExists()
        {
            bool eventSubscribed = false;
            try
            {
                _initialsEntryScreen.OnInitialsConfirmed.AddListener(() => { });
                eventSubscribed = true;
            }
            catch
            {
                eventSubscribed = false;
            }
            Assert.IsTrue(eventSubscribed);
        }
    }
}
