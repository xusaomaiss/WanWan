using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class BossPhaseBehaviorTests
    {
        [Test]
        public void PhaseConfig_HasEnragedFlag()
        {
            var configType = typeof(BossPhaseConfig);
            var prop = configType.GetProperty("IsEnraged");
            Assert.IsNotNull(prop);
        }

        [Test]
        public void PhaseConfig_HasMovementSpeedMultiplier()
        {
            var configType = typeof(BossPhaseConfig);
            var prop = configType.GetProperty("MovementSpeedMultiplier");
            Assert.IsNotNull(prop);
        }

        [Test]
        public void PhaseConfig_DefaultMovementSpeed_IsOne()
        {
            var config = new BossPhaseConfig(0.7f, 1.5f, 4, 45f, false);
            Assert.AreEqual(1f, config.MovementSpeedMultiplier);
        }

        [Test]
        public void PhaseConfig_DefaultEnraged_IsFalse()
        {
            var config = new BossPhaseConfig(0.7f, 1.5f, 4, 45f, false);
            Assert.IsFalse(config.IsEnraged);
        }

        [Test]
        public void PhaseConfig_CustomValues_AreStored()
        {
            var config = new BossPhaseConfig(0.1f, 0.9f, 7, 82f, true, true, 1.5f, true);
            Assert.AreEqual(1.5f, config.MovementSpeedMultiplier);
            Assert.IsTrue(config.IsEnraged);
        }
    }
}
