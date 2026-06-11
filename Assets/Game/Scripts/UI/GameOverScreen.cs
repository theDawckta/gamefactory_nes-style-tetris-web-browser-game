using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Tetris.Services;

namespace Tetris.UI
{
    public class GameOverScreen : BaseScreen
    {
        private Label _headerText;
        private Label _finalScoreText;
        private Label _finalLevelText;
        private Label _topScoreMessageText;
        private Label _continuePromptText;

        public UnityEvent OnTopScoreAchieved = new UnityEvent();
        public UnityEvent OnContinuePressed = new UnityEvent();

        protected override void Awake()
        {
            base.Awake();

            _headerText = GetElement("game-over-header") as Label;
            _finalScoreText = GetElement("final-score-value") as Label;
            _finalLevelText = GetElement("final-level-value") as Label;
            _topScoreMessageText = GetElement("top-score-message") as Label;
            _continuePromptText = GetElement("continue-prompt") as Label;
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

        public void Show(int finalScore, int finalLevel, List<LeaderboardEntry> currentLeaderboard)
        {
            base.Show();

            if (_finalScoreText != null)
                _finalScoreText.text = finalScore.ToString();
            if (_finalLevelText != null)
                _finalLevelText.text = finalLevel.ToString();

            bool isTopScore = IsTopScore(finalScore, currentLeaderboard);

            if (isTopScore)
            {
                if (_topScoreMessageText != null)
                    _topScoreMessageText.style.display = DisplayStyle.Flex;
                if (_continuePromptText != null)
                    _continuePromptText.style.display = DisplayStyle.None;
                OnTopScoreAchieved?.Invoke();
            }
            else
            {
                if (_topScoreMessageText != null)
                    _topScoreMessageText.style.display = DisplayStyle.None;
                if (_continuePromptText != null)
                    _continuePromptText.style.display = DisplayStyle.Flex;
            }
        }

        public override void Hide()
        {
            base.Hide();

            if (_finalScoreText != null)
                _finalScoreText.text = "";
            if (_finalLevelText != null)
                _finalLevelText.text = "";
            if (_topScoreMessageText != null)
                _topScoreMessageText.style.display = DisplayStyle.None;
            if (_continuePromptText != null)
                _continuePromptText.style.display = DisplayStyle.None;
        }

        private bool IsTopScore(int finalScore, List<LeaderboardEntry> leaderboard)
        {
            if (leaderboard == null || leaderboard.Count == 0)
                return true;

            if (leaderboard.Count < 5)
                return true;

            return finalScore > leaderboard[leaderboard.Count - 1].score;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                OnContinuePressed?.Invoke();
                evt.StopPropagation();
            }
        }
    }
}
