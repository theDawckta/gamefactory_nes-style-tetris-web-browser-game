using UnityEngine;

namespace Tetris.UI
{
    public class GameScreen : BaseScreen
    {
        private ScoreWidget _scoreWidget;
        private LevelWidget _levelWidget;
        private LinesWidget _linesWidget;
        private NextPieceWidget _nextPieceWidget;

        protected override void Awake()
        {
            base.Awake();
            var rootElement = GetElement("root");
            _scoreWidget = new ScoreWidget(rootElement);
            _levelWidget = new LevelWidget(rootElement);
            _linesWidget = new LinesWidget(rootElement);
            _nextPieceWidget = new NextPieceWidget(rootElement);
        }

        public void UpdateScore(int score)
        {
            _scoreWidget?.UpdateScore(score);
        }

        public void UpdateLevel(int level)
        {
            _levelWidget?.UpdateLevel(level);
        }

        public void UpdateLines(int lines)
        {
            _linesWidget?.UpdateLines(lines);
        }

        public void UpdateNextPiece(Tetris.Data.PieceType type)
        {
            _nextPieceWidget?.UpdatePiece(type);
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
