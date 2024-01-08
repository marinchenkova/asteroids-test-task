using System;
using Entities.Core;
using Entities.Unity;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class ViewBindingComponent : IEntityComponent {

        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _applyCustomScale;
        [SerializeField] private Vector3 _customScale = Vector3.one;

        public GameObject Prefab { get => _prefab; set => _prefab = value; }
        public Vector3 CustomScale { get => _customScale; set => _customScale = value; }
        public bool ApplyCustomScale { get => _applyCustomScale; set => _applyCustomScale = value; }
        public Transform ViewTransform { get; private set; }

        private Vector3 _initialScale;
        private EntityView _entityView;

        public void OnAttach(Entity entity) {
            var transformComponent = entity.GetComponent<TransformComponent>();
            var position = transformComponent?.Position ?? Vector3.zero;
            var rotation = transformComponent?.Rotation ?? Quaternion.identity;

            ViewTransform = entity.world.CreateView(_prefab, active: false).transform;

            ViewTransform.SetPositionAndRotation(position, rotation);
            _initialScale = ViewTransform.localScale;
            if (_applyCustomScale) ViewTransform.localScale = _customScale;

            _entityView = ViewTransform.GetComponent<EntityView>();
            if (_entityView != null) _entityView.Entity = entity;

            ViewTransform.gameObject.SetActive(true);
        }

        public void OnDetach(Entity entity) {
            if (_entityView != null) _entityView.Entity = default;

            entity.world.DisposeView(ViewTransform.gameObject);
            ViewTransform.localScale = _initialScale;

            _entityView = null;
            ViewTransform = null;
        }
    }

}
