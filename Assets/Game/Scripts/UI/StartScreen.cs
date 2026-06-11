using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

            var rootElement = GetElement("");
            _leaderboardWidget = new LeaderboardWidget(rootElement);
            _startPromptText = GetElement("start-prompt-text") as Label;

            if (_leaderboardService == null)
            {
                _leaderboardService = UnityEngine.Object.FindAnyObjectByType<LeaderboardService>();
            }

            _onScoresFetched = OnScoresFetched;
        }

        private void OnEnable()
        {
            var rootElement = GetElement("");
            if (rootElement != null)
            {
                rootElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            }
        }

        private void OnDisable()
        {
            var rootElement = GetElement("");
            if (rootElement != null)
            {
                rootElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            }
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
                _leaderboardWidget?.Populate(new List<LeaderboardEntry>());
                return;
            }

            _leaderboardWidget?.Populate(scores);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                OnStartPressed?.Invoke();
                evt.StopPropagation();
            }
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
