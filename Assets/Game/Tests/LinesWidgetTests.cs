using NUnit.Framework;
using UnityEngine.UIElements;
using Tetris.UI;

namespace Tetris.Tests.UI
{
    public class LinesWidgetTests
    {
        private VisualElement _root;
        private Label _linesValue;
        private LinesWidget _widget;

        [SetUp]
        public void SetUp()
        {
            _root = new VisualElement();
            _linesValue = new Label();
            _linesValue.name = "lines-value";
            _root.Add(_linesValue);
            _widget = new LinesWidget(_root);
        }

        [Test]
        public void UpdateLines_SetsValueText()
        {
            _widget.UpdateLines(50);
            Assert.AreEqual("50", _linesValue.text);
        }

        [Test]
        public void UpdateLines_Zero_SetsZeroText()
        {
            _widget.UpdateLines(0);
            Assert.AreEqual("0", _linesValue.text);
        }

        [Test]
        public void UpdateLines_WithNullRoot_DoesNotThrow()
        {
            var widget = new LinesWidget(null);
            Assert.DoesNotThrow(() => widget.UpdateLines(100));
        }
    }
}
