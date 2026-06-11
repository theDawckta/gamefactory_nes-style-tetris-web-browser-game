namespace Game.Gameplay
{
    public class ScoringSystem
    {
        private int _score;
        private int _level;
        private int _totalLines;

        private const int LinesPerLevel = 10;

        private static readonly int[] BaseScores = { 40, 100, 300, 1200 };

        public int Score => _score;
        public int Level => _level;
        public int TotalLines => _totalLines;

        public ScoringSystem()
        {
            Reset();
        }

        public void AddLines(int linesCleared)
        {
            if (linesCleared < 1 || linesCleared > 4)
                return;

            int baseScore = BaseScores[linesCleared - 1];
            int pointsAwarded = baseScore * (_level + 1);

            _score += pointsAwarded;
            _totalLines += linesCleared;

            int newLevel = _totalLines / LinesPerLevel;
            _level = newLevel;
        }

        public void Reset()
        {
            _score = 0;
            _level = 0;
            _totalLines = 0;
        }
    }
}
