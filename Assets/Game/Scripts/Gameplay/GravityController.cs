namespace Game.Gameplay
{
    public class GravityController
    {
        // NES Tetris gravity table: frames per row drop, indexed by level 0-29+
        private static readonly int[] GravityTable = new int[]
        {
            48, 43, 38, 33, 28, 23, 18, 13, 8, 6,
            5, 5, 5, 4, 4, 4, 3, 3, 3, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 1
        };

        private const int LockDelayFrames = 30;

        private int _currentLevel;
        private int _framesPerRowDrop;
        private int _dropFrameCounter;
        private int _lockDelayCounter;

        public GravityController()
        {
            _currentLevel = 0;
            _framesPerRowDrop = GravityTable[0];
            _dropFrameCounter = 0;
            _lockDelayCounter = 0;
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
            // Level 29+ uses 1 frame per row
            _framesPerRowDrop = level >= GravityTable.Length ? 1 : GravityTable[level];
        }

        public bool Tick()
        {
            _dropFrameCounter++;

            if (_dropFrameCounter >= _framesPerRowDrop)
            {
                _dropFrameCounter = 0;
                return true;
            }

            return false;
        }

        public void ResetDropTimer()
        {
            _dropFrameCounter = 0;
        }

        public bool TickLockDelay()
        {
            _lockDelayCounter++;

            if (_lockDelayCounter >= LockDelayFrames)
            {
                return true;
            }

            return false;
        }

        public void ResetLockDelay()
        {
            _lockDelayCounter = 0;
        }
    }
}
