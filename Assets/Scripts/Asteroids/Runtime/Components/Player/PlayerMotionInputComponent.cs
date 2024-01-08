using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class PlayerMotionInputComponent : IEntityComponent, IUpdatable {

        [SerializeField] [Min(0f)] private float _acceleration = 2f;
        [SerializeField] [Min(0f)] private float _maxForwardSpeed = 5f;
        [SerializeField] [Min(0f)] private float _rotationSpeed = 360f;

        private Entity _entity;

        public void OnAttach(Entity entity) {
            _entity = entity;
            _entity.world.SubscribeUpdate(this);
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
        }

        public void OnUpdate(float dt) {
            if (_entity.GetComponent<PlayerInputComponent>() is not {} inputComponent) return;
            if (_entity.GetComponent<VelocityComponent>() is not {} velocityComponent) return;
            if (_entity.GetComponent<TransformComponent>() is not {} transformComponent) return;

            var playerInputMap = inputComponent.InputActions.Player;

            float angle = playerInputMap.Rotate.ReadValue<float>() * _rotationSpeed;
            var currentRotation = transformComponent.Rotation;
            var targetRotation = currentRotation * Quaternion.Euler(0f, 0f, angle * dt);

            transformComponent.Rotation = targetRotation;

            float forward = playerInputMap.MoveForward.ReadValue<float>() * _maxForwardSpeed;
            var currentVelocity = velocityComponent.Velocity;
            var targetVelocity = targetRotation * new Vector3(0f, forward, 0f);

            velocityComponent.Velocity = Vector3.Lerp(currentVelocity, targetVelocity, _acceleration * dt);
        }
    }

}
