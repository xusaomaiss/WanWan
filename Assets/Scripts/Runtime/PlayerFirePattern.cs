using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct PlayerShotSpec
    {
        public PlayerShotSpec(Vector3 offset, Vector2 direction, float width)
        {
            Offset = offset;
            Direction = direction.normalized;
            Width = width;
        }

        public Vector3 Offset { get; }
        public Vector2 Direction { get; }
        public float Width { get; }
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
