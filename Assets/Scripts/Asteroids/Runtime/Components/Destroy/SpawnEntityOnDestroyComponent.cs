using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Components {

    [Serializable]
    public sealed class SpawnEntityOnDestroyComponent : IEntityComponent {

        [SerializeField] private SpawnSettings[] _spawnSettings;

        [Serializable]
        private struct SpawnSettings {
            public EntityPrefab prefab;
            [Min(0)] public int minAmount;
            [Min(0)] public int maxAmount;
        }

        public void OnDestroy(Entity entity) {
            var transform = entity.GetComponent<TransformComponent>();
            var position = transform?.Position ?? Vector3.zero;
            var rotation = transform?.Rotation ?? Quaternion.identity;

            for (int i = 0; i < _spawnSettings.Length; i++) {
                var spawnSettings = _spawnSettings[i];

                int minAmount = spawnSettings.minAmount > spawnSettings.maxAmount ? spawnSettings.maxAmount : spawnSettings.minAmount;
                int maxAmount = spawnSettings.maxAmount;

                int amount = Random.Range(minAmount, maxAmount);

                for (int j = 0; j < amount; j++) {
                    var instance = entity.world.CreateEntity();

                    instance.SetComponent(new TransformComponent { Position = position, Rotation = rotation });
                    spawnSettings.prefab.CopyComponentsInto(instance);
                }
            }
        }
    }

}
