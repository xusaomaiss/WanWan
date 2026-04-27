using UnityEngine;

namespace Wanwan.Runtime.Pools
{
    public class PoolCollection
    {
        public GameObjectPool Bullets { get; }
        public GameObjectPool Enemies { get; }
        public GameObjectPool Particles { get; }
        public GameObjectPool Pickups { get; }
        public GameObjectPool Fireballs { get; }
        public GameObjectPool Labels { get; }

        public PoolCollection(VisualEffectsQuality quality)
        {
            Bullets = new GameObjectPool("Bullet", PoolConfig.GetBulletPoolSize(quality));
            Enemies = new GameObjectPool("Enemy", PoolConfig.GetEnemyPoolSize(quality));
            Particles = new GameObjectPool("Particle", PoolConfig.GetParticlePoolSize(quality));
            Pickups = new GameObjectPool("Pickup", PoolConfig.GetPickupPoolSize(quality));
            Fireballs = new GameObjectPool("Fireball", PoolConfig.GetFireballPoolSize(quality));
            Labels = new GameObjectPool("Label", PoolConfig.GetLabelPoolSize(quality));

            Bullets.PreWarm();
            Enemies.PreWarm();
            Particles.PreWarm();
            Pickups.PreWarm();
            Fireballs.PreWarm();
            Labels.PreWarm();
        }

        public GameObject RentBullet() => Bullets.Rent();
        public void ReturnBullet(GameObject obj) => Bullets.Return(obj);
        public GameObject RentEnemy() => Enemies.Rent();
        public void ReturnEnemy(GameObject obj) => Enemies.Return(obj);
        public GameObject RentParticle() => Particles.Rent();
        public void ReturnParticle(GameObject obj) => Particles.Return(obj);
        public GameObject RentPickup() => Pickups.Rent();
        public void ReturnPickup(GameObject obj) => Pickups.Return(obj);
        public GameObject RentFireball() => Fireballs.Rent();
        public void ReturnFireball(GameObject obj) => Fireballs.Return(obj);
        public GameObject RentLabel() => Labels.Rent();
        public void ReturnLabel(GameObject obj) => Labels.Return(obj);
    }
}
