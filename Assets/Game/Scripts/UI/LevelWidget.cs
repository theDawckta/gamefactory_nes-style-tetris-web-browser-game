using UnityEngine.UIElements;

namespace Tetris.UI
{
    public class LevelWidget
    {
        private readonly Label _levelValue;

        public LevelWidget(VisualElement rootElement)
        {
            _levelValue = rootElement?.Q<Label>("level-value");
        }

        public void UpdateLevel(int level)
        {
            if (_levelValue != null)
                _levelValue.text = level.ToString();
        }
    }
}
