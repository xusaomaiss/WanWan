using UnityEngine;

namespace Wanwan.Runtime
{
    public class PlayerWeaponState
    {
        public WeaponType CurrentWeaponType { get; private set; } = WeaponType.Spread;
        public int FireLevel { get; private set; } = FireLevelState.MinLevel;

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

        public string GetCurrentWeaponDisplayText()
        {
            return $"Weapon: {WeaponConfig.Get(CurrentWeaponType).DisplayName} Lv.{FireLevel}";
        }
    }
}
