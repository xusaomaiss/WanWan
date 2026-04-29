using NUnit.Framework;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class MenuBootstrapTests
    {
        [Test]
        public void MenuStartup_SkipsLogoInterlude()
        {
            Assert.That(MenuBootstrap.ShowStartupLogo, Is.False);
        }

        [Test]
        public void TitleHighScore_UsesSaveButtonFontSize()
        {
            Assert.That(MenuBootstrap.TitleHighScoreFontSize, Is.EqualTo(30));
            Assert.That(MenuBootstrap.TitleHighScoreFontSize, Is.EqualTo(MenuBootstrap.TitleCircleButtonFontSize));
        }

        [Test]
        public void TitleMenu_UsesIndependentHeroFighterLayer()
        {
            Assert.That(MenuBootstrap.TitlePrimaryLabels, Is.EqualTo(new[] { "开始游戏", "退出" }));
            Assert.That(MenuBootstrap.StartScreenHitAreaNames, Is.EqualTo(new[] { "ArcadeStart", "ClassicExit", "Settings", "Leaderboard", "ShipSelect" }));
            Assert.That(MenuBootstrap.ModernTitleButtonSize, Is.EqualTo(new Vector2(486f, 96f)));
            Assert.That(MenuBootstrap.TitleStartScreenResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_start_screen_ai"));
            Assert.That(MenuBootstrap.TitleStartButtonSliceResourcePath, Is.EqualTo("MainMenu/Buttons/button_start_game"));
            Assert.That(MenuBootstrap.TitleExitButtonSliceResourcePath, Is.EqualTo("MainMenu/Buttons/button_exit"));
            Assert.That(MenuBootstrap.TitleStarfieldResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_space_far"));
            Assert.That(MenuBootstrap.TitleSparkleOverlayResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_space_front_stars"));
            Assert.That(MenuBootstrap.TitleLogoBackplateResourcePath, Is.EqualTo("MainMenu/Titles/title_raiden"));
            Assert.That(RuntimeSpriteFactory.RaidenFighterJetResourcePath, Is.EqualTo("RaidenArt/Ships/fighter_jet_128"));
            Assert.That(RuntimeSpriteFactory.MenuStormTitleResourcePath, Is.EqualTo("RaidenArt/Cinematics/menu_storm_title_ai"));
        }

        [Test]
        public void ShipSelectRoster_UsesFiveFighterList()
        {
            Assert.That(MenuBootstrap.ShipSelectRoster, Is.EqualTo(new[]
            {
                PlayerShipType.Green,
                PlayerShipType.Blue,
                PlayerShipType.Yellow,
                PlayerShipType.Purple,
                PlayerShipType.Azure
            }));
        }

        [Test]
        public void TitleMenu_ResourcesExistForSpaceShooterSkin()
        {
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleStarfieldResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleStartScreenResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleSparkleOverlayResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleLogoBackplateResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleMidNebulaResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleWideButtonResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleStartButtonSliceResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleExitButtonSliceResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.TitleIconFrameResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconControlResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconSettingsLargeResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconTrophyResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconLeaderboardLargeResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconShopResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconShipSelectLargeResourcePath), Is.Not.Null);
            Assert.That(Resources.Load<Texture2D>(MenuBootstrap.IconHelpResourcePath), Is.Not.Null);
        }

        [Test]
        public void TitleMenu_PrefabAssetsExistForReusableUiParts()
        {
            string[] prefabNames =
            {
                "MainMenuRoot",
                "TitleSpaceShooter",
                "TitleRaiden",
                "ButtonMain",
                "ButtonSquare",
                "IconInfo",
                "IconSettings",
                "IconControl",
                "IconTrophy",
                "IconShop",
                "IconHelp"
            };

            foreach (string prefabName in prefabNames)
            {
                Assert.That(System.IO.File.Exists($"Assets/Prefabs/UI/MainMenu/{prefabName}.prefab"), Is.True, prefabName);
            }
        }

        [Test]
        public void MainMenuScripts_ExposeCommercialMenuBehaviors()
        {
            Assert.That(typeof(StarfieldParallax), Is.Not.Null);
            Assert.That(typeof(MenuButtonAnimator), Is.Not.Null);
            Assert.That(typeof(MainMenuController), Is.Not.Null);
        }

        [Test]
        public void CreateSpaceMenuButton_UsesSpriteAndKeepsDecorationsOutOfRaycasts()
        {
            GameObject parent = new GameObject("SpaceButtonParent");
            try
            {
                Button button = UiFactory.CreateSpaceMenuButton(parent.transform, "START", MenuBootstrap.TitleWideButtonResourcePath, ArcadeTheme.ElectricBlue, new Vector2(300f, 76f), Vector2.zero);

                Assert.That(button.interactable, Is.True);
                Assert.That(button.GetComponent<Image>().sprite, Is.Not.Null);
                Assert.That(button.GetComponent<MenuButtonAnimator>(), Is.Not.Null);
                Assert.That(button.GetComponentInChildren<Text>().text, Is.EqualTo("START"));

                foreach (Image image in button.GetComponentsInChildren<Image>())
                {
                    if (image.gameObject != button.gameObject)
                    {
                        Assert.That(image.raycastTarget, Is.False, image.gameObject.name);
                    }
                }
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }

        [Test]
        public void TitleMenu_CreatesMappedHitAreaButtons()
        {
            GameObject host = new GameObject("MenuBootstrapHost");
            try
            {
                MenuBootstrap menu = host.AddComponent<MenuBootstrap>();
                typeof(MenuBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);

                foreach (string hitArea in MenuBootstrap.StartScreenHitAreaNames)
                {
                    Button button = GameObject.Find(hitArea + "Button")?.GetComponent<Button>();
                    Assert.That(button, Is.Not.Null, hitArea);
                    Assert.That(button.interactable, Is.True, hitArea);
                    Assert.That(button.targetGraphic, Is.Not.Null, hitArea);
                }

                Assert.That(GameObject.Find("MainMenuRoot").GetComponent<MainMenuController>(), Is.Not.Null);
                Assert.That(GameObject.Find("StartGameButtonSlice")?.GetComponent<Image>()?.raycastTarget, Is.False);
                Assert.That(GameObject.Find("ExitButtonSlice")?.GetComponent<Image>()?.raycastTarget, Is.False);
                Assert.That(GameObject.Find("SettingsIconSlice")?.GetComponent<Image>()?.raycastTarget, Is.False);
                Assert.That(GameObject.Find("LeaderboardIconSlice")?.GetComponent<Image>()?.raycastTarget, Is.False);
                Assert.That(GameObject.Find("ShipSelectIconSlice")?.GetComponent<Image>()?.raycastTarget, Is.False);
                RawImage titleBackground = GameObject.Find("TitleBackgroundBG")?.GetComponent<RawImage>();
                Assert.That(titleBackground, Is.Not.Null);
                Assert.That(titleBackground.texture, Is.Not.Null);
                Assert.That(titleBackground.raycastTarget, Is.False);
            }
            finally
            {
                foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }

                foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(camera.gameObject);
                }

                foreach (EventSystem eventSystem in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(eventSystem.gameObject);
                }

                Object.DestroyImmediate(host);
            }
        }

        [Test]
        public void ShipSelect_CreatesFiveFighterRowsWithStats()
        {
            GameObject host = new GameObject("MenuBootstrapShipSelectHost");
            try
            {
                MenuBootstrap menu = host.AddComponent<MenuBootstrap>();
                typeof(MenuBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);
                typeof(MenuBootstrap).GetMethod("ShowShipSelect", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);

                int rowCount = 0;
                int statTrackCount = 0;
                foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
                {
                    if (button.gameObject.name.EndsWith("ShipRow"))
                    {
                        rowCount++;
                    }
                }

                foreach (Image image in Object.FindObjectsByType<Image>(FindObjectsSortMode.None))
                {
                    if (image.gameObject.name.EndsWith("Track"))
                    {
                        statTrackCount++;
                    }
                }

                Assert.That(rowCount, Is.EqualTo(5));
                Assert.That(statTrackCount, Is.EqualTo(20));
                Assert.That(FindButtonByLabel("确认"), Is.Not.Null);
                Assert.That(FindButtonByLabel("返回"), Is.Not.Null);
            }
            finally
            {
                foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }

                foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(camera.gameObject);
                }

                foreach (EventSystem eventSystem in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
                {
                    Object.DestroyImmediate(eventSystem.gameObject);
                }

                Object.DestroyImmediate(host);
            }
        }

        private static Button FindButtonByLabel(string label)
        {
            foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                Text text = button.GetComponentInChildren<Text>();
                if (text != null && text.text == label)
                {
                    return button;
                }
            }

            return null;
        }

        [Test]
        public void CreateCanvas_ProvidesStandaloneInputForButtons()
        {
            EventSystem existing = Object.FindFirstObjectByType<EventSystem>();
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            Canvas canvas = UiFactory.CreateCanvas("InputCanvas");
            try
            {
                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();

                Assert.That(eventSystem, Is.Not.Null);
                Assert.That(eventSystem.GetComponent<StandaloneInputModule>(), Is.Not.Null);
                Assert.That(eventSystem.GetComponent<TouchInputModule>(), Is.Null);
                Assert.That(canvas.GetComponent<GraphicRaycaster>(), Is.Not.Null);
            }
            finally
            {
                if (canvas != null)
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }

                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
                if (eventSystem != null)
                {
                    Object.DestroyImmediate(eventSystem.gameObject);
                }
            }
        }
    }
}
