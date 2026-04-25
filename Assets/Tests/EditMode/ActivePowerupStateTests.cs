using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class ActivePowerupStateTests
    {
        [Test]
        public void Activate_SetsTypeAndDuration()
        {
            ActivePowerupState state = new ActivePowerupState();

            state.Activate(AmmoPowerupType.Scatter, 8f);

            Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.Scatter));
            Assert.That(state.RemainingSeconds, Is.EqualTo(8f));
            Assert.That(state.HasActivePowerup, Is.True);
        }

        [Test]
        public void Tick_ClearsState_WhenDurationExpires()
        {
            ActivePowerupState state = new ActivePowerupState();
            state.Activate(AmmoPowerupType.Pierce, 2f);

            state.Tick(2.1f);

            Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.None));
            Assert.That(state.RemainingSeconds, Is.EqualTo(0f));
            Assert.That(state.HasActivePowerup, Is.False);
        }

        [Test]
        public void Activate_RefreshesExistingPowerup()
        {
            ActivePowerupState state = new ActivePowerupState();
            state.Activate(AmmoPowerupType.RapidFire, 8f);
            state.Tick(3.5f);

            state.Activate(AmmoPowerupType.Scatter, 8f);

            Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.Scatter));
            Assert.That(state.RemainingSeconds, Is.EqualTo(8f));
        }

        [Test]
        public void Activate_SameType_IncreasesLevelAndRefreshesDuration()
        {
            ActivePowerupState state = new ActivePowerupState();
            state.Activate(AmmoPowerupType.Laser, 8f);
            state.Tick(3f);

            state.Activate(AmmoPowerupType.Laser, 8f);

            Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.Laser));
            Assert.That(state.Level, Is.EqualTo(2));
            Assert.That(state.RemainingSeconds, Is.EqualTo(8f));
        }

        [Test]
        public void Activate_SameType_ClampsLevelAtThree()
        {
            ActivePowerupState state = new ActivePowerupState();

            state.Activate(AmmoPowerupType.Homing, 8f);
            state.Activate(AmmoPowerupType.Homing, 8f);
            state.Activate(AmmoPowerupType.Homing, 8f);
            state.Activate(AmmoPowerupType.Homing, 8f);

            Assert.That(state.Level, Is.EqualTo(3));
        }

        [Test]
        public void Activate_DifferentType_ResetsLevelToOne()
        {
            ActivePowerupState state = new ActivePowerupState();
            state.Activate(AmmoPowerupType.Scatter, 8f);
            state.Activate(AmmoPowerupType.Scatter, 8f);

            state.Activate(AmmoPowerupType.Plasma, 8f);

            Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.Plasma));
            Assert.That(state.Level, Is.EqualTo(1));
        }

        [Test]
        public void Tick_ClearsLevel_WhenDurationExpires()
        {
            ActivePowerupState state = new ActivePowerupState();
            state.Activate(AmmoPowerupType.Burst, 1f);
            state.Activate(AmmoPowerupType.Burst, 1f);

            state.Tick(1.1f);

            Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.None));
            Assert.That(state.Level, Is.EqualTo(0));
            Assert.That(state.HasActivePowerup, Is.False);
        }
    }
}
