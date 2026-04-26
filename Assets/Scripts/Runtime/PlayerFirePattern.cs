using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct PlayerShotSpec
    {
        public PlayerShotSpec(Vector3 offset, Vector2 direction, float width)
            : this(offset, direction, width, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 1f, 0f, 0f)
        {
        }

        public PlayerShotSpec(Vector3 offset, Vector2 direction, float width, WeaponType weaponType, bool canPierce, int pierceHits, int damage, BulletMotionType motionType, float homingStrength, float explosionRadius)
            : this(offset, direction, width, weaponType, canPierce, pierceHits, damage, motionType, homingStrength, explosionRadius, 1f, 0f, 0f)
        {
        }

        public PlayerShotSpec(Vector3 offset, Vector2 direction, float width, WeaponType weaponType, bool canPierce, int pierceHits, int damage, BulletMotionType motionType, float homingStrength, float explosionRadius, float speedMultiplier, float waveAmplitude, float waveFrequency)
        {
            Offset = offset;
            Direction = direction.normalized;
            Width = width;
            WeaponType = weaponType;
            CanPierce = canPierce;
            PierceHits = Mathf.Max(1, pierceHits);
            Damage = Mathf.Max(1, damage);
            MotionType = motionType;
            HomingStrength = Mathf.Max(0f, homingStrength);
            ExplosionRadius = Mathf.Max(0f, explosionRadius);
            SpeedMultiplier = Mathf.Max(0.1f, speedMultiplier);
            WaveAmplitude = Mathf.Max(0f, waveAmplitude);
            WaveFrequency = Mathf.Max(0f, waveFrequency);
        }

        public Vector3 Offset { get; }
        public Vector2 Direction { get; }
        public float Width { get; }
        public WeaponType WeaponType { get; }
        public bool CanPierce { get; }
        public int PierceHits { get; }
        public int Damage { get; }
        public BulletMotionType MotionType { get; }
        public float HomingStrength { get; }
        public float ExplosionRadius { get; }
        public float SpeedMultiplier { get; }
        public float WaveAmplitude { get; }
        public float WaveFrequency { get; }
    }

    public static class PlayerFirePattern
    {
        public static int GetShotCount(int fireLevel)
        {
            return GetShots(fireLevel).Length;
        }

        public static PlayerShotSpec[] GetShots(int fireLevel)
        {
            switch (Mathf.Clamp(fireLevel, FireLevelState.MinLevel, FireLevelState.MaxLevel))
            {
                case 2:
                    return new[]
                    {
                        new PlayerShotSpec(new Vector3(-0.22f, 0.45f, 0f), Vector2.up, 0.17f),
                        new PlayerShotSpec(new Vector3(0.22f, 0.45f, 0f), Vector2.up, 0.17f)
                    };
                case 3:
                    return new[]
                    {
                        new PlayerShotSpec(new Vector3(-0.28f, 0.42f, 0f), new Vector2(-0.16f, 1f), 0.16f),
                        new PlayerShotSpec(new Vector3(0f, 0.48f, 0f), Vector2.up, 0.18f),
                        new PlayerShotSpec(new Vector3(0.28f, 0.42f, 0f), new Vector2(0.16f, 1f), 0.16f)
                    };
                case 4:
                    return new[]
                    {
                        new PlayerShotSpec(new Vector3(-0.48f, 0.34f, 0f), new Vector2(-0.24f, 1f), 0.145f),
                        new PlayerShotSpec(new Vector3(-0.24f, 0.46f, 0f), new Vector2(-0.08f, 1f), 0.16f),
                        new PlayerShotSpec(new Vector3(0f, 0.54f, 0f), Vector2.up, 0.18f),
                        new PlayerShotSpec(new Vector3(0.24f, 0.46f, 0f), new Vector2(0.08f, 1f), 0.16f),
                        new PlayerShotSpec(new Vector3(0.48f, 0.34f, 0f), new Vector2(0.24f, 1f), 0.145f),
                        new PlayerShotSpec(new Vector3(-0.12f, 0.18f, 0f), new Vector2(-0.02f, 1f), 0.14f),
                        new PlayerShotSpec(new Vector3(0.12f, 0.18f, 0f), new Vector2(0.02f, 1f), 0.14f)
                    };
                default:
                    return new[]
                    {
                        new PlayerShotSpec(Vector3.up * 0.45f, Vector2.up, 0.192f)
                    };
            }
        }
    }
}
