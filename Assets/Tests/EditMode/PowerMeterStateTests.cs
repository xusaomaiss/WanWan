using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PowerMeterStateTests
    {
        [Test]
        public void CollectCapsule_AdvancesHighlightedSlotUntilShield()
        {
            PowerMeterState state = new PowerMeterState();

            state.CollectCapsule();
            state.CollectCapsule();
            state.CollectCapsule();
            state.CollectCapsule();

            Assert.That(state.HighlightedUpgrade, Is.EqualTo(PowerMeterUpgrade.Laser));
            Assert.That(state.HighlightedIndex, Is.EqualTo(3));
        }

        [Test]
        public void ActivateHighlightedUpgrade_ReturnsUpgradeAndResetsMeter()
        {
            PowerMeterState state = new PowerMeterState();
            state.CollectCapsule();
            state.CollectCapsule();
            state.CollectCapsule();

            PowerMeterUpgrade upgrade = state.ActivateHighlightedUpgrade();

            Assert.That(upgrade, Is.EqualTo(PowerMeterUpgrade.Double));
            Assert.That(state.HighlightedUpgrade, Is.EqualTo(PowerMeterUpgrade.None));
            Assert.That(state.HighlightedIndex, Is.EqualTo(-1));
        }

        [Test]
        public void CollectCapsule_ClampsAtShield()
        {
            PowerMeterState state = new PowerMeterState();

            for (int i = 0; i < 12; i++)
            {
                state.CollectCapsule();
            }

            Assert.That(state.HighlightedUpgrade, Is.EqualTo(PowerMeterUpgrade.Shield));
            Assert.That(state.HighlightedIndex, Is.EqualTo(5));
        }

        [Test]
        public void BuildHudText_MarksHighlightedSlot()
        {
            PowerMeterState state = new PowerMeterState();
            state.CollectCapsule();
            state.CollectCapsule();

            Assert.That(state.BuildHudText(), Is.EqualTo("SPEED >MISSILE< DOUBLE LASER OPTION SHIELD"));
        }
    }
}
