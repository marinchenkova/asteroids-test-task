using System;
using Entities.Core;
using Entities.Unity;
using Tick.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Components {

    [Serializable]
    public sealed class EnemySpawnerComponent : IEntityComponent, IUpdatable {

        [SerializeField] private EntityPrefab _prefab;
        [SerializeField] [Min(0)] private int _minSpawnAmount;
        [SerializeField] [Min(0)] private int _maxSpawnAmount;
        [SerializeField] [Min(0f)] private float _initialSpawnPeriod;
        [SerializeField] [Min(0f)] private float _minSpawnPeriod;
        [SerializeField] private float _spawnPeriodIncrement;
        [SerializeField] private bool _randomizeRotation;
        [SerializeField] private int _spawnAtStartAmount;

        [SerializeField] private Vector2 _xBound;
        [SerializeField] private Vector2 _yBound;

        private Entity _entity;
        private float _spawnPeriod;
        private float _timer;

        public void OnAttach(Entity entity) {
            _entity = entity;

            _spawnPeriod = Mathf.Max(_initialSpawnPeriod, _minSpawnPeriod);
            _timer = 0f;

            if (_spawnPeriod > 0f) _entity.world.SubscribeUpdate(this);

            Spawn(_prefab, _spawnAtStartAmount);
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            _timer = 0f;
            _spawnPeriod = 0f;
        }

        public void OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            _timer += dt;
            if (_timer < _spawnPeriod) return;

            _timer = 0f;
            _spawnPeriod = Mathf.Max(_spawnPeriod + _spawnPeriodIncrement, _minSpawnPeriod);

            int amount = _minSpawnAmount > _maxSpawnAmount
                ? _maxSpawnAmount
                : Random.Range(_minSpawnAmount, _maxSpawnAmount);

            Spawn(_prefab, amount);
        }

        private void Spawn(EntityPrefab prefab, int amount) {
            if (amount <= 0) return;

            for (int i = 0; i < amount; i++) {
                var entity = _entity.world.CreateEntity();

                var position = GetRandomPosition();
                var rotation = _randomizeRotation ? GetRandomRotation() : Quaternion.identity;

                entity.SetComponent(new TransformComponent { Position = position, Rotation = rotation });
                prefab.CopyComponentsInto(entity);
            }
        }

        private Vector3 GetRandomPosition() {
            return new Vector3(Random.Range(_xBound.x, _xBound.y), Random.Range(_yBound.x, _yBound.y), 0f);
        }

        private Quaternion GetRandomRotation() {
            return Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }
    }

}
