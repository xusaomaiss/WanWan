using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class PlayerHealthStateTests
    {
        [Test]
        public void Constructor_StartsWithTenHealth()
        {
            PlayerHealthState state = new PlayerHealthState();

            Assert.That(state.MaxHealth, Is.EqualTo(10));
            Assert.That(state.CurrentHealth, Is.EqualTo(10));
            Assert.That(state.IsDepleted, Is.False);
        }

        [Test]
        public void ApplyDamage_SubtractsOnePerHit()
        {
            PlayerHealthState state = new PlayerHealthState();

            int damageApplied = state.ApplyDamage(1);

            Assert.That(damageApplied, Is.EqualTo(1));
            Assert.That(state.CurrentHealth, Is.EqualTo(9));
        }

        [Test]
        public void ApplyDamage_ClampsAtZeroAndReportsDepleted()
        {
            PlayerHealthState state = new PlayerHealthState();

            int damageApplied = state.ApplyDamage(12);

            Assert.That(damageApplied, Is.EqualTo(10));
            Assert.That(state.CurrentHealth, Is.EqualTo(0));
            Assert.That(state.IsDepleted, Is.True);
        }

        [Test]
        public void Heal_RestoresHealthAndClampsAtMax()
        {
            PlayerHealthState state = new PlayerHealthState();
            state.ApplyDamage(3);

            int healed = state.Heal(2);
            int overHeal = state.Heal(8);

            Assert.That(healed, Is.EqualTo(2));
            Assert.That(overHeal, Is.EqualTo(1));
            Assert.That(state.CurrentHealth, Is.EqualTo(state.MaxHealth));
        }

        [Test]
        public void Heal_IgnoresNonPositiveAmounts()
        {
            PlayerHealthState state = new PlayerHealthState();
            state.ApplyDamage(2);

            int healed = state.Heal(0);

            Assert.That(healed, Is.EqualTo(0));
            Assert.That(state.CurrentHealth, Is.EqualTo(8));
        }

        [Test]
        public void HealthPickupController_UsesTwoPointRecovery()
        {
            Assert.That(HealthPickupController.HealAmount, Is.EqualTo(2));
        }

        [Test]
        public void DamageFeedback_VibratesOnlyWhenEnabledAndDamageWasApplied()
        {
            Assert.That(PlayerDamageFeedback.ShouldVibrate(true, 1), Is.True);
            Assert.That(PlayerDamageFeedback.ShouldVibrate(true, 0), Is.False);
            Assert.That(PlayerDamageFeedback.ShouldVibrate(false, 1), Is.False);
        }

        [Test]
        public void PlayerHealthHudPresentation_ConvertsHealthIntoClampedFill()
        {
            Assert.That(PlayerHealthHudPresentation.GetFillAmount(3, 10), Is.EqualTo(0.3f).Within(0.001f));
            Assert.That(PlayerHealthHudPresentation.GetFillAmount(12, 10), Is.EqualTo(1f).Within(0.001f));
            Assert.That(PlayerHealthHudPresentation.GetFillAmount(-2, 10), Is.EqualTo(0f).Within(0.001f));
            Assert.That(PlayerHealthHudPresentation.GetFillAmount(4, 0), Is.EqualTo(0f).Within(0.001f));
        }
    }
}
