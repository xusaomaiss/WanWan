using UnityEngine;

namespace Wanwan.Runtime
{
    public readonly struct GameplayRewardConfig
    {
        public static readonly GameplayRewardConfig Default = new GameplayRewardConfig(
            4,
            5,
            4,
            100,
            3,
            2.5f,
            0.5f);

        public GameplayRewardConfig(int coinsPerEnemy, int scorePerCoin, int coinsLostPerEscapedEnemy, int coinsPerBomb, int maxBombsPerStage, float coinMagnetRadiusWorld, float coinCollectRadiusWorld)
        {
            CoinsPerEnemy = Mathf.Max(0, coinsPerEnemy);
            ScorePerCoin = Mathf.Max(0, scorePerCoin);
            CoinsLostPerEscapedEnemy = Mathf.Max(0, coinsLostPerEscapedEnemy);
            CoinsPerBomb = Mathf.Max(1, coinsPerBomb);
            MaxBombsPerStage = Mathf.Max(0, maxBombsPerStage);
            CoinMagnetRadiusWorld = Mathf.Max(0f, coinMagnetRadiusWorld);
            CoinCollectRadiusWorld = Mathf.Max(0f, coinCollectRadiusWorld);
        }

        public int CoinsPerEnemy { get; }
        public int ScorePerCoin { get; }
        public int CoinsLostPerEscapedEnemy { get; }
        public int CoinsPerBomb { get; }
        public int MaxBombsPerStage { get; }
        public float CoinMagnetRadiusWorld { get; }
        public float CoinCollectRadiusWorld { get; }
    }
}
