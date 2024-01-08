using UnityEngine;

namespace Asteroids.Session {

    [CreateAssetMenu(fileName = nameof(GameSessionStorage), menuName = "Asteroids/" + nameof(GameSessionStorage))]
    public sealed class GameSessionStorage : ScriptableObject {

        [SerializeField] private int _highScore;
        [SerializeField] private int _score;

        public int Score { get => _score; set => _score = value; }
        public int HighScore { get => _highScore; set => _highScore = value; }
    }

}
