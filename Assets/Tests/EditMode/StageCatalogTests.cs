using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class StageCatalogTests
    {
        [Test]
        public void GetStage_ReturnsEightRaidenStyleThemes()
        {
            Assert.That(StageCatalog.StageCount, Is.EqualTo(8));
            Assert.That(StageCatalog.GetStage(0).Name, Is.EqualTo("城市上空"));
            Assert.That(StageCatalog.GetStage(7).Name, Is.EqualTo("外星母舰"));
            Assert.That(StageCatalog.GetStage(0).VictorySummary, Does.Contain("城市空域"));
            Assert.That(StageCatalog.GetStage(7).VictorySummary, Does.Contain("Cranassian核心"));
        }

        [Test]
        public void GetNextStageIndex_WrapsAfterFinalStage()
        {
            Assert.That(StageCatalog.GetNextStageIndex(0), Is.EqualTo(1));
            Assert.That(StageCatalog.GetNextStageIndex(7), Is.EqualTo(0));
            Assert.That(StageCatalog.IsLoopAdvance(7), Is.True);
        }

        [Test]
        public void GetDifficultyMultiplier_IncreasesByStageAndLoop()
        {
            Assert.That(StageCatalog.GetDifficultyMultiplier(1, 0), Is.GreaterThan(StageCatalog.GetDifficultyMultiplier(0, 0)));
            Assert.That(StageCatalog.GetDifficultyMultiplier(0, 1), Is.GreaterThan(StageCatalog.GetDifficultyMultiplier(0, 0)));
        }

        [Test]
        public void Stages_HaveDistinctCombatAndBossStyles()
        {
            for (int i = 0; i < StageCatalog.StageCount; i++)
            {
                StageDefinition stage = StageCatalog.GetStage(i);

                Assert.That(stage.CombatStyle, Is.EqualTo((StageCombatStyle)i));
                Assert.That(stage.BossPattern, Is.EqualTo((BossPatternStyle)i));
                Assert.That(stage.BackgroundSpeedMultiplier, Is.GreaterThan(0.8f));
            }
        }
    }
}
