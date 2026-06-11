#if UNITY_EDITOR
using NUnit.Framework;
using Game.Gameplay;

namespace Tetris.Tests
{
    public class DASControllerTests
    {
        private DASController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new DASController();
        }

        [Test]
        public void FirstFrame_LeftHeld_OutputsMoveLeft()
        {
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void FirstFrame_RightHeld_OutputsMoveRight()
        {
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void FirstFrame_NoInput_OutputsZero()
        {
            _controller.Update(false, false);
            Assert.AreEqual(0, _controller.GetHorizontalDelta());
        }

        [Test]
        public void InitialDelay_LeftHeld_NoOutputUntilAfterDelay()
        {
            // First frame outputs move
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Frames 2-16 (15 frames) should output zero
            for (int i = 0; i < 15; i++)
            {
                _controller.Update(true, false);
                Assert.AreEqual(0, _controller.GetHorizontalDelta(), $"Frame {i + 2} should output 0");
            }

            // Frame 17 (after 16-frame delay) should output move
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void InitialDelay_RightHeld_NoOutputUntilAfterDelay()
        {
            // First frame outputs move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());

            // Frames 2-16 should output zero
            for (int i = 0; i < 15; i++)
            {
                _controller.Update(false, true);
                Assert.AreEqual(0, _controller.GetHorizontalDelta(), $"Frame {i + 2} should output 0");
            }

            // Frame 17 should output move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void RepeatInterval_AfterInitialDelay_OutputsEvery6Frames()
        {
            // First frame outputs move
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Skip 15 frames of initial delay
            for (int i = 0; i < 15; i++)
            {
                _controller.Update(true, false);
            }

            // Frame 17 (after delay) outputs move
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Frames 18-22 should output zero (5 frames)
            for (int i = 0; i < 5; i++)
            {
                _controller.Update(true, false);
                Assert.AreEqual(0, _controller.GetHorizontalDelta(), $"Frame {18 + i} should output 0");
            }

            // Frame 23 should output move (6 frames after frame 17)
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Next cycle: frames 24-28 should output zero
            for (int i = 0; i < 5; i++)
            {
                _controller.Update(true, false);
                Assert.AreEqual(0, _controller.GetHorizontalDelta(), $"Frame {24 + i} should output 0");
            }

            // Frame 29 should output move again
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void BothDirectionsHeld_OutputsZero()
        {
            _controller.Update(true, true);
            Assert.AreEqual(0, _controller.GetHorizontalDelta());

            for (int i = 0; i < 20; i++)
            {
                _controller.Update(true, true);
                Assert.AreEqual(0, _controller.GetHorizontalDelta(), $"Frame {i + 2} should output 0 when both held");
            }
        }

        [Test]
        public void ReleaseAndRepress_Left_ResetsDelay()
        {
            // Hold left, get first move
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Wait through some of the initial delay
            for (int i = 0; i < 8; i++)
            {
                _controller.Update(true, false);
            }

            // Release left
            _controller.Update(false, false);
            Assert.AreEqual(0, _controller.GetHorizontalDelta());

            // Re-press left - should output move immediately (first frame of new press)
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void ReleaseAndRepress_Right_ResetsDelay()
        {
            // Hold right, get first move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());

            // Wait through some of the initial delay
            for (int i = 0; i < 8; i++)
            {
                _controller.Update(false, true);
            }

            // Release right
            _controller.Update(false, false);
            Assert.AreEqual(0, _controller.GetHorizontalDelta());

            // Re-press right - should output move immediately
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void Reset_ClearsAllState()
        {
            // Set up some state
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            for (int i = 0; i < 10; i++)
            {
                _controller.Update(true, false);
            }

            // Reset
            _controller.Reset();

            // After reset, no input should produce zero
            _controller.Update(false, false);
            Assert.AreEqual(0, _controller.GetHorizontalDelta());

            // New press should produce move
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void Reset_DuringHold_StopsOutput()
        {
            // Hold left
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Reset while holding
            _controller.Reset();

            // Input is still held, but state is cleared
            // The next Update with held=true should be treated as a new press
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void SwitchDirection_MidDelay_ResetsDelay()
        {
            // Hold left
            _controller.Update(true, false);
            Assert.AreEqual(-1, _controller.GetHorizontalDelta());

            // Wait through 8 frames of initial delay
            for (int i = 0; i < 8; i++)
            {
                _controller.Update(true, false);
            }

            // Switch to right (release left, hold right)
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());

            // Wait through initial delay for right
            for (int i = 0; i < 15; i++)
            {
                _controller.Update(false, true);
                Assert.AreEqual(0, _controller.GetHorizontalDelta(), $"Frame {i + 2} should output 0 during new initial delay");
            }

            // Next frame should output right move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());
        }

        [Test]
        public void NoInput_Always_OutputsZero()
        {
            for (int i = 0; i < 50; i++)
            {
                _controller.Update(false, false);
                Assert.AreEqual(0, _controller.GetHorizontalDelta());
            }
        }

        [Test]
        public void RepeatInterval_Consistent_RightDirection()
        {
            // First frame outputs move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());

            // Skip 15 frames of initial delay
            for (int i = 0; i < 15; i++)
            {
                _controller.Update(false, true);
            }

            // Frame 17 outputs move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());

            // Frames 18-22 output zero
            for (int i = 0; i < 5; i++)
            {
                _controller.Update(false, true);
                Assert.AreEqual(0, _controller.GetHorizontalDelta());
            }

            // Frame 23 outputs move
            _controller.Update(false, true);
            Assert.AreEqual(1, _controller.GetHorizontalDelta());
        }
    }
}
#endif
