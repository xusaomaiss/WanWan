using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class StageClearTargetTests
    {
        [Test]
        public void GetRequiredKills_ReturnsDifficultyTargets()
        {
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low), Is.EqualTo(75));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Medium), Is.EqualTo(82));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.High), Is.EqualTo(90));
        }

        [Test]
        public void GetRequiredKills_IncreasesByStageAndLoop()
        {
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 1, 0), Is.EqualTo(77));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 7, 0), Is.EqualTo(89));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 0, 1), Is.EqualTo(87));
        }

        [Test]
        public void GetRequiredKills_DialsBackSecondStagePressure()
        {
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Low, 1, 0), Is.EqualTo(77));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.Medium, 1, 0), Is.EqualTo(84));
            Assert.That(StageClearTarget.GetRequiredKills(GameDifficulty.High, 1, 0), Is.EqualTo(92));
        }
    }
}
