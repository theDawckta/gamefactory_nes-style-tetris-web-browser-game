using NUnit.Framework;
using UnityEngine.UIElements;
using Tetris.UI;

namespace Tetris.Tests.UI
{
    public class ScoreWidgetTests
    {
        private VisualElement _root;
        private Label _scoreValue;
        private ScoreWidget _widget;

        [SetUp]
        public void SetUp()
        {
            _root = new VisualElement();
            _scoreValue = new Label();
            _scoreValue.name = "score-value";
            _root.Add(_scoreValue);
            _widget = new ScoreWidget(_root);
        }

        [Test]
        public void UpdateScore_SetsValueText()
        {
            _widget.UpdateScore(12345);
            Assert.AreEqual("12345", _scoreValue.text);
        }

        [Test]
        public void UpdateScore_Zero_SetsZeroText()
        {
            _widget.UpdateScore(0);
            Assert.AreEqual("0", _scoreValue.text);
        }

        [Test]
        public void UpdateScore_WithNullRoot_DoesNotThrow()
        {
            var widget = new ScoreWidget(null);
            Assert.DoesNotThrow(() => widget.UpdateScore(100));
        }
    }
}
