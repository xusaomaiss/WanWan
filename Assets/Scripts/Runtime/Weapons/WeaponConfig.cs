using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct WeaponConfig
    {
        public WeaponConfig(WeaponType type, string displayName, float fireInterval, float bulletSpeed, int baseDamage, bool pierce, float explosionRadius, float homingStrength, Color uiColor)
        {
            Type = type;
            DisplayName = displayName;
            FireInterval = fireInterval;
            BulletSpeed = bulletSpeed;
            BaseDamage = Mathf.Max(1, baseDamage);
            Pierce = pierce;
            ExplosionRadius = Mathf.Max(0f, explosionRadius);
            HomingStrength = Mathf.Max(0f, homingStrength);
            UiColor = uiColor;
        }

        public WeaponType Type { get; }
        public string DisplayName { get; }
        public float FireInterval { get; }
        public float BulletSpeed { get; }
        public int BaseDamage { get; }
        public bool Pierce { get; }
        public float ExplosionRadius { get; }
        public float HomingStrength { get; }
        public Color UiColor { get; }

        public static WeaponConfig Get(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Laser:
                    return new WeaponConfig(type, "激光弹", 0.12f, 27.5f, 2, true, 0f, 0f, new Color(0.2f, 0.82f, 1f));
                case WeaponType.Homing:
                    return new WeaponConfig(type, "追踪弹", 0.15f, 24.5f, 1, false, 0f, 3.8f, new Color(0.28f, 0.72f, 1f));
                case WeaponType.Burst:
                    return new WeaponConfig(type, "爆裂弹", 0.18f, 24f, 1, false, 0.82f, 0f, new Color(1f, 0.22f, 0.34f));
                case WeaponType.Plasma:
                    return new WeaponConfig(type, "等离子弹", 0.2f, 23.5f, 2, false, 1.02f, 0f, new Color(0.7f, 0.38f, 1f));
                default:
                    return new WeaponConfig(WeaponType.Spread, "扇形弹", 0.13f, 26.5f, 1, false, 0f, 0f, new Color(1f, 0.48f, 0.32f));
            }
        }
    }
}
