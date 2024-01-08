using Asteroids.Components;
using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Asteroids.Session {

    public sealed class GameSessionLauncher : MonoBehaviour {

        [SerializeField] private WorldLauncher _worldLauncher;
        [SerializeField] private bool _launchAtStart;
        [SerializeReference] private IEntityComponent[] _gameStateComponents;

        public Entity GameState { get; private set; }

        private void OnEnable() {
            var world = _worldLauncher.World;
            if (world == null) return;

            world.OnCreateEntity -= OnCreateEntity;
            world.OnCreateEntity += OnCreateEntity;

            world.OnDestroyEntity -= OnDestroyEntity;
            world.OnDestroyEntity += OnDestroyEntity;
        }

        private void OnDisable() {
            var world = _worldLauncher.World;
            if (world == null) return;

            world.OnCreateEntity -= OnCreateEntity;
            world.OnDestroyEntity -= OnDestroyEntity;
        }

        private void Start() {
            var world = _worldLauncher.World;
            if (world == null) return;

            world.OnCreateEntity -= OnCreateEntity;
            world.OnCreateEntity += OnCreateEntity;

            world.OnDestroyEntity -= OnDestroyEntity;
            world.OnDestroyEntity += OnDestroyEntity;

            if (_launchAtStart) RestartGameSession();
        }

        private void OnDestroy() {
            DestroyGameSession();
        }

        public void RestartGameSession() {
            DestroyGameSession();
            CreateGameState();
        }

        public void DestroyGameSession() {
            _worldLauncher.World.DestroyAll();
        }

        private void CreateGameState() {
            GameState = _worldLauncher.World.CreateEntity();

            for (int i = 0; i < _gameStateComponents.Length; i++) {
                GameState.SetComponentCopy(_gameStateComponents[i]);
            }
        }

        private void OnCreateEntity(Entity entity) {
            if (!GameState.IsAlive() || entity == GameState) return;

            entity.SetComponent(new GameStateReferenceComponent { GameState = GameState });
        }

        private void OnDestroyEntity(Entity entity) {

        }
    }

}
