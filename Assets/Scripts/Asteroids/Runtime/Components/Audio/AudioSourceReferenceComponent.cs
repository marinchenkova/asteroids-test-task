using System;
using Entities.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class AudioSourceReferenceComponent : IEntityComponent {

        [SerializeField] private AudioSource _audioSource;

        public AudioSource AudioSource { get => _audioSource; set => _audioSource = value; }
    }

}
