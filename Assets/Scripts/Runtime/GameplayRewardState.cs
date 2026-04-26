using UnityEngine;

namespace Wanwan.Runtime
{
    public class GameplayRewardState
    {
        private readonly GameplayRewardConfig config;

        public GameplayRewardState(GameplayRewardConfig rewardConfig)
        {
            config = rewardConfig;
        }

        public int CoinsCollected { get; private set; }
        public int ScoreFromCoins { get; private set; }
        public int BombPickupsEarned { get; private set; }

        public int CollectCoins(int count)
        {
            int safeCount = Mathf.Max(0, count);
            if (safeCount == 0)
            {
                return 0;
            }

            CoinsCollected += safeCount;
            ScoreFromCoins += safeCount * config.ScorePerCoin;

            int currentBombThreshold = CoinsCollected / config.CoinsPerBomb;
            int targetBombPickups = Mathf.Min(config.MaxBombsPerStage, currentBombThreshold);
            int newBombPickups = Mathf.Max(0, targetBombPickups - BombPickupsEarned);
            BombPickupsEarned = Mathf.Max(BombPickupsEarned, targetBombPickups);
            return newBombPickups;
        }

        public int DeductCoins(int count)
        {
            int safeCount = Mathf.Max(0, count);
            int deducted = Mathf.Min(CoinsCollected, safeCount);
            CoinsCollected -= deducted;
            return deducted;
        }
    }
}
