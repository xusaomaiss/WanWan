using NUnit.Framework;
using Wanwan.Runtime;
using Wanwan.Runtime.Pools;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class PoolConfigTests
    {
        [Test]
        public void FullQuality_AlwaysGivesLargerOrEqualSizes()
        {
            Assert.GreaterOrEqual(PoolConfig.GetBulletPoolSize(VisualEffectsQuality.Full),
                PoolConfig.GetBulletPoolSize(VisualEffectsQuality.BatterySaver));
            Assert.GreaterOrEqual(PoolConfig.GetEnemyPoolSize(VisualEffectsQuality.Full),
                PoolConfig.GetEnemyPoolSize(VisualEffectsQuality.BatterySaver));
            Assert.GreaterOrEqual(PoolConfig.GetParticlePoolSize(VisualEffectsQuality.Full),
                PoolConfig.GetParticlePoolSize(VisualEffectsQuality.BatterySaver));
        }

        [TestCase(VisualEffectsQuality.Full, 128)]
        [TestCase(VisualEffectsQuality.BatterySaver, 64)]
        public void BulletPoolSize_MatchesExpected(VisualEffectsQuality quality, int expected)
        {
            Assert.AreEqual(expected, PoolConfig.GetBulletPoolSize(quality));
        }

        [TestCase(VisualEffectsQuality.Full, 48)]
        [TestCase(VisualEffectsQuality.BatterySaver, 24)]
        public void EnemyPoolSize_MatchesExpected(VisualEffectsQuality quality, int expected)
        {
            Assert.AreEqual(expected, PoolConfig.GetEnemyPoolSize(quality));
        }

        [TestCase(VisualEffectsQuality.Full, 96)]
        [TestCase(VisualEffectsQuality.BatterySaver, 32)]
        public void ParticlePoolSize_MatchesExpected(VisualEffectsQuality quality, int expected)
        {
            Assert.AreEqual(expected, PoolConfig.GetParticlePoolSize(quality));
        }
    }
}
