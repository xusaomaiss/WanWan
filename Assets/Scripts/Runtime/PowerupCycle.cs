using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PowerupCycle
    {
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
    }
}
