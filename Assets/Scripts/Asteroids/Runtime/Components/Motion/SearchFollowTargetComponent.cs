using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class SearchFollowTargetComponent : IEntityComponent, IUpdatable {

        [SerializeField] [Min(1)] private int _maxHits = 6;
        [SerializeField] [Min(0f)] private float _radius;
        [SerializeField] private LayerMask _includeLayers;

        private Collider2D[] _hits;

        private Entity _entity;

        public void OnAttach(Entity entity) {
            _entity = entity;
            _entity.world.SubscribeUpdate(this);
            _hits = new Collider2D[_maxHits];
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            _hits = null;
        }

        public void OnUpdate(float dt) {
            if (!_entity.IsAlive()) return;

            if (_entity.GetComponent<TransformComponent>() is not { } transformComponent) {
                return;
            }

            if (_entity.GetComponent<EntityFollowingComponent>() is not { } entityFollowingComponent) {
                return;
            }

            var followTarget = entityFollowingComponent.Target;
            if (followTarget.IsAlive()) return;

            int hitCount = Physics2D.OverlapCircleNonAlloc(
                transformComponent.Position,
                _radius,
                _hits,
                _includeLayers
            );

            for (int i = 0; i < hitCount; i++) {
                var hit = _hits[i];

                var entityViewCollider = hit.transform.GetComponent<EntityViewCollider>();
                if (entityViewCollider == null || !entityViewCollider.Entity.IsAlive()) continue;

                followTarget = entityViewCollider.Entity;
                entityFollowingComponent.Target = followTarget;
                return;
            }
        }
    }

}
