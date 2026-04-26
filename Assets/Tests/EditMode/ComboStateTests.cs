using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class ComboStateTests
    {
        [Test]
        public void Constructor_StartsAtZeroComboAndBaseMultiplier()
        {
            ComboState state = new ComboState();

            Assert.That(state.CurrentCombo, Is.EqualTo(0));
            Assert.That(state.CurrentMultiplier, Is.EqualTo(1));
        }

        [Test]
        public void RegisterKill_IncreasesCombo()
        {
            ComboState state = new ComboState();

            state.RegisterKill();
            state.RegisterKill();

            Assert.That(state.CurrentCombo, Is.EqualTo(2));
        }

        [TestCase(10, 2)]
        [TestCase(25, 3)]
        [TestCase(50, 4)]
        [TestCase(100, 5)]
        public void GetMultiplier_ReturnsExpectedBoundaryMultiplier(int combo, int expectedMultiplier)
        {
            ComboState state = new ComboState();

            state.RegisterKill(combo);

            Assert.That(state.CurrentMultiplier, Is.EqualTo(expectedMultiplier));
        }

        [Test]
        public void BreakCombo_PreservesMaxCombo()
        {
            ComboState state = new ComboState();

            state.RegisterKill(30);
            state.BreakCombo();

            Assert.That(state.CurrentCombo, Is.EqualTo(0));
            Assert.That(state.MaxCombo, Is.EqualTo(30));
            Assert.That(state.CurrentMultiplier, Is.EqualTo(1));
        }

        [Test]
        public void BossEvents_AddComboBonuses()
        {
            ComboState state = new ComboState();

            state.RegisterBossPhaseClear();
            state.RegisterBossDefeated();

            Assert.That(state.CurrentCombo, Is.EqualTo(15));
            Assert.That(state.CurrentMultiplier, Is.EqualTo(2));
        }
    }
}
