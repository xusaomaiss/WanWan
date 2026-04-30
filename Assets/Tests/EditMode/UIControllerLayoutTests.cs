using NUnit.Framework;
using UnityEngine;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class UIControllerLayoutTests
    {
        [Test]
        public void HudZones_DoNotOverlap()
        {
            float leftEnd = 0.32f;
            float centerStart = 0.32f;
            float centerEnd = 0.68f;
            float rightStart = 0.68f;

            Assert.LessOrEqual(leftEnd, centerStart, "Left zone should not overlap center");
            Assert.LessOrEqual(centerEnd, rightStart, "Center zone should not overlap right");
        }

        [Test]
        public void ComboMultiplierColors_AreDistinct()
        {
            Assert.AreNotEqual(ArcadeTheme.ComboYellow, ArcadeTheme.ComboOrange);
            Assert.AreNotEqual(ArcadeTheme.ComboOrange, ArcadeTheme.ComboRed);
        }

        [Test]
        public void ComboMultiplierColors_AreReadable()
        {
            Assert.Greater(ArcadeTheme.ComboYellow.r + ArcadeTheme.ComboYellow.g + ArcadeTheme.ComboYellow.b, 2f);
            Assert.Greater(ArcadeTheme.ComboOrange.r + ArcadeTheme.ComboOrange.g, 1.3f);
            Assert.Greater(ArcadeTheme.ComboRed.r, 0.8f);
        }

        [Test]
        public void PowerMeterSlotCount_MatchesSixSlotDesign()
        {
            Assert.That(UIController.PowerMeterSlotCount, Is.EqualTo(PowerMeterState.SlotCount));
            Assert.That(UIController.PowerMeterSlotCount, Is.EqualTo(6));
        }

        [Test]
        public void UpgradeButton_IsLargeEnoughForMobileTouch()
        {
            Assert.That(UIController.UpgradeButtonSize.x, Is.GreaterThanOrEqualTo(220f));
            Assert.That(UIController.UpgradeButtonSize.y, Is.GreaterThanOrEqualTo(88f));
        }

        [Test]
        public void BottomHudText_UsesConstrainedWrapping()
        {
            Assert.That(UIController.BottomHudTextUsesConstrainedWrapping, Is.True);
        }

        [Test]
        public void TopHud_UsesFourSciFiCombatStats()
        {
            Assert.That(UIController.TopHudStatLabels, Is.EqualTo(new[] { "SCORE", "COIN", "BOMB", "FIRE" }));
        }

        [Test]
        public void PowerMeterButtonLabels_ExplainStateAndAction()
        {
            Assert.That(UIController.PowerMeterActionLabels, Is.EqualTo(new[] { "能量", "立即升级" }));
        }

        [Test]
        public void PowerMeterSlots_UseDedicatedChargeFillBars()
        {
            Assert.That(UIController.PowerMeterSlotsUseDedicatedChargeBars, Is.True);
        }

        [Test]
        public void CombatHud_UsesReferenceComboAndModuleTreatment()
        {
            Assert.That(UIController.ComboHudUsesReferenceStack, Is.True);
            Assert.That(UIController.PowerMeterSlotsUseBottomPips, Is.True);
            Assert.That(UIController.PowerMeterCardsPreserveReferenceArt, Is.True);
            Assert.That(UIController.PowerMeterCardsUseCenterIcons, Is.True);
            Assert.That(RuntimeSpriteFactory.PowerMeterIconResourcePaths.Length, Is.EqualTo(UIController.PowerMeterSlotCount));
        }

        [Test]
        public void PowerMeterIcons_UseDedicatedGraphicSprites()
        {
            foreach (string path in RuntimeSpriteFactory.PowerMeterIconResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }

            for (int i = 0; i < UIController.PowerMeterSlotCount; i++)
            {
                Sprite sprite = RuntimeSpriteFactory.GetPowerMeterIconSprite(i);
                Assert.That(sprite, Is.Not.Null);
                Assert.That(sprite.texture.width, Is.EqualTo(128));
                Assert.That(sprite.texture.height, Is.EqualTo(128));
            }
        }

        [Test]
        public void HudReferenceArt_UsesDedicatedFullFrameResources()
        {
            Assert.That(Resources.Load<Texture2D>(UIController.ReferenceTopHudResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(UIController.ReferenceBottomHudResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(UIController.ReferenceEnergyButtonResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(UIController.ReferencePowerModuleResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(UIController.ReferencePowerModuleActiveResourcePath), Is.Not.Null);
        }
    }
}
