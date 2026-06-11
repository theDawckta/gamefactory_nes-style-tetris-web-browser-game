using UnityEngine;

namespace Tetris.UI
{
    public class GameScreen : BaseScreen
    {
        private ScoreWidget _scoreWidget;
        private LevelWidget _levelWidget;

        protected override void Awake()
        {
            base.Awake();
            var rootElement = GetElement("");
            _scoreWidget = new ScoreWidget(rootElement);
            _levelWidget = new LevelWidget(rootElement);
        }

        public void UpdateScore(int score)
        {
            _scoreWidget?.UpdateScore(score);
        }

        public void UpdateLevel(int level)
        {
            _levelWidget?.UpdateLevel(level);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
