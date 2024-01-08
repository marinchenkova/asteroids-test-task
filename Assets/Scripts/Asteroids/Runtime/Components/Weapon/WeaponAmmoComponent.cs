using System;
using Entities.Core;
using UnityEngine;

namespace Asteroids.Components {
    
    [Serializable]
    public sealed class WeaponAmmoComponent : IEntityComponent {

        [SerializeField] [Min(0)] private int _initialAmmoCount;
        [SerializeField] [Min(0)] private int _maxAmmoCount;

        public int AmmoCount { get; private set; }
        public int MaxAmmoCount { get => _maxAmmoCount; set => SetMaxAmmoCount(value); }

        public bool TryLoadAmmo(int amount) {
            if (amount <= 0) return false;
            if (AmmoCount + amount > _maxAmmoCount) return false;

            AmmoCount += amount;
            return true;
        }

        public bool TryTakeAmmo(int amount) {
            if (amount <= 0) return false;
            if (amount > AmmoCount) return false;

            AmmoCount -= amount;
            return true;
        }

        public void OnAttach(Entity entity) {
            AmmoCount = _initialAmmoCount;
        }

        public void OnDetach(Entity entity) {
            AmmoCount = 0;
        }

        private void SetMaxAmmoCount(int value) {
            if (value <= 0) {
                _maxAmmoCount = 0;
                AmmoCount = 0;
                return;
            }

            _maxAmmoCount = value;
            if (AmmoCount > _maxAmmoCount) AmmoCount = _maxAmmoCount;
        }
    }
    
}
