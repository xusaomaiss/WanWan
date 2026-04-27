using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;
using Wanwan.Runtime.Pools;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class PoolCollectionTests
    {
        [Test]
        public void Construction_CreatesAllPools()
        {
            var pools = new PoolCollection(VisualEffectsQuality.Full);
            Assert.IsNotNull(pools.Bullets);
            Assert.IsNotNull(pools.Enemies);
            Assert.IsNotNull(pools.Particles);
            Assert.IsNotNull(pools.Pickups);
            Assert.IsNotNull(pools.Fireballs);
            Assert.IsNotNull(pools.Labels);
        }

        [Test]
        public void RentAndReturn_RoundTripsCorrectly()
        {
            var pools = new PoolCollection(VisualEffectsQuality.Full);
            var bullet = pools.RentBullet();
            pools.ReturnBullet(bullet);
            var reused = pools.RentBullet();
            Assert.AreSame(bullet, reused);
        }

        [Test]
        public void AllPoolTypes_CanRentAndReturn()
        {
            var pools = new PoolCollection(VisualEffectsQuality.Full);

            var bullet = pools.RentBullet();
            pools.ReturnBullet(bullet);

            var enemy = pools.RentEnemy();
            pools.ReturnEnemy(enemy);

            var particle = pools.RentParticle();
            pools.ReturnParticle(particle);

            var pickup = pools.RentPickup();
            pools.ReturnPickup(pickup);

            var fireball = pools.RentFireball();
            pools.ReturnFireball(fireball);

            var label = pools.RentLabel();
            pools.ReturnLabel(label);

            Assert.Pass();
        }
    }
}
