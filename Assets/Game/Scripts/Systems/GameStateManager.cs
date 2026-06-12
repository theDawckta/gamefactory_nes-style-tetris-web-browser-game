using System.Collections.Generic;
using UnityEngine;
using OneTimeGames.CoreSystems;
using Game.Gameplay;
using Tetris.UI;
using Tetris.Services;

namespace Tetris.Systems
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField] private GameplayController _gameplayController;
        [SerializeField] private StartScreen _startScreen;
        [SerializeField] private GameScreen _gameScreen;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private InitialsEntryScreen _initialsEntryScreen;
        [SerializeField] private LeaderboardService _leaderboardService;

        private readonly GameStateMachine _stateMachine = new GameStateMachine();

        private int _cachedFinalScore;
        private int _cachedFinalLevel;
        private List<LeaderboardEntry> _cachedLeaderboard;

        private const string StateStartScreen = "StartScreen";
        private const string StatePlaying = "Playing";
        private const string StateGameOver = "GameOver";
        private const string StateInitialsEntry = "InitialsEntry";

        private void Start()
        {
            _stateMachine.RegisterState(StateStartScreen, OnEnterStartScreen, null, null);
            _stateMachine.RegisterState(StatePlaying, OnEnterPlaying, null, null);
            _stateMachine.RegisterState(StateGameOver, OnEnterGameOver, null, null);
            _stateMachine.RegisterState(StateInitialsEntry, OnEnterInitialsEntry, null, null);

            _startScreen.OnStartPressed += HandleStartPressed;
            _gameplayController.OnGameOver += HandleGameOver;
            _gameplayController.OnScoreChanged += HandleScoreChanged;
            _gameOverScreen.OnTopScoreAchieved.AddListener(HandleTopScoreAchieved);
            _gameOverScreen.OnContinuePressed.AddListener(HandleContinuePressed);
            _initialsEntryScreen.OnInitialsConfirmed.AddListener(HandleInitialsConfirmed);

            _stateMachine.TransitionTo(StateStartScreen);
        }

        private void OnDestroy()
        {
            if (_startScreen != null) _startScreen.OnStartPressed -= HandleStartPressed;
            if (_gameplayController != null)
            {
                _gameplayController.OnGameOver -= HandleGameOver;
                _gameplayController.OnScoreChanged -= HandleScoreChanged;
            }
            if (_gameOverScreen != null)
            {
                _gameOverScreen.OnTopScoreAchieved.RemoveListener(HandleTopScoreAchieved);
                _gameOverScreen.OnContinuePressed.RemoveListener(HandleContinuePressed);
            }
            if (_initialsEntryScreen != null)
                _initialsEntryScreen.OnInitialsConfirmed.RemoveListener(HandleInitialsConfirmed);
        }

        private void HandleStartPressed() => _stateMachine.TransitionTo(StatePlaying);
        private void HandleGameOver() => _stateMachine.TransitionTo(StateGameOver);
        private void HandleTopScoreAchieved() => _stateMachine.TransitionTo(StateInitialsEntry);
        private void HandleContinuePressed() => _stateMachine.TransitionTo(StateStartScreen);
        private void HandleInitialsConfirmed() => _stateMachine.TransitionTo(StateStartScreen);

        private void HandleScoreChanged(int score, int level, int lines)
        {
            _cachedFinalScore = score;
            _cachedFinalLevel = level;
        }

        private void OnEnterStartScreen()
        {
            _gameOverScreen?.Hide();
            _gameScreen?.Hide();
            _initialsEntryScreen?.Hide();
            _startScreen?.Show();

            if (_leaderboardService != null)
                _leaderboardService.FetchScores(scores => _cachedLeaderboard = scores);
        }

        private void OnEnterPlaying()
        {
            _startScreen?.Hide();
            _gameOverScreen?.Hide();
            _initialsEntryScreen?.Hide();
            _gameScreen?.Show();
            _gameplayController?.StartGame();
        }

        private void OnEnterGameOver()
        {
            _gameplayController?.StopGame();
            _startScreen?.Hide();
            _gameScreen?.Hide();
            _initialsEntryScreen?.Hide();
            _gameOverScreen?.Show(_cachedFinalScore, _cachedFinalLevel, _cachedLeaderboard);
        }

        private void OnEnterInitialsEntry()
        {
            _gameOverScreen?.Hide();
            _startScreen?.Hide();
            _gameScreen?.Hide();
            _initialsEntryScreen?.Show(_cachedFinalScore);
        }
    }
}
