using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Core;

namespace Entities.Services {

    internal sealed class EntityComponentStorage : IEntityComponentStorage {

        private readonly Dictionary<EntityComponentId, EntityComponentContainer> _componentData
            = new Dictionary<EntityComponentId, EntityComponentContainer>();

        private int _version;

        public IEnumerable<IEntityComponent> GetComponents(Entity entity) {
            return new ComponentCollection(this, entity);
        }

        public T GetComponent<T>(Entity entity) where T : class, IEntityComponent {
            var id = new EntityComponentId(entity, typeof(T));

            if (_componentData.TryGetValue(id, out var data) && !data.isDisposed) {
                return (T) data.component;
            }

            return default;
        }

        public void SetComponent(Entity entity, IEntityComponent component) {
            var componentType = component?.GetType();
            var id = new EntityComponentId(entity, componentType);

            if (_componentData.TryGetValue(id, out var data)) {
                data.component?.OnDetach(entity);

                _componentData[id] = new EntityComponentContainer(component, data.next);
                _version++;

                component?.OnAttach(entity);

                return;
            }

            var firstEntryId = new EntityComponentId(entity);
            var nextComponentType = _componentData.TryGetValue(firstEntryId, out data) ? data.next : null;

            _componentData[firstEntryId] = new EntityComponentContainer(default, componentType);
            _componentData[id] = new EntityComponentContainer(component, nextComponentType);

            _version++;

            component?.OnAttach(entity);
        }

        public void RemoveComponent<T>(Entity entity) where T : class, IEntityComponent {
            var id = new EntityComponentId(entity, typeof(T));

            if (_componentData.TryGetValue(id, out var data) && !data.isDisposed) {
                _componentData[id] = new EntityComponentContainer(default, data.next, isDisposed: true);
                _version++;

                data.component.OnDetach(entity);
            }
        }

        public void RemoveComponents(Entity entity, bool notifyDestroy) {
            var id = new EntityComponentId(entity);

            if (notifyDestroy) {
                while (_componentData.TryGetValue(id, out var data)) {
                    if (!data.isDisposed) data.component?.OnDestroy(entity);
                    if (data.next == null) break;

                    id = new EntityComponentId(entity, data.next);
                }

                id = new EntityComponentId(entity);
            }

            while (_componentData.TryGetValue(id, out var data)) {
                _componentData.Remove(id);
                _version++;

                if (!data.isDisposed) data.component?.OnDetach(entity);

                id = new EntityComponentId(entity, data.next);
            }
        }

        public void Clear() {
            _componentData.Clear();
            _version++;
        }

        private readonly struct ComponentCollection : IEnumerable<IEntityComponent> {

            private readonly EntityComponentStorage _storage;
            private readonly Entity _entity;

            public ComponentCollection(EntityComponentStorage storage, Entity entity) {
                _storage = storage;
                _entity = entity;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public IEnumerator<IEntityComponent> GetEnumerator() {
                return new ComponentEnumerator(_storage, _entity);
            }

            private struct ComponentEnumerator : IEnumerator<IEntityComponent> {

                public IEntityComponent Current { get; private set; }
                object IEnumerator.Current => Current;

                private readonly EntityComponentStorage _storage;
                private readonly Entity _entity;
                private readonly int _version;

                private Type _nextComponentType;

                public ComponentEnumerator(EntityComponentStorage storage, Entity entity) {
                    _storage = storage;
                    _entity = entity;
                    _version = _storage._version;

                    Current = null;

                    _nextComponentType = _storage._componentData.TryGetValue(new EntityComponentId(_entity), out var container)
                        ? container.next
                        : null;
                }

                public bool MoveNext() {
                    if (_version != _storage._version) {
                        throw new InvalidOperationException($"{nameof(ComponentEnumerator)} is invalid");
                    }

                    if (_nextComponentType == null) {
                        return false;
                    }

                    var id = new EntityComponentId(_entity, _nextComponentType);

                    while (_storage._componentData.TryGetValue(id, out var container)) {
                        if (container.isDisposed) {
                            id = new EntityComponentId(_entity, container.next);
                            continue;
                        }

                        _nextComponentType = container.next;
                        Current = container.component;
                        return true;
                    }

                    return false;
                }

                public void Reset() {
                    if (_version != _storage._version) {
                        throw new InvalidOperationException($"{nameof(ComponentEnumerator)} is invalid");
                    }

                    _nextComponentType = null;
                    Current = null;
                }

                public void Dispose() { }
            }
        }
    }

}
