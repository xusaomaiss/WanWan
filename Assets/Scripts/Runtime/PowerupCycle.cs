using UnityEngine;

namespace Wanwan.Runtime
{
    public static class PowerupCycle
    {
        private static readonly WeaponType[] WeaponPool =
        {
            WeaponType.Spread,
            WeaponType.Laser,
            WeaponType.Burst,
            WeaponType.Plasma
        };

        private static readonly AmmoPowerupType[] CyclePool =
        {
            AmmoPowerupType.Scatter,
            AmmoPowerupType.RapidFire,
            AmmoPowerupType.Pierce,
            AmmoPowerupType.Laser,
            AmmoPowerupType.Homing,
            AmmoPowerupType.Burst,
            AmmoPowerupType.Wave,
            AmmoPowerupType.Plasma,
            AmmoPowerupType.Guard
        };

        public static AmmoPowerupType GetTypeAt(AmmoPowerupType startingType, int index)
        {
            int startIndex = GetPowerupIndex(startingType);
            int safeIndex = Mathf.Abs(startIndex + index) % CyclePool.Length;
            AmmoPowerupType[] pool = CyclePool;
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
            return GetLabel(ToAmmoPowerupType(type));
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
                case AmmoPowerupType.Burst:
                    return WeaponType.Burst;
                case AmmoPowerupType.Plasma:
                    return WeaponType.Plasma;
                default:
                    return WeaponType.Spread;
            }
        }

        public static WeaponModuleType ToWeaponModuleType(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.RapidFire:
                    return WeaponModuleType.RapidFire;
                case AmmoPowerupType.Pierce:
                    return WeaponModuleType.Pierce;
                case AmmoPowerupType.Homing:
                    return WeaponModuleType.Homing;
                case AmmoPowerupType.Wave:
                    return WeaponModuleType.Wave;
                case AmmoPowerupType.Guard:
                    return WeaponModuleType.Guard;
                default:
                    return WeaponModuleType.None;
            }
        }

        public static AmmoPowerupType ToAmmoPowerupType(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Laser:
                    return AmmoPowerupType.Laser;
                case WeaponType.Burst:
                    return AmmoPowerupType.Burst;
                case WeaponType.Plasma:
                    return AmmoPowerupType.Plasma;
                default:
                    return AmmoPowerupType.Scatter;
            }
        }

        public static AmmoPowerupType ToAmmoPowerupType(WeaponModuleType type)
        {
            switch (type)
            {
                case WeaponModuleType.RapidFire:
                    return AmmoPowerupType.RapidFire;
                case WeaponModuleType.Pierce:
                    return AmmoPowerupType.Pierce;
                case WeaponModuleType.Homing:
                    return AmmoPowerupType.Homing;
                case WeaponModuleType.Wave:
                    return AmmoPowerupType.Wave;
                case WeaponModuleType.Guard:
                    return AmmoPowerupType.Guard;
                default:
                    return AmmoPowerupType.None;
            }
        }

        public static string GetLabel(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    return "散";
                case AmmoPowerupType.RapidFire:
                    return "速";
                case AmmoPowerupType.Burst:
                    return "爆";
                case AmmoPowerupType.Laser:
                    return "激";
                case AmmoPowerupType.Homing:
                    return "追";
                case AmmoPowerupType.Pierce:
                    return "穿";
                case AmmoPowerupType.Plasma:
                    return "离";
                case AmmoPowerupType.Wave:
                    return "波";
                case AmmoPowerupType.Guard:
                    return "护";
                default:
                    return "无";
            }
        }

        public static string GetModuleLabel(WeaponModuleType type)
        {
            return GetLabel(ToAmmoPowerupType(type));
        }

        public static Color GetCategoryColor(AmmoPowerupType type)
        {
            if (type == AmmoPowerupType.Scatter || type == AmmoPowerupType.RapidFire || type == AmmoPowerupType.Burst)
            {
                return new Color(1f, 0.28f, 0.26f);
            }

            if (type == AmmoPowerupType.Laser || type == AmmoPowerupType.Homing || type == AmmoPowerupType.Pierce)
            {
                return new Color(0.24f, 0.68f, 1f);
            }

            if (type == AmmoPowerupType.Plasma || type == AmmoPowerupType.Wave || type == AmmoPowerupType.Guard)
            {
                return new Color(0.78f, 0.38f, 1f);
            }

            return RuntimeSpriteFactory.GetWeaponColor(type);
        }

        private static int GetPowerupIndex(AmmoPowerupType type)
        {
            for (int i = 0; i < CyclePool.Length; i++)
            {
                if (CyclePool[i] == type)
                {
                    return i;
                }
            }

            return 0;
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
