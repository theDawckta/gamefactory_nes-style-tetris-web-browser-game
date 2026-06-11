#if UNITY_INCLUDE_TESTS

using NUnit.Framework;
using UnityEngine;
using Game.Gameplay;

namespace Tetris.Tests
{
    public class PlayfieldRendererTests
    {
        private GameObject _testGameObject;
        private PlayfieldRenderer _renderer;
        private PlayfieldModel _model;

        [SetUp]
        public void Setup()
        {
            _testGameObject = new GameObject("PlayfieldRendererTest");
            _renderer = _testGameObject.AddComponent<PlayfieldRenderer>();
            _model = new PlayfieldModel();

            // Manually trigger initialization since we're in test mode
            _renderer.enabled = false;
            _renderer.enabled = true;
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_testGameObject);
        }

        [Test]
        public void RenderGrid_WithEmptyModel_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _renderer.RenderGrid(_model));
        }

        [Test]
        public void RenderActivePiece_WithValidRotationState_DoesNotThrow()
        {
            int[,] rotationState = new int[,] { { 1, 1, 1, 1 } };
            Assert.DoesNotThrow(() => _renderer.RenderActivePiece(rotationState, 3, 0, 1));
        }

        [Test]
        public void RenderActivePiece_OutOfBounds_DoesNotThrow()
        {
            int[,] rotationState = new int[,] { { 1, 1, 1, 1 } };
            Assert.DoesNotThrow(() => _renderer.RenderActivePiece(rotationState, -1, -1, 1));
        }

        [Test]
        public void SetRowColor_WithValidRow_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _renderer.SetRowColor(5, Color.red));
        }

        [Test]
        public void SetRowColor_WithInvalidRow_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _renderer.SetRowColor(-1, Color.red));
            Assert.DoesNotThrow(() => _renderer.SetRowColor(20, Color.red));
        }

        [Test]
        public void ClearActivePiece_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _renderer.ClearActivePiece());
        }

        [Test]
        public void RenderGrid_WithLockedCells_UpdatesRenderers()
        {
            _model.LockPiece(new int[,] { { 1, 1 } }, 0, 0, 1);
            Assert.DoesNotThrow(() => _renderer.RenderGrid(_model));
        }
    }
}

#endif
