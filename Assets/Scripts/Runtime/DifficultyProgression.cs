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
            return Mathf.Max(0.35f, 1.1f - (elapsedSeconds * 0.006f));
        }

        public static float GetBlockSpeed(float elapsedSeconds, bool isTough)
        {
            float baseSpeed = 2.25f + (elapsedSeconds * 0.03f);
            return isTough ? baseSpeed * 0.85f : baseSpeed;
        }

        public static float GetToughChance(float elapsedSeconds)
        {
            return Mathf.Clamp(0.12f + (elapsedSeconds * 0.0035f), 0f, MaxToughChance);
        }

        public static float GetAmmoPackChance(float elapsedSeconds)
        {
            return Mathf.Clamp(MinAmmoChance + (elapsedSeconds * 0.00045f), MinAmmoChance, MaxAmmoChance);
        }

        public static float GetAmmoPackSpeed(float elapsedSeconds)
        {
            return 2.05f + (elapsedSeconds * 0.022f);
        }

        public static float GetEnemyFireInterval(float elapsedSeconds, bool isTough)
        {
            float baseInterval = Mathf.Max(1.55f, 3.6f - (elapsedSeconds * 0.01f));
            return isTough ? baseInterval * 0.78f : baseInterval;
        }

        public static float GetEnemyFireballSpeed(float elapsedSeconds, bool isTough)
        {
            float baseSpeed = 3.2f + (elapsedSeconds * 0.02f);
            return isTough ? baseSpeed * 1.12f : baseSpeed;
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
