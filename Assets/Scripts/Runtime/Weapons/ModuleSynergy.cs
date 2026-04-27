using System;

namespace Wanwan.Runtime.Weapons
{
    public struct SynergyDefinition
    {
        public WeaponModuleType ModuleA;
        public WeaponModuleType ModuleB;
        public string DisplayName;
    }

    public static class ModuleSynergy
    {
        private static readonly SynergyDefinition[] Synergies = new[]
        {
            new SynergyDefinition
            {
                ModuleA = WeaponModuleType.Pierce,
                ModuleB = WeaponModuleType.Wave,
                DisplayName = "贯穿弹附带横向扩散"
            },
            new SynergyDefinition
            {
                ModuleA = WeaponModuleType.Homing,
                ModuleB = WeaponModuleType.RapidFire,
                DisplayName = "追踪弹射速提升"
            }
        };

        public static SynergyDefinition? Check(WeaponModuleType[] equipped)
        {
            if (equipped == null || equipped.Length < 2)
                return null;

            for (int i = 0; i < Synergies.Length; i++)
            {
                var synergy = Synergies[i];
                bool hasA = Array.Exists(equipped, m => m == synergy.ModuleA);
                bool hasB = Array.Exists(equipped, m => m == synergy.ModuleB);
                if (hasA && hasB)
                    return synergy;
            }

            return null;
        }
    }
}
