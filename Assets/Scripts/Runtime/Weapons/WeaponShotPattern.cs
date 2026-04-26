using UnityEngine;
using System.Collections.Generic;

namespace Wanwan.Runtime
{
    public static class WeaponShotPattern
    {
        public static PlayerShotSpec[] GetShots(WeaponType type, int fireLevel)
        {
            return GetShots(type, fireLevel, new WeaponModuleType[0]);
        }

        public static PlayerShotSpec[] GetShots(WeaponType type, int fireLevel, WeaponModuleType[] modules)
        {
            int level = Mathf.Clamp(fireLevel, FireLevelState.MinLevel, FireLevelState.MaxLevel);
            PlayerShotSpec[] baseShots;
            switch (type)
            {
                case WeaponType.Laser:
                    baseShots = GetLaserShots(level);
                    break;
                case WeaponType.Homing:
                    baseShots = GetHomingShots(level);
                    break;
                case WeaponType.Burst:
                    baseShots = GetBurstShots(level);
                    break;
                case WeaponType.Plasma:
                    baseShots = GetPlasmaShots(level);
                    break;
                default:
                    baseShots = GetSpreadShots(level);
                    break;
            }

            return ApplyModules(baseShots, modules);
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

        private static PlayerShotSpec[] GetPlasmaShots(int level)
        {
            float radius = level >= 4 ? 1.28f : 1.02f;
            if (level <= 1)
            {
                return new[] { Build(WeaponType.Plasma, Vector3.up * 0.45f, Vector2.up, 0.3f, false, 1, 2, BulletMotionType.Straight, 0f, radius) };
            }

            if (level == 2)
            {
                return new[]
                {
                    Build(WeaponType.Plasma, new Vector3(-0.2f, 0.42f, 0f), new Vector2(-0.04f, 1f), 0.26f, false, 1, 2, BulletMotionType.Straight, 0f, radius),
                    Build(WeaponType.Plasma, new Vector3(0.2f, 0.42f, 0f), new Vector2(0.04f, 1f), 0.26f, false, 1, 2, BulletMotionType.Straight, 0f, radius)
                };
            }

            return new[]
            {
                Build(WeaponType.Plasma, new Vector3(-0.34f, 0.36f, 0f), new Vector2(-0.1f, 1f), 0.23f, false, 1, 1, BulletMotionType.Straight, 0f, radius),
                Build(WeaponType.Plasma, Vector3.up * 0.5f, Vector2.up, 0.3f, false, 1, level >= 4 ? 3 : 2, BulletMotionType.Straight, 0f, radius),
                Build(WeaponType.Plasma, new Vector3(0.34f, 0.36f, 0f), new Vector2(0.1f, 1f), 0.23f, false, 1, 1, BulletMotionType.Straight, 0f, radius)
            };
        }

        private static PlayerShotSpec[] ApplyModules(PlayerShotSpec[] baseShots, WeaponModuleType[] modules)
        {
            if (modules == null || modules.Length == 0)
            {
                return baseShots;
            }

            List<PlayerShotSpec> shots = new List<PlayerShotSpec>(baseShots);
            float speedMultiplier = 1f;
            int pierceBonus = 0;
            bool homing = false;
            bool wave = false;
            bool guard = false;

            for (int i = 0; i < modules.Length; i++)
            {
                switch (modules[i])
                {
                    case WeaponModuleType.RapidFire:
                        speedMultiplier = Mathf.Max(speedMultiplier, 1.18f);
                        break;
                    case WeaponModuleType.Pierce:
                        pierceBonus = Mathf.Max(pierceBonus, 2);
                        break;
                    case WeaponModuleType.Homing:
                        homing = true;
                        break;
                    case WeaponModuleType.Wave:
                        wave = true;
                        break;
                    case WeaponModuleType.Guard:
                        guard = true;
                        break;
                }
            }

            for (int i = 0; i < shots.Count; i++)
            {
                PlayerShotSpec shot = shots[i];
                bool canPierce = shot.CanPierce || pierceBonus > 0;
                int pierceHits = shot.PierceHits + pierceBonus;
                int damage = shot.Damage;
                BulletMotionType motionType = shot.MotionType;
                float homingStrength = shot.HomingStrength;
                float explosionRadius = shot.ExplosionRadius;
                float waveAmplitude = shot.WaveAmplitude;
                float waveFrequency = shot.WaveFrequency;

                if (homing && i % 2 == 0)
                {
                    motionType = BulletMotionType.Homing;
                    homingStrength = Mathf.Max(homingStrength, 3.6f);
                }

                if (wave && i % 2 == 1)
                {
                    motionType = BulletMotionType.Wave;
                    waveAmplitude = Mathf.Max(waveAmplitude, 0.18f + (Mathf.Abs(shot.Offset.x) * 0.18f));
                    waveFrequency = Mathf.Max(waveFrequency, 10f);
                }

                shots[i] = new PlayerShotSpec(shot.Offset, shot.Direction, shot.Width, shot.WeaponType, canPierce, pierceHits, damage, motionType, homingStrength, explosionRadius, speedMultiplier, waveAmplitude, waveFrequency);
            }

            if (guard)
            {
                WeaponType type = shots.Count > 0 ? shots[0].WeaponType : WeaponType.Spread;
                shots.Add(new PlayerShotSpec(new Vector3(-0.54f, -0.02f, 0f), new Vector2(-0.03f, 1f), 0.14f, type, pierceBonus > 0, Mathf.Max(1, 1 + pierceBonus), 1, wave ? BulletMotionType.Wave : BulletMotionType.Straight, homing ? 3f : 0f, 0f, speedMultiplier, wave ? 0.16f : 0f, wave ? 11f : 0f));
                shots.Add(new PlayerShotSpec(new Vector3(0.54f, -0.02f, 0f), new Vector2(0.03f, 1f), 0.14f, type, pierceBonus > 0, Mathf.Max(1, 1 + pierceBonus), 1, wave ? BulletMotionType.Wave : BulletMotionType.Straight, homing ? 3f : 0f, 0f, speedMultiplier, wave ? 0.16f : 0f, wave ? 11f : 0f));
            }

            return shots.ToArray();
        }

        private static PlayerShotSpec Build(WeaponType type, Vector3 offset, Vector2 direction, float width, bool canPierce = false, int pierceHits = 1, int damage = 1, BulletMotionType motionType = BulletMotionType.Straight, float homingStrength = 0f, float explosionRadius = 0f)
        {
            return new PlayerShotSpec(offset, direction, width, type, canPierce, pierceHits, damage, motionType, homingStrength, explosionRadius);
        }
    }
}
