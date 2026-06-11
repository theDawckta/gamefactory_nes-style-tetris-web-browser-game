namespace Game.Gameplay
{
    public class DASController
    {
        private const int InitialDelayFrames = 16;
        private const int RepeatIntervalFrames = 6;

        private bool _leftHeld;
        private bool _rightHeld;
        private bool _leftActive;
        private bool _rightActive;
        private int _delayCounter;
        private int _repeatCounter;
        private int _horizontalDelta;

        public DASController()
        {
            Reset();
        }

        public void Update(bool leftHeld, bool rightHeld)
        {
            _horizontalDelta = 0;

            // Detect direction changes
            bool leftPressed = leftHeld && !_leftHeld;
            bool rightPressed = rightHeld && !_rightHeld;
            bool leftReleased = !leftHeld && _leftHeld;
            bool rightReleased = !rightHeld && _rightHeld;

            _leftHeld = leftHeld;
            _rightHeld = rightHeld;

            // If both directions are held, no output
            if (leftHeld && rightHeld)
            {
                _leftActive = false;
                _rightActive = false;
                _delayCounter = 0;
                _repeatCounter = 0;
                return;
            }

            // Handle releases
            if (leftReleased)
            {
                _leftActive = false;
                _delayCounter = 0;
                _repeatCounter = 0;
            }

            if (rightReleased)
            {
                _rightActive = false;
                _delayCounter = 0;
                _repeatCounter = 0;
            }

            // Handle new presses
            if (leftPressed)
            {
                _leftActive = true;
                _rightActive = false;
                _delayCounter = 0;
                _repeatCounter = 0;
                _horizontalDelta = -1;
                return;
            }

            if (rightPressed)
            {
                _rightActive = true;
                _leftActive = false;
                _delayCounter = 0;
                _repeatCounter = 0;
                _horizontalDelta = 1;
                return;
            }

            // Handle held direction
            if (_leftActive && leftHeld)
            {
                _delayCounter++;

                if (_delayCounter == InitialDelayFrames)
                {
                    _horizontalDelta = -1;
                    _repeatCounter = 0;
                }
                else if (_delayCounter > InitialDelayFrames)
                {
                    _repeatCounter++;
                    if (_repeatCounter >= RepeatIntervalFrames)
                    {
                        _repeatCounter = 0;
                        _horizontalDelta = -1;
                    }
                }
            }
            else if (_rightActive && rightHeld)
            {
                _delayCounter++;

                if (_delayCounter == InitialDelayFrames)
                {
                    _horizontalDelta = 1;
                    _repeatCounter = 0;
                }
                else if (_delayCounter > InitialDelayFrames)
                {
                    _repeatCounter++;
                    if (_repeatCounter >= RepeatIntervalFrames)
                    {
                        _repeatCounter = 0;
                        _horizontalDelta = 1;
                    }
                }
            }
        }

        public int GetHorizontalDelta()
        {
            return _horizontalDelta;
        }

        public void Reset()
        {
            _leftHeld = false;
            _rightHeld = false;
            _leftActive = false;
            _rightActive = false;
            _delayCounter = 0;
            _repeatCounter = 0;
            _horizontalDelta = 0;
        }
    }
}
