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
            Assert.That(MenuBootstrap.TitleStartScreenSliceResourcePaths, Is.EqualTo(new[]
            {
                "MainMenu/Backgrounds/bg_start_screen_ai_0",
                "MainMenu/Backgrounds/bg_start_screen_ai_1",
                "MainMenu/Backgrounds/bg_start_screen_ai_2",
                "MainMenu/Backgrounds/bg_start_screen_ai_3"
            }));
            Assert.That(MenuBootstrap.TitleStartButtonSliceResourcePath, Is.EqualTo("MainMenu/Buttons/button_start_game"));
            Assert.That(MenuBootstrap.TitleExitButtonSliceResourcePath, Is.EqualTo("MainMenu/Buttons/button_exit"));
            Assert.That(MenuBootstrap.ShipSelectSlicedResourcePaths, Does.Contain("MainMenu/ShipSelect/panel_frame"));
            Assert.That(MenuBootstrap.ShipSelectSlicedResourcePaths, Does.Contain("MainMenu/ShipSelect/card_selected_frame"));
            Assert.That(MenuBootstrap.SettingsSlicedResourcePaths, Does.Contain("MainMenu/Settings/panel_frame"));
            Assert.That(MenuBootstrap.SettingsSlicedResourcePaths, Does.Contain("MainMenu/Settings/icon_ship"));
            Assert.That(MenuBootstrap.LeaderboardSlicedResourcePaths, Does.Contain("MainMenu/Leaderboard/panel_frame"));
            Assert.That(MenuBootstrap.LeaderboardSlicedResourcePaths, Does.Contain("MainMenu/Leaderboard/row_frame_gold"));
            Assert.That(MenuBootstrap.TitleStarfieldResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_space_far"));
            Assert.That(MenuBootstrap.TitleSparkleOverlayResourcePath, Is.EqualTo("MainMenu/Backgrounds/bg_space_front_stars"));
            Assert.That(MenuBootstrap.TitleLogoBackplateResourcePath, Is.EqualTo("MainMenu/Titles/title_raiden"));
            Assert.That(RuntimeSpriteFactory.RaidenFighterJetResourcePath, Is.EqualTo("RaidenArt/Ships/fighter_jet_128"));
            Assert.That(RuntimeSpriteFactory.GetPlayerShipResourcePath(PlayerShipType.Green), Is.EqualTo("RaidenArt/Ships/player_ship_red"));
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
            foreach (string path in MenuBootstrap.TitleStartScreenSliceResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }
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
            foreach (string path in MenuBootstrap.ShipSelectSlicedResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }
            foreach (string path in MenuBootstrap.SettingsSlicedResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }
            foreach (string path in MenuBootstrap.LeaderboardSlicedResourcePaths)
            {
                Assert.That(Resources.Load<Texture2D>(path), Is.Not.Null, path);
            }
            foreach (PlayerShipType shipType in MenuBootstrap.ShipSelectRoster)
            {
                Assert.That(Resources.Load<Texture2D>(RuntimeSpriteFactory.GetPlayerShipResourcePath(shipType)), Is.Not.Null, shipType.ToString());
            }
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
                for (int i = 0; i < MenuBootstrap.TitleStartScreenSliceResourcePaths.Length; i++)
                {
                    Image titleBackground = GameObject.Find("TitleBackgroundBG" + i)?.GetComponent<Image>();
                    Assert.That(titleBackground, Is.Not.Null, i.ToString());
                    Assert.That(titleBackground.sprite, Is.Not.Null, i.ToString());
                    Assert.That(titleBackground.raycastTarget, Is.False, i.ToString());
                }
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
                int badgeCount = 0;
                int shipImageCount = 0;
                var shipTextures = new System.Collections.Generic.HashSet<Texture2D>();
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

                    if (image.gameObject.name == "RankBadgeOuter")
                    {
                        badgeCount++;
                        Assert.That(Mathf.Abs(image.rectTransform.sizeDelta.x - image.rectTransform.sizeDelta.y), Is.LessThanOrEqualTo(0.1f), image.transform.parent.name);
                    }

                    if (image.gameObject.name == "ShipImage")
                    {
                        shipImageCount++;
                        Assert.That(image.sprite, Is.Not.Null);
                        shipTextures.Add(image.sprite.texture);
                    }
                }

                Assert.That(rowCount, Is.EqualTo(5));
                Assert.That(statTrackCount, Is.EqualTo(20));
                Assert.That(badgeCount, Is.EqualTo(5));
                Assert.That(shipImageCount, Is.EqualTo(5));
                Assert.That(shipTextures.Count, Is.EqualTo(5));
                Assert.That(FindTextByContent("赤焰战机"), Is.Not.Null);
                Assert.That(FindTextByContent("蓝翼A1"), Is.Not.Null);
                Assert.That(FindTextByContent("黄蜂战机"), Is.Not.Null);
                Assert.That(FindTextByContent("紫电战机"), Is.Not.Null);
                Assert.That(FindTextByContent("苍蓝战机"), Is.Not.Null);
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

        [Test]
        public void ShipSelectConfirm_ReturnsToTitleWithoutDifficultyScreen()
        {
            GameObject host = new GameObject("MenuBootstrapShipSelectConfirmHost");
            try
            {
                MenuBootstrap menu = host.AddComponent<MenuBootstrap>();
                typeof(MenuBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);
                typeof(MenuBootstrap).GetMethod("ShowShipSelect", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);

                Button confirm = FindButtonByLabel("确认");
                Assert.That(confirm, Is.Not.Null);

                confirm.onClick.Invoke();

                Assert.That(FindTextByContent("选择难度"), Is.Null);
                Assert.That(GameObject.Find("ArcadeStartButton"), Is.Not.Null);
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
        public void TitleBottomIconSlices_AreEqualAspectSquares()
        {
            AssertSquareAnchors(MenuBootstrap.TitleSettingsSliceAnchorMin, MenuBootstrap.TitleSettingsSliceAnchorMax);
            AssertSquareAnchors(MenuBootstrap.TitleLeaderboardSliceAnchorMin, MenuBootstrap.TitleLeaderboardSliceAnchorMax);
            AssertSquareAnchors(MenuBootstrap.TitleShipSelectSliceAnchorMin, MenuBootstrap.TitleShipSelectSliceAnchorMax);
        }

        [Test]
        public void Settings_CreatesUnifiedHudOptionsAndColorIcons()
        {
            GameObject host = new GameObject("MenuBootstrapSettingsHost");
            try
            {
                MenuBootstrap menu = host.AddComponent<MenuBootstrap>();
                typeof(MenuBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);
                typeof(MenuBootstrap).GetMethod("ShowSettings", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);

                Assert.That(GameObject.Find("SettingsPanel")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(FindTextByContent("音乐音量"), Is.Not.Null);
                Assert.That(FindTextByContent("音效音量"), Is.Not.Null);
                Assert.That(FindTextByContent("默认战机"), Is.Not.Null);
                Assert.That(FindTextByContent(ShipDefinition.Get(SessionState.SelectedShip).DisplayName), Is.Not.Null);
                Assert.That(FindTextByContent("默认难度"), Is.Not.Null);
                Assert.That(FindTextByContent("灵敏度"), Is.Not.Null);
                Assert.That(FindTextByContent("榜名"), Is.Not.Null);
                Assert.That(FindTextByContent(SessionState.LeaderboardName), Is.Not.Null);
                Assert.That(FindButtonByLabel("恢复默认"), Is.Not.Null);
                Assert.That(FindButtonByLabel("返回标题"), Is.Not.Null);
                Assert.That(FindButtonByLabel("保存设置"), Is.Not.Null);

                string[] iconNames =
                {
                    "SettingsSoundIcon",
                    "音乐音量Icon",
                    "音效音量Icon",
                    "OptionIcon"
                };

                foreach (string iconName in iconNames)
                {
                    Assert.That(GameObject.Find(iconName)?.GetComponent<Image>()?.sprite, Is.Not.Null, iconName);
                }
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
        public void Leaderboard_CreatesHudRowsAndBackButton()
        {
            GameObject host = new GameObject("MenuBootstrapLeaderboardHost");
            try
            {
                SessionState.ResetProgress();
                SessionState.SelectDifficulty(GameDifficulty.Low);
                SessionState.CommitRunScore(148815, true);
                MenuBootstrap menu = host.AddComponent<MenuBootstrap>();
                typeof(MenuBootstrap).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);
                typeof(MenuBootstrap).GetMethod("ShowLeaderboard", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(menu, null);

                Assert.That(GameObject.Find("LeaderboardPanel")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(GameObject.Find("LeaderboardHeaderRow")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(GameObject.Find("LeaderboardEntryRow0")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(GameObject.Find("MedalIcon")?.GetComponent<Image>()?.sprite, Is.Not.Null);
                Assert.That(FindTextByContent("本地排行榜"), Is.Not.Null);
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
                SessionState.ResetProgress();
            }
        }

        private static Button FindButtonByLabel(string label)
        {
            foreach (Button button in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                foreach (Text text in button.GetComponentsInChildren<Text>(true))
                {
                    if (text.text == label)
                    {
                        return button;
                    }
                }
            }

            return null;
        }

        private static void AssertSquareAnchors(Vector2 min, Vector2 max)
        {
            float width = max.x - min.x;
            float height = max.y - min.y;
            Assert.That(width / height, Is.EqualTo(16f / 9f).Within(0.08f));
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

        private static Text FindTextStartingWith(string prefix)
        {
            foreach (Text text in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if (text.text.StartsWith(prefix))
                {
                    return text;
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
