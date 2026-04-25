using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class DifficultyProgressionTests
    {
        [Test]
        public void SpawnInterval_Decreases_AsRunProgresses()
        {
            float openingInterval = DifficultyProgression.GetSpawnInterval(0f);
            float lateInterval = DifficultyProgression.GetSpawnInterval(90f);

            Assert.That(lateInterval, Is.LessThan(openingInterval));
            Assert.That(lateInterval, Is.GreaterThanOrEqualTo(0.35f));
        }

        [Test]
        public void BlockSpeed_Increases_AsRunProgresses()
        {
            float openingSpeed = DifficultyProgression.GetBlockSpeed(0f, false);
            float lateSpeed = DifficultyProgression.GetBlockSpeed(90f, false);

            Assert.That(lateSpeed, Is.GreaterThan(openingSpeed));
        }

        [Test]
        public void ToughBlocks_AppearMoreOften_LaterInRun()
        {
            float openingChance = DifficultyProgression.GetToughChance(0f);
            float lateChance = DifficultyProgression.GetToughChance(120f);

            Assert.That(lateChance, Is.GreaterThan(openingChance));
            Assert.That(lateChance, Is.LessThanOrEqualTo(0.55f));
        }

        [Test]
        public void ToughBlocks_GetMoreHitPoints_ThanNormalBlocks()
        {
            Assert.That(DifficultyProgression.GetHitPoints(false, 45f), Is.EqualTo(1));
            Assert.That(DifficultyProgression.GetHitPoints(true, 10f), Is.EqualTo(2));
            Assert.That(DifficultyProgression.GetHitPoints(true, 80f), Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void AmmoPacks_AppearAtRareButStableRate()
        {
            float openingChance = DifficultyProgression.GetAmmoPackChance(0f);
            float lateChance = DifficultyProgression.GetAmmoPackChance(240f);

            Assert.That(openingChance, Is.GreaterThan(0.05f));
            Assert.That(lateChance, Is.GreaterThan(openingChance));
            Assert.That(lateChance, Is.LessThanOrEqualTo(0.18f));
        }

        [Test]
        public void EnemyFireInterval_IsFaster_ForToughEnemies()
        {
            float normalInterval = DifficultyProgression.GetEnemyFireInterval(40f, false);
            float toughInterval = DifficultyProgression.GetEnemyFireInterval(40f, true);

            Assert.That(toughInterval, Is.LessThan(normalInterval));
        }

        [Test]
        public void OpeningEnemyFireballs_AreReadableBeforeDifficultyRamps()
        {
            float openingSpeed = DifficultyProgression.GetEnemyFireballSpeed(0f, false);
            float earlySpeed = DifficultyProgression.GetEnemyFireballSpeed(20f, false);
            float lateSpeed = DifficultyProgression.GetEnemyFireballSpeed(120f, false);

            Assert.That(openingSpeed, Is.LessThanOrEqualTo(3.1f));
            Assert.That(earlySpeed, Is.LessThanOrEqualTo(3.25f));
            Assert.That(lateSpeed, Is.GreaterThan(earlySpeed));
        }

        [Test]
        public void OpeningFireInterval_GivesPlayerBreathingRoom()
        {
            float openingInterval = DifficultyProgression.GetEnemyFireInterval(0f, false);
            float midInterval = DifficultyProgression.GetEnemyFireInterval(60f, false);
            float lateInterval = DifficultyProgression.GetEnemyFireInterval(140f, false);

            Assert.That(openingInterval, Is.GreaterThanOrEqualTo(3.7f));
            Assert.That(midInterval, Is.LessThan(openingInterval));
            Assert.That(lateInterval, Is.LessThan(midInterval));
            Assert.That(lateInterval, Is.GreaterThanOrEqualTo(1.72f));
        }
    }
}
