using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PowerupCycleTests
    {
        [Test]
        public void GetTypeAt_CyclesRedWeaponPool()
        {
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 0), Is.EqualTo(WeaponType.Spread));
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 1), Is.EqualTo(WeaponType.Laser));
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 2), Is.EqualTo(WeaponType.Homing));
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 3), Is.EqualTo(WeaponType.Burst));
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 4), Is.EqualTo(WeaponType.Spread));
        }

        [TestCase(WeaponType.Spread, "散")]
        [TestCase(WeaponType.Laser, "激")]
        [TestCase(WeaponType.Homing, "追")]
        [TestCase(WeaponType.Burst, "爆")]
        public void GetLabel_ReturnsChineseArcadeLabel(WeaponType type, string expectedLabel)
        {
            Assert.That(PowerupCycle.GetLabel(type), Is.EqualTo(expectedLabel));
        }
    }
}
