using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class GameOverBootstrapTests
    {
        [Test]
        public void ResultActionButtons_DoNotWrapChineseLabels()
        {
            GameObject parent = new GameObject("ButtonParent");
            try
            {
                Button nextButton = UiFactory.CreatePixelButton(parent.transform, "继续下一关", ArcadeTheme.ElectricBlue, GameOverBootstrap.ResultActionButtonSize, Vector2.zero);
                Button menuButton = UiFactory.CreatePixelButton(parent.transform, "返回主页", ArcadeTheme.EnergyYellow, GameOverBootstrap.ResultActionButtonSize, Vector2.zero);

                AssertSingleLineText(nextButton, "继续下一关");
                AssertSingleLineText(menuButton, "返回主页");
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void ResultTitle_CanBeConfiguredAsSingleLine()
        {
            GameObject parent = new GameObject("TitleParent");
            try
            {
                Text title = UiFactory.CreateArcadeLabel(parent.transform, "任务完成", 92, TextAnchor.MiddleCenter, ArcadeTheme.ElectricBlue, FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);

                UiFactory.ConfigureSingleLine(title);

                Assert.That(title.text, Is.EqualTo("任务完成"));
                Assert.That(title.horizontalOverflow, Is.EqualTo(HorizontalWrapMode.Overflow));
                Assert.That(title.verticalOverflow, Is.EqualTo(VerticalWrapMode.Overflow));
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void VictoryActionButtons_SitBelowMountShopPanel()
        {
            const float referenceHeight = 1920f;
            const float minGap = 24f;
            float panelBottom = GameOverBootstrap.MountShopAnchorMin.y * referenceHeight;
            float buttonTop = (referenceHeight * 0.5f) + GameOverBootstrap.VictoryActionButtonY + (GameOverBootstrap.ResultActionButtonSize.y * 0.5f);

            Assert.That(buttonTop, Is.LessThan(panelBottom - minGap));
        }

        private static void AssertSingleLineText(Button button, string expected)
        {
            Text text = button.GetComponentInChildren<Text>();

            Assert.That(text.text, Is.EqualTo(expected));
            Assert.That(text.horizontalOverflow, Is.EqualTo(HorizontalWrapMode.Overflow));
            Assert.That(text.verticalOverflow, Is.EqualTo(VerticalWrapMode.Overflow));
        }
    }
}
