using System;
using Entities.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class TransformComponent : IEntityComponent {

        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _rotation;

        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => Quaternion.Euler(_rotation); set => _rotation = value.eulerAngles; }

        public override string ToString() {
            return $"{nameof(TransformComponent)}(position {_position}, rotation {_rotation})";
        }
    }

}
