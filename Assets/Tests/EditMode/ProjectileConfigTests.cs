using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class ProjectileConfigTests
    {
        [TestCase(WeaponType.Spread, 1, 1)]
        [TestCase(WeaponType.Spread, 2, 2)]
        [TestCase(WeaponType.Spread, 3, 3)]
        [TestCase(WeaponType.Spread, 4, 4)]
        [TestCase(WeaponType.Laser, 1, 1)]
        [TestCase(WeaponType.Laser, 2, 2)]
        [TestCase(WeaponType.Laser, 3, 3)]
        [TestCase(WeaponType.Laser, 4, 3)]
        [TestCase(WeaponType.Homing, 1, 1)]
        [TestCase(WeaponType.Homing, 2, 2)]
        [TestCase(WeaponType.Homing, 3, 3)]
        [TestCase(WeaponType.Homing, 4, 4)]
        [TestCase(WeaponType.Burst, 1, 1)]
        [TestCase(WeaponType.Burst, 2, 2)]
        [TestCase(WeaponType.Burst, 3, 3)]
        [TestCase(WeaponType.Burst, 4, 3)]
        public void GetShots_ReturnsExpectedCountForWeaponLevel(WeaponType type, int level, int expectedCount)
        {
            Assert.That(WeaponShotPattern.GetShots(type, level).Length, Is.EqualTo(expectedCount));
        }
    }
}
