using System;
using Entities.Core;
using Tick.Core;
using UnityEngine;

namespace Asteroids.Components {
    
    [Serializable]
    public sealed class WeaponAmmoLoaderComponent : IEntityComponent, IUpdatable {

        [SerializeField] [Min(0)] private int _loadAmount = 1;
        [SerializeField] [Min(0f)] private float _loadTime;

        private float _timer;
        private Entity _entity;

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
            if (_entity.GetComponent<WeaponAmmoComponent>() is not { } weaponAmmoComponent) {
                _timer = 0f;
                return;
            }

            int currentAmmo = weaponAmmoComponent.AmmoCount;
            int maxAmmo = weaponAmmoComponent.MaxAmmoCount;

            if (currentAmmo >= maxAmmo) {
                _timer = 0f;
                return;
            }

            _timer += dt;
            if (_timer < _loadTime) return;

            _timer = 0f;
            bool loaded = weaponAmmoComponent.TryLoadAmmo(_loadAmount);

            if (!loaded) {
                int retryLoadAmount = Math.Min(_loadAmount, maxAmmo - currentAmmo);
                weaponAmmoComponent.TryLoadAmmo(retryLoadAmount);
            }
        }
    }
    
}
