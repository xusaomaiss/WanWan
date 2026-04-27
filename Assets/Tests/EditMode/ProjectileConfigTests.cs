using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class ProjectileConfigTests
    {
        [TestCase(WeaponType.Spread, 1, 2)]
        [TestCase(WeaponType.Spread, 2, 2)]
        [TestCase(WeaponType.Spread, 3, 3)]
        [TestCase(WeaponType.Spread, 4, 4)]
        [TestCase(WeaponType.Laser, 1, 1)]
        [TestCase(WeaponType.Laser, 2, 2)]
        [TestCase(WeaponType.Laser, 3, 3)]
        [TestCase(WeaponType.Laser, 4, 3)]
        [TestCase(WeaponType.Homing, 1, 1)]
        [TestCase(WeaponType.Homing, 2, 2)]
        [TestCase(WeaponType.Homing, 3, 3)]
        [TestCase(WeaponType.Homing, 4, 4)]
        [TestCase(WeaponType.Burst, 1, 1)]
        [TestCase(WeaponType.Burst, 2, 2)]
        [TestCase(WeaponType.Burst, 3, 3)]
        [TestCase(WeaponType.Burst, 4, 3)]
        public void GetShots_ReturnsExpectedCountForWeaponLevel(WeaponType type, int level, int expectedCount)
        {
            Assert.That(WeaponShotPattern.GetShots(type, level).Length, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetShots_RapidAndGuardModulesAddSupportFire()
        {
            PlayerShotSpec[] shots = WeaponShotPattern.GetShots(WeaponType.Spread, 2, new[] { WeaponModuleType.RapidFire, WeaponModuleType.Guard });

            Assert.That(shots.Length, Is.GreaterThan(WeaponShotPattern.GetShots(WeaponType.Spread, 2).Length));
            Assert.That(shots[0].SpeedMultiplier, Is.GreaterThan(1f));
            Assert.That(shots, Has.Some.Matches<PlayerShotSpec>(shot => shot.WeaponType == WeaponType.Spread && shot.Offset.x < -0.4f));
            Assert.That(shots, Has.Some.Matches<PlayerShotSpec>(shot => shot.WeaponType == WeaponType.Spread && shot.Offset.x > 0.4f));
        }

        [Test]
        public void GetShots_PierceHomingWaveModulesMutateShotPropertiesAndPlasmaHasBlast()
        {
            PlayerShotSpec[] pierceHomingShots = WeaponShotPattern.GetShots(WeaponType.Laser, 1, new[] { WeaponModuleType.Pierce, WeaponModuleType.Homing });
            PlayerShotSpec[] waveShots = WeaponShotPattern.GetShots(WeaponType.Burst, 2, new[] { WeaponModuleType.Wave });
            PlayerShotSpec[] plasmaShots = WeaponShotPattern.GetShots(WeaponType.Plasma, 1);

            Assert.That(pierceHomingShots, Has.All.Matches<PlayerShotSpec>(shot => shot.CanPierce));
            Assert.That(pierceHomingShots, Has.Some.Matches<PlayerShotSpec>(shot => shot.MotionType == BulletMotionType.Homing));
            Assert.That(waveShots, Has.Some.Matches<PlayerShotSpec>(shot => shot.MotionType == BulletMotionType.Wave));
            Assert.That(plasmaShots, Has.Some.Matches<PlayerShotSpec>(shot => shot.ExplosionRadius >= 1f));
        }

        [Test]
        public void GetMountShots_AddsMissileAndDefenseDroneSupportFire()
        {
            PlayerShotSpec[] missileShots = MountShotPattern.GetShots(MountType.MissilePod);
            PlayerShotSpec[] droneShots = MountShotPattern.GetShots(MountType.DefenseDrone);
            PlayerShotSpec[] shieldShots = MountShotPattern.GetShots(MountType.ShieldEmitter);

            Assert.That(missileShots, Has.Some.Matches<PlayerShotSpec>(shot => shot.WeaponType == WeaponType.Burst && shot.MotionType == BulletMotionType.Homing));
            Assert.That(droneShots.Length, Is.EqualTo(2));
            Assert.That(droneShots, Has.Some.Matches<PlayerShotSpec>(shot => shot.Offset.x < -0.45f));
            Assert.That(droneShots, Has.Some.Matches<PlayerShotSpec>(shot => shot.Offset.x > 0.45f));
            Assert.That(shieldShots, Is.Empty);
        }

        [Test]
        public void BurstExplosion_DoesNotReserveOutOfRangeTargetsForFuturePierceHits()
        {
            GameObject bulletObject = new GameObject("TestBurstBullet");
            GameObject targetObject = new GameObject("OutOfRangeTarget");
            try
            {
                BulletController bullet = bulletObject.AddComponent<BulletController>();
                bullet.Initialize(0f, 1, Vector2.up, 10f, -5f, 5f, true, 3, BulletMotionType.Straight, 0f, 0f, 0f, 0.5f);

                targetObject.transform.position = new Vector3(4f, 4f, 0f);
                BlockController target = targetObject.AddComponent<BlockController>();

                LogAssert.ignoreFailingMessages = true;
                typeof(BulletController)
                    .GetMethod("ResolveExplosion", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(bullet, null);

                HashSet<int> hitTargets = (HashSet<int>)typeof(BulletController)
                    .GetField("hitTargets", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(bullet);
                Assert.That(hitTargets.Contains(target.GetInstanceID()), Is.False);
            }
            finally
            {
                LogAssert.ignoreFailingMessages = false;
                Object.DestroyImmediate(bulletObject);
                Object.DestroyImmediate(targetObject);
            }
        }
    }
}
