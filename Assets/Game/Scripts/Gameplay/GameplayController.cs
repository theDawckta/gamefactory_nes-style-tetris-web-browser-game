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

        private PlayfieldModel _playfield;
        private GravityController _gravity;
        private DASController _das;
        private ScoringSystem _scoring;

        private PieceType _currentPieceType;
        private int _currentRotation;
        private int _currentCol;
        private int _currentRow;

        private bool _isPlaying;
        private bool _isInLockDelay;

        public delegate void GameOverEventHandler();
        public delegate void ScoreChangedEventHandler(int score, int level, int lines);

        public event GameOverEventHandler OnGameOver;
        public event ScoreChangedEventHandler OnScoreChanged;

        private const int LineClearAnimationFrames = 30;

        private void Awake()
        {
            _playfield = new PlayfieldModel();
            _gravity = new GravityController();
            _das = new DASController();
            _scoring = new ScoringSystem();
        }

        public void StartGame()
        {
            _playfield.Reset();
            _gravity.SetLevel(0);
            _das.Reset();
            _scoring.Reset();
            _isPlaying = true;
            _isInLockDelay = false;

            SpawnNextPiece();
        }

        public void StopGame()
        {
            _isPlaying = false;
            StopAllCoroutines();
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            // Process input
            bool leftHeld = Keyboard.current.leftArrowKey.isPressed;
            bool rightHeld = Keyboard.current.rightArrowKey.isPressed;
            bool downHeld = Keyboard.current.downArrowKey.isPressed;
            bool rotatePressed = Keyboard.current.zKey.wasPressedThisFrame;

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

            // Apply rotation
            if (rotatePressed)
            {
                PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
                int nextRotation = (_currentRotation + 1) % pieceData.RotationTable.Length;
                int[,] nextRotationState = ConvertRotationState(pieceData.GetRotationState(nextRotation).Grid);

                if (_playfield.IsValidPosition(nextRotationState, _currentCol, _currentRow))
                {
                    _currentRotation = nextRotation;
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
                    _scoring.AddSoftDropScore(1);
                    OnScoreChanged?.Invoke(_scoring.Score, _scoring.Level, _scoring.TotalLines);
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
        }

        private void LockPiece()
        {
            PieceData pieceData = TetrominoData.GetPieceData(_currentPieceType);
            int[,] rotationState = ConvertRotationState(pieceData.GetRotationState(_currentRotation).Grid);

            _playfield.LockPiece(rotationState, _currentCol, _currentRow, pieceData.ColorIndex);
            _isInLockDelay = false;

            int linesCleared = _playfield.ClearFullLines();

            if (linesCleared > 0)
            {
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
            OnScoreChanged?.Invoke(_scoring.Score, _scoring.Level, _scoring.TotalLines);

            SpawnNextPiece();
        }

        private void SpawnNextPiece()
        {
            PieceType newType = (PieceType)Random.Range(0, 7);
            PieceData pieceData = TetrominoData.GetPieceData(newType);

            _currentPieceType = newType;
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
