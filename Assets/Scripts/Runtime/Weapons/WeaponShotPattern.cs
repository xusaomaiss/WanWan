using UnityEngine;

namespace Wanwan.Runtime
{
    public static class WeaponShotPattern
    {
        public static PlayerShotSpec[] GetShots(WeaponType type, int fireLevel)
        {
            int level = Mathf.Clamp(fireLevel, FireLevelState.MinLevel, FireLevelState.MaxLevel);
            switch (type)
            {
                case WeaponType.Laser:
                    return GetLaserShots(level);
                case WeaponType.Homing:
                    return GetHomingShots(level);
                case WeaponType.Burst:
                    return GetBurstShots(level);
                default:
                    return GetSpreadShots(level);
            }
        }

        private static PlayerShotSpec[] GetSpreadShots(int level)
        {
            switch (level)
            {
                case 2:
                    return new[]
                    {
                        Build(WeaponType.Spread, new Vector3(-0.22f, 0.45f, 0f), new Vector2(-0.08f, 1f), 0.17f),
                        Build(WeaponType.Spread, new Vector3(0.22f, 0.45f, 0f), new Vector2(0.08f, 1f), 0.17f)
                    };
                case 3:
                    return new[]
                    {
                        Build(WeaponType.Spread, new Vector3(-0.28f, 0.42f, 0f), new Vector2(-0.16f, 1f), 0.16f),
                        Build(WeaponType.Spread, new Vector3(0f, 0.48f, 0f), Vector2.up, 0.18f),
                        Build(WeaponType.Spread, new Vector3(0.28f, 0.42f, 0f), new Vector2(0.16f, 1f), 0.16f)
                    };
                case 4:
                    return new[]
                    {
                        Build(WeaponType.Spread, new Vector3(-0.42f, 0.38f, 0f), new Vector2(-0.24f, 1f), 0.15f),
                        Build(WeaponType.Spread, new Vector3(-0.14f, 0.5f, 0f), new Vector2(-0.08f, 1f), 0.16f),
                        Build(WeaponType.Spread, new Vector3(0.14f, 0.5f, 0f), new Vector2(0.08f, 1f), 0.16f),
                        Build(WeaponType.Spread, new Vector3(0.42f, 0.38f, 0f), new Vector2(0.24f, 1f), 0.15f)
                    };
                default:
                    return new[]
                    {
                        Build(WeaponType.Spread, Vector3.up * 0.45f, Vector2.up, 0.192f)
                    };
            }
        }

        private static PlayerShotSpec[] GetLaserShots(int level)
        {
            int damage = level >= 4 ? 3 : 2;
            if (level <= 1)
            {
                return new[] { Build(WeaponType.Laser, Vector3.up * 0.45f, Vector2.up, 0.14f, true, 4, damage) };
            }

            if (level == 2)
            {
                return new[]
                {
                    Build(WeaponType.Laser, new Vector3(-0.18f, 0.44f, 0f), Vector2.up, 0.12f, true, 4, damage),
                    Build(WeaponType.Laser, new Vector3(0.18f, 0.44f, 0f), Vector2.up, 0.12f, true, 4, damage)
                };
            }

            return new[]
            {
                Build(WeaponType.Laser, new Vector3(-0.3f, 0.42f, 0f), Vector2.up, 0.11f, true, 5, damage),
                Build(WeaponType.Laser, Vector3.up * 0.5f, Vector2.up, 0.14f, true, 5, damage),
                Build(WeaponType.Laser, new Vector3(0.3f, 0.42f, 0f), Vector2.up, 0.11f, true, 5, damage)
            };
        }

        private static PlayerShotSpec[] GetHomingShots(int level)
        {
            if (level <= 1)
            {
                return new[] { Build(WeaponType.Homing, Vector3.up * 0.45f, Vector2.up, 0.18f, false, 1, 1, BulletMotionType.Homing, 3.4f) };
            }

            if (level == 2)
            {
                return new[]
                {
                    Build(WeaponType.Homing, new Vector3(-0.22f, 0.4f, 0f), new Vector2(-0.08f, 1f), 0.16f, false, 1, 1, BulletMotionType.Homing, 3.2f),
                    Build(WeaponType.Homing, new Vector3(0.22f, 0.4f, 0f), new Vector2(0.08f, 1f), 0.16f, false, 1, 1, BulletMotionType.Homing, 3.2f)
                };
            }

            if (level == 3)
            {
                return new[]
                {
                    Build(WeaponType.Homing, new Vector3(-0.28f, 0.38f, 0f), new Vector2(-0.12f, 1f), 0.15f, false, 1, 1, BulletMotionType.Homing, 3.2f),
                    Build(WeaponType.Homing, Vector3.up * 0.48f, Vector2.up, 0.17f, false, 1, 1, BulletMotionType.Homing, 3.6f),
                    Build(WeaponType.Homing, new Vector3(0.28f, 0.38f, 0f), new Vector2(0.12f, 1f), 0.15f, false, 1, 1, BulletMotionType.Homing, 3.2f)
                };
            }

            return new[]
            {
                Build(WeaponType.Homing, new Vector3(-0.42f, 0.34f, 0f), new Vector2(-0.16f, 1f), 0.14f, false, 1, 1, BulletMotionType.Homing, 3.2f),
                Build(WeaponType.Homing, new Vector3(-0.14f, 0.48f, 0f), new Vector2(-0.04f, 1f), 0.15f, false, 1, 1, BulletMotionType.Homing, 3.6f),
                Build(WeaponType.Homing, new Vector3(0.14f, 0.48f, 0f), new Vector2(0.04f, 1f), 0.15f, false, 1, 1, BulletMotionType.Homing, 3.6f),
                Build(WeaponType.Homing, new Vector3(0.42f, 0.34f, 0f), new Vector2(0.16f, 1f), 0.14f, false, 1, 1, BulletMotionType.Homing, 3.2f)
            };
        }

        private static PlayerShotSpec[] GetBurstShots(int level)
        {
            float radius = level >= 4 ? 1.05f : 0.72f;
            if (level <= 1)
            {
                return new[] { Build(WeaponType.Burst, Vector3.up * 0.45f, Vector2.up, 0.24f, false, 1, 1, BulletMotionType.Straight, 0f, radius) };
            }

            if (level == 2)
            {
                return new[]
                {
                    Build(WeaponType.Burst, new Vector3(-0.24f, 0.42f, 0f), new Vector2(-0.06f, 1f), 0.22f, false, 1, 1, BulletMotionType.Straight, 0f, radius),
                    Build(WeaponType.Burst, new Vector3(0.24f, 0.42f, 0f), new Vector2(0.06f, 1f), 0.22f, false, 1, 1, BulletMotionType.Straight, 0f, radius)
                };
            }

            return new[]
            {
                Build(WeaponType.Burst, new Vector3(-0.34f, 0.38f, 0f), new Vector2(-0.12f, 1f), 0.2f, false, 1, 1, BulletMotionType.Straight, 0f, radius),
                Build(WeaponType.Burst, Vector3.up * 0.48f, Vector2.up, 0.23f, false, 1, level >= 4 ? 2 : 1, BulletMotionType.Straight, 0f, radius),
                Build(WeaponType.Burst, new Vector3(0.34f, 0.38f, 0f), new Vector2(0.12f, 1f), 0.2f, false, 1, 1, BulletMotionType.Straight, 0f, radius)
            };
        }

        private static PlayerShotSpec Build(WeaponType type, Vector3 offset, Vector2 direction, float width, bool canPierce = false, int pierceHits = 1, int damage = 1, BulletMotionType motionType = BulletMotionType.Straight, float homingStrength = 0f, float explosionRadius = 0f)
        {
            return new PlayerShotSpec(offset, direction, width, type, canPierce, pierceHits, damage, motionType, homingStrength, explosionRadius);
        }
    }
}
