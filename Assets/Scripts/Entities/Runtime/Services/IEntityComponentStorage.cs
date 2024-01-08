using System.Collections.Generic;
using Entities.Core;

namespace Entities.Services {

    public interface IEntityComponentStorage {

        IEnumerable<IEntityComponent> GetComponents(Entity entity);

        T GetComponent<T>(Entity entity) where T : class, IEntityComponent;

        void SetComponent(Entity entity, IEntityComponent component);

        void RemoveComponent<T>(Entity entity) where T : class, IEntityComponent;

        void RemoveComponents(Entity entity, bool notifyDestroy);

        void Clear();
    }

}
