using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class FireLevelStateTests
    {
        [Test]
        public void Constructor_StartsAtLevelOne()
        {
            FireLevelState state = new FireLevelState();

            Assert.That(state.Level, Is.EqualTo(1));
        }

        [Test]
        public void Increase_ClampsAtLevelFour()
        {
            FireLevelState state = new FireLevelState();

            state.Increase();
            state.Increase();
            state.Increase();
            state.Increase();

            Assert.That(state.Level, Is.EqualTo(4));
        }

        [TestCase(1, 2)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        public void GetShotCount_ReturnsExpectedPatternSize(int level, int expectedShotCount)
        {
            Assert.That(WeaponShotPattern.GetShots(WeaponType.Spread, level).Length, Is.EqualTo(expectedShotCount));
        }
    }
}
