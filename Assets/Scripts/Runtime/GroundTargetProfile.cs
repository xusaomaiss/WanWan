using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct GroundTargetProfile
    {
        public GroundTargetProfile(GroundTargetType type, int hitPoints, int scoreValue, float fireInterval, Color accentColor)
        {
            Type = type;
            HitPoints = Mathf.Max(1, hitPoints);
            ScoreValue = Mathf.Max(0, scoreValue);
            FireInterval = Mathf.Max(0.25f, fireInterval);
            AccentColor = accentColor;
        }

        public GroundTargetType Type { get; }
        public int HitPoints { get; }
        public int ScoreValue { get; }
        public float FireInterval { get; }
        public Color AccentColor { get; }

        public static GroundTargetProfile Get(GroundTargetType type, float difficultyMultiplier)
        {
            float safeMultiplier = Mathf.Max(1f, difficultyMultiplier);
            switch (type)
            {
                case GroundTargetType.Turret:
                    return new GroundTargetProfile(
                        type,
                        Mathf.CeilToInt(3 * safeMultiplier),
                        Mathf.CeilToInt(180 * safeMultiplier),
                        Mathf.Lerp(2.35f, 1.35f, Mathf.Clamp01((safeMultiplier - 1f) * 0.85f)),
                        new Color(1f, 0.72f, 0.28f));
                default:
                    return new GroundTargetProfile(
                        GroundTargetType.Tank,
                        Mathf.CeilToInt(2 * safeMultiplier),
                        Mathf.CeilToInt(140 * safeMultiplier),
                        Mathf.Lerp(2.8f, 1.7f, Mathf.Clamp01((safeMultiplier - 1f) * 0.72f)),
                        new Color(0.48f, 0.88f, 0.46f));
            }
        }
    }
}
