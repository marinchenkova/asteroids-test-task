using Asteroids.Session;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Asteroids.UI {

    public sealed class MainMenuUIController : MonoBehaviour {

        [Header("Settings")]
        [SerializeField] private GameSessionStorage _gameSessionStorage;
        [SerializeField] private string _mainMenuScene;
        [SerializeField] private string _gameplayScene;

        [Header("UI Components")]
        [SerializeField] private Button _buttonPlay;
        [SerializeField] private TMP_Text _textScore;
        [SerializeField] private TMP_Text _textHighScore;

        private Button.ButtonClickedEvent _playClick;

        private void Awake() {
            _playClick = new Button.ButtonClickedEvent();
            _buttonPlay.onClick = _playClick;

            _textScore.text = AsteroidsUIExtensions.FormatScore(_gameSessionStorage.Score);
            _textHighScore.text = AsteroidsUIExtensions.FormatScore(_gameSessionStorage.HighScore);
        }

        private void OnEnable() {
            _playClick.AddListener(OnPlayButtonClick);
        }

        private void OnDisable() {
            _playClick.RemoveAllListeners();
        }

        private void OnPlayButtonClick() {
            SceneManager.UnloadSceneAsync(_mainMenuScene);
            SceneManager.LoadScene(_gameplayScene, LoadSceneMode.Additive);
        }
    }

}
