using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PowerupCycleTests
    {
        [Test]
        public void GetTypeAt_CyclesRedWeaponPool()
        {
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Scatter, 0), Is.EqualTo(AmmoPowerupType.Scatter));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Scatter, 1), Is.EqualTo(AmmoPowerupType.RapidFire));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Scatter, 2), Is.EqualTo(AmmoPowerupType.Burst));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Scatter, 3), Is.EqualTo(AmmoPowerupType.Scatter));
        }

        [Test]
        public void GetTypeAt_CyclesBlueWeaponPool()
        {
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Laser, 0), Is.EqualTo(AmmoPowerupType.Laser));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Laser, 1), Is.EqualTo(AmmoPowerupType.Homing));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Laser, 2), Is.EqualTo(AmmoPowerupType.Pierce));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Laser, 3), Is.EqualTo(AmmoPowerupType.Laser));
        }

        [Test]
        public void GetTypeAt_CyclesPurpleWeaponPool()
        {
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Plasma, 0), Is.EqualTo(AmmoPowerupType.Plasma));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Plasma, 1), Is.EqualTo(AmmoPowerupType.Wave));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Plasma, 2), Is.EqualTo(AmmoPowerupType.Guard));
            Assert.That(PowerupCycle.GetTypeAt(AmmoPowerupType.Plasma, 3), Is.EqualTo(AmmoPowerupType.Plasma));
        }

        [TestCase(AmmoPowerupType.Scatter, "S")]
        [TestCase(AmmoPowerupType.RapidFire, "R")]
        [TestCase(AmmoPowerupType.Burst, "B")]
        [TestCase(AmmoPowerupType.Laser, "L")]
        [TestCase(AmmoPowerupType.Homing, "H")]
        [TestCase(AmmoPowerupType.Pierce, "P")]
        [TestCase(AmmoPowerupType.Plasma, "O")]
        [TestCase(AmmoPowerupType.Wave, "W")]
        [TestCase(AmmoPowerupType.Guard, "G")]
        public void GetLabel_ReturnsArcadeLetter(AmmoPowerupType type, string expectedLabel)
        {
            Assert.That(PowerupCycle.GetLabel(type), Is.EqualTo(expectedLabel));
        }
    }
}
