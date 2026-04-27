using NUnit.Framework;
using Wanwan.Runtime.Achievement;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class AchievementStateTests
    {
        [Test]
        public void AchievementType_AllValuesDefined()
        {
            var values = System.Enum.GetValues(typeof(AchievementType));
            Assert.GreaterOrEqual(values.Length, 10);
            Assert.Contains(AchievementType.FirstClear, values);
            Assert.Contains(AchievementType.MaxCombo100, values);
            Assert.Contains(AchievementType.NoBombClear, values);
        }

        [Test]
        public void GetDisplayName_ReturnsDistinctLabels()
        {
            Assert.AreEqual("初次通关", AchievementType.FirstClear.GetDisplayName());
            Assert.AreEqual("无伤通关", AchievementType.PerfectStage.GetDisplayName());
            Assert.AreEqual("百连击", AchievementType.MaxCombo100.GetDisplayName());
        }

        [Test]
        public void AchievementState_Default_IsUnlockedFalse()
        {
            var state = new AchievementState();
            Assert.IsFalse(state.IsUnlocked(AchievementType.FirstClear));
        }

        [Test]
        public void AchievementState_Unlock_ThenIsUnlocked()
        {
            var state = new AchievementState();
            state.Unlock(AchievementType.FirstClear);
            Assert.IsTrue(state.IsUnlocked(AchievementType.FirstClear));
        }

        [Test]
        public void AchievementState_DoubleUnlock_Idempotent()
        {
            var state = new AchievementState();
            state.Unlock(AchievementType.FirstClear);
            state.Unlock(AchievementType.FirstClear);
            Assert.IsTrue(state.IsUnlocked(AchievementType.FirstClear));
        }

        [Test]
        public void AchievementState_AddProgress_UnlocksAtThreshold()
        {
            var state = new AchievementState();
            state.AddProgress(AchievementType.MaxCombo50, 25);
            Assert.IsFalse(state.IsUnlocked(AchievementType.MaxCombo50));
            state.AddProgress(AchievementType.MaxCombo50, 25);
            Assert.IsTrue(state.IsUnlocked(AchievementType.MaxCombo50));
        }

        [Test]
        public void AchievementState_AlreadyUnlocked_DoesNotDoubleCount()
        {
            var state = new AchievementState();
            state.Unlock(AchievementType.FirstClear);
            // Adding progress after unlock should be safe
            state.AddProgress(AchievementType.FirstClear, 1);
            Assert.IsTrue(state.IsUnlocked(AchievementType.FirstClear));
        }
    }
}
