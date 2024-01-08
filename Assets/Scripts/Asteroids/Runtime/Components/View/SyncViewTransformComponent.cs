using System;
using Entities.Core;
using Tick.Core;

namespace Asteroids.Components {

    [Serializable]
    public sealed class SyncViewTransformComponent : IEntityComponent, IUpdatable {

        private Entity _entity;

        void IEntityComponent.OnAttach(Entity entity) {
            _entity = entity;
            _entity.world.SubscribeUpdate(this);
            WriteTransform(_entity);
        }

        void IEntityComponent.OnDetach(Entity entity) {
            _entity.world.UnsubscribeUpdate(this);
            _entity = default;
        }

        void IUpdatable.OnUpdate(float dt) {
            WriteTransform(_entity);
        }

        private static void WriteTransform(Entity entity) {
            if (entity.GetComponent<ViewBindingComponent>() is not {} viewBindingComponent) return;
            if (entity.GetComponent<TransformComponent>() is not {} transformComponent) return;

            viewBindingComponent.ViewTransform.SetPositionAndRotation(transformComponent.Position, transformComponent.Rotation);
        }
    }

}
