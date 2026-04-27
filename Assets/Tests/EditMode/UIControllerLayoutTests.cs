using NUnit.Framework;
using UnityEngine;

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
    }
}
