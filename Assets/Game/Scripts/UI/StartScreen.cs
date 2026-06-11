using System;
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
        }

        public override void Hide()
        {
            base.Hide();
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
    }
}
