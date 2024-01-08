using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class DestroyOutOfBoundsComponent : IEntityComponent, IUpdatable {

        [SerializeField] private Vector2 _xBound;
        [SerializeField] private Vector2 _yBound;

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
            if (_entity.GetComponent<TransformComponent>() is not { } transformComponent) {
                return;
            }

            var position = transformComponent.Position;

            if (position.x < _xBound.x || position.x > _xBound.y ||
                position.y < _yBound.x || position.y > _yBound.y
            ) {
                _entity.Destroy();
            }
        }
    }

}
