using System.Collections.Generic;
using UnityEngine.UIElements;
using Tetris.Services;

namespace Tetris.UI
{
    public class LeaderboardWidget
    {
        private const int MaxRows = 5;
        private readonly VisualElement _tableContainer;
        private readonly VisualElement _loadingLabel;
        private readonly List<VisualElement> _rows = new List<VisualElement>();

        public LeaderboardWidget(VisualElement rootElement)
        {
            _tableContainer = rootElement?.Q("leaderboard-table");
            _loadingLabel = rootElement?.Q("leaderboard-loading");

            if (_tableContainer != null)
            {
                _rows.Clear();
                for (int i = 0; i < MaxRows; i++)
                {
                    var row = CreateRow(i);
                    _tableContainer.Add(row);
                    _rows.Add(row);
                }
            }
        }

        private VisualElement CreateRow(int index)
        {
            var row = new VisualElement();
            row.name = $"leaderboard-row-{index}";
            row.AddToClassList("leaderboard-row");

            var rankLabel = new Label { text = "-" };
            rankLabel.name = $"leaderboard-rank-{index}";
            rankLabel.AddToClassList("leaderboard-cell");
            rankLabel.AddToClassList("leaderboard-rank");
            row.Add(rankLabel);

            var initialsLabel = new Label { text = "---" };
            initialsLabel.name = $"leaderboard-initials-{index}";
            initialsLabel.AddToClassList("leaderboard-cell");
            initialsLabel.AddToClassList("leaderboard-initials");
            row.Add(initialsLabel);

            var scoreLabel = new Label { text = "-" };
            scoreLabel.name = $"leaderboard-score-{index}";
            scoreLabel.AddToClassList("leaderboard-cell");
            scoreLabel.AddToClassList("leaderboard-score");
            row.Add(scoreLabel);

            return row;
        }

        public void Populate(List<LeaderboardEntry> entries)
        {
            if (_tableContainer == null)
                return;

            for (int i = 0; i < MaxRows; i++)
            {
                if (i < _rows.Count)
                {
                    var row = _rows[i];
                    if (i < entries?.Count)
                    {
                        var entry = entries[i];
                        var rankLabel = row.Q<Label>($"leaderboard-rank-{i}");
                        var initialsLabel = row.Q<Label>($"leaderboard-initials-{i}");
                        var scoreLabel = row.Q<Label>($"leaderboard-score-{i}");

                        if (rankLabel != null)
                            rankLabel.text = $"{entry.rank}.";
                        if (initialsLabel != null)
                            initialsLabel.text = entry.initials ?? "---";
                        if (scoreLabel != null)
                            scoreLabel.text = entry.score.ToString();
                    }
                    else
                    {
                        var rankLabel = row.Q<Label>($"leaderboard-rank-{i}");
                        var initialsLabel = row.Q<Label>($"leaderboard-initials-{i}");
                        var scoreLabel = row.Q<Label>($"leaderboard-score-{i}");

                        if (rankLabel != null)
                            rankLabel.text = "-";
                        if (initialsLabel != null)
                            initialsLabel.text = "---";
                        if (scoreLabel != null)
                            scoreLabel.text = "-";
                    }
                }
            }
        }

        public void SetLoading(bool loading)
        {
            if (_tableContainer != null)
                _tableContainer.style.display = loading ? DisplayStyle.None : DisplayStyle.Flex;
            if (_loadingLabel != null)
                _loadingLabel.style.display = loading ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
