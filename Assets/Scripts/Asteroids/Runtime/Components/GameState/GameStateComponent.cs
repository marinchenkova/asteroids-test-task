using System;
using Asteroids.Session;
using Entities.Core;
using Entities.Unity;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class GameStateComponent : IEntityComponent, IUpdatable {

        [SerializeField] private GameSessionStorage _gameSessionStorage;
        [SerializeField] private EntityPrefab _playerPrefab;
        [SerializeField] private EntityPrefab[] _gameEntityPrefabs;

        public Entity Player { get; private set; }
        public int Score { get => _score; set => SetScore(value); }

        private Entity _entity;
        private Entity[] _gameEntities;
        private int _score;

        public void OnAttach(Entity entity) {
            _entity = entity;

            SetScore(0);
            CreatePlayer();
            CreateGameEntities();

            _entity.world.SubscribeUpdate(this);
        }

        public void OnDetach(Entity entity) {
            DestroyPlayer();
            DestroyGameEntities();

            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
        }

        public void OnUpdate(float dt) {
            if (Player.IsAlive()) return;

            Player = default;
            _entity.world.UnsubscribeUpdate(this);

            DestroyGameEntities();
        }

        private void CreatePlayer() {
            Player = _entity.world.CreateEntity();
            _playerPrefab.CopyComponentsInto(Player);
        }

        private void DestroyPlayer() {
            if (!Player.IsAlive()) return;

            Player.Destroy();
            Player = default;
        }

        private void CreateGameEntities() {
            _gameEntities = new Entity[_gameEntityPrefabs.Length];

            for (int i = 0; i < _gameEntityPrefabs.Length; i++) {
                var entity = _entity.world.CreateEntity();

                _gameEntityPrefabs[i].CopyComponentsInto(entity);
                _gameEntities[i] = entity;
            }
        }

        private void DestroyGameEntities() {
            if (_gameEntities == null) return;

            for (int i = 0; i < _gameEntities.Length; i++) {
                ref var entity = ref _gameEntities[i];
                if (entity.IsAlive()) entity.Destroy();

                entity = default;
            }

            _gameEntities = null;
        }

        private void SetScore(int value) {
            _score = value;

            _gameSessionStorage.Score = value;

            if (_gameSessionStorage.HighScore < value) {
                _gameSessionStorage.HighScore = value;
            }
        }
    }

}
