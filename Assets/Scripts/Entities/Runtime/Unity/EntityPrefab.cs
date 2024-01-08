using Entities.Core;
using UnityEngine;

namespace Entities.Unity {

    [CreateAssetMenu(fileName = nameof(EntityPrefab), menuName = "Entities/" + nameof(EntityPrefab))]
    public sealed class EntityPrefab : ScriptableObject {

        [SerializeReference] private IEntityComponent[] _components;

        public void CopyComponentsInto(Entity entity) {
            for (int i = 0; i < _components.Length; i++) {
                entity.SetComponentCopy(_components[i]);
            }
        }
    }
}
