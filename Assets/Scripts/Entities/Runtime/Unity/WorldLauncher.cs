using Entities.Core;
using Entities.Services;
using Tick.Core;
using UnityEngine;

namespace Entities.Unity {

    public sealed class WorldLauncher : MonoBehaviour {

        [SerializeField] private EntityViewProvider _entityViewProvider;

        public World World { get; private set; }

        private TickSource _tickSource;

        private void Awake() {
            _tickSource = new TickSource();
            World = CreateWorld(_entityViewProvider, _tickSource);
        }

        private void OnDestroy() {
            World.DestroyAll();
            World = null;
            _tickSource = null;
        }

        private void Update() {
           _tickSource.Tick(Time.deltaTime);
        }

        private static World CreateWorld(IEntityViewProvider entityViewProvider, ITickSource tickSource) {
            return new World(
                new IncrementalEntityIdProvider(),
                new EntityStorage(),
                new EntityComponentStorage(),
                new EntityComponentFactory(),
                entityViewProvider,
                tickSource
            );
        }
    }

}
