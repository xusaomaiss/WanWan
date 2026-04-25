using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class BossPhaseConfigTests
    {
        [TestCase(GameDifficulty.Low)]
        [TestCase(GameDifficulty.Medium)]
        [TestCase(GameDifficulty.High)]
        public void BuildBossPhases_ReturnsFourEscalatingPhases(GameDifficulty difficulty)
        {
            BossPhaseConfig[] phases = BlockSpawner.BuildBossPhases(difficulty);

            Assert.That(phases, Has.Length.EqualTo(4));
            Assert.That(phases[0].TriggerHealthNormalized, Is.EqualTo(0.7f));
            Assert.That(phases[1].TriggerHealthNormalized, Is.EqualTo(0.4f));
            Assert.That(phases[2].TriggerHealthNormalized, Is.EqualTo(0.1f));
            Assert.That(phases[3].TriggerHealthNormalized, Is.EqualTo(0f));
            Assert.That(phases[0].FireInterval, Is.GreaterThan(phases[3].FireInterval));
            Assert.That(phases[2].ExtraRingShot, Is.True);
            Assert.That(phases[3].ExtraRingShot, Is.True);
        }

        [Test]
        public void BuildBossPhases_AppliesStagePatternModifiers()
        {
            BossPhaseConfig[] standard = BlockSpawner.BuildBossPhases(GameDifficulty.Medium, BossPatternStyle.Standard);
            BossPhaseConfig[] orbit = BlockSpawner.BuildBossPhases(GameDifficulty.Medium, BossPatternStyle.Orbit);
            BossPhaseConfig[] needle = BlockSpawner.BuildBossPhases(GameDifficulty.Medium, BossPatternStyle.Needle);

            Assert.That(orbit[0].SalvoCount, Is.GreaterThan(standard[0].SalvoCount));
            Assert.That(orbit[0].SpreadAngle, Is.GreaterThan(standard[0].SpreadAngle));
            Assert.That(orbit[0].ExtraRingShot, Is.True);
            Assert.That(needle[0].SpreadAngle, Is.LessThan(standard[0].SpreadAngle));
            Assert.That(needle[0].AimedCoreShot, Is.True);
        }
    }
}
