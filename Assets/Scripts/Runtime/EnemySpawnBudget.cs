using UnityEngine;

namespace Wanwan.Runtime
{
    public static class EnemySpawnBudget
    {
        private const float RewardEnemyMultiplier = 1.6f;

        public static int GetMinimumEnemiesForFullBombs(GameplayRewardConfig config)
        {
            if (config.CoinsPerEnemy <= 0)
            {
                return 0;
            }

            return Mathf.CeilToInt((config.CoinsPerBomb * config.MaxBombsPerStage) / (float)config.CoinsPerEnemy);
        }

        public static int GetAdjustedTotalCount(int originalTotal, GameplayRewardConfig config)
        {
            return Mathf.Max(originalTotal, Mathf.CeilToInt(originalTotal * RewardEnemyMultiplier), GetMinimumEnemiesForFullBombs(config));
        }

        public static int GetAdjustedCount(int originalCount)
        {
            return Mathf.Max(originalCount, Mathf.CeilToInt(originalCount * RewardEnemyMultiplier));
        }
    }
}
