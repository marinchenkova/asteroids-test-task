using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Asteroids.Components {

    public sealed class EntityViewCollider : MonoBehaviour {

        [SerializeField] private EntityView _entityView;

        public Entity Entity => _entityView.Entity;

    }

}
