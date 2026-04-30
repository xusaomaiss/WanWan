using NUnit.Framework;
using System.Reflection;
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

        [Test]
        public void VictoryScreen_UsesSlicedFramesInsteadOfFullCompositeBackdrop()
        {
            Assert.That(GameOverBootstrap.VictorySupplyScreenResourcePath, Is.EqualTo("RaidenArt/Cinematics/victory_supply_screen_ai"));
            Texture2D compositeBackdrop = Resources.Load<Texture2D>(GameOverBootstrap.VictorySupplyScreenResourcePath);
            Assert.That(compositeBackdrop, Is.Not.Null);
            foreach (string path in GameOverBootstrap.VictorySlicedResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }

            GameObject host = BuildVictoryGameOver();
            try
            {
                Image background = GameObject.Find("Background").GetComponent<Image>();
                Assert.That(background.sprite.texture, Is.Not.SameAs(compositeBackdrop));
            }
            finally
            {
                CleanupGameOver(host);
            }
        }

        [Test]
        public void FailureScreen_UsesDedicatedHudFramesAndShortButtons()
        {
            foreach (string path in GameOverBootstrap.FailureSlicedResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }

            GameObject host = BuildFailureGameOver();
            try
            {
                Assert.That(GameObject.Find("FailureTitleFrame")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(GameObject.Find("FailureStageFrame")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(FindTextByContent("任务失败"), Is.Not.Null);
                Assert.That(FindTextByContent("重新挑战"), Is.Not.Null);
                Assert.That(FindTextByContent("返回"), Is.Not.Null);
            }
            finally
            {
                CleanupGameOver(host);
            }
        }


        [Test]
        public void VictoryMountShop_CreatesVerticalRows()
        {
            GameObject host = BuildVictoryGameOver();
            try
            {
                RectTransform missile = GameObject.Find("导弹舱MountRow").GetComponent<RectTransform>();
                RectTransform drone = GameObject.Find("防卫机MountRow").GetComponent<RectTransform>();
                RectTransform shield = GameObject.Find("护盾发生器MountRow").GetComponent<RectTransform>();

                Assert.That(missile.anchorMin.x, Is.EqualTo(drone.anchorMin.x).Within(0.001f));
                Assert.That(drone.anchorMin.x, Is.EqualTo(shield.anchorMin.x).Within(0.001f));
                Assert.That(missile.anchorMin.y, Is.GreaterThan(drone.anchorMin.y));
                Assert.That(drone.anchorMin.y, Is.GreaterThan(shield.anchorMin.y));
                Assert.That(GameObject.Find("导弹舱BuyButton"), Is.Not.Null);
                Assert.That(GameObject.Find("防卫机BuyButton"), Is.Not.Null);
                Assert.That(GameObject.Find("护盾发生器BuyButton"), Is.Not.Null);
                Assert.That(GameObject.Find("MountShopPanel")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(GameObject.Find("VictoryTitleFrame")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(FindTextByContent("继续"), Is.Not.Null);
                Assert.That(FindTextByContent("返回"), Is.Not.Null);
            }
            finally
            {
                CleanupGameOver(host);
            }
        }

        [Test]
        public void VictoryMountShop_BuyButtonAddsMoreOfSameMount()
        {
            MountConfig missile = MountConfig.Get(MountType.MissilePod);
            GameObject host = BuildVictoryGameOver(missile.Cost * 2);
            try
            {
                Button buyButton = GameObject.Find("导弹舱BuyButton").GetComponent<Button>();

                buyButton.onClick.Invoke();
                Assert.That(SessionState.PendingMountUnits, Is.EqualTo(missile.PurchaseUnits));
                Assert.That(SessionState.SpendableScore, Is.EqualTo(missile.Cost));
                Assert.That(buyButton.GetComponentInChildren<Text>().text, Is.EqualTo("加购"));

                buyButton.onClick.Invoke();
                Assert.That(SessionState.PendingMount, Is.EqualTo(MountType.MissilePod));
                Assert.That(SessionState.PendingMountUnits, Is.EqualTo(missile.PurchaseUnits * 2));
                Assert.That(SessionState.SpendableScore, Is.EqualTo(0));
                Assert.That(buyButton.interactable, Is.False);
            }
            finally
            {
                CleanupGameOver(host);
            }
        }

        private static void AssertSingleLineText(Button button, string expected)
        {
            Text text = button.GetComponentInChildren<Text>();

            Assert.That(text.text, Is.EqualTo(expected));
            Assert.That(text.horizontalOverflow, Is.EqualTo(HorizontalWrapMode.Overflow));
            Assert.That(text.verticalOverflow, Is.EqualTo(VerticalWrapMode.Overflow));
        }

        private static Text FindTextByContent(string content)
        {
            foreach (Text text in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if (text.text == content)
                {
                    return text;
                }
            }

            return null;
        }

        private static GameObject BuildVictoryGameOver(int spendableScore = 100000)
        {
            SessionState.ResetProgress();
            SessionState.AddSpendableScore(spendableScore);
            SessionState.CommitRunScore(0, true);
            GameObject host = new GameObject("GameOverBootstrapHost");
            GameOverBootstrap bootstrap = host.AddComponent<GameOverBootstrap>();
            typeof(GameOverBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(bootstrap, null);
            return host;
        }

        private static GameObject BuildFailureGameOver()
        {
            SessionState.ResetProgress();
            SessionState.CommitRunScore(1000, false);
            GameObject host = new GameObject("GameOverBootstrapFailureHost");
            GameOverBootstrap bootstrap = host.AddComponent<GameOverBootstrap>();
            typeof(GameOverBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(bootstrap, null);
            return host;
        }

        private static void CleanupGameOver(GameObject host)
        {
            foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(canvas.gameObject);
            }

            foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(camera.gameObject);
            }

            if (host != null)
            {
                Object.DestroyImmediate(host);
            }

            SessionState.ResetProgress();
        }
    }
}
