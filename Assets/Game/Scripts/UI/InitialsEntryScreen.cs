using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Tetris.Services;

namespace Tetris.UI
{
    public class InitialsEntryScreen : BaseScreen
    {
        private const string CHARACTER_SET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int TOTAL_SLOTS = 3;

        private Label _promptText;
        private VisualElement _characterSlotsRegion;
        private VisualElement _confirmRegion;
        private Label _confirmPromptText;
        private CharacterSlotsWidget _characterSlotsWidget;

        private int[] _currentCharacterIndices = new int[TOTAL_SLOTS];
        private int _activeSlot = 0;
        private int _storedScore = 0;
        private LeaderboardService _leaderboardService;
        private bool[] _slotConfirmed = new bool[TOTAL_SLOTS];

        public UnityEvent OnInitialsConfirmed = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();

            _promptText = GetElement("prompt-text") as Label;
            _characterSlotsRegion = GetElement("character-slots-region");
            _confirmRegion = GetElement("confirm-region");
            _confirmPromptText = GetElement("confirm-prompt") as Label;

            if (_characterSlotsRegion != null)
            {
                _characterSlotsWidget = new CharacterSlotsWidget(_characterSlotsRegion);
            }

            ResetSlotState();
        }

        private void Update()
        {
            _characterSlotsWidget?.Update();

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)       CycleCharacterPrevious();
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) CycleCharacterNext();
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)    DeleteCurrentCharacter();
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)  ConfirmCharacterAndAdvance();
        }

        public void Show(int score)
        {
            base.Show();

            _storedScore = score;
            ResetSlotState();

            if (_confirmRegion != null)
                _confirmRegion.style.display = DisplayStyle.None;
        }

        public override void Hide()
        {
            base.Hide();
            ResetSlotState();
        }

        private void ResetSlotState()
        {
            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                _currentCharacterIndices[i] = -1;
                _slotConfirmed[i] = false;
            }
            _activeSlot = 0;
            _characterSlotsWidget?.Reset();
        }

        private void CycleCharacterNext()
        {
            if (_slotConfirmed[_activeSlot])
                return;

            _currentCharacterIndices[_activeSlot]++;
            if (_currentCharacterIndices[_activeSlot] >= CHARACTER_SET.Length)
                _currentCharacterIndices[_activeSlot] = 0;

            UpdateSlotDisplay();
        }

        private void CycleCharacterPrevious()
        {
            if (_slotConfirmed[_activeSlot])
                return;

            _currentCharacterIndices[_activeSlot]--;
            if (_currentCharacterIndices[_activeSlot] < 0)
                _currentCharacterIndices[_activeSlot] = CHARACTER_SET.Length - 1;

            UpdateSlotDisplay();
        }

        private void DeleteCurrentCharacter()
        {
            if (_slotConfirmed[_activeSlot])
            {
                _slotConfirmed[_activeSlot] = false;
                _characterSlotsWidget?.SetSlotConfirmed(_activeSlot, false);
                if (_confirmRegion != null)
                    _confirmRegion.style.display = DisplayStyle.None;
            }

            _currentCharacterIndices[_activeSlot] = -1;
            _characterSlotsWidget?.SetSlotCharacter(_activeSlot, '_');

            if (_activeSlot > 0)
            {
                _activeSlot--;
                _slotConfirmed[_activeSlot] = false;
                _characterSlotsWidget?.SetSlotConfirmed(_activeSlot, false);
                _characterSlotsWidget?.SetActiveSlot(_activeSlot);
            }
        }

        private void ConfirmCharacterAndAdvance()
        {
            if (_activeSlot < TOTAL_SLOTS - 1 && !_slotConfirmed[_activeSlot])
            {
                if (_currentCharacterIndices[_activeSlot] < 0)
                    return;

                _slotConfirmed[_activeSlot] = true;
                _characterSlotsWidget?.SetSlotConfirmed(_activeSlot, true);
                _activeSlot++;
                _characterSlotsWidget?.SetActiveSlot(_activeSlot);
            }
            else if (_activeSlot == TOTAL_SLOTS - 1 && !_slotConfirmed[_activeSlot])
            {
                if (_currentCharacterIndices[_activeSlot] < 0)
                    return;

                _slotConfirmed[_activeSlot] = true;
                _characterSlotsWidget?.SetSlotConfirmed(_activeSlot, true);
                ShowConfirmPrompt();
            }
            else if (AllSlotsConfirmed())
            {
                SubmitInitials();
            }
        }

        private void ShowConfirmPrompt()
        {
            if (_confirmRegion != null)
                _confirmRegion.style.display = DisplayStyle.Flex;
        }

        private bool AllSlotsConfirmed()
        {
            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                if (!_slotConfirmed[i])
                    return false;
            }
            return true;
        }

        private void UpdateSlotDisplay()
        {
            if (_characterSlotsWidget == null)
                return;

            char displayChar = _currentCharacterIndices[_activeSlot] >= 0 && _currentCharacterIndices[_activeSlot] < CHARACTER_SET.Length
                ? CHARACTER_SET[_currentCharacterIndices[_activeSlot]]
                : '_';

            _characterSlotsWidget.SetSlotCharacter(_activeSlot, displayChar);
        }

        private void SubmitInitials()
        {
            if (_confirmPromptText != null)
                _confirmPromptText.text = "SAVING...";

            string initials = GetInitialsString();

            if (_leaderboardService == null)
                _leaderboardService = FindAnyObjectByType<LeaderboardService>();

            if (_leaderboardService != null)
            {
                _leaderboardService.SubmitScore(initials, _storedScore, (result) =>
                {
                    if (result == null)
                    {
                        Debug.LogError("Failed to submit score to leaderboard");
                    }
                    OnInitialsConfirmed?.Invoke();
                });
            }
            else
            {
                Debug.LogError("LeaderboardService not found");
                OnInitialsConfirmed?.Invoke();
            }
        }

        private string GetInitialsString()
        {
            string initials = "";
            for (int i = 0; i < TOTAL_SLOTS; i++)
            {
                if (_currentCharacterIndices[i] >= 0 && _currentCharacterIndices[i] < CHARACTER_SET.Length)
                    initials += CHARACTER_SET[_currentCharacterIndices[i]];
                else
                    initials += " ";
            }
            return initials;
        }
    }
}
