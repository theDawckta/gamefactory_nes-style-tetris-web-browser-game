using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class LinesWidget
    {
        private readonly Label _linesValue;

        public LinesWidget(VisualElement rootElement)
        {
            _linesValue = rootElement?.Q<Label>("lines-value");
        }

        public void UpdateLines(int lines)
        {
            if (_linesValue != null)
                _linesValue.text = lines.ToString();
        }
    }
}
