using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class WeaponComponent : IEntityComponent {

        [SerializeField] private EntityPrefab _bulletPrefab;
        [SerializeField] [Min(0)] private int _bulletsPerShot = 1;
        [SerializeField] private Vector3 _bulletPositionOffset;
        [SerializeField] private Vector3 _bulletRotationOffset;
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private bool _parentBullets;

        private Entity _entity;

        public void OnAttach(Entity entity) {
            _entity = entity;
        }

        public void OnDetach(Entity entity) {
            _entity = default;
        }

        public bool TryFire() {
            if (!_entity.IsAlive()) return false;

            var cooldownComponent = _entity.GetComponent<WeaponCooldownComponent>();
            if (cooldownComponent is { CooldownTimer: > 0f }) {
                return false;
            }

            var ammoComponent = _entity.GetComponent<WeaponAmmoComponent>();
            if (ammoComponent != null && !ammoComponent.TryTakeAmmo(_bulletsPerShot)) {
                return false;
            }

            cooldownComponent?.Restart();

            var transformComponent = _entity.GetComponent<TransformComponent>();
            var position = transformComponent?.Position ?? Vector3.zero;
            var rotation = transformComponent?.Rotation ?? Quaternion.identity;

            var bulletPosition = position + rotation * _bulletPositionOffset;
            var bulletRotation = rotation * Quaternion.Euler(_bulletRotationOffset);
            var bulletVelocity = bulletRotation * new Vector3(0f, _bulletSpeed, 0f);

            for (int i = 0; i < _bulletsPerShot; i++) {
                var bullet = _entity.world.CreateEntity();

                bullet.SetComponent(new TransformComponent { Position = bulletPosition, Rotation = bulletRotation });
                bullet.SetComponent(new VelocityComponent { Velocity = bulletVelocity });

                if (_parentBullets) {
                    bullet.SetComponent(new EntityParentingComponent {
                        Parent = _entity,
                        PositionOffset = _bulletPositionOffset,
                        RotationOffset = Quaternion.Euler(_bulletRotationOffset),
                    });
                }

                _bulletPrefab.CopyComponentsInto(bullet);
            }

            return true;
        }
    }

}
