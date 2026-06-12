using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Gameplay;
using Tetris.Data;
using System.Collections;

namespace Game.Gameplay
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private PlayfieldRenderer _renderer;

        private PlayfieldModel _playfield = new PlayfieldModel();
        private GravityController _gravity = new GravityController();
        private DASController _das = new DASController();
        private ScoringSystem _scoring = new ScoringSystem();

        private PieceType _currentPieceType;
        private PieceType _nextPieceType;
        private PieceType _previousPieceType = (PieceType)7;
        private int _currentRotation;
        private int _currentCol;
        private int _currentRow;

        private bool _isPlaying;
        private bool _isInLockDelay;
        private bool _isInLineClearAnimation;

        public delegate void GameOverEventHandler();
        public delegate void ScoreChangedEventHandler(int score, int level, int lines);

        public event GameOverEventHandler OnGameOver;
        public event ScoreChangedEventHandler OnScoreChanged;
        public event Action<PieceType> OnNextPieceChanged;

        private const int LineClearAnimationFrames = 30;

        private void Start()
        {
            _renderer?.gameObject.SetActive(false);
        }

        public void StartGame()
        {
            _renderer?.gameObject.SetActive(true);
            _playfield.Reset();
            _gravity.SetLevel(0);
            _das.Reset();
            _scoring.Reset();
            _isPlaying = true;
            _isInLockDelay = false;
            _isInLineClearAnimation = false;
            _previousPieceType = (PieceType)7;
            _nextPieceType = NesRandomPiece();

            _renderer?.RenderGrid(_playfield);
            SpawnNextPiece();
        }

        public void StopGame()
        {
            _isPlaying = false;
            StopAllCoroutines();
            _renderer?.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            // Process input
            bool leftHeld = Keyboard.current.leftArrowKey.isPressed;
            bool rightHeld = Keyboard.current.rightArrowKey.isPressed;
            bool downHeld = Keyboard.current.downArrowKey.isPressed;
            bool rotatePressed = Keyboard.current.zKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame;

            _das.Update(leftHeld, rightHeld);

            // Apply horizontal movement from DAS
            int horizontalDelta = _das.GetHorizontalDelta();
            if (horizontalDelta != 0)
            {
                int newCol = _currentCol + horizontalDelta;
                PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
                int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(_currentRotation).Grid);

                if (_playfield.IsValidPosition(rotationState, newCol, _currentRow))
                {
                    _currentCol = newCol;
                }
            }

            // Apply rotation with wall kicks
            if (rotatePressed)
            {
                PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
                int nextRotation = (_currentRotation + 1) % pieceData.RotationTable.Length;
                int[,] nextRotationState = ConvertRotationState(pieceData.GetRotationState(nextRotation).Grid);

                foreach (int kick in new[] { 0, -1, 1, -2, 2 })
                {
                    if (_playfield.IsValidPosition(nextRotationState, _currentCol + kick, _currentRow))
                    {
                        _currentCol += kick;
                        _currentRotation = nextRotation;
                        break;
                    }
                }
            }

            // If in lock delay but piece can now fall (after a move/rotation), cancel the lock delay
            if (_isInLockDelay)
            {
                PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
                int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(_currentRotation).Grid);
                if (_playfield.IsValidPosition(rotationState, _currentCol, _currentRow + 1))
                {
                    _isInLockDelay = false;
                    _gravity.ResetLockDelay();
                }
            }

            // Apply soft drop (down key held): drop one row per frame, score 1 point per row soft-dropped
            if (downHeld)
            {
                PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
                int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(_currentRotation).Grid);

                if (_playfield.IsValidPosition(rotationState, _currentCol, _currentRow + 1))
                {
                    _currentRow++;
                    _gravity.ResetDropTimer();
                }
            }

            // Apply gravity
            if (!_isInLockDelay && _gravity.Tick())
            {
                PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
                int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(_currentRotation).Grid);

                if (_playfield.IsValidPosition(rotationState, _currentCol, _currentRow + 1))
                {
                    _currentRow++;
                }
                else
                {
                    // Can't drop further, enter lock delay
                    _isInLockDelay = true;
                    _gravity.ResetLockDelay();
                }
            }

            // Apply lock delay
            if (_isInLockDelay && _gravity.TickLockDelay())
            {
                LockPiece();
            }

            // Render active piece each frame (skip during line clear animation)
            if (_isPlaying && !_isInLineClearAnimation)
            {
                PieceData activePieceData = TetrominoData.GetPieceData(_currentPieceType);
                int[,] activeState = ConvertRotationState(activePieceData.GetRotationState(_currentRotation).Grid);
                _renderer?.RenderActivePiece(activeState, _currentCol, _currentRow, activePieceData.ColorIndex);
            }
        }

        private void LockPiece()
        {
            PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
            int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(_currentRotation).Grid);

            _playfield.LockPiece(rotationState, _currentCol, _currentRow, pieceData.ColorIndex);
            _isInLockDelay = false;

            _renderer?.ClearActivePiece();
            _renderer?.RenderGrid(_playfield);

            int linesCleared = _playfield.ClearFullLines();

            if (linesCleared > 0)
            {
                _isInLineClearAnimation = true;
                StartCoroutine(LineCleanAnimationCoroutine(linesCleared));
            }
            else
            {
                SpawnNextPiece();
            }
        }

        private IEnumerator LineCleanAnimationCoroutine(int linesCleared)
        {
            // 30-frame delay for line clear animation; tiles in clearing rows should be white
            // TODO: Set clearing rows to white via _renderer
            for (int i = 0; i < LineClearAnimationFrames; i++)
            {
                yield return null;
            }

            _scoring.AddLines(linesCleared);
            _gravity.SetLevel(_scoring.Level);
            OnScoreChanged?.Invoke(_scoring.Score, _scoring.Level, _scoring.TotalLines);

            _renderer?.RenderGrid(_playfield);
            SpawnNextPiece();
        }

        private void SpawnNextPiece()
        {
            _isInLineClearAnimation = false;
            PieceType newType = _nextPieceType;
            PieceData pieceData = TetrominoData.GetPieceData(newType);

            _previousPieceType = newType;
            _currentPieceType = newType;
            _nextPieceType = NesRandomPiece();
            OnNextPieceChanged?.Invoke(_nextPieceType);
            _currentRotation = 0;
            _currentCol = pieceData.SpawnColumn;
            _currentRow = pieceData.SpawnRow;

            int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(0).Grid);

            // Check for top-out (spawn position blocked after lock)
            if (!_playfield.IsValidPosition(rotationState, _currentCol, _currentRow))
            {
                _isPlaying = false;
                OnGameOver?.Invoke();
            }

            _gravity.ResetDropTimer();
            _gravity.ResetLockDelay();
        }

        private PieceType NesRandomPiece()
        {
            int roll = UnityEngine.Random.Range(0, 8);
            if (roll == (int)_previousPieceType || roll == 7)
                roll = UnityEngine.Random.Range(0, 7);
            return (PieceType)roll;
        }

        private int[,] ConvertRotationState(bool[,] boolGrid)
        {
            int rows = boolGrid.GetLength(0);
            int cols = boolGrid.GetLength(1);
            int[,] intGrid = new int[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    intGrid[r, c] = boolGrid[r, c] ? 1 : 0;
                }
            }

            return intGrid;
        }
    }
}
