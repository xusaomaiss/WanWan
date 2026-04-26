using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class WeaponPickupStateTests
    {
        [Test]
        public void ApplyWeaponPickup_DifferentWeaponSwitchesWithoutIncreasingFireLevel()
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.UpgradeFireLevel();
            state.ApplyWeaponPickup(WeaponType.Laser);

            Assert.That(state.CurrentWeaponType, Is.EqualTo(WeaponType.Laser));
            Assert.That(state.FireLevel, Is.EqualTo(2));
            Assert.That(state.GetEquippedModules(), Is.Empty);
        }

        [Test]
        public void ApplyWeaponPickup_SameWeaponIncreasesFireLevel()
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.ApplyWeaponPickup(WeaponType.Spread);

            Assert.That(state.CurrentWeaponType, Is.EqualTo(WeaponType.Spread));
            Assert.That(state.FireLevel, Is.EqualTo(2));
        }

        [Test]
        public void ApplyWeaponPickup_FireLevelClampsAtFour()
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.ApplyWeaponPickup(WeaponType.Spread);
            state.ApplyWeaponPickup(WeaponType.Spread);
            state.ApplyWeaponPickup(WeaponType.Spread);
            state.ApplyWeaponPickup(WeaponType.Spread);

            Assert.That(state.FireLevel, Is.EqualTo(4));
        }

        [Test]
        public void ApplyDeathPenalty_ResetsWeaponAndReportsLostUpgrades()
        {
            PlayerWeaponState state = new PlayerWeaponState();
            state.ApplyWeaponPickup(WeaponType.Laser);
            state.ApplyWeaponPickup(WeaponType.Laser);
            state.ApplyPowerup(AmmoPowerupType.RapidFire);

            bool lostUpgrades = state.ApplyDeathPenalty();

            Assert.That(lostUpgrades, Is.True);
            Assert.That(state.CurrentWeaponType, Is.EqualTo(WeaponType.Spread));
            Assert.That(state.FireLevel, Is.EqualTo(1));
            Assert.That(state.GetEquippedModules(), Is.Empty);
        }

        [Test]
        public void ApplyDeathPenalty_DefaultWeaponReportsNoLostUpgrades()
        {
            PlayerWeaponState state = new PlayerWeaponState();

            bool lostUpgrades = state.ApplyDeathPenalty();

            Assert.That(lostUpgrades, Is.False);
            Assert.That(state.CurrentWeaponType, Is.EqualTo(WeaponType.Spread));
            Assert.That(state.FireLevel, Is.EqualTo(1));
        }

        [Test]
        public void ApplyPowerup_ModulePickupFillsTwoSlotsThenReplacesOldest()
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.ApplyPowerup(AmmoPowerupType.RapidFire);
            state.ApplyPowerup(AmmoPowerupType.Pierce);
            state.ApplyPowerup(AmmoPowerupType.Homing);

            Assert.That(state.GetEquippedModules(), Is.EqualTo(new[] { WeaponModuleType.Pierce, WeaponModuleType.Homing }));
            Assert.That(state.GetCurrentWeaponDisplayText(), Is.EqualTo("武器 扇形弹 1级 | 穿 | 追"));
        }

        [Test]
        public void ApplyPowerup_PrimaryPickupSwitchesWeaponAndSamePrimaryUpgrades()
        {
            PlayerWeaponState state = new PlayerWeaponState();

            state.ApplyPowerup(AmmoPowerupType.Plasma);
            state.ApplyPowerup(AmmoPowerupType.Plasma);

            Assert.That(state.CurrentWeaponType, Is.EqualTo(WeaponType.Plasma));
            Assert.That(state.FireLevel, Is.EqualTo(2));
        }
    }
}
