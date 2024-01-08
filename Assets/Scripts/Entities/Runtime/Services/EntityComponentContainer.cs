using System;
using Entities.Core;

namespace Entities.Services {

    internal readonly struct EntityComponentContainer {

        public readonly IEntityComponent component;
        public readonly Type next;
        public readonly bool isDisposed;

        public EntityComponentContainer(IEntityComponent component, Type next, bool isDisposed = false) {
            this.component = component;
            this.next = next;
            this.isDisposed = isDisposed;
        }
    }

}
