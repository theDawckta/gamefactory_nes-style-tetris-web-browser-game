using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UIElements;
using Tetris.Services;
using Tetris.UI;

namespace Tetris.Tests
{
    [TestFixture]
    public class LeaderboardWidgetTests
    {
        private VisualElement _rootElement;
        private VisualElement _tableContainer;
        private VisualElement _loadingLabel;
        private LeaderboardWidget _widget;

        [SetUp]
        public void SetUp()
        {
            _rootElement = new VisualElement();
            _tableContainer = new VisualElement();
            _tableContainer.name = "leaderboard-table";
            _rootElement.Add(_tableContainer);

            _loadingLabel = new Label { text = "LOADING..." };
            _loadingLabel.name = "leaderboard-loading";
            _rootElement.Add(_loadingLabel);

            _widget = new LeaderboardWidget(_rootElement);
        }

        [Test]
        public void PopulateCreates5Rows()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 1000 }
            };

            _widget.Populate(entries);

            Assert.That(_tableContainer.childCount, Is.EqualTo(5));
        }

        [Test]
        public void PopulateFilledEntriesCorrectly()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 1000 },
                new LeaderboardEntry { rank = 2, initials = "BBB", score = 900 }
            };

            _widget.Populate(entries);

            var firstRow = _tableContainer.ElementAt(0);
            var firstRankLabel = firstRow.Q<Label>("leaderboard-rank-0");
            var firstInitialsLabel = firstRow.Q<Label>("leaderboard-initials-0");
            var firstScoreLabel = firstRow.Q<Label>("leaderboard-score-0");

            Assert.That(firstRankLabel.text, Is.EqualTo("1."));
            Assert.That(firstInitialsLabel.text, Is.EqualTo("AAA"));
            Assert.That(firstScoreLabel.text, Is.EqualTo("1000"));

            var secondRow = _tableContainer.ElementAt(1);
            var secondRankLabel = secondRow.Q<Label>("leaderboard-rank-1");
            var secondInitialsLabel = secondRow.Q<Label>("leaderboard-initials-1");
            var secondScoreLabel = secondRow.Q<Label>("leaderboard-score-1");

            Assert.That(secondRankLabel.text, Is.EqualTo("2."));
            Assert.That(secondInitialsLabel.text, Is.EqualTo("BBB"));
            Assert.That(secondScoreLabel.text, Is.EqualTo("900"));
        }

        [Test]
        public void PopulateEmptyEntriesShowDashes()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 1000 }
            };

            _widget.Populate(entries);

            var thirdRow = _tableContainer.ElementAt(2);
            var thirdRankLabel = thirdRow.Q<Label>("leaderboard-rank-2");
            var thirdInitialsLabel = thirdRow.Q<Label>("leaderboard-initials-2");
            var thirdScoreLabel = thirdRow.Q<Label>("leaderboard-score-2");

            Assert.That(thirdRankLabel.text, Is.EqualTo("-"));
            Assert.That(thirdInitialsLabel.text, Is.EqualTo("---"));
            Assert.That(thirdScoreLabel.text, Is.EqualTo("-"));
        }

        [Test]
        public void PopulateWithNullListShowsDashes()
        {
            _widget.Populate(null);

            var firstRow = _tableContainer.ElementAt(0);
            var firstRankLabel = firstRow.Q<Label>("leaderboard-rank-0");
            var firstInitialsLabel = firstRow.Q<Label>("leaderboard-initials-0");
            var firstScoreLabel = firstRow.Q<Label>("leaderboard-score-0");

            Assert.That(firstRankLabel.text, Is.EqualTo("-"));
            Assert.That(firstInitialsLabel.text, Is.EqualTo("---"));
            Assert.That(firstScoreLabel.text, Is.EqualTo("-"));
        }

        [Test]
        public void PopulateWithEmptyListShowsDashes()
        {
            _widget.Populate(new List<LeaderboardEntry>());

            var firstRow = _tableContainer.ElementAt(0);
            var firstRankLabel = firstRow.Q<Label>("leaderboard-rank-0");
            var firstInitialsLabel = firstRow.Q<Label>("leaderboard-initials-0");
            var firstScoreLabel = firstRow.Q<Label>("leaderboard-score-0");

            Assert.That(firstRankLabel.text, Is.EqualTo("-"));
            Assert.That(firstInitialsLabel.text, Is.EqualTo("---"));
            Assert.That(firstScoreLabel.text, Is.EqualTo("-"));
        }

        [Test]
        public void SetLoadingTrueHidesTableShowsLoading()
        {
            _widget.SetLoading(true);

            Assert.That(_tableContainer.style.display.value, Is.EqualTo(DisplayStyle.None));
            Assert.That(_loadingLabel.style.display.value, Is.EqualTo(DisplayStyle.Flex));
        }

        [Test]
        public void SetLoadingFalseShowsTableHidesLoading()
        {
            _widget.SetLoading(true);
            _widget.SetLoading(false);

            Assert.That(_tableContainer.style.display.value, Is.EqualTo(DisplayStyle.Flex));
            Assert.That(_loadingLabel.style.display.value, Is.EqualTo(DisplayStyle.None));
        }

        [Test]
        public void PopulateWith5EntriesFillsAllRows()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = "AAA", score = 1000 },
                new LeaderboardEntry { rank = 2, initials = "BBB", score = 900 },
                new LeaderboardEntry { rank = 3, initials = "CCC", score = 800 },
                new LeaderboardEntry { rank = 4, initials = "DDD", score = 700 },
                new LeaderboardEntry { rank = 5, initials = "EEE", score = 600 }
            };

            _widget.Populate(entries);

            for (int i = 0; i < 5; i++)
            {
                var row = _tableContainer.ElementAt(i);
                var rankLabel = row.Q<Label>($"leaderboard-rank-{i}");
                var initialsLabel = row.Q<Label>($"leaderboard-initials-{i}");
                var scoreLabel = row.Q<Label>($"leaderboard-score-{i}");

                Assert.That(rankLabel.text, Is.EqualTo($"{i + 1}."));
                Assert.That(initialsLabel.text, Is.Not.EqualTo("---"));
                Assert.That(scoreLabel.text, Is.Not.EqualTo("-"));
            }
        }

        [Test]
        public void PopulateHandlesNullInitials()
        {
            var entries = new List<LeaderboardEntry>
            {
                new LeaderboardEntry { rank = 1, initials = null, score = 1000 }
            };

            _widget.Populate(entries);

            var firstRow = _tableContainer.ElementAt(0);
            var firstInitialsLabel = firstRow.Q<Label>("leaderboard-initials-0");

            Assert.That(firstInitialsLabel.text, Is.EqualTo("---"));
        }
    }
}
