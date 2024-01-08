using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class DestroyTimerComponent : IEntityComponent, IUpdatable {

        [SerializeField] [Min(0f)] private float _destroyTime;

        private Entity _entity;
        private float _timer;

        public void OnAttach(Entity entity) {
            _entity = entity;
            _entity.world.SubscribeUpdate(this);
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            _timer = 0f;
        }

        public void OnUpdate(float dt) {
            _timer += dt;
            if (_timer < _destroyTime) return;

            _entity.Destroy();
        }
    }

}
