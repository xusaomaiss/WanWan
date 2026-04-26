using UnityEngine;

namespace Wanwan.Runtime
{
    public static class VisualEffectsBudget
    {
        public const int FullGroundDetailTileLimit = 18;
        public const int BatterySaverGroundDetailTileLimit = 8;
        public const int FullBossExplosionCascadeCount = 7;
        public const int BatterySaverBossExplosionCascadeCount = 3;

        public static int GetGroundDetailTileLimit(VisualEffectsQuality quality)
        {
            return quality == VisualEffectsQuality.BatterySaver ? BatterySaverGroundDetailTileLimit : FullGroundDetailTileLimit;
        }

        public static int GetExplosionCascadeCount(VisualEffectsQuality quality)
        {
            return quality == VisualEffectsQuality.BatterySaver ? BatterySaverBossExplosionCascadeCount : FullBossExplosionCascadeCount;
        }

        public static int GetParticleCount(VisualEffectsQuality quality, int requestedCount)
        {
            int safeCount = Mathf.Max(0, requestedCount);
            return quality == VisualEffectsQuality.BatterySaver ? Mathf.CeilToInt(safeCount * 0.48f) : safeCount;
        }

        public static float GetShakeMagnitude(VisualEffectsQuality quality, float requestedMagnitude)
        {
            return quality == VisualEffectsQuality.BatterySaver ? requestedMagnitude * 0.55f : requestedMagnitude;
        }

        public static float GetShakeDuration(VisualEffectsQuality quality, float requestedDuration)
        {
            return quality == VisualEffectsQuality.BatterySaver ? requestedDuration * 0.72f : requestedDuration;
        }
    }
}
