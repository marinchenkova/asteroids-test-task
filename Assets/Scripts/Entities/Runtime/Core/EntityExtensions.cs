namespace Entities.Core {

    public static class EntityExtensions {

        public static bool IsAlive(this Entity entity) {
            return entity.world?.ContainsEntity(entity) ?? false;
        }

        public static void Destroy(this Entity entity) {
            entity.world?.DestroyEntity(entity);
        }

        public static T GetComponent<T>(this Entity entity) where T : class, IEntityComponent {
            return entity.world?.GetComponent<T>(entity);
        }

        public static void SetComponent<T>(this Entity entity, T component) where T : class, IEntityComponent {
            entity.world?.SetComponent<T>(entity, component);
        }

        public static void SetComponentCopy<T>(this Entity entity, T component) where T : class, IEntityComponent {
            entity.world?.SetComponentCopy<T>(entity, component);
        }

        public static void RemoveComponent<T>(this Entity entity) where T : class, IEntityComponent {
            entity.world?.RemoveComponent<T>(entity);
        }

        public static void RemoveComponents(this Entity entity) {
            entity.world?.RemoveComponents(entity);
        }
    }

}
