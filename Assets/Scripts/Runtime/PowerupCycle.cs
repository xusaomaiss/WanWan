using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PowerupCycle
    {
        private static readonly WeaponType[] WeaponPool =
        {
            WeaponType.Spread,
            WeaponType.Laser,
            WeaponType.Homing,
            WeaponType.Burst
        };

        private static readonly AmmoPowerupType[] RedPool =
        {
            AmmoPowerupType.Scatter,
            AmmoPowerupType.RapidFire,
            AmmoPowerupType.Burst
        };

        private static readonly AmmoPowerupType[] BluePool =
        {
            AmmoPowerupType.Laser,
            AmmoPowerupType.Homing,
            AmmoPowerupType.Pierce
        };

        private static readonly AmmoPowerupType[] PurplePool =
        {
            AmmoPowerupType.Plasma,
            AmmoPowerupType.Wave,
            AmmoPowerupType.Guard
        };

        public static AmmoPowerupType GetTypeAt(AmmoPowerupType startingType, int index)
        {
            AmmoPowerupType[] pool = GetPool(startingType);
            int safeIndex = Mathf.Abs(index) % pool.Length;
            return pool[safeIndex];
        }

        public static WeaponType GetWeaponTypeAt(WeaponType startingType, int index)
        {
            int startIndex = GetWeaponIndex(startingType);
            int safeIndex = Mathf.Abs(startIndex + index) % WeaponPool.Length;
            return WeaponPool[safeIndex];
        }

        public static string GetLabel(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Laser:
                    return "L";
                case WeaponType.Homing:
                    return "H";
                case WeaponType.Burst:
                    return "B";
                default:
                    return "S";
            }
        }

        public static Color GetCategoryColor(WeaponType type)
        {
            return RuntimeSpriteFactory.GetWeaponColor(type);
        }

        public static WeaponType ToWeaponType(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Laser:
                case AmmoPowerupType.Pierce:
                    return WeaponType.Laser;
                case AmmoPowerupType.Homing:
                case AmmoPowerupType.Wave:
                    return WeaponType.Homing;
                case AmmoPowerupType.Burst:
                case AmmoPowerupType.Plasma:
                    return WeaponType.Burst;
                default:
                    return WeaponType.Spread;
            }
        }

        public static AmmoPowerupType ToAmmoPowerupType(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Laser:
                    return AmmoPowerupType.Laser;
                case WeaponType.Homing:
                    return AmmoPowerupType.Homing;
                case WeaponType.Burst:
                    return AmmoPowerupType.Burst;
                default:
                    return AmmoPowerupType.Scatter;
            }
        }

        public static string GetLabel(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    return "S";
                case AmmoPowerupType.RapidFire:
                    return "R";
                case AmmoPowerupType.Burst:
                    return "B";
                case AmmoPowerupType.Laser:
                    return "L";
                case AmmoPowerupType.Homing:
                    return "H";
                case AmmoPowerupType.Pierce:
                    return "P";
                case AmmoPowerupType.Plasma:
                    return "O";
                case AmmoPowerupType.Wave:
                    return "W";
                case AmmoPowerupType.Guard:
                    return "G";
                default:
                    return "N";
            }
        }

        public static Color GetCategoryColor(AmmoPowerupType type)
        {
            if (IsInPool(type, RedPool))
            {
                return new Color(1f, 0.28f, 0.26f);
            }

            if (IsInPool(type, BluePool))
            {
                return new Color(0.24f, 0.68f, 1f);
            }

            if (IsInPool(type, PurplePool))
            {
                return new Color(0.78f, 0.38f, 1f);
            }

            return RuntimeSpriteFactory.GetWeaponColor(type);
        }

        private static AmmoPowerupType[] GetPool(AmmoPowerupType type)
        {
            if (IsInPool(type, BluePool))
            {
                return BluePool;
            }

            if (IsInPool(type, PurplePool))
            {
                return PurplePool;
            }

            return RedPool;
        }

        private static bool IsInPool(AmmoPowerupType type, AmmoPowerupType[] pool)
        {
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] == type)
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetWeaponIndex(WeaponType type)
        {
            for (int i = 0; i < WeaponPool.Length; i++)
            {
                if (WeaponPool[i] == type)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
