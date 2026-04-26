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
    }
}
