using System;

namespace Asteroids.UI {

    public static class AsteroidsUIExtensions {

        public static string FormatScore(int score) {
            return Math.Abs(score) < 10 ? $"0{score}" : $"{score}";
        }
    }

}
