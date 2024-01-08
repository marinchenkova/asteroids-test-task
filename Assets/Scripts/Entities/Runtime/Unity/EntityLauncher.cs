using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    public sealed class EntityLauncher : MonoBehaviour {

        [SerializeField] private WorldLauncher _worldLauncher;
        [SerializeReference] private IEntityComponent[] _components;

        private Entity _entity;

        private void Start() {
            _entity = CreateEntity();
        }

        private void OnDestroy() {
            if (_entity.IsAlive()) _entity.Destroy();
        }

        private Entity CreateEntity() {
            var entity = _worldLauncher.World.CreateEntity();

            for (int i = 0; i < _components.Length; i++) {
                entity.SetComponentCopy(_components[i]);
            }

            return entity;
        }
    }

}
