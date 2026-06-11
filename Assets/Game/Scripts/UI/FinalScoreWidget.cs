using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class FinalScoreWidget
    {
        private readonly Label _finalScoreValue;
        private readonly Label _finalLevelValue;

        public FinalScoreWidget(VisualElement rootElement)
        {
            _finalScoreValue = rootElement?.Q<Label>("final-score-value");
            _finalLevelValue = rootElement?.Q<Label>("final-level-value");
        }

        public void SetFinalScore(int score, int level)
        {
            if (_finalScoreValue != null)
                _finalScoreValue.text = score.ToString();
            if (_finalLevelValue != null)
                _finalLevelValue.text = level.ToString();
        }
    }
}
