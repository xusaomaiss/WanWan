using NUnit.Framework;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class GrazeStateTests
    {
        [Test]
        public void InitialState_IsZero()
        {
            var state = new Wanwan.Runtime.GrazeState();
            Assert.AreEqual(0, state.CurrentGrazeStreak);
            Assert.AreEqual(0, state.TotalGrazeCount);
            Assert.AreEqual(0f, state.FocusMeter);
            Assert.IsFalse(state.IsFocusActive);
        }

        [Test]
        public void RegisterGraze_IncrementsStreakAndCount()
        {
            var state = new Wanwan.Runtime.GrazeState();
            state.RegisterGraze();
            Assert.AreEqual(1, state.CurrentGrazeStreak);
            Assert.AreEqual(1, state.TotalGrazeCount);
            state.RegisterGraze();
            Assert.AreEqual(2, state.CurrentGrazeStreak);
            Assert.AreEqual(2, state.TotalGrazeCount);
        }

        [Test]
        public void FocusMeter_IncreasesWithEachGraze()
        {
            var state = new Wanwan.Runtime.GrazeState();
            for (int i = 0; i < 10; i++)
            {
                state.RegisterGraze();
            }
            Assert.Greater(state.FocusMeter, 0f);
        }

        [Test]
        public void Focus_Activates_WhenThresholdReached()
        {
            var state = new Wanwan.Runtime.GrazeState();
            for (int i = 0; i < 10; i++)
            {
                state.RegisterGraze();
            }
            Assert.IsTrue(state.IsFocusActive);
        }

        [Test]
        public void FocusMeter_Decays_OverTime()
        {
            var state = new Wanwan.Runtime.GrazeState();
            for (int i = 0; i < 10; i++)
            {
                state.RegisterGraze();
            }
            float beforeDecay = state.FocusMeter;
            state.Tick(1f);
            Assert.Less(state.FocusMeter, beforeDecay);
        }

        [Test]
        public void BreakStreak_ResetsCurrentStreak()
        {
            var state = new Wanwan.Runtime.GrazeState();
            state.RegisterGraze();
            state.RegisterGraze();
            Assert.AreEqual(2, state.CurrentGrazeStreak);
            state.BreakStreak();
            Assert.AreEqual(0, state.CurrentGrazeStreak);
        }

        [Test]
        public void TotalGrazeCount_PersistsAfterBreakStreak()
        {
            var state = new Wanwan.Runtime.GrazeState();
            state.RegisterGraze();
            state.RegisterGraze();
            state.BreakStreak();
            Assert.AreEqual(2, state.TotalGrazeCount);
        }

        [Test]
        public void GrazeDetector_Exists()
        {
            var type = typeof(Wanwan.Runtime.GrazeDetector);
            Assert.IsNotNull(type);
        }
    }
}
