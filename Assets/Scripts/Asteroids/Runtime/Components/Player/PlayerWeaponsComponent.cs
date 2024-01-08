using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class PlayerWeaponsComponent : IEntityComponent {

        [SerializeField] private EntityPrefab _mainWeaponPrefab;
        [SerializeField] private EntityPrefab _alternativeWeaponPrefab;

        public Entity MainWeapon { get; private set; }
        public Entity AlternativeWeapon { get; private set; }

        public void OnAttach(Entity entity) {
            MainWeapon = CreateWeapon(entity, _mainWeaponPrefab);
            AlternativeWeapon = CreateWeapon(entity, _alternativeWeaponPrefab);
        }

        public void OnDetach(Entity entity) {
            MainWeapon = default;
            AlternativeWeapon = default;
        }

        public bool TryFireMainWeapon() {
            return TryFire(MainWeapon);
        }

        public bool TryFireAlternativeWeapon() {
            return TryFire(AlternativeWeapon);
        }

        private static bool TryFire(Entity weapon) {
            return weapon.IsAlive() && (weapon.GetComponent<WeaponComponent>()?.TryFire() ?? false);
        }

        private static Entity CreateWeapon(Entity player, EntityPrefab weaponPrefab) {
            if (weaponPrefab == null) return default;

            var weapon = player.world.CreateEntity();

            weapon.SetComponent(new EntityParentingComponent { Parent = player });
            weaponPrefab.CopyComponentsInto(weapon);

            return weapon;
        }
    }

}
