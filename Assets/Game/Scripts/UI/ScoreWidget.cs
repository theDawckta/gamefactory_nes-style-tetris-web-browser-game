using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class ScoreWidget
    {
        private readonly Label _scoreValue;

        public ScoreWidget(VisualElement rootElement)
        {
            _scoreValue = rootElement?.Q<Label>("score-value");
        }

        public void UpdateScore(int score)
        {
            if (_scoreValue != null)
                _scoreValue.text = score.ToString();
        }
    }
}
