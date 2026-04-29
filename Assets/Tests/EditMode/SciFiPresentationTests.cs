using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class SciFiPresentationTests
    {
        [Test]
        public void CreateSciFiWideButton_UsesUnifiedButtonSpriteAndNonBlockingText()
        {
            GameObject parent = new GameObject("SciFiButtonParent");
            try
            {
                Button button = UiFactory.CreateSciFiWideButton(parent.transform, "RESUME", ArcadeTheme.ElectricBlue, new Vector2(320f, 72f), Vector2.zero);

                Assert.That(button.GetComponent<Image>().sprite, Is.SameAs(RuntimeSpriteFactory.GetSciFiButtonSprite(SciFiButtonSpriteKind.WideBlue)));
                Assert.That(button.targetGraphic, Is.EqualTo(button.GetComponent<Image>()));
                Assert.That(button.GetComponentInChildren<Text>().raycastTarget, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void ScreenShakeController_CanBeConfiguredWithoutStartingGameplay()
        {
            GameObject cameraObject = new GameObject("ShakeCamera");
            Camera camera = cameraObject.AddComponent<Camera>();
            try
            {
                ScreenShakeController shake = cameraObject.AddComponent<ScreenShakeController>();
                shake.Initialize(camera);

                Assert.That(shake.TargetCamera, Is.SameAs(camera));
            }
            finally
            {
                Object.DestroyImmediate(cameraObject);
            }
        }

        [Test]
        public void CoinMagnetEffect_ProvidesVisualTrailContract()
        {
            Assert.That(CoinMagnetEffect.AttractTrailObjectName, Is.EqualTo("CoinMagnetTrail"));
            Assert.That(CoinMagnetEffect.AttractingScaleMultiplier, Is.GreaterThan(1f));
        }
    }
}
