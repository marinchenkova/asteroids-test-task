using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {
    
    [Serializable]
    public sealed class EntityParentingComponent : IEntityComponent, IUpdatable {

        public Entity Parent { get => _parent; set => SetParent(value); }
        public Vector3 PositionOffset { get; set; }
        public Quaternion RotationOffset { get; set; } = Quaternion.identity;

        private Entity _entity;
        private Entity _parent;

        public void OnAttach(Entity entity) {
            _entity = entity;
            if (Parent.IsAlive()) _entity.world.SubscribeUpdate(this);
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            _parent = default;

            PositionOffset = Vector3.zero;
            RotationOffset = Quaternion.identity;
        }

        public void OnUpdate(float dt) {
            if (_entity.GetComponent<TransformComponent>() is not { } transformComponent) {
                return;
            }

            if (!_parent.IsAlive()) {
                _entity.world.UnsubscribeUpdate(this);
                return;
            }

            var parentTransformComponent = _parent.GetComponent<TransformComponent>();
            var position = parentTransformComponent?.Position ?? Vector3.zero;
            var rotation = parentTransformComponent?.Rotation ?? Quaternion.identity;

            transformComponent.Position = position + rotation * PositionOffset;
            transformComponent.Rotation = rotation * RotationOffset;
        }

        private void SetParent(Entity parent) {
            _parent = parent;

            if (!_entity.IsAlive()) return;

            if (_parent.IsAlive()) _entity.world.SubscribeUpdate(this);
            else _entity.world.UnsubscribeUpdate(this);
        }
    }
    
}
