using UnityEngine;

namespace Tetris.UI
{
    public class GameScreen : BaseScreen
    {
        private ScoreWidget _scoreWidget;

        protected override void Awake()
        {
            base.Awake();
            var rootElement = GetElement("");
            _scoreWidget = new ScoreWidget(rootElement);
        }

        public void UpdateScore(int score)
        {
            _scoreWidget?.UpdateScore(score);
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
