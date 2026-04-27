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
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 2), Is.EqualTo(WeaponType.Burst));
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 3), Is.EqualTo(WeaponType.Plasma));
            Assert.That(PowerupCycle.GetWeaponTypeAt(WeaponType.Spread, 4), Is.EqualTo(WeaponType.Spread));
        }

        [TestCase(WeaponType.Spread, "散")]
        [TestCase(WeaponType.Laser, "激")]
        [TestCase(WeaponType.Burst, "爆")]
        [TestCase(WeaponType.Plasma, "离")]
        public void GetLabel_ReturnsChineseArcadeLabel(WeaponType type, string expectedLabel)
        {
            Assert.That(PowerupCycle.GetLabel(type), Is.EqualTo(expectedLabel));
        }

        [Test]
        public void GetColorCategory_GroupsPrimaryWeaponsByRaidenStyleColor()
        {
            Assert.That(PowerupCycle.GetColorCategory(WeaponType.Spread), Is.EqualTo(PowerupColorCategory.Red));
            Assert.That(PowerupCycle.GetColorCategory(AmmoPowerupType.Burst), Is.EqualTo(PowerupColorCategory.Red));
            Assert.That(PowerupCycle.GetColorCategory(WeaponType.Laser), Is.EqualTo(PowerupColorCategory.Blue));
            Assert.That(PowerupCycle.GetColorCategory(AmmoPowerupType.Plasma), Is.EqualTo(PowerupColorCategory.Purple));
        }

        [TestCase(AmmoPowerupType.Scatter, true)]
        [TestCase(AmmoPowerupType.Laser, true)]
        [TestCase(AmmoPowerupType.Plasma, true)]
        [TestCase(AmmoPowerupType.RapidFire, false)]
        [TestCase(AmmoPowerupType.Homing, false)]
        public void IsPrimaryWeaponPowerup_SeparatesCyclingCrystalsFromLetteredModules(AmmoPowerupType type, bool expected)
        {
            Assert.That(PowerupCycle.IsPrimaryWeaponPowerup(type), Is.EqualTo(expected));
        }
    }
}
