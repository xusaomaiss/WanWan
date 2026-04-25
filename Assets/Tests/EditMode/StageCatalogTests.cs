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
    }
}
