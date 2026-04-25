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
    }
}
