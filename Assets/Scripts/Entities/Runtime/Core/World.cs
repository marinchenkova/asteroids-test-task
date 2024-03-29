using System;
using Entities.Services;
using Tick.Core;
using UnityEngine;

namespace Entities.Core {

    /// <summary>
    /// World is a storage for entities and their components.
    /// Any class can subscribe to world updates by implementing <see cref="IUpdatable"/> interface.
    /// </summary>
    public sealed class World {

        public event Action<Entity> OnCreateEntity = delegate {  };
        public event Action<Entity> OnDestroyEntity = delegate {  };

        private readonly IEntityIdProvider _entityIdProvider;
        private readonly IEntityStorage _entityStorage;
        private readonly IEntityComponentStorage _entityComponentStorage;
        private readonly IEntityComponentFactory _entityComponentFactory;
        private readonly IEntityViewProvider _entityViewProvider;
        private readonly ITickSource _tickSource;

        private bool _isDestroyingAll;

        public World(
            IEntityIdProvider entityIdProvider,
            IEntityStorage entityStorage,
            IEntityComponentStorage entityComponentStorage,
            IEntityComponentFactory entityComponentFactory,
            IEntityViewProvider entityViewProvider,
            ITickSource tickSource
        ) {
            _entityIdProvider = entityIdProvider;
            _entityStorage = entityStorage;
            _entityComponentStorage = entityComponentStorage;
            _entityComponentFactory = entityComponentFactory;
            _entityViewProvider = entityViewProvider;
            _tickSource = tickSource;
        }

        public void SubscribeUpdate(IUpdatable updatable) {
            if (!_isDestroyingAll) _tickSource.Subscribe(updatable);
        }

        public void UnsubscribeUpdate(IUpdatable updatable) {
            _tickSource.Unsubscribe(updatable);
        }

        public GameObject CreateView(GameObject prefab, bool active = false) {
            return _entityViewProvider.CreateView(prefab, active);
        }

        public void DisposeView(GameObject view) {
            _entityViewProvider.DisposeView(view);
        }

        public void DestroyAll() {
            _isDestroyingAll = true;

            var entities = _entityStorage.Entities;
            foreach (var entity in entities) {
                _entityComponentStorage.RemoveComponents(entity, notifyDestroy: false);
            }

            _entityStorage.Clear();
            _entityComponentStorage.Clear();

            _isDestroyingAll = false;
        }

        public Entity CreateEntity() {
            if (_isDestroyingAll) return default;

            long id = _entityIdProvider.GetNextId();
            var entity = new Entity(this, id);

            if (_entityStorage.ContainsEntity(entity)) {
                throw new ArgumentException("Failed to create new entity: " +
                                            "entity id provider returned new id that is already occupied.");
            }

            _entityStorage.AddEntity(entity);
            OnCreateEntity.Invoke(entity);

            return entity;
        }

        public void DestroyEntity(Entity entity) {
            if (_isDestroyingAll) return;

            if (!ContainsEntity(entity)) {
                throw new ArgumentException($"Failed to destroy entity {entity}: " +
                                            "entity is not found.");
            }

            OnDestroyEntity.Invoke(entity);

            _entityComponentStorage.RemoveComponents(entity, notifyDestroy: true);
            _entityStorage.RemoveEntity(entity);
        }

        public bool ContainsEntity(Entity entity) {
            return entity.world == this && _entityStorage.ContainsEntity(entity);
        }

        public T GetComponent<T>(Entity entity) where T : class, IEntityComponent {
            return _entityComponentStorage.GetComponent<T>(entity);
        }

        public void SetComponent<T>(Entity entity, T component) where T : class, IEntityComponent {
            if (!ContainsEntity(entity)) {
                throw new ArgumentException($"Failed to set component of type {typeof(T)} on entity {entity}: " +
                                            "entity is not found.");
            }

            _entityComponentStorage.SetComponent(entity, component);
        }

        public void SetComponentCopy<T>(Entity entity, T component) where T : class, IEntityComponent {
            if (!ContainsEntity(entity)) {
                throw new ArgumentException($"Failed to copy component of type {typeof(T)} on entity {entity}: " +
                                            "entity is not found.");
            }

            _entityComponentStorage.SetComponent(entity, _entityComponentFactory.Copy(component));
        }

        public void RemoveComponent<T>(Entity entity) where T : class, IEntityComponent {
            if (!ContainsEntity(entity)) {
                throw new ArgumentException($"Failed to remove component of type {typeof(T)} on entity {entity}: " +
                                            "entity is not found.");
            }

            _entityComponentStorage.RemoveComponent<T>(entity);
        }

        public void RemoveComponents(Entity entity) {
            if (!ContainsEntity(entity)) {
                throw new ArgumentException($"Failed to remove components from entity {entity}: " +
                                            "entity is not found.");
            }

            _entityComponentStorage.RemoveComponents(entity, notifyDestroy: false);
        }
    }

}
