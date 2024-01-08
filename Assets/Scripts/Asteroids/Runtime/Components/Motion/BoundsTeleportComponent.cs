using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class BoundsTeleportComponent : IEntityComponent, IUpdatable {

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
            bool needTeleport = false;

            if (position.x < _xBound.x) {
                needTeleport = true;
                position.x = _xBound.y;
            }
            else if (position.x > _xBound.y) {
                needTeleport = true;
                position.x = _xBound.x;
            }

            if (position.y < _yBound.x) {
                needTeleport = true;
                position.y = _yBound.y;
            }
            else if (position.y > _yBound.y) {
                needTeleport = true;
                position.y = _yBound.x;
            }

            if (needTeleport) {
                transformComponent.Position = position;
            }
        }
    }

}
