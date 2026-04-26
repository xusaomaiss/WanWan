using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class GameplayRewardStateTests
    {
        [Test]
        public void CollectCoin_AddsCoinAndScore()
        {
            GameplayRewardState state = new GameplayRewardState(GameplayRewardConfig.Default);

            int bombPickups = state.CollectCoins(1);

            Assert.That(state.CoinsCollected, Is.EqualTo(1));
            Assert.That(state.ScoreFromCoins, Is.EqualTo(5));
            Assert.That(bombPickups, Is.EqualTo(0));
        }

        [Test]
        public void CollectCoin_EveryHundredCoinsCreatesBombPickup()
        {
            GameplayRewardState state = new GameplayRewardState(GameplayRewardConfig.Default);

            int bombPickups = state.CollectCoins(100);

            Assert.That(state.CoinsCollected, Is.EqualTo(100));
            Assert.That(state.ScoreFromCoins, Is.EqualTo(500));
            Assert.That(state.BombPickupsEarned, Is.EqualTo(1));
            Assert.That(bombPickups, Is.EqualTo(1));
        }

        [Test]
        public void CollectCoin_ClampsBombPickupsAtStageMaximum()
        {
            GameplayRewardState state = new GameplayRewardState(GameplayRewardConfig.Default);

            int bombPickups = state.CollectCoins(420);

            Assert.That(state.BombPickupsEarned, Is.EqualTo(3));
            Assert.That(bombPickups, Is.EqualTo(3));
        }
    }
}
