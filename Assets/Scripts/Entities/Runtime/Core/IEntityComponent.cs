namespace Entities.Core {

    /// <summary>
    /// Entity component is a container for entity data and logic.
    /// World sends entity events to the related components.
    /// </summary>
    public interface IEntityComponent {

        /// <summary>
        /// Called after component was attached to an entity.
        /// </summary>
        void OnAttach(Entity entity) {}

        /// <summary>
        /// Called after component was detached from an entity.
        /// </summary>
        void OnDetach(Entity entity) {}

        /// <summary>
        /// Called before entity is destroyed and before <see cref="OnDetach"/> call,
        /// to be able to read any components before they are destroyed with entity.
        /// </summary>
        void OnDestroy(Entity entity) {}
    }

}
