using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class EntityFollowingComponent : IEntityComponent, IUpdatable {

        [SerializeField] [Min(0f)] private float _speed;

        public Entity Target { get => _target; set => SetTarget(value); }
        public float Speed { get => _speed; set => _speed = Mathf.Max(value, 0f); }

        private Entity _entity;
        private Entity _target;

        public void OnAttach(Entity entity) {
            _entity = entity;
            SetTarget(Target);
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            _target = default;
            _speed = default;
        }

        public void OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            if (!_target.IsAlive()) {
                _entity.world.UnsubscribeUpdate(this);
                return;
            }

            if (_entity.GetComponent<TransformComponent>() is not { } transformComponent) {
                return;
            }

            if (_target.GetComponent<TransformComponent>() is not { } targetTransformComponent) {
                return;
            }

            var targetPosition = targetTransformComponent.Position;
            var diff = targetPosition - transformComponent.Position;

            if (_speed <= 0f || diff.sqrMagnitude <= 0f) {
                transformComponent.Position = targetPosition;
                return;
            }

            if (_entity.GetComponent<VelocityComponent>() is { } velocityComponent) {
                velocityComponent.Velocity = diff.normalized * _speed;
            }
        }

        private void SetTarget(Entity target) {
            _target = target;

            if (!_entity.IsAlive()) {
                return;
            }

            if (!_target.IsAlive()) {
                _entity.world.UnsubscribeUpdate(this);
                return;
            }

            _entity.world.SubscribeUpdate(this);
        }
    }

}
