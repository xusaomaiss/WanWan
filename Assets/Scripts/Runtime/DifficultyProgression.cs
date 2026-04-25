using UnityEngine;

namespace Wanwan.Runtime
{
    public static class DifficultyProgression
    {
        private const float MaxToughChance = 0.55f;
        private const float MinAmmoChance = 0.11f;
        private const float MaxAmmoChance = 0.18f;

        public static float GetSpawnInterval(float elapsedSeconds)
        {
            float rampSeconds = Mathf.Max(0f, elapsedSeconds - 8f);
            return Mathf.Max(0.42f, 1.18f - (rampSeconds * 0.0052f));
        }

        public static float GetBlockSpeed(float elapsedSeconds, bool isTough)
        {
            float rampSeconds = Mathf.Max(0f, elapsedSeconds - 10f);
            float baseSpeed = 2.18f + (rampSeconds * 0.024f);
            return isTough ? baseSpeed * 0.85f : baseSpeed;
        }

        public static float GetToughChance(float elapsedSeconds)
        {
            float rampSeconds = Mathf.Max(0f, elapsedSeconds - 14f);
            return Mathf.Clamp(0.08f + (rampSeconds * 0.0031f), 0f, MaxToughChance);
        }

        public static float GetAmmoPackChance(float elapsedSeconds)
        {
            return Mathf.Clamp(MinAmmoChance + (elapsedSeconds * 0.00045f), MinAmmoChance, MaxAmmoChance);
        }

        public static float GetAmmoPackSpeed(float elapsedSeconds)
        {
            float rampSeconds = Mathf.Max(0f, elapsedSeconds - 10f);
            return 1.95f + (rampSeconds * 0.018f);
        }

        public static float GetEnemyFireInterval(float elapsedSeconds, bool isTough)
        {
            float rampSeconds = Mathf.Max(0f, elapsedSeconds - 12f);
            float baseInterval = Mathf.Max(1.72f, 3.82f - (rampSeconds * 0.0088f));
            return isTough ? baseInterval * 0.82f : baseInterval;
        }

        public static float GetEnemyFireballSpeed(float elapsedSeconds, bool isTough)
        {
            float rampSeconds = Mathf.Max(0f, elapsedSeconds - 10f);
            float baseSpeed = 3.05f + (rampSeconds * 0.016f);
            return isTough ? baseSpeed * 1.08f : baseSpeed;
        }

        public static int GetHitPoints(bool isTough, float elapsedSeconds)
        {
            if (!isTough)
            {
                return 1;
            }

            return elapsedSeconds >= 60f ? 3 : 2;
        }

        public static int GetScoreValue(bool isTough, int hitPoints)
        {
            return isTough ? 90 + (hitPoints * 30) : 45;
        }
    }
}
