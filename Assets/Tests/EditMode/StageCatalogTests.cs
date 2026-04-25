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
            Assert.That(StageCatalog.GetStage(0).Name, Is.EqualTo("乡村"));
            Assert.That(StageCatalog.GetStage(1).Name, Is.EqualTo("城市"));
            Assert.That(StageCatalog.GetStage(7).Name, Is.EqualTo("外星基地"));
            Assert.That(StageCatalog.GetStage(0).VictorySummary, Does.Contain("乡村低空"));
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
        public void SecondStage_UsesReleaseTunedDifficultyRamp()
        {
            Assert.That(StageCatalog.GetStage(1).DifficultyMultiplier, Is.LessThanOrEqualTo(1.03f));
            Assert.That(StageCatalog.GetStage(1).DifficultyMultiplier, Is.GreaterThan(StageCatalog.GetStage(0).DifficultyMultiplier));
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

        [Test]
        public void BuildPreviewSummary_DescribesStageBossAndCombat()
        {
            string preview = StageCatalog.BuildPreviewSummary(0);

            Assert.That(preview, Does.Contain("第1关"));
            Assert.That(preview, Does.Contain("乡村"));
            Assert.That(preview, Does.Contain("乡村防卫旗舰"));
            Assert.That(preview, Does.Contain("均衡"));
        }

        [Test]
        public void RaidenStageBackgroundPaths_CoverAllEightStages()
        {
            string[] expected =
            {
                "RaidenArt/Backgrounds/stage_01_countryside",
                "RaidenArt/Backgrounds/stage_02_city",
                "RaidenArt/Backgrounds/stage_03_coastline",
                "RaidenArt/Backgrounds/stage_04_ruins",
                "RaidenArt/Backgrounds/stage_05_wasteland",
                "RaidenArt/Backgrounds/stage_06_floating_continent",
                "RaidenArt/Backgrounds/stage_07_space_station",
                "RaidenArt/Backgrounds/stage_08_alien_base"
            };

            for (int stageNumber = 1; stageNumber <= StageCatalog.StageCount; stageNumber++)
            {
                Assert.That(RuntimeSpriteFactory.GetRaidenStageBackgroundResourcePath(stageNumber), Is.EqualTo(expected[stageNumber - 1]));
            }

            Assert.That(RuntimeSpriteFactory.GetRaidenStageBackgroundResourcePath(0), Is.EqualTo(expected[0]));
            Assert.That(RuntimeSpriteFactory.GetRaidenStageBackgroundResourcePath(99), Is.EqualTo(expected[7]));
            Assert.That(RuntimeSpriteFactory.RaidenFighterJetResourcePath, Is.EqualTo("RaidenArt/Ships/fighter_jet_64"));
        }
    }
}
