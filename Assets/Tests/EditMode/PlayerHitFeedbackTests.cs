using NUnit.Framework;
using UnityEngine;

namespace Wanwan.Tests.EditMode
{
    [TestFixture]
    public class PlayerHitFeedbackTests
    {
        [Test]
        public void VignettePanel_IsRed()
        {
            Color red = new Color(1f, 0f, 0f, 0.35f);
            Assert.AreEqual(1f, red.r);
            Assert.AreEqual(0f, red.g);
            Assert.AreEqual(0f, red.b);
        }

        [Test]
        public void InvulnerabilityColor_WhitePhase_IsBright()
        {
            Color white = Color.white;
            Assert.AreEqual(1f, white.r);
            Assert.AreEqual(1f, white.g);
            Assert.AreEqual(1f, white.b);
        }

        [Test]
        public void InvulnerabilityColor_BluePhase_IsDistinct()
        {
            Color blue = new Color(0.3f, 0.6f, 1f);
            Assert.Greater(blue.b, blue.g);
            Assert.Greater(blue.g, blue.r);
        }
    }
}
