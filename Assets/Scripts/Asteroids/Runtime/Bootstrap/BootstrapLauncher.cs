using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids.Bootstrap {

    public sealed class BootstrapLauncher : MonoBehaviour {

        [SerializeField] private string _startScene;
        [SerializeField] [Range(1, 240)] private int _targetFrameRate = 120;

        private void Awake() {
            Application.targetFrameRate = _targetFrameRate;
        }

        private void Start() {
            SceneManager.LoadScene(_startScene, LoadSceneMode.Additive);
        }

        private void OnValidate() {
            Application.targetFrameRate = _targetFrameRate;
        }
    }

}
