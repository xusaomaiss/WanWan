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
    }
}
