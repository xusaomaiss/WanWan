using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class RaidenEnemySpriteTests
    {
        [Test]
        public void EnemyInterceptorSprite_UsesRaidenEnemyJetResource()
        {
            Texture2D resource = Resources.Load<Texture2D>(RuntimeSpriteFactory.RaidenEnemyJetResourcePath);

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.width, Is.EqualTo(128));
            Assert.That(resource.height, Is.EqualTo(128));
            Assert.That(RuntimeSpriteFactory.GetEnemyInterceptorSprite().texture, Is.SameAs(resource));
        }

        [Test]
        public void AdvancedEnemySprites_UseAiGeneratedResources()
        {
            Texture2D toughResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.ToughEnemyResourcePath);
            Texture2D eliteResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.EliteEnemyResourcePath);
            Texture2D bossResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.BossFlagshipResourcePath);

            Assert.That(toughResource, Is.Not.Null);
            Assert.That(eliteResource, Is.Not.Null);
            Assert.That(bossResource, Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetToughInterceptorSprite().texture, Is.SameAs(toughResource));
            Assert.That(RuntimeSpriteFactory.GetEliteInterceptorSprite().texture, Is.SameAs(eliteResource));
            Assert.That(RuntimeSpriteFactory.GetBossFlagshipSprite().texture, Is.SameAs(bossResource));
        }

        [Test]
        public void CoinSprite_UsesAiGeneratedPickupResource()
        {
            Texture2D resource = Resources.Load<Texture2D>(RuntimeSpriteFactory.CoinResourcePath);

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.width, Is.EqualTo(128));
            Assert.That(resource.height, Is.EqualTo(128));
            Assert.That(RuntimeSpriteFactory.GetCoinSprite().texture, Is.SameAs(resource));
        }

        [Test]
        public void CinematicSprites_UseAiGeneratedResources()
        {
            Texture2D introResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.LaunchWeatherIntroResourcePath);
            Texture2D menuResource = Resources.Load<Texture2D>(RuntimeSpriteFactory.MenuStormTitleResourcePath);

            Assert.That(introResource, Is.Not.Null);
            Assert.That(menuResource, Is.Not.Null);
            Assert.That(introResource.height, Is.GreaterThan(introResource.width));
            Assert.That(menuResource.height, Is.GreaterThan(menuResource.width));
            Assert.That(RuntimeSpriteFactory.GetLaunchWeatherIntroSprite().texture, Is.SameAs(introResource));
            Assert.That(RuntimeSpriteFactory.GetMenuStormTitleSprite().texture, Is.SameAs(menuResource));
        }

        [Test]
        public void LaunchTakeoffFrames_UseAiGeneratedSequenceResources()
        {
            Assert.That(RuntimeSpriteFactory.LaunchTakeoffFrameCount, Is.GreaterThanOrEqualTo(6));
            Assert.That(RuntimeSpriteFactory.GetLaunchTakeoffFrameResourcePath(0), Is.EqualTo("RaidenArt/Cinematics/Takeoff/launch_takeoff_frame_00"));

            Texture2D firstFrame = Resources.Load<Texture2D>(RuntimeSpriteFactory.GetLaunchTakeoffFrameResourcePath(0));

            Assert.That(firstFrame, Is.Not.Null);
            Assert.That(firstFrame.height, Is.GreaterThanOrEqualTo(firstFrame.width));
            Assert.That(RuntimeSpriteFactory.GetLaunchTakeoffFrameSprites()[0].texture, Is.SameAs(firstFrame));
        }

        [Test]
        public void ArcadeEffectSprites_UseRaidenEffectResources()
        {
            Texture2D spread = Resources.Load<Texture2D>(RuntimeSpriteFactory.BulletSpreadArcadeResourcePath);
            Texture2D laser = Resources.Load<Texture2D>(RuntimeSpriteFactory.BulletLaserArcadeResourcePath);
            Texture2D homing = Resources.Load<Texture2D>(RuntimeSpriteFactory.BulletHomingArcadeResourcePath);
            Texture2D burst = Resources.Load<Texture2D>(RuntimeSpriteFactory.BulletBurstArcadeResourcePath);
            Texture2D explosion = Resources.Load<Texture2D>(RuntimeSpriteFactory.ExplosionArcadeResourcePath);

            Assert.That(spread, Is.Not.Null);
            Assert.That(laser, Is.Not.Null);
            Assert.That(homing, Is.Not.Null);
            Assert.That(burst, Is.Not.Null);
            Assert.That(explosion, Is.Not.Null);
            Assert.That(laser.height, Is.GreaterThan(laser.width));
            Assert.That(explosion.width, Is.EqualTo(explosion.height));
            Assert.That(RuntimeSpriteFactory.GetBulletSprite(AmmoPowerupType.Normal).texture, Is.SameAs(spread));
            Assert.That(RuntimeSpriteFactory.GetBulletSprite(AmmoPowerupType.Laser).texture, Is.SameAs(laser));
            Assert.That(RuntimeSpriteFactory.GetBulletSprite(AmmoPowerupType.Homing).texture, Is.SameAs(homing));
            Assert.That(RuntimeSpriteFactory.GetBulletSprite(AmmoPowerupType.Burst).texture, Is.SameAs(burst));
            Assert.That(RuntimeSpriteFactory.GetExplosionSprite().texture, Is.SameAs(explosion));
        }
    }
}
