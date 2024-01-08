using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class CollisionComponent : IEntityComponent, IUpdatable {

        [Header("Raycast Settings")]
        [SerializeField] [Min(1)] private int _maxHits = 6;
        [SerializeField] [Min(0f)] private float _radius;
        [SerializeField] private LayerMask _includeLayers;
        [SerializeField] private RaycastDistanceSource _raycastDistanceSource;
        [SerializeField] private RaycastDirectionSource _raycastDirectionSource;
        [SerializeField] [Min(0f)] private float _maxDistance;
        [SerializeField] private Vector3 _raycastDirection;

        [Header("Collision Settings")]
        [SerializeField] private bool _destroyCollidedEntities;
        [SerializeField] private bool _destroySelf;

        private enum RaycastDistanceSource {
            Fixed,
            Velocity,
        }

        private enum RaycastDirectionSource {
            Fixed,
            Rotation,
            Velocity
        }

        private RaycastHit2D[] _hits;
        private Entity _entity;

        public void OnAttach(Entity entity) {
            _entity = entity;
            _entity.world.SubscribeUpdate(this);
            _hits = new RaycastHit2D[_maxHits];
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            _hits = null;
        }

        public void OnUpdate(float dt) {
            if (!_entity.IsAlive()) {
                return;
            }

            if (_entity.GetComponent<TransformComponent>() is not { } transformComponent) {
                return;
            }

            var velocityComponent = _entity.GetComponent<VelocityComponent>();
            var origin = transformComponent.Position;
            var direction = transformComponent.Rotation * Vector3.up;
            var velocity = velocityComponent?.Velocity ?? Vector3.zero;

            float maxDistance = _raycastDistanceSource switch {
                RaycastDistanceSource.Fixed => _maxDistance,
                RaycastDistanceSource.Velocity => velocity.magnitude * dt,
                _ => throw new ArgumentOutOfRangeException()
            };

            direction = _raycastDirectionSource switch {
                RaycastDirectionSource.Fixed => _raycastDirection.normalized,
                RaycastDirectionSource.Rotation => direction,
                RaycastDirectionSource.Velocity => velocity.sqrMagnitude > 0f ? velocity.normalized : direction,
                _ => throw new ArgumentOutOfRangeException()
            };

            int hitCount = Physics2D.CircleCastNonAlloc(
                origin,
                _radius,
                direction,
                _hits,
                maxDistance,
                _includeLayers
            );

            int entityHitCount = 0;

            for (int i = 0; i < hitCount; i++) {
                var hit = _hits[i];

                var entityViewCollider = hit.transform.GetComponent<EntityViewCollider>();
                if (entityViewCollider == null || !entityViewCollider.Entity.IsAlive()) continue;

                entityHitCount++;
                if (_destroyCollidedEntities) entityViewCollider.Entity.Destroy();
            }

            if (entityHitCount > 0 && _destroySelf) _entity.Destroy();
        }
    }

}
