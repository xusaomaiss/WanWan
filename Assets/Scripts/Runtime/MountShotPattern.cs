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
                        new PlayerShotSpec(new Vector3(-0.36f, 0.18f, 0f), new Vector2(-0.06f, 1f), 0.2f, WeaponType.Burst, false, 1, 1, BulletMotionType.Homing, 3.2f, 0.58f, 0.92f, 0f, 0f),
                        new PlayerShotSpec(new Vector3(0.36f, 0.18f, 0f), new Vector2(0.06f, 1f), 0.2f, WeaponType.Burst, false, 1, 1, BulletMotionType.Homing, 3.2f, 0.58f, 0.92f, 0f, 0f)
                    };
                case MountType.DefenseDrone:
                    return new[]
                    {
                        new PlayerShotSpec(new Vector3(-0.62f, 0.14f, 0f), new Vector2(-0.03f, 1f), 0.13f, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0.86f, 0f, 0f),
                        new PlayerShotSpec(new Vector3(0.62f, 0.14f, 0f), new Vector2(0.03f, 1f), 0.13f, WeaponType.Spread, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0.86f, 0f, 0f)
                    };
                default:
                    return new PlayerShotSpec[0];
            }
        }
    }
}
