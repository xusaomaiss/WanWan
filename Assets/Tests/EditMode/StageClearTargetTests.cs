using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class StageClearTargetTests
    {
        [Test]
        public void GetRequiredKills_ReturnsDifficultyTargets()
        {
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low), Is.EqualTo(20));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Medium), Is.EqualTo(30));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.High), Is.EqualTo(40));
        }

        [Test]
        public void GetRequiredKills_IncreasesByStageAndLoop()
        {
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 1, 0), Is.EqualTo(22));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 7, 0), Is.EqualTo(34));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 0, 1), Is.EqualTo(28));
        }
    }
}
