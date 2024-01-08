using Entities.Core;

namespace Asteroids.Components {

    public sealed class GameStateReferenceComponent : IEntityComponent {

        public Entity GameState { get; set; }

    }

}
