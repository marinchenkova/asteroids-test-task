using System;
using Entities.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class PlayAudioClipComponent : IEntityComponent {

        [SerializeField] private AudioClip _audioClip;
        [SerializeField] [Range(0f, 1f)] private float _volume;
        [SerializeField] private bool _useGameStateAudioSource = true;
        [SerializeField] private PlayMode _playMode;

        private enum PlayMode {
            OnAttach,
            OnDestroy,
        }

        public void OnAttach(Entity entity) {
            if (_playMode == PlayMode.OnAttach) Play(entity);
        }

        public void OnDestroy(Entity entity) {
            if (_playMode == PlayMode.OnDestroy) Play(entity);
        }

        private void Play(Entity entity) {
            var audioSourceHost = _useGameStateAudioSource
                ? entity.GetComponent<GameStateReferenceComponent>()?.GameState ?? default
                : entity;

            var audioSource = audioSourceHost.GetComponent<AudioSourceReferenceComponent>()?.AudioSource;
            if (audioSource == null) return;

            audioSource.PlayOneShot(_audioClip, _volume);
        }

        private AudioSource GetAudioSource(Entity entity) {
            var audioSourceHost = _useGameStateAudioSource
                ? entity.GetComponent<GameStateReferenceComponent>()?.GameState ?? default
                : entity;

            return audioSourceHost.GetComponent<AudioSourceReferenceComponent>()?.AudioSource;
        }
    }

}
