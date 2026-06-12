using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Tetris.Services;

namespace Tetris.UI
{
    public class StartScreen : BaseScreen
    {
        [SerializeField]
        private LeaderboardService _leaderboardService;

        private LeaderboardWidget _leaderboardWidget;
        private Label _startPromptText;
        private Action<List<LeaderboardEntry>> _onScoresFetched;
        private Coroutine _blinkCoroutine;

        public event Action OnStartPressed;

        protected override void Awake()
        {
            base.Awake();

            var rootElement = GetElement("root");
            _leaderboardWidget = new LeaderboardWidget(rootElement);
            _startPromptText = GetElement("start-prompt-text") as Label;

            if (_leaderboardService == null)
            {
                _leaderboardService = UnityEngine.Object.FindAnyObjectByType<LeaderboardService>();
            }

            _onScoresFetched = OnScoresFetched;
        }

        private void Update()
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
                OnStartPressed?.Invoke();
        }

        public override void Show()
        {
            base.Show();
            FetchAndDisplayScores();
            StartBlinking();
        }

        public override void Hide()
        {
            base.Hide();
            StopBlinking();
        }

        private void FetchAndDisplayScores()
        {
            if (_leaderboardService == null)
                return;

            _leaderboardWidget?.SetLoading(true);
            _leaderboardService.FetchScores(_onScoresFetched);
        }

        private void OnScoresFetched(List<LeaderboardEntry> scores)
        {
            _leaderboardWidget?.SetLoading(false);

            if (scores == null || scores.Count == 0)
            {
                var placeholder = new List<LeaderboardEntry>
                {
                    new LeaderboardEntry { rank = 1, initials = "AAA", score = 9999 },
                    new LeaderboardEntry { rank = 2, initials = "BBB", score = 8888 },
                    new LeaderboardEntry { rank = 3, initials = "CCC", score = 7777 },
                    new LeaderboardEntry { rank = 4, initials = "DDD", score = 6666 },
                    new LeaderboardEntry { rank = 5, initials = "EEE", score = 5555 },
                };
                _leaderboardWidget?.Populate(placeholder);
                return;
            }

            _leaderboardWidget?.Populate(scores);
        }

        private void StartBlinking()
        {
            StopBlinking();
            if (_startPromptText != null)
            {
                _blinkCoroutine = StartCoroutine(BlinkCoroutine());
            }
        }

        private void StopBlinking()
        {
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
        }

        private IEnumerator BlinkCoroutine()
        {
            while (true)
            {
                _startPromptText.style.display = DisplayStyle.Flex;
                yield return new WaitForSecondsRealtime(0.5f);
                _startPromptText.style.display = DisplayStyle.None;
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }
}
