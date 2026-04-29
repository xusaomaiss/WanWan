using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class SciFiResourcePathsTests
    {
        [Test]
        public void SciFiResourcePaths_ExposeReplaceableVisualContracts()
        {
            Assert.That(SciFiResourcePaths.BackgroundDeep, Is.EqualTo("SciFi/Backgrounds/space_deep_01"));
            Assert.That(SciFiResourcePaths.BackgroundNebula, Is.EqualTo("SciFi/Backgrounds/space_nebula_01"));
            Assert.That(SciFiResourcePaths.HudTopFrame, Is.EqualTo("SciFi/UI/Panels/hud_top_frame"));
            Assert.That(SciFiResourcePaths.ButtonWideBlue, Is.EqualTo("SciFi/UI/Buttons/btn_wide_blue"));
            Assert.That(SciFiResourcePaths.ProjectilePlayerBlue, Is.EqualTo("SciFi/Projectiles/bullet_player_blue"));
            Assert.That(SciFiResourcePaths.PickupCoin, Is.EqualTo("SciFi/Pickups/coin"));
            Assert.That(SciFiResourcePaths.EffectExplosionCore, Is.EqualTo("SciFi/Effects/explosion_core"));
            Assert.That(SciFiResourcePaths.EffectFlashRadial, Is.EqualTo("SciFi/Effects/flash_radial"));
        }

        [Test]
        public void RuntimeSpriteFactory_SciFiSpritesResolveWithFallbacks()
        {
            Assert.That(RuntimeSpriteFactory.GetSciFiBackgroundSprite(SciFiBackgroundLayerKind.Deep), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiBackgroundSprite(SciFiBackgroundLayerKind.Nebula), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiBackgroundSprite(SciFiBackgroundLayerKind.NearStars), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiHudSprite(SciFiHudSpriteKind.TopFrame), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiButtonSprite(SciFiButtonSpriteKind.WideBlue), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiProjectileSprite(AmmoPowerupType.Laser), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiCoinSprite(), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiBombSprite(), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiExplosionSprite(), Is.Not.Null);
            Assert.That(RuntimeSpriteFactory.GetSciFiFlashSprite(), Is.Not.Null);
        }
    }
}
