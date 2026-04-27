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
            Assert.That(state.CollectedCapsules, Is.EqualTo(4));
            Assert.That(state.IsFull, Is.True);
        }

        [Test]
        public void CanActivate_OnlyAfterFourCapsules()
        {
            PowerMeterState state = new PowerMeterState();

            state.CollectCapsule();
            state.CollectCapsule();
            state.CollectCapsule();

            Assert.That(state.CanActivate, Is.False);

            state.CollectCapsule();

            Assert.That(state.CanActivate, Is.True);
        }

        [Test]
        public void ActivateHighlightedUpgrade_ReturnsUpgradeAndResetsMeterWhenFull()
        {
            PowerMeterState state = new PowerMeterState();
            state.CollectCapsule();
            state.CollectCapsule();
            state.CollectCapsule();
            state.CollectCapsule();

            PowerMeterUpgrade upgrade = state.ActivateHighlightedUpgrade();

            Assert.That(upgrade, Is.EqualTo(PowerMeterUpgrade.Laser));
            Assert.That(state.HighlightedUpgrade, Is.EqualTo(PowerMeterUpgrade.None));
            Assert.That(state.HighlightedIndex, Is.EqualTo(-1));
            Assert.That(state.CollectedCapsules, Is.EqualTo(0));
        }

        [Test]
        public void CollectCapsule_ClampsAtShield()
        {
            PowerMeterState state = new PowerMeterState();

            for (int i = 0; i < 12; i++)
            {
                state.CollectCapsule();
            }

            Assert.That(state.HighlightedUpgrade, Is.EqualTo(PowerMeterUpgrade.Laser));
            Assert.That(state.HighlightedIndex, Is.EqualTo(3));
        }

        [Test]
        public void BuildHudText_MarksHighlightedSlot()
        {
            PowerMeterState state = new PowerMeterState();
            state.CollectCapsule();
            state.CollectCapsule();

            Assert.That(state.BuildHudText(), Is.EqualTo("[SPEED] [>MISSILE<] DOUBLE LASER"));
        }

        [Test]
        public void StageProfiles_IncreaseCapsuleBudgetByTwoPerStage()
        {
            for (int i = 0; i < StageCatalog.StageCount; i++)
            {
                Assert.That(StageCatalog.GetGameplayProfile(i).PowerMeterCapsuleBudget, Is.EqualTo(6 + (i * 2)));
            }
        }
    }
}
