using System;
using Asteroids.Components;
using Asteroids.Session;
using Entities.Core;
using TMPro;
using UnityEngine;

namespace Asteroids.UI {

    public sealed class PlayerStatsUIController : MonoBehaviour {

        [Header("Settings")]
        [SerializeField] private GameSessionLauncher _gameSessionLauncher;

        [Header("UI Components")]
        [SerializeField] private TMP_Text _textPlayerPosition;
        [SerializeField] private TMP_Text _textPlayerAngle;
        [SerializeField] private TMP_Text _textPlayerSpeed;
        [SerializeField] private TMP_Text _textPlayerLaser;

        private const float TOLERANCE = 0.001f;
        private const float SQR_TOLERANCE = TOLERANCE * TOLERANCE;

        private bool _isPlayerAlive;
        private Vector3 _lastPosition;
        private float _lastAngle;
        private float _lastSqrVelocity;

        private int _lastLaserShotsLeft;
        private int _lastLaserShotsMax;
        private float _lastLaserCooldown;
        private float _lastLaserCooldownLeft;

        private void OnEnable() {
            FetchGameState(forceUpdateUI: true);
        }

        private void OnDisable() {
            ShowPlayerStats(false);
        }

        private void LateUpdate() {
            FetchGameState();
        }

        private void FetchGameState(bool forceUpdateUI = false) {
            var gameStateComponent = _gameSessionLauncher.GameState.GetComponent<GameStateComponent>();

            var player = gameStateComponent?.Player ?? default;
            bool isPlayerAlive = player.IsAlive();

            if (isPlayerAlive != _isPlayerAlive) {
                _isPlayerAlive = isPlayerAlive;
                ShowPlayerStats(_isPlayerAlive);
            }

            FetchPlayerStats(player, forceUpdateUI);
        }

        private void FetchPlayerStats(Entity player, bool forceUpdateUI = false) {
            var transformComponent = player.GetComponent<TransformComponent>();
            var velocityComponent = player.GetComponent<VelocityComponent>();
            var weaponsComponent = player.GetComponent<PlayerWeaponsComponent>();

            var position = transformComponent?.Position ?? Vector3.zero;
            float angle = transformComponent?.Rotation.eulerAngles.z ?? 0f;
            var velocity = velocityComponent?.Velocity ?? Vector3.zero;
            float sqrVelocity = velocity.sqrMagnitude;

            var laser = weaponsComponent?.AlternativeWeapon ?? default;
            var laserAmmoComponent = laser.GetComponent<WeaponAmmoComponent>();
            var laserCooldownComponent = laser.GetComponent<WeaponCooldownComponent>();
            int laserShotsLeft = laserAmmoComponent?.AmmoCount ?? 0;
            int laserShotsMax = laserAmmoComponent?.MaxAmmoCount ?? 0;
            float laserCooldown = laserCooldownComponent?.Cooldown ?? 0f;
            float laserCooldownLeft = laserCooldownComponent?.CooldownTimer ?? 0f;

            if (forceUpdateUI || IsChanged(position, _lastPosition)) {
                _lastPosition = position;
                _textPlayerPosition.text = $"Position [{position.x:0.00}, {position.y:0.00}]";
            }

            if (forceUpdateUI || IsChanged(angle, _lastAngle)) {
                _lastAngle = angle;
                _textPlayerAngle.text = $"Angle {angle:0.00}";
            }


            if (forceUpdateUI || IsChanged(sqrVelocity, _lastSqrVelocity, SQR_TOLERANCE)) {
                _lastSqrVelocity = sqrVelocity;
                _textPlayerSpeed.text = $"Velocity {velocity.magnitude:0.00}";
            }

            if (forceUpdateUI ||
                IsChanged(laserShotsLeft, _lastLaserShotsLeft) ||
                IsChanged(laserShotsMax, _lastLaserShotsMax) ||
                IsChanged(laserCooldown, _lastLaserCooldown) ||
                IsChanged(laserCooldownLeft, _lastLaserCooldownLeft) ||
                laserCooldownLeft <= 0f && _lastLaserCooldownLeft > 0f
            ) {
                _lastLaserShotsLeft = laserShotsLeft;
                _lastLaserShotsMax = laserShotsMax;
                _lastLaserCooldown = laserCooldown;
                _lastLaserCooldownLeft = laserCooldownLeft;

                string laserState = laserCooldownLeft > 0f
                    ? "charge"
                    : laserShotsLeft <= 0
                        ? "reload"
                        : "ready";

                _textPlayerLaser.text = $"Laser {laserShotsLeft}/{laserShotsMax} {laserCooldownLeft:0.00} {laserState}";
            }
        }

        private static bool IsChanged(Vector3 oldValue, Vector3 newValue) {
            return IsChanged(oldValue.x, newValue.x) ||
                   IsChanged(oldValue.y, newValue.y) ||
                   IsChanged(oldValue.z, newValue.z);
        }

        private static bool IsChanged(float oldValue, float newValue, float tolerance = TOLERANCE) {
            return Math.Abs(oldValue - newValue) >= tolerance;
        }

        private static bool IsChanged(int oldValue, int newValue) {
            return oldValue != newValue;
        }

        private void ShowPlayerStats(bool show) {
            _textPlayerPosition.gameObject.SetActive(show);
            _textPlayerAngle.gameObject.SetActive(show);
            _textPlayerSpeed.gameObject.SetActive(show);
            _textPlayerLaser.gameObject.SetActive(show);
        }
    }

}
