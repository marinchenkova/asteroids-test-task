using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class WeaponCooldownComponent : IEntityComponent, IUpdatable {

        [SerializeField] [Min(0f)] private float _cooldown;

        public float Cooldown { get => _cooldown; set => _cooldown = Mathf.Max(value, 0f); }
        public float CooldownTimer { get; private set; }

        private Entity _entity;

        public void OnAttach(Entity entity) {
            _entity = entity;
        }

        public void OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
            CooldownTimer = 0f;
        }

        public void Restart() {
            CooldownTimer = _cooldown;
            _entity.world.SubscribeUpdate(this);
        }

        public void OnUpdate(float dt) {
            CooldownTimer = Mathf.Max(CooldownTimer - dt, 0f);
            if (CooldownTimer <= 0f) _entity.world.UnsubscribeUpdate(this);
        }
    }

}
