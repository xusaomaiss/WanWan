using UnityEngine;

namespace Wanwan.Runtime
{
    public static class MountShotPattern
    {
        public static PlayerShotSpec[] GetShots(MountType type)
        {
            switch (type)
            {
                case MountType.MissilePod:
                    return new[]
                    {
                        new PlayerShotSpec(new Vector3(-0.42f, 0.18f, 0f), new Vector2(-0.08f, 1f), 0.34f, WeaponType.Burst, false, 1, 2, BulletMotionType.Homing, 5.2f, 0.72f, 0.9f, 0f, 0f),
                        new PlayerShotSpec(new Vector3(0.42f, 0.18f, 0f), new Vector2(0.08f, 1f), 0.34f, WeaponType.Burst, false, 1, 2, BulletMotionType.Homing, 5.2f, 0.72f, 0.9f, 0f, 0f)
                    };
                case MountType.DefenseDrone:
                    return new[]
                    {
                        new PlayerShotSpec(new Vector3(-0.72f, 0.18f, 0f), new Vector2(-0.12f, 1f), 0.14f, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 1f, 0f, 0f),
                        new PlayerShotSpec(new Vector3(-0.52f, 0.24f, 0f), new Vector2(0.02f, 1f), 0.14f, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 1f, 0f, 0f),
                        new PlayerShotSpec(new Vector3(0.52f, 0.24f, 0f), new Vector2(-0.02f, 1f), 0.14f, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 1f, 0f, 0f),
                        new PlayerShotSpec(new Vector3(0.72f, 0.18f, 0f), new Vector2(0.12f, 1f), 0.14f, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 1f, 0f, 0f)
                    };
                default:
                    return new PlayerShotSpec[0];
            }
        }
    }
}
