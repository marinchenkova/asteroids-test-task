using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class VelocityComponent : IEntityComponent, IUpdatable {

        [SerializeField] private Vector3 _velocity;

        public Vector3 Velocity {
            get => _velocity;
            set => _velocity = value;
        }

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
            if (_entity.GetComponent<TransformComponent>() is not {} transformComponent) return;

            transformComponent.Position += _velocity * dt;
        }
    }

}
