using System;
using Entities.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Components {

    [Serializable]
    public sealed class RandomViewBindingComponent : IEntityComponent {

        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private bool _applyCustomScale;
        [SerializeField] private Vector3 _customScale = Vector3.one;

        public void OnAttach(Entity entity) {
            if (_prefabs.Length == 0) return;

            int index = Random.Range(0, _prefabs.Length - 1);

            entity.SetComponent(new ViewBindingComponent {
                Prefab = _prefabs[index],
                ApplyCustomScale = _applyCustomScale,
                CustomScale = _customScale
            });
        }
    }

}
