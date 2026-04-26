using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class EnemySpawnBudgetTests
    {
        [Test]
        public void GetMinimumEnemiesForFullBombs_MatchesRewardEconomy()
        {
            Assert.That(EnemySpawnBudget.GetMinimumEnemiesForFullBombs(GameplayRewardConfig.Default), Is.EqualTo(75));
        }

        [Test]
        public void GetAdjustedCount_LiftsSparseStagesTowardBombBudget()
        {
            Assert.That(EnemySpawnBudget.GetAdjustedTotalCount(48, GameplayRewardConfig.Default), Is.GreaterThanOrEqualTo(75));
        }

        [Test]
        public void GetAdjustedCount_DoesNotReduceDenseStages()
        {
            Assert.That(EnemySpawnBudget.GetAdjustedCount(7), Is.GreaterThanOrEqualTo(7));
        }
    }
}
