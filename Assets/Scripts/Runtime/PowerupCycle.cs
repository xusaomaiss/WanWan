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

        private static readonly AmmoPowerupType[] PrimaryCyclePool =
        {
            AmmoPowerupType.Scatter,
            AmmoPowerupType.Laser,
            AmmoPowerupType.Plasma
        };

        public static AmmoPowerupType GetTypeAt(AmmoPowerupType startingType, int index)
        {
            int startIndex = GetPowerupIndex(startingType);
            int safeIndex = Mathf.Abs(startIndex + index) % CyclePool.Length;
            AmmoPowerupType[] pool = CyclePool;
            return pool[safeIndex];
        }

        public static AmmoPowerupType GetPrimaryTypeAt(AmmoPowerupType startingType, int index)
        {
            AmmoPowerupType primaryStart = IsPrimaryWeaponPowerup(startingType) ? startingType : AmmoPowerupType.Scatter;
            if (primaryStart == AmmoPowerupType.Burst)
            {
                primaryStart = AmmoPowerupType.Scatter;
            }

            int startIndex = GetPrimaryPowerupIndex(primaryStart);
            int safeIndex = Mathf.Abs(startIndex + index) % PrimaryCyclePool.Length;
            return PrimaryCyclePool[safeIndex];
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

        public static bool IsPrimaryWeaponPowerup(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                case AmmoPowerupType.Laser:
                case AmmoPowerupType.Burst:
                case AmmoPowerupType.Plasma:
                    return true;
                default:
                    return false;
            }
        }

        public static PowerupColorCategory GetColorCategory(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Spread:
                case WeaponType.Burst:
                    return PowerupColorCategory.Red;
                case WeaponType.Laser:
                case WeaponType.Homing:
                    return PowerupColorCategory.Blue;
                case WeaponType.Plasma:
                    return PowerupColorCategory.Purple;
                default:
                    return PowerupColorCategory.None;
            }
        }

        public static PowerupColorCategory GetColorCategory(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                case AmmoPowerupType.RapidFire:
                case AmmoPowerupType.Burst:
                    return PowerupColorCategory.Red;
                case AmmoPowerupType.Laser:
                case AmmoPowerupType.Homing:
                case AmmoPowerupType.Pierce:
                    return PowerupColorCategory.Blue;
                case AmmoPowerupType.Plasma:
                case AmmoPowerupType.Wave:
                case AmmoPowerupType.Guard:
                    return PowerupColorCategory.Purple;
                default:
                    return PowerupColorCategory.None;
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

        public static string GetPickupLabel(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                case AmmoPowerupType.Burst:
                    return "V";
                case AmmoPowerupType.Laser:
                    return "L";
                case AmmoPowerupType.Plasma:
                    return "P";
                case AmmoPowerupType.Homing:
                    return "H";
                case AmmoPowerupType.Guard:
                    return "B";
                case AmmoPowerupType.RapidFire:
                case AmmoPowerupType.Pierce:
                    return "M";
                case AmmoPowerupType.Wave:
                    return "W";
                default:
                    return GetLabel(type);
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

        private static int GetPrimaryPowerupIndex(AmmoPowerupType type)
        {
            for (int i = 0; i < PrimaryCyclePool.Length; i++)
            {
                if (PrimaryCyclePool[i] == type)
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
