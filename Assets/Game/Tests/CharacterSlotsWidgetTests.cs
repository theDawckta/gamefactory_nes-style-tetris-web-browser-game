using NUnit.Framework;
using UnityEngine.UIElements;
using Tetris.UI;

namespace Tetris.Tests.UI
{
    [TestFixture]
    public class CharacterSlotsWidgetTests
    {
        private VisualElement _rootElement;
        private CharacterSlotsWidget _widget;

        [SetUp]
        public void SetUp()
        {
            _rootElement = new VisualElement();
            _widget = new CharacterSlotsWidget(_rootElement);
        }

        [Test]
        public void SetSlotCharacterUpdatesDisplay()
        {
            _widget.SetSlotCharacter(0, 'A');

            var charLabel = _rootElement.Q<Label>("character-label-0");
            Assert.That(charLabel.text, Is.EqualTo("A"));
        }

        [Test]
        public void SetSlotCharacterSecondSlot()
        {
            _widget.SetSlotCharacter(1, 'B');

            var charLabel = _rootElement.Q<Label>("character-label-1");
            Assert.That(charLabel.text, Is.EqualTo("B"));
        }

        [Test]
        public void SetSlotCharacterThirdSlot()
        {
            _widget.SetSlotCharacter(2, 'Z');

            var charLabel = _rootElement.Q<Label>("character-label-2");
            Assert.That(charLabel.text, Is.EqualTo("Z"));
        }

        [Test]
        public void SetSlotConfirmedHidesCursor()
        {
            _widget.SetActiveSlot(0);
            _widget.SetSlotCharacter(0, 'A');
            _widget.SetSlotConfirmed(0, true);

            var cursorLabel = _rootElement.Q<Label>("cursor-label-0");
            Assert.That(cursorLabel.style.display.value, Is.EqualTo(DisplayStyle.None));
        }

        [Test]
        public void SetActiveSlotsMovesActive()
        {
            _widget.SetActiveSlot(1);

            var cursor1 = _rootElement.Q<Label>("cursor-label-1");
            Assert.That(cursor1.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void ResetClearsAllSlots()
        {
            _widget.SetSlotCharacter(0, 'A');
            _widget.SetSlotCharacter(1, 'B');
            _widget.SetSlotCharacter(2, 'C');
            _widget.SetSlotConfirmed(0, true);
            _widget.SetSlotConfirmed(1, true);

            _widget.Reset();

            var charLabel0 = _rootElement.Q<Label>("character-label-0");
            var charLabel1 = _rootElement.Q<Label>("character-label-1");
            var charLabel2 = _rootElement.Q<Label>("character-label-2");

            Assert.That(charLabel0.text, Is.EqualTo("_"));
            Assert.That(charLabel1.text, Is.EqualTo("_"));
            Assert.That(charLabel2.text, Is.EqualTo("_"));
        }

        [Test]
        public void GetInitialsReturnsThreeCharacters()
        {
            _widget.SetSlotCharacter(0, 'A');
            _widget.SetSlotCharacter(1, 'B');
            _widget.SetSlotCharacter(2, 'C');

            string initials = _widget.GetInitials();

            Assert.That(initials, Is.EqualTo("ABC"));
            Assert.That(initials.Length, Is.EqualTo(3));
        }

        [Test]
        public void GetInitialsWithUnderscoreShowsSpace()
        {
            _widget.SetSlotCharacter(0, 'A');
            _widget.SetSlotCharacter(1, '_');
            _widget.SetSlotCharacter(2, 'C');

            string initials = _widget.GetInitials();

            Assert.That(initials[0], Is.EqualTo('A'));
            Assert.That(initials[1], Is.EqualTo(' '));
            Assert.That(initials[2], Is.EqualTo('C'));
        }

        [Test]
        public void InitialStateSlotsAreUnderscores()
        {
            var charLabel0 = _rootElement.Q<Label>("character-label-0");
            var charLabel1 = _rootElement.Q<Label>("character-label-1");
            var charLabel2 = _rootElement.Q<Label>("character-label-2");

            Assert.That(charLabel0.text, Is.EqualTo("_"));
            Assert.That(charLabel1.text, Is.EqualTo("_"));
            Assert.That(charLabel2.text, Is.EqualTo("_"));
        }

        [Test]
        public void InitialStateActiveSlotIsZero()
        {
            var cursor0 = _rootElement.Q<Label>("cursor-label-0");

            Assert.That(cursor0.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void SetActiveSlotsHidesPreviousActive()
        {
            _widget.SetActiveSlot(0);
            _widget.SetActiveSlot(1);

            var cursor0 = _rootElement.Q<Label>("cursor-label-0");
            var cursor1 = _rootElement.Q<Label>("cursor-label-1");

            Assert.That(cursor0.style.display.value, Is.EqualTo(DisplayStyle.None));
            Assert.That(cursor1.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void NumericCharactersWork()
        {
            _widget.SetSlotCharacter(0, '0');
            _widget.SetSlotCharacter(1, '5');
            _widget.SetSlotCharacter(2, '9');

            var charLabel0 = _rootElement.Q<Label>("character-label-0");
            var charLabel1 = _rootElement.Q<Label>("character-label-1");
            var charLabel2 = _rootElement.Q<Label>("character-label-2");

            Assert.That(charLabel0.text, Is.EqualTo("0"));
            Assert.That(charLabel1.text, Is.EqualTo("5"));
            Assert.That(charLabel2.text, Is.EqualTo("9"));
        }

        [Test]
        public void SetSlotConfirmedMultipleSlots()
        {
            _widget.SetSlotCharacter(0, 'A');
            _widget.SetSlotCharacter(1, 'B');
            _widget.SetSlotCharacter(2, 'C');

            _widget.SetSlotConfirmed(0, true);
            _widget.SetSlotConfirmed(1, true);

            var cursor0 = _rootElement.Q<Label>("cursor-label-0");
            var cursor1 = _rootElement.Q<Label>("cursor-label-1");

            Assert.That(cursor0.style.display.value, Is.EqualTo(DisplayStyle.None));
            Assert.That(cursor1.style.display.value, Is.EqualTo(DisplayStyle.None));
        }
    }
}
