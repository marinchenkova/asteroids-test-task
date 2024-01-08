using System;
using Entities.Core;
using Tick.Core;

namespace Asteroids.Components {

    [Serializable]
    public sealed class PlayerWeaponInputComponent : IEntityComponent, IUpdatable {

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
            if (_entity.GetComponent<PlayerInputComponent>() is not { } inputComponent) {
                return;
            }

            if (_entity.GetComponent<PlayerWeaponsComponent>() is not { } weaponsComponent) {
                return;
            }

            var playerInputMap = inputComponent.InputActions.Player;

            if (playerInputMap.Fire.IsPressed()) weaponsComponent.TryFireMainWeapon();
            if (playerInputMap.FireAlternative.IsPressed()) weaponsComponent.TryFireAlternativeWeapon();
        }
    }

}
