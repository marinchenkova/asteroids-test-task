using Asteroids.Components;
using Asteroids.Session;
using Entities.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Asteroids.UI {

    public sealed class GameplayUIController : MonoBehaviour {

        [Header("Settings")]
        [SerializeField] private GameSessionStorage _gameSessionStorage;
        [SerializeField] private GameSessionLauncher _gameSessionLauncher;
        [SerializeField] private string _gameplayScene;
        [SerializeField] private string _mainMenuScene;

        [Header("UI Components")]
        [SerializeField] private TMP_Text _textHighScore;
        [SerializeField] private TMP_Text _textScore;
        [SerializeField] private TMP_Text _textGameOver;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonMainMenu;

        private Button.ButtonClickedEvent _restartClick;
        private Button.ButtonClickedEvent _mainMenuClick;
        private bool _isPlayerAlive;
        private int _lastScore;
        private int _lastHighScore;

        private void Awake() {
            _restartClick = new Button.ButtonClickedEvent();
            _mainMenuClick = new Button.ButtonClickedEvent();

            _buttonRestart.onClick = _restartClick;
            _buttonMainMenu.onClick = _mainMenuClick;

            _textHighScore.text = AsteroidsUIExtensions.FormatScore(_gameSessionStorage.HighScore);
        }

        private void OnEnable() {
            _restartClick.AddListener(OnRestartButtonClick);
            _mainMenuClick.AddListener(OnMainMenuButtonClick);
        }

        private void OnDisable() {
            _restartClick.RemoveAllListeners();
            _mainMenuClick.RemoveAllListeners();
        }

        private void LateUpdate() {
            FetchGameState();
        }

        private void FetchGameState() {
            var gameStateComponent = _gameSessionLauncher.GameState.GetComponent<GameStateComponent>();

            var player = gameStateComponent?.Player ?? default;
            bool isPlayerAlive = player.IsAlive();

            if (isPlayerAlive != _isPlayerAlive) {
                _isPlayerAlive = isPlayerAlive;
                ShowGameOver(!_isPlayerAlive);
            }

            int score = _gameSessionStorage.Score;
            if (score != _lastScore) {
                _lastScore = score;
                OnScoreChanged(_lastScore);
            }

            int highScore = _gameSessionStorage.HighScore;
            if (highScore != _lastHighScore) {
                _lastHighScore = highScore;
                OnHighScoreChanged(_lastHighScore);
            }
        }

        private void OnRestartButtonClick() {
            _gameSessionLauncher.RestartGameSession();
        }

        private void OnMainMenuButtonClick() {
            SceneManager.UnloadSceneAsync(_gameplayScene);
            SceneManager.LoadScene(_mainMenuScene, LoadSceneMode.Additive);
        }

        private void ShowGameOver(bool show) {
            _textGameOver.gameObject.SetActive(show);
            _buttonRestart.gameObject.SetActive(show);
            _buttonMainMenu.gameObject.SetActive(show);
        }

        private void OnScoreChanged(int value) {
            _textScore.text = AsteroidsUIExtensions.FormatScore(value);
        }

        private void OnHighScoreChanged(int value) {
            _textHighScore.text = AsteroidsUIExtensions.FormatScore(value);
        }
    }

}
