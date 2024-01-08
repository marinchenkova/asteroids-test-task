using System;
using Entities.Core;
using UnityEngine;

namespace Asteroids.Components {

    [Serializable]
    public sealed class AddGameScoreOnDestroyComponent : IEntityComponent {

        [SerializeField] [Min(0)] private int _score;

        public int Score { get => _score; set => _score = value; }

        public void OnDestroy(Entity entity) {
            var gameStateComponent = entity
                .GetComponent<GameStateReferenceComponent>()
                ?.GameState
                .GetComponent<GameStateComponent>();

            if (gameStateComponent == null) return;

            gameStateComponent.Score += _score;
        }
    }

}
