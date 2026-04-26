using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class RaidenVisualUpgradeTests
    {
        [Test]
        public void GroundDetailResources_CoverEveryStageWithThreeTiles()
        {
            Assert.That(RuntimeSpriteFactory.GroundDetailTileCountPerStage, Is.EqualTo(3));

            for (int stage = 1; stage <= 8; stage++)
            {
                string[] paths = RuntimeSpriteFactory.GetGroundDetailResourcePaths(stage);

                Assert.That(paths.Length, Is.EqualTo(3));
                foreach (string path in paths)
                {
                    Texture2D texture = Resources.Load<Texture2D>(path);
                    Assert.That(texture, Is.Not.Null, path);
                    Assert.That(texture.width, Is.EqualTo(128), path);
                    Assert.That(texture.height, Is.EqualTo(128), path);
                }
            }
        }

        [Test]
        public void ArcadeExplosionFrames_UseGeneratedSequenceResources()
        {
            Assert.That(RuntimeSpriteFactory.ArcadeExplosionFrameCount, Is.GreaterThanOrEqualTo(12));

            for (int frame = 0; frame < RuntimeSpriteFactory.ArcadeExplosionFrameCount; frame++)
            {
                string path = RuntimeSpriteFactory.GetArcadeExplosionFrameResourcePath(frame);
                Texture2D texture = Resources.Load<Texture2D>(path);

                Assert.That(texture, Is.Not.Null, path);
                Assert.That(texture.width, Is.EqualTo(texture.height), path);
            }

            Assert.That(RuntimeSpriteFactory.GetArcadeExplosionFrameSprites().Length, Is.EqualTo(RuntimeSpriteFactory.ArcadeExplosionFrameCount));
        }

        [Test]
        public void HudDecorResources_AreAvailableForRuntimeHud()
        {
            Assert.That(RuntimeSpriteFactory.HudDecorResourcePaths.Length, Is.GreaterThanOrEqualTo(6));

            foreach (string path in RuntimeSpriteFactory.HudDecorResourcePaths)
            {
                Texture2D texture = Resources.Load<Texture2D>(path);

                Assert.That(texture, Is.Not.Null, path);
                Assert.That(texture.width, Is.GreaterThanOrEqualTo(64), path);
                Assert.That(texture.height, Is.GreaterThanOrEqualTo(64), path);
            }
        }

        [Test]
        public void VisualEffectsBudget_ReducesWorkInBatterySaverMode()
        {
            Assert.That(VisualEffectsBudget.GetGroundDetailTileLimit(VisualEffectsQuality.Full), Is.GreaterThan(VisualEffectsBudget.GetGroundDetailTileLimit(VisualEffectsQuality.BatterySaver)));
            Assert.That(VisualEffectsBudget.GetExplosionCascadeCount(VisualEffectsQuality.Full), Is.GreaterThan(VisualEffectsBudget.GetExplosionCascadeCount(VisualEffectsQuality.BatterySaver)));
            Assert.That(VisualEffectsBudget.GetParticleCount(VisualEffectsQuality.Full, 44), Is.GreaterThan(VisualEffectsBudget.GetParticleCount(VisualEffectsQuality.BatterySaver, 44)));
            Assert.That(VisualEffectsBudget.GetShakeMagnitude(VisualEffectsQuality.BatterySaver, 0.2f), Is.LessThan(0.2f));
        }

        [Test]
        public void HudLayoutConstants_KeepCombatInformationReadable()
        {
            Assert.That(UIController.TopHudAnchorHeight, Is.GreaterThanOrEqualTo(0.1f));
            Assert.That(UIController.BottomHudAnchorHeight, Is.GreaterThanOrEqualTo(0.1f));
            Assert.That(UIController.PowerMeterSlotCount, Is.EqualTo(6));
        }
    }
}
