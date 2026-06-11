using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class CharacterSlotsWidget
    {
        private const int TOTAL_SLOTS = 3;
        private const float CURSOR_BLINK_RATE = 1f;
        private const string CHARACTER_SET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private readonly VisualElement _rootElement;
        private readonly List<Label> _slotLabels = new List<Label>();
        private readonly List<Label> _cursorLabels = new List<Label>();
        private readonly bool[] _slotConfirmed = new bool[TOTAL_SLOTS];
        private readonly char[] _slotCharacters = new char[TOTAL_SLOTS];

        private int _activeSlot = 0;
        private float _blinkTimer = 0f;
        private bool _cursorVisible = true;

        public CharacterSlotsWidget(VisualElement rootElement)
        {
            _rootElement = rootElement;
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            if (_rootElement == null)
                return;

            var slotsContainer = new VisualElement();
            slotsContainer.style.flexDirection = FlexDirection.Row;
            slotsContainer.style.justifyContent = Justify.Center;
            slotsContainer.style.alignItems = Align.Center;
            _rootElement.Add(slotsContainer);

            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                var slotContainer = new VisualElement();
                slotContainer.name = $"character-slot-{i}";
                slotContainer.style.flexDirection = FlexDirection.Column;
                slotContainer.style.justifyContent = Justify.Center;
                slotContainer.style.alignItems = Align.Center;
                slotContainer.style.width = new Length(60, LengthUnit.Pixel);
                slotContainer.style.height = new Length(80, LengthUnit.Pixel);
                slotContainer.style.marginLeft = new Length(10, LengthUnit.Pixel);
                slotContainer.style.marginRight = new Length(10, LengthUnit.Pixel);

                var characterLabel = new Label("_");
                characterLabel.name = $"character-label-{i}";
                characterLabel.style.fontSize = 48;
                characterLabel.style.color = new Color(1f, 1f, 1f, 1f);
                characterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                slotContainer.Add(characterLabel);

                var cursorLabel = new Label("_");
                cursorLabel.name = $"cursor-label-{i}";
                cursorLabel.style.fontSize = 16;
                cursorLabel.style.color = new Color(1f, 1f, 1f, 1f);
                cursorLabel.style.marginTop = new Length(-8, LengthUnit.Pixel);
                cursorLabel.style.display = DisplayStyle.None;
                slotContainer.Add(cursorLabel);

                slotsContainer.Add(slotContainer);
                _slotLabels.Add(characterLabel);
                _cursorLabels.Add(cursorLabel);
                _slotCharacters[i] = '_';
            }

            SetActiveSlot(0);
        }

        public void SetSlotCharacter(int slotIndex, char c)
        {
            if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
                return;

            _slotCharacters[slotIndex] = c;
            UpdateSlotDisplay(slotIndex);
        }

        public void SetSlotConfirmed(int slotIndex, bool confirmed)
        {
            if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
                return;

            _slotConfirmed[slotIndex] = confirmed;
            UpdateSlotDisplay(slotIndex);
        }

        public void SetActiveSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
                return;

            _activeSlot = slotIndex;
            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                UpdateSlotDisplay(i);
            }
            _blinkTimer = 0f;
            _cursorVisible = true;
        }

        public void Reset()
        {
            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                _slotCharacters[i] = '_';
                _slotConfirmed[i] = false;
                UpdateSlotDisplay(i);
            }
            SetActiveSlot(0);
        }

        public string GetInitials()
        {
            string initials = "";
            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                if (_slotCharacters[i] == '_' || _slotCharacters[i] == ' ')
                    initials += " ";
                else
                    initials += _slotCharacters[i];
            }
            return initials;
        }

        public void Update()
        {
            if (_slotConfirmed[_activeSlot])
                return;

            _blinkTimer += Time.deltaTime;
            if (_blinkTimer >= (1f / CURSOR_BLINK_RATE))
            {
                _blinkTimer = 0f;
                _cursorVisible = !_cursorVisible;
                UpdateSlotDisplay(_activeSlot);
            }
        }

        private void UpdateSlotDisplay(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= TOTAL_SLOTS)
                return;

            var slotLabel = _slotLabels[slotIndex];
            var cursorLabel = _cursorLabels[slotIndex];

            if (slotLabel == null || cursorLabel == null)
                return;

            var displayChar = _slotCharacters[slotIndex];
            if (displayChar == '_')
                slotLabel.text = "_";
            else
                slotLabel.text = displayChar.ToString();

            if (slotIndex == _activeSlot && !_slotConfirmed[slotIndex])
            {
                cursorLabel.style.display = _cursorVisible ? DisplayStyle.Flex : DisplayStyle.None;
            }
            else
            {
                cursorLabel.style.display = DisplayStyle.None;
            }

            var color = _slotConfirmed[slotIndex] ? new Color(1f, 1f, 1f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);
            slotLabel.style.color = color;
        }
    }
}
