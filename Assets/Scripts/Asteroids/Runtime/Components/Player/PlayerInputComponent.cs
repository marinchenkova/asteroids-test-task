using System;
using Asteroids.Input;
using Entities.Core;

namespace Asteroids.Components {

    [Serializable]
    public sealed class PlayerInputComponent : IEntityComponent {

        public InputActions_Gameplay InputActions { get; private set; }

        public void OnAttach(Entity entity) {
            InputActions = new InputActions_Gameplay();
            InputActions.Enable();
        }

        public void OnDetach(Entity entity) {
            InputActions.Disable();
            InputActions.Dispose();
            InputActions = null;
        }
    }

}
