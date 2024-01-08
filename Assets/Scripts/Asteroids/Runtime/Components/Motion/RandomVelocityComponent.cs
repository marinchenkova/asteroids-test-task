using System;
using Entities.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Components {

    [Serializable]
    public sealed class RandomVelocityComponent : IEntityComponent {

        [SerializeField] [Min(0f)] private float _minSpeed;
        [SerializeField] [Min(0f)] private float _maxSpeed;

        public void OnAttach(Entity entity) {
            float from = _minSpeed > _maxSpeed ? _maxSpeed : _minSpeed;
            float to = _maxSpeed;

            var velocity = Random.Range(from, to) * Random.insideUnitCircle.normalized;

            entity.SetComponent(new VelocityComponent { Velocity = new Vector3(velocity.x, velocity.y) });
        }
    }

}
