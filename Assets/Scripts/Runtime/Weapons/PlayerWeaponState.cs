using UnityEngine;
using Wanwan.Runtime.Weapons;

namespace Wanwan.Runtime
{
    public class PlayerWeaponState
    {
        private const int ModuleSlotCount = 2;
        private readonly WeaponModuleType[] modules = new WeaponModuleType[ModuleSlotCount];

        public WeaponType CurrentWeaponType { get; private set; } = WeaponType.Spread;
        public int FireLevel { get; private set; } = FireLevelState.MinLevel;

        public AmmoPickupOutcome ApplyPowerup(AmmoPowerupType type)
        {
            WeaponModuleType module = PowerupCycle.ToWeaponModuleType(type);
            if (module != WeaponModuleType.None)
            {
                EquipModule(module);
                WeaponModuleType[] equipped = GetEquippedModules();
                if (ModuleSynergy.Check(equipped).HasValue)
                {
                    return AmmoPickupOutcome.ModuleSynergy;
                }
                return AmmoPickupOutcome.ModuleEquipped;
            }

            if (!PowerupCycle.IsPrimaryWeaponPowerup(type))
            {
                return AmmoPickupOutcome.None;
            }

            PowerupColorCategory currentCategory = PowerupCycle.GetColorCategory(CurrentWeaponType);
            PowerupColorCategory pickupCategory = PowerupCycle.GetColorCategory(type);
            if (currentCategory != PowerupColorCategory.None && currentCategory == pickupCategory)
            {
                UpgradeFireLevel();
                return AmmoPickupOutcome.FireLevelUp;
            }

            return AmmoPickupOutcome.OffColorShield;
        }

        public void ApplyWeaponPickup(WeaponType pickupType)
        {
            if (CurrentWeaponType == pickupType)
            {
                UpgradeFireLevel();
                return;
            }

            SetWeapon(pickupType);
        }

        public void SetWeapon(WeaponType type)
        {
            CurrentWeaponType = type;
        }

        public void UpgradeFireLevel()
        {
            FireLevel = Mathf.Min(FireLevelState.MaxLevel, FireLevel + 1);
        }

        public bool ApplyDeathPenalty()
        {
            bool hadUpgrades = CurrentWeaponType != WeaponType.Spread || FireLevel > FireLevelState.MinLevel || GetEquippedModules().Length > 0;
            CurrentWeaponType = WeaponType.Spread;
            FireLevel = FireLevelState.MinLevel;
            ClearModules();
            return hadUpgrades;
        }

        public WeaponModuleType[] GetEquippedModules()
        {
            int count = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] != WeaponModuleType.None)
                {
                    count++;
                }
            }

            WeaponModuleType[] result = new WeaponModuleType[count];
            int writeIndex = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] != WeaponModuleType.None)
                {
                    result[writeIndex] = modules[i];
                    writeIndex++;
                }
            }

            return result;
        }

        public string GetCurrentWeaponDisplayText()
        {
            string text = $"武器 {WeaponConfig.Get(CurrentWeaponType).DisplayName} {FireLevel}级";
            WeaponModuleType[] equippedModules = GetEquippedModules();
            for (int i = 0; i < equippedModules.Length; i++)
            {
                text += " | " + PowerupCycle.GetModuleLabel(equippedModules[i]);
            }

            return text;
        }

        private void EquipModule(WeaponModuleType module)
        {
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] == module)
                {
                    MoveModuleToNewest(i, module);
                    return;
                }
            }

            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] == WeaponModuleType.None)
                {
                    modules[i] = module;
                    return;
                }
            }

            modules[0] = modules[1];
            modules[1] = module;
        }

        private void MoveModuleToNewest(int index, WeaponModuleType module)
        {
            for (int i = index; i < modules.Length - 1; i++)
            {
                modules[i] = modules[i + 1];
            }

            modules[modules.Length - 1] = module;
        }

        private void ClearModules()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i] = WeaponModuleType.None;
            }
        }
    }
}
