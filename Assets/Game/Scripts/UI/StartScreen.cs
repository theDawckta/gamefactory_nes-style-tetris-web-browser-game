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

        private ScrollView _scoresContainer;
        private VisualElement _loadingIndicator;
        private Label _startPromptText;
        private Action<List<LeaderboardEntry>> _onScoresFetched;

        public event Action OnStartPressed;

        protected override void Awake()
        {
            base.Awake();

            _scoresContainer = GetElement("scores-container") as ScrollView;
            _loadingIndicator = GetElement("loading-indicator");
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
            ClearLeaderboard();
            base.Hide();
        }

        private void FetchAndDisplayScores()
        {
            if (_leaderboardService == null)
                return;

            ShowLoadingIndicator(true);
            _leaderboardService.FetchScores(_onScoresFetched);
        }

        private void OnScoresFetched(List<LeaderboardEntry> scores)
        {
            ShowLoadingIndicator(false);

            if (_scoresContainer == null)
                return;

            if (scores == null || scores.Count == 0)
            {
                _scoresContainer.Clear();
                _scoresContainer.Add(new Label { text = "NO SCORES YET" });
                return;
            }

            DisplayScores(scores);
        }

        private void DisplayScores(List<LeaderboardEntry> scores)
        {
            if (_scoresContainer == null)
                return;

            _scoresContainer.Clear();

            int displayCount = Mathf.Min(5, scores.Count);
            for (int i = 0; i < displayCount; i++)
            {
                LeaderboardEntry entry = scores[i];
                string scoreText = $"{entry.rank}. {entry.initials} {entry.score}";
                _scoresContainer.Add(new Label { text = scoreText });
            }
        }

        private void ShowLoadingIndicator(bool show)
        {
            if (_loadingIndicator != null)
            {
                _loadingIndicator.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private void ClearLeaderboard()
        {
            if (_scoresContainer != null)
            {
                _scoresContainer.Clear();
            }
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
