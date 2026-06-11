#if UNITY_EDITOR
using NUnit.Framework;
using Game.Gameplay;

namespace Tetris.Tests
{
    public class GravityControllerTests
    {
        private GravityController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new GravityController();
        }

        [Test]
        public void SetLevel_UpdatesGravityRate()
        {
            _controller.SetLevel(0);
            for (int i = 0; i < 47; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level0_Returns48FrameInterval()
        {
            _controller.SetLevel(0);
            for (int i = 0; i < 47; i++)
            {
                Assert.IsFalse(_controller.Tick(), $"Frame {i} should not trigger drop");
            }
            Assert.IsTrue(_controller.Tick(), "Frame 47 (0-indexed) should trigger drop");
        }

        [Test]
        public void Tick_Level1_Returns43FrameInterval()
        {
            _controller.SetLevel(1);
            for (int i = 0; i < 42; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level8_Returns8FrameInterval()
        {
            _controller.SetLevel(8);
            for (int i = 0; i < 7; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level9_Returns6FrameInterval()
        {
            _controller.SetLevel(9);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level10_Returns5FrameInterval()
        {
            _controller.SetLevel(10);
            for (int i = 0; i < 4; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level19_Returns2FrameInterval()
        {
            _controller.SetLevel(19);
            Assert.IsFalse(_controller.Tick());
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level29_Returns1FrameInterval()
        {
            _controller.SetLevel(29);
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_Level30Plus_Returns1FrameInterval()
        {
            _controller.SetLevel(50);
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void ResetDropTimer_ResetsFrameCounter()
        {
            _controller.SetLevel(0);
            for (int i = 0; i < 30; i++)
            {
                _controller.Tick();
            }
            _controller.ResetDropTimer();

            for (int i = 0; i < 47; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void Tick_AfterDrop_RestartsCycle()
        {
            _controller.SetLevel(0);
            for (int i = 0; i < 48; i++)
            {
                _controller.Tick();
            }

            for (int i = 0; i < 47; i++)
            {
                Assert.IsFalse(_controller.Tick());
            }
            Assert.IsTrue(_controller.Tick());
        }

        [Test]
        public void TickLockDelay_Returns30FrameInterval()
        {
            for (int i = 0; i < 29; i++)
            {
                Assert.IsFalse(_controller.TickLockDelay(), $"Frame {i} should not expire lock delay");
            }
            Assert.IsTrue(_controller.TickLockDelay(), "Frame 29 (0-indexed) should expire lock delay");
        }

        [Test]
        public void TickLockDelay_ContinuesAfterExpiry()
        {
            for (int i = 0; i < 30; i++)
            {
                _controller.TickLockDelay();
            }

            Assert.IsTrue(_controller.TickLockDelay());
            Assert.IsTrue(_controller.TickLockDelay());
        }

        [Test]
        public void ResetLockDelay_ResetsCounter()
        {
            for (int i = 0; i < 20; i++)
            {
                _controller.TickLockDelay();
            }
            _controller.ResetLockDelay();

            for (int i = 0; i < 29; i++)
            {
                Assert.IsFalse(_controller.TickLockDelay());
            }
            Assert.IsTrue(_controller.TickLockDelay());
        }

        [Test]
        public void LockDelay_NotResetByMovement()
        {
            // Simulate piece moving while resting: TickLockDelay should not reset
            for (int i = 0; i < 10; i++)
            {
                _controller.TickLockDelay();
            }

            // Movement happens (simulated) - but we don't call ResetLockDelay

            // Continue ticking lock delay
            for (int i = 0; i < 20; i++)
            {
                _controller.TickLockDelay();
            }

            Assert.IsTrue(_controller.TickLockDelay());
        }

        [Test]
        public void GravityController_AllTableValues()
        {
            int[] expectedTable = new int[]
            {
                48, 43, 38, 33, 28, 23, 18, 13, 8, 6,
                5, 5, 5, 4, 4, 4, 3, 3, 3, 2,
                2, 2, 2, 2, 2, 2, 2, 2, 2, 1
            };

            for (int level = 0; level < expectedTable.Length; level++)
            {
                _controller.SetLevel(level);
                int framesPerDrop = expectedTable[level];

                for (int i = 0; i < framesPerDrop - 1; i++)
                {
                    Assert.IsFalse(_controller.Tick(), $"Level {level}: Frame {i} should not trigger drop");
                }
                Assert.IsTrue(_controller.Tick(), $"Level {level}: Frame {framesPerDrop - 1} should trigger drop");

                _controller.ResetDropTimer();
            }
        }
    }
}
#endif
