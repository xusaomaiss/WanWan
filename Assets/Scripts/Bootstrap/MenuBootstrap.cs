using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class MenuBootstrap : MonoBehaviour
    {
        public static readonly bool ShowStartupLogo = false;
        public const int TitleCircleButtonFontSize = 30;
        public const int TitleHighScoreFontSize = TitleCircleButtonFontSize;
        public const string TitleHeroFighterObjectName = "TitleHeroFighter";
        public const string TitleStartScreenResourcePath = "MainMenu/Backgrounds/bg_start_screen_ai";
        public static readonly string[] TitleStartScreenSliceResourcePaths =
        {
            "MainMenu/Backgrounds/bg_start_screen_ai_0",
            "MainMenu/Backgrounds/bg_start_screen_ai_1",
            "MainMenu/Backgrounds/bg_start_screen_ai_2",
            "MainMenu/Backgrounds/bg_start_screen_ai_3"
        };
        public const string TitleFarStarsResourcePath = "MainMenu/Backgrounds/bg_space_far";
        public const string TitleMidNebulaResourcePath = "MainMenu/Backgrounds/bg_space_mid_nebula";
        public const string TitleFrontStarsResourcePath = "MainMenu/Backgrounds/bg_space_front_stars";
        public const string TitleStarfieldResourcePath = TitleFarStarsResourcePath;
        public const string TitleSparkleOverlayResourcePath = TitleFrontStarsResourcePath;
        public const string TitleLogoBackplateResourcePath = "MainMenu/Titles/title_raiden";
        public const string TitleSpaceShooterResourcePath = "MainMenu/Titles/title_space_shooter";
        public const string TitleWideButtonResourcePath = "MainMenu/Buttons/ui_button_main";
        public const string TitleStartButtonSliceResourcePath = "MainMenu/Buttons/button_start_game";
        public const string TitleExitButtonSliceResourcePath = "MainMenu/Buttons/button_exit";
        public const string TitleIconFrameResourcePath = "MainMenu/Buttons/ui_button_square";
        public const string IconInfoResourcePath = "MainMenu/Icons/icon_info";
        public const string IconSettingsResourcePath = "MainMenu/Icons/icon_settings";
        public const string IconSettingsLargeResourcePath = "MainMenu/Icons/icon_settings_large";
        public const string IconControlResourcePath = "MainMenu/Icons/icon_control";
        public const string IconTrophyResourcePath = "MainMenu/Icons/icon_trophy";
        public const string IconLeaderboardLargeResourcePath = "MainMenu/Icons/icon_leaderboard_large";
        public const string IconShopResourcePath = "MainMenu/Icons/icon_shop";
        public const string IconShipSelectLargeResourcePath = "MainMenu/Icons/icon_ship_select_large";
        public const string IconHelpResourcePath = "MainMenu/Icons/icon_help";
        public const string ParticleStarResourcePath = "MainMenu/Effects/particle_star";
        public const string ShipSelectPanelFrameResourcePath = "MainMenu/ShipSelect/panel_frame";
        public const string ShipSelectCardFrameResourcePath = "MainMenu/ShipSelect/card_frame";
        public const string ShipSelectCardSelectedFrameResourcePath = "MainMenu/ShipSelect/card_selected_frame";
        public const string ShipSelectButtonBackResourcePath = "MainMenu/ShipSelect/button_back";
        public const string ShipSelectButtonConfirmResourcePath = "MainMenu/ShipSelect/button_confirm";
        public const string ShipSelectBadgeFrameResourcePath = "MainMenu/ShipSelect/badge_frame";
        public const string ShipSelectStatTrackResourcePath = "MainMenu/ShipSelect/stat_track";
        public const string ShipSelectStatFillResourcePath = "MainMenu/ShipSelect/stat_fill";
        public const string ShipSelectStarFullResourcePath = "MainMenu/ShipSelect/star_full";
        public const string ShipSelectStarEmptyResourcePath = "MainMenu/ShipSelect/star_empty";
        public const string SettingsPanelFrameResourcePath = "MainMenu/Settings/panel_frame";
        public const string SettingsOptionFrameResourcePath = "MainMenu/Settings/option_frame";
        public const string SettingsOptionGoldFrameResourcePath = "MainMenu/Settings/option_frame_gold";
        public const string SettingsButtonBackResourcePath = "MainMenu/Settings/button_back";
        public const string SettingsButtonSaveResourcePath = "MainMenu/Settings/button_save";
        public const string SettingsSliderTrackResourcePath = "MainMenu/Settings/slider_track";
        public const string SettingsSliderFillResourcePath = "MainMenu/Settings/slider_fill";
        public const string SettingsToggleOnResourcePath = "MainMenu/Settings/toggle_on";
        public const string SettingsToggleOffResourcePath = "MainMenu/Settings/toggle_off";
        public const string SettingsIconSoundResourcePath = "MainMenu/Settings/icon_sound";
        public const string SettingsIconMusicResourcePath = "MainMenu/Settings/icon_music";
        public const string SettingsIconSfxResourcePath = "MainMenu/Settings/icon_sfx";
        public const string SettingsIconShipResourcePath = "MainMenu/Settings/icon_ship";
        public const string SettingsIconDifficultyResourcePath = "MainMenu/Settings/icon_difficulty";
        public const string SettingsIconSensitivityResourcePath = "MainMenu/Settings/icon_sensitivity";
        public const string SettingsIconRankResourcePath = "MainMenu/Settings/icon_rank";
        public const string SettingsIconVibrationResourcePath = "MainMenu/Settings/icon_vibration";
        public const string SettingsIconDamageResourcePath = "MainMenu/Settings/icon_damage";
        public const string SettingsIconEffectsResourcePath = "MainMenu/Settings/icon_effects";
        public const string SettingsIconResetResourcePath = "MainMenu/Settings/icon_reset";
        public const string LeaderboardPanelFrameResourcePath = "MainMenu/Leaderboard/panel_frame";
        public const string LeaderboardRowFrameResourcePath = "MainMenu/Leaderboard/row_frame";
        public const string LeaderboardGoldRowFrameResourcePath = "MainMenu/Leaderboard/row_frame_gold";
        public const string LeaderboardButtonBackResourcePath = "MainMenu/Leaderboard/button_back";
        public const string LeaderboardButtonClearResourcePath = "MainMenu/Leaderboard/button_clear";
        public const string LeaderboardMedalGoldResourcePath = "MainMenu/Leaderboard/medal_gold";
        public const string LeaderboardMedalSilverResourcePath = "MainMenu/Leaderboard/medal_silver";
        public const string LeaderboardMedalBronzeResourcePath = "MainMenu/Leaderboard/medal_bronze";
        public static readonly string[] ShipSelectSlicedResourcePaths =
        {
            ShipSelectPanelFrameResourcePath,
            ShipSelectCardFrameResourcePath,
            ShipSelectCardSelectedFrameResourcePath,
            ShipSelectButtonBackResourcePath,
            ShipSelectButtonConfirmResourcePath,
            ShipSelectBadgeFrameResourcePath,
            ShipSelectStatTrackResourcePath,
            ShipSelectStatFillResourcePath,
            ShipSelectStarFullResourcePath,
            ShipSelectStarEmptyResourcePath
        };
        public static readonly string[] SettingsSlicedResourcePaths =
        {
            SettingsPanelFrameResourcePath,
            SettingsOptionFrameResourcePath,
            SettingsOptionGoldFrameResourcePath,
            SettingsButtonBackResourcePath,
            SettingsButtonSaveResourcePath,
            SettingsSliderTrackResourcePath,
            SettingsSliderFillResourcePath,
            SettingsToggleOnResourcePath,
            SettingsToggleOffResourcePath,
            SettingsIconSoundResourcePath,
            SettingsIconMusicResourcePath,
            SettingsIconSfxResourcePath,
            SettingsIconShipResourcePath,
            SettingsIconDifficultyResourcePath,
            SettingsIconSensitivityResourcePath,
            SettingsIconRankResourcePath,
            SettingsIconVibrationResourcePath,
            SettingsIconDamageResourcePath,
            SettingsIconEffectsResourcePath,
            SettingsIconResetResourcePath
        };
        public static readonly string[] LeaderboardSlicedResourcePaths =
        {
            LeaderboardPanelFrameResourcePath,
            LeaderboardRowFrameResourcePath,
            LeaderboardGoldRowFrameResourcePath,
            LeaderboardButtonBackResourcePath,
            LeaderboardButtonClearResourcePath,
            LeaderboardMedalGoldResourcePath,
            LeaderboardMedalSilverResourcePath,
            LeaderboardMedalBronzeResourcePath
        };
        public static readonly string[] TitlePrimaryLabels = { "开始游戏", "退出" };
        public static readonly Vector2 ModernTitleButtonSize = new Vector2(486f, 96f);
        public static readonly Vector2 ModernTitleIconSize = new Vector2(112f, 112f);
        public static readonly string[] StartScreenHitAreaNames = { "ArcadeStart", "ClassicExit", "Settings", "Leaderboard", "ShipSelect" };
        public static readonly Vector2 TitleStartButtonSliceAnchorMin = new Vector2(0.2232f, 0.2572f);
        public static readonly Vector2 TitleStartButtonSliceAnchorMax = new Vector2(0.7779f, 0.3481f);
        public static readonly Vector2 TitleExitButtonSliceAnchorMin = new Vector2(0.2232f, 0.1507f);
        public static readonly Vector2 TitleExitButtonSliceAnchorMax = new Vector2(0.7779f, 0.2416f);
        public static readonly Vector2 TitleSettingsSliceAnchorMin = new Vector2(0.090f, 0.027f);
        public static readonly Vector2 TitleSettingsSliceAnchorMax = new Vector2(0.270f, 0.128f);
        public static readonly Vector2 TitleLeaderboardSliceAnchorMin = new Vector2(0.410f, 0.027f);
        public static readonly Vector2 TitleLeaderboardSliceAnchorMax = new Vector2(0.590f, 0.128f);
        public static readonly Vector2 TitleShipSelectSliceAnchorMin = new Vector2(0.730f, 0.027f);
        public static readonly Vector2 TitleShipSelectSliceAnchorMax = new Vector2(0.910f, 0.128f);
        public static readonly string[] RightSideMenuLabels = { "CONTROL", "ACHIEVEMENT", "SHOP", "HELP" };
        public static readonly PlayerShipType[] ShipSelectRoster =
        {
            PlayerShipType.Green,
            PlayerShipType.Blue,
            PlayerShipType.Yellow,
            PlayerShipType.Purple,
            PlayerShipType.Azure
        };

        private static readonly Vector2 ShipPreviewSize = new Vector2(138f, 112f);

        private Canvas canvas;
        private MenuUiState state;
        private PlayerShipType selectedShip;
        private Text audioStatusText;
        private Text musicValueText;
        private Text sfxValueText;
        private Text opacityValueText;
        private Text saveStatusText;
        private float saveStatusTimer;
        private MenuTransitionController transitionController;
        private Image backgroundImage; // Store reference to background image

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            selectedShip = SessionState.SelectedShip;
            EnsureCamera(ArcadeTheme.BackgroundBlack);
            canvas = UiFactory.CreateCanvas("MenuCanvas");
            var transitionGo = new GameObject("MenuTransition");
            transitionGo.transform.SetParent(canvas.transform, false);
            transitionController = transitionGo.AddComponent<MenuTransitionController>();
            if (TryShowAndroidDebugScreen())
            {
                return;
            }

            if (ShowStartupLogo)
            {
                ShowLogo();
                StartCoroutine(LogoToTitle());
            }
            else
            {
                ShowTitle();
            }
        }

        private bool TryShowAndroidDebugScreen()
        {
            string debugScreen = GetAndroidDebugScreen();
            if (string.IsNullOrEmpty(debugScreen))
            {
                return false;
            }

            SessionState.ResetProgress();
            SessionState.SelectShip(PlayerShipType.Blue);
            SessionState.SelectDifficulty(GameDifficulty.Low);

            switch (debugScreen.ToLowerInvariant())
            {
                case "settings":
                    ShowSettings();
                    return true;
                case "leaderboard":
                    SessionState.RecordLeaderboardScore("AAA", 148815, GameDifficulty.Low, 1, 0);
                    SessionState.RecordLeaderboardScore("ABA", 146210, GameDifficulty.Low, 0, 0);
                    ShowLeaderboard();
                    return true;
                case "victory":
                    SessionState.AddSpendableScore(2895840);
                    SessionState.CommitRunScore(116915, true, "甲", "安卓调试胜利截图");
                    SceneNavigator.LoadGameOver();
                    return true;
                default:
                    return false;
            }
        }

        private static string GetAndroidDebugScreen()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent"))
                {
                    return intent.Call<string>("getStringExtra", "wanwan_debug_screen") ?? string.Empty;
                }
            }
            catch (AndroidJavaException exception)
            {
                Debug.LogWarning("Unable to read Android debug screen extra: " + exception.Message);
            }
#endif
            return string.Empty;
        }

        private void Update()
        {
            RefreshSaveStatus();
        }

        private static void EnsureCamera(Color background)
        {
            if (Camera.main != null)
            {
                Camera.main.backgroundColor = background;
                Camera.main.orthographic = true;
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraComponent.orthographic = true;
            cameraComponent.backgroundColor = background;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }

        private IEnumerator LogoToTitle()
        {
            yield return new WaitForSeconds(2f);
            if (state == MenuUiState.Logo)
            {
                ShowTitle();
            }
        }

        private void ClearCanvas()
        {
            saveStatusText = null;
            saveStatusTimer = 0f;
            for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = canvas.transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }
        }

        private Image CreateBackground(string name)
        {
            ClearCanvas();

            Image root = UiFactory.CreatePanel(canvas.transform, name, Color.clear, Vector2.zero, Vector2.one);
            root.raycastTarget = false;

            if (state == MenuUiState.Title)
            {
                CreateTitleBackgroundSlices(root.transform, name + "BG");
                return root;
            }

            backgroundImage = UiFactory.CreatePanel(root.transform, name + "BG", Color.white, Vector2.zero, Vector2.one);
            backgroundImage.sprite = RuntimeSpriteFactory.GetSkyBackgroundSprite();
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.preserveAspect = false;
            backgroundImage.raycastTarget = false;


            Image shade = UiFactory.CreatePanel(root.transform, name + "Shade", new Color(0.01f, 0.02f, 0.05f, 0.34f), Vector2.zero, Vector2.one);
            shade.raycastTarget = false;

            Image topVignette = UiFactory.CreatePanel(root.transform, name + "TopVignette", new Color(0f, 0f, 0f, 0.18f), new Vector2(0f, 0.72f), Vector2.one);
            topVignette.raycastTarget = false;

            Image bottomVignette = UiFactory.CreatePanel(root.transform, name + "BottomVignette", new Color(0f, 0f, 0f, 0.58f), Vector2.zero, new Vector2(1f, 0.42f));
            bottomVignette.raycastTarget = false;

            CreateScanlines(root.transform);

            return root;
        }

        private static void CreateTitleBackgroundSlices(Transform parent, string name)
        {
            for (int i = 0; i < TitleStartScreenSliceResourcePaths.Length; i++)
            {
                float yMax = 1f - (i / (float)TitleStartScreenSliceResourcePaths.Length);
                float yMin = 1f - ((i + 1) / (float)TitleStartScreenSliceResourcePaths.Length);
                Image image = UiFactory.CreateSpritePanel(
                    parent,
                    name + i,
                    TitleStartScreenSliceResourcePaths[i],
                    Color.white,
                    new Vector2(0f, yMin),
                    new Vector2(1f, yMax));
                image.preserveAspect = false;
                image.raycastTarget = false;
            }
        }

        private static Image CreateTitleParallaxLayer(Transform parent, string name, string resourcePath, Color tint, float speed, float y, Vector2 drift)
        {
            Image layer = UiFactory.CreateSpritePanel(parent, name, resourcePath, tint, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransform rect = layer.rectTransform;
            rect.sizeDelta = new Vector2(1080f, 1920f);
            rect.anchoredPosition = new Vector2(0f, y);
            layer.raycastTarget = false;
            StarfieldParallax parallax = layer.gameObject.AddComponent<StarfieldParallax>();
            parallax.Configure(speed, 1920f, drift);
            return layer;
        }

        private static Sprite LoadTitleBackgroundSprite()
        {
            Texture2D texture = Resources.Load<Texture2D>(TitleStartScreenResourcePath);
            return texture == null
                ? RuntimeSpriteFactory.GetMenuStormTitleSprite()
                : Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
        }

        private static void CreateScanlines(Transform parent)
        {
            for (int i = 0; i < 24; i++)
            {
                float y = i / 24f;
                UiFactory.CreateDivider(parent, "Scanline" + i, new Color(1f, 1f, 1f, 0.025f), new Vector2(0f, y), new Vector2(1f, y + 0.0035f));
            }
        }

        private void ShowLogo()
        {
            state = MenuUiState.Logo;
            Image background = CreateBackground("LogoBackground");
            UiFactory.CreateArcadeLabel(background.transform, "WANWAN", ArcadeTheme.LogoSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.65f), Vector2.zero);
            UiFactory.CreateArcadeLabel(background.transform, "空投爆破", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.56f), Vector2.zero);
            UiFactory.CreateDivider(background.transform, "LogoDivider", ArcadeTheme.WarningRed, new Vector2(0.2f, 0.47f), new Vector2(0.8f, 0.476f));
            UiFactory.CreateArcadeLabel(background.transform, "街机空战指挥部", ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.ElectricBlue, FontStyle.Bold, new Vector2(0.08f, 0.39f), new Vector2(0.92f, 0.45f), Vector2.zero);
        }

        private void ShowTitle()
        {
            state = MenuUiState.Title;
            Image background = CreateBackground("TitleBackground");
            Image root = UiFactory.CreatePanel(background.transform, "MainMenuRoot", Color.clear, Vector2.zero, Vector2.one);
            root.raycastTarget = false;
            MainMenuController controller = root.gameObject.AddComponent<MainMenuController>();
            controller.Initialize(
                SceneNavigator.LoadGame,
                ShowShipSelect,
                Application.Quit,
                ShowAutoSaveStatus,
                ShowSettings,
                ShowSettings,
                ShowLeaderboard,
                ShowAutoSaveStatus,
                ShowSettings);
            Transform t = root.transform;

            CreateTitleMenuSlices(t);

            Button arcade = CreateTitleHitButton(t, "ArcadeStart", new Vector2(0.22f, 0.255f), new Vector2(0.78f, 0.335f));
            arcade.onClick.AddListener(controller.StartGame);
            Button classic = CreateTitleHitButton(t, "ClassicExit", new Vector2(0.22f, 0.145f), new Vector2(0.78f, 0.225f));
            classic.onClick.AddListener(controller.ExitGame);
            Button settings = CreateTitleHitButton(t, "Settings", new Vector2(0.045f, 0.01f), new Vector2(0.235f, 0.14f));
            settings.onClick.AddListener(controller.OpenSettings);
            Button leaderboard = CreateTitleHitButton(t, "Leaderboard", new Vector2(0.365f, 0.005f), new Vector2(0.635f, 0.13f));
            leaderboard.onClick.AddListener(controller.OpenAchievement);
            Button shipSelect = CreateTitleHitButton(t, "ShipSelect", new Vector2(0.765f, 0.01f), new Vector2(0.955f, 0.14f));
            shipSelect.onClick.AddListener(controller.OpenMap);

            saveStatusText = UiFactory.CreateArcadeLabel(t, string.Empty, ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.14f, 0.30f), new Vector2(0.86f, 0.34f), Vector2.zero);
            saveStatusText.gameObject.SetActive(false);

            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(t));
        }

        private static void CreateTitleMenuSlices(Transform parent)
        {
            CreateTitleSlice(parent, "StartGameButtonSlice", TitleStartButtonSliceResourcePath, TitleStartButtonSliceAnchorMin, TitleStartButtonSliceAnchorMax);
            CreateTitleSlice(parent, "ExitButtonSlice", TitleExitButtonSliceResourcePath, TitleExitButtonSliceAnchorMin, TitleExitButtonSliceAnchorMax);
            CreateTitleSlice(parent, "SettingsIconSlice", IconSettingsLargeResourcePath, TitleSettingsSliceAnchorMin, TitleSettingsSliceAnchorMax);
            CreateTitleSlice(parent, "LeaderboardIconSlice", IconLeaderboardLargeResourcePath, TitleLeaderboardSliceAnchorMin, TitleLeaderboardSliceAnchorMax);
            CreateTitleSlice(parent, "ShipSelectIconSlice", IconShipSelectLargeResourcePath, TitleShipSelectSliceAnchorMin, TitleShipSelectSliceAnchorMax);
        }

        private static Image CreateTitleSlice(Transform parent, string name, string resourcePath, Vector2 anchorMin, Vector2 anchorMax)
        {
            Image image = UiFactory.CreateSpritePanel(parent, name, resourcePath, Color.white, anchorMin, anchorMax);
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private static Button CreateTitleHitButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            Button button = UiFactory.CreateButton(parent, string.Empty, new Color(1f, 1f, 1f, 0.001f), Color.clear, Vector2.zero, Vector2.zero, anchorMin, anchorMax);
            button.gameObject.name = name + "Button";
            Image image = button.GetComponent<Image>();
            image.raycastTarget = true;
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(1f, 1f, 1f, 0.001f);
            colors.highlightedColor = new Color(0.5f, 1f, 1f, 0.08f);
            colors.pressedColor = new Color(0.2f, 0.8f, 1f, 0.16f);
            colors.selectedColor = colors.normalColor;
            colors.disabledColor = Color.clear;
            button.colors = colors;

            Text label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.raycastTarget = false;
            }

            return button;
        }

        private static void CreateMetallicTitle(Transform parent)
        {
            Image subtitlePlate = UiFactory.CreateSpritePanel(parent, "TitleSpaceShooterPlate", TitleSpaceShooterResourcePath, new Color(0.62f, 0.94f, 1f, 0.18f), new Vector2(0.22f, 0.72f), new Vector2(0.78f, 0.9f));
            subtitlePlate.raycastTarget = false;
            UiFactory.CreateArcadeLabel(parent, "SPACE SHOOTER", 42, TextAnchor.MiddleCenter, new Color(0.58f, 0.94f, 1f, 0.96f), FontStyle.Bold, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.88f), Vector2.zero);

            Image titlePlate = UiFactory.CreateSpritePanel(parent, "TitleRaidenPlate", TitleLogoBackplateResourcePath, new Color(0.7f, 0.96f, 1f, 0.22f), new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.78f));
            titlePlate.raycastTarget = false;

            Text glow = UiFactory.CreateArcadeLabel(parent, "RAIDEN", 112, TextAnchor.MiddleCenter, new Color(0.2f, 0.86f, 1f, 0.38f), FontStyle.Bold, new Vector2(0.02f, 0.34f), new Vector2(0.98f, 0.74f), Vector2.zero);
            Outline glowOutline = glow.gameObject.AddComponent<Outline>();
            glowOutline.effectColor = new Color(0.26f, 0.95f, 1f, 0.8f);
            glowOutline.effectDistance = new Vector2(7f, -7f);
            glow.raycastTarget = false;

            Text shadow = UiFactory.CreateArcadeLabel(parent, "RAIDEN", 112, TextAnchor.MiddleCenter, new Color(0.02f, 0.08f, 0.14f, 0.9f), FontStyle.Bold, new Vector2(0.02f, 0.32f), new Vector2(0.98f, 0.72f), new Vector2(0f, -8f));
            shadow.raycastTarget = false;

            Text title = UiFactory.CreateArcadeLabel(parent, "RAIDEN", 112, TextAnchor.MiddleCenter, new Color(0.86f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.02f, 0.34f), new Vector2(0.98f, 0.74f), Vector2.zero);
            Outline outline = title.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.05f, 0.42f, 0.6f, 0.95f);
            outline.effectDistance = new Vector2(3f, -3f);
            title.raycastTarget = false;

            UiFactory.CreateDivider(parent, "TitleMetallicHighlight", new Color(1f, 1f, 1f, 0.58f), new Vector2(0.24f, 0.58f), new Vector2(0.76f, 0.594f));
            UiFactory.CreateDivider(parent, "TitleCyanUnderGlow", new Color(0.22f, 0.92f, 1f, 0.42f), new Vector2(0.24f, 0.35f), new Vector2(0.76f, 0.365f));
        }

        private void CreateRightSideMenu(Transform parent, MainMenuController controller)
        {
            Sprite[] icons =
            {
                LoadTitleIcon(IconControlResourcePath),
                LoadTitleIcon(IconTrophyResourcePath),
                LoadTitleIcon(IconShopResourcePath),
                LoadTitleIcon(IconHelpResourcePath)
            };
            UnityEngine.Events.UnityAction[] actions =
            {
                controller.OpenControl,
                controller.OpenAchievement,
                controller.OpenShop,
                controller.OpenHelp
            };
            Color[] accents =
            {
                ArcadeTheme.ElectricBlue,
                ArcadeTheme.EnergyYellow,
                ArcadeTheme.EnergyYellow,
                ArcadeTheme.ElectricBlue
            };

            const float firstY = -435f;
            const float spacing = 144f;
            for (int i = 0; i < icons.Length; i++)
            {
                Button button = UiFactory.CreateSpaceIconButton(parent, icons[i], string.Empty, TitleIconFrameResourcePath, accents[i], ModernTitleIconSize, new Vector2(-82f, firstY - (i * spacing)), Vector2.one, Vector2.one);
                button.gameObject.name = RightSideMenuLabels[i] + "Button";
                button.onClick.AddListener(actions[i]);
            }
        }

        private static void CreateSparkleParticles(Transform parent)
        {
            for (int i = 0; i < 22; i++)
            {
                float x = 64f + ((i * 137f) % 950f);
                float y = 80f + ((i * 211f) % 1780f);
                float size = 6f + (i % 4) * 4f;
                Image sparkle = UiFactory.CreatePanel(parent, "SparkleParticle" + i, new Color(0.6f, 0.95f, 1f, 0.18f + (i % 3) * 0.08f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                sparkle.sprite = LoadTitleIcon(ParticleStarResourcePath);
                sparkle.raycastTarget = false;
                RectTransform rect = sparkle.rectTransform;
                rect.sizeDelta = new Vector2(size, size);
                rect.anchoredPosition = new Vector2(x - 540f, y - 960f);
                sparkle.gameObject.AddComponent<StarfieldParallax>().Configure(5f + (i % 5) * 2f, 1920f, Vector2.zero);
            }
        }

        private static Sprite LoadTitleIcon(string resourcePath)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            return texture == null ? RuntimeSpriteFactory.GetCircleSprite() : Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
        }

        private IEnumerator AnimateFighter(Image fighter)
        {
            if (fighter == null) yield break;
            RectTransform rt = fighter.rectTransform;
            Vector2 basePos = rt.anchoredPosition;
            float elapsed = 0f;
            while (fighter != null && fighter.isActiveAndEnabled)
            {
                elapsed += Time.unscaledDeltaTime;
                float yOffset = Mathf.Sin(elapsed * 1.2f) * 12f;
                float rot = Mathf.Sin(elapsed * 0.8f) * 3f;
                rt.anchoredPosition = basePos + new Vector2(0f, yOffset);
                rt.localRotation = Quaternion.Euler(0f, 0f, rot);
                yield return null;
            }
        }

        private static void CreateTitleHeroFighter(Transform parent)
        {
            // Removed - the storm background already shows fighter jets
        }

        private void ShowShipSelect()
        {
            state = MenuUiState.ShipSelect;
            Image background = CreateBackground("ShipSelectBackground");
            Transform t = background.transform;
            Image frame = UiFactory.CreateSpritePanel(t, "ShipSelectFrame", ShipSelectPanelFrameResourcePath, Color.white, new Vector2(0.035f, 0.14f), new Vector2(0.965f, 0.895f));
            frame.raycastTarget = false;
            UiFactory.CreateDivider(frame.transform, "ShipSelectTopGlow", new Color(0.38f, 0.95f, 1f, 0.52f), new Vector2(0.06f, 0.965f), new Vector2(0.94f, 0.972f));
            UiFactory.CreateDivider(frame.transform, "ShipSelectBottomGlow", new Color(0.38f, 0.95f, 1f, 0.36f), new Vector2(0.06f, 0.025f), new Vector2(0.94f, 0.032f));

            UiFactory.CreateArcadeLabel(t, "选择战机", 58, TextAnchor.MiddleCenter, new Color(0.86f, 1f, 0.96f, 0.96f), FontStyle.Bold, new Vector2(0.08f, 0.90f), new Vector2(0.92f, 0.965f), Vector2.zero);
            for (int i = 0; i < ShipSelectRoster.Length; i++)
            {
                const float top = 0.845f;
                const float rowHeight = 0.126f;
                const float gap = 0.012f;
                float rowTop = top - (i * (rowHeight + gap));
                CreateShipCard(background.transform, ShipSelectRoster[i], i + 1, new Vector2(0.07f, rowTop - rowHeight), new Vector2(0.93f, rowTop));
            }

            Button back = CreateShipSelectActionButton(background.transform, "返回", ShipSelectButtonBackResourcePath, new Vector2(-270f, -820f));
            back.onClick.AddListener(ShowTitle);
            Button next = CreateShipSelectActionButton(background.transform, "确认", ShipSelectButtonConfirmResourcePath, new Vector2(270f, -820f));
            next.onClick.AddListener(() =>
            {
                SessionState.SelectShip(selectedShip);
                ShowTitle();
            });
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
        }

        private void CreateShipCard(Transform parent, PlayerShipType shipType, int rank, Vector2 anchorMin, Vector2 anchorMax)
        {
            ShipDefinition ship = ShipDefinition.Get(shipType);
            bool selected = selectedShip == shipType;
            Button cardButton = UiFactory.CreateButton(parent, string.Empty, Color.clear, Color.clear, Vector2.zero, Vector2.zero, anchorMin, anchorMax);
            cardButton.gameObject.name = ship.DisplayName + "ShipRow";
            Image card = cardButton.GetComponent<Image>();
            card.sprite = null;
            card.color = new Color(1f, 1f, 1f, 0.001f);
            card.raycastTarget = true;
            RectTransform cardRect = card.rectTransform;
            cardRect.offsetMin = Vector2.zero;
            cardRect.offsetMax = Vector2.zero;
            Text emptyLabel = cardButton.GetComponentInChildren<Text>();
            if (emptyLabel != null)
            {
                emptyLabel.raycastTarget = false;
            }

            Image frame = UiFactory.CreateSpritePanel(card.transform, selected ? "SelectedCardSlice" : "CardSlice", selected ? ShipSelectCardSelectedFrameResourcePath : ShipSelectCardFrameResourcePath, Color.white, Vector2.zero, Vector2.one);
            frame.raycastTarget = false;
            Image accentWash = UiFactory.CreatePanel(card.transform, "AccentWash", new Color(ship.AccentColor.r, ship.AccentColor.g, ship.AccentColor.b, selected ? 0.15f : 0.055f), new Vector2(0.02f, 0.08f), new Vector2(0.98f, 0.92f));
            accentWash.raycastTarget = false;

            CreateRankBadge(card.transform, rank, selected, ship.AccentColor);
            CreateSelectionChevron(card.transform, rank, selected, ship.AccentColor);

            Image shipPanel = UiFactory.CreatePanel(card.transform, "ShipPanel", new Color(0.02f, 0.1f, 0.12f, 0.52f), new Vector2(0.205f, 0.06f), new Vector2(0.43f, 0.94f));
            shipPanel.raycastTarget = false;
            UiFactory.CreateDivider(shipPanel.transform, "ShipPanelGlowTop", new Color(0.3f, 1f, 0.82f, 0.30f), new Vector2(0.05f, 0.94f), new Vector2(0.95f, 0.97f));
            Image shipImage = UiFactory.CreatePanel(shipPanel.transform, "ShipImage", Color.white, Vector2.zero, Vector2.one);
            ConfigureShipPreview(shipImage, shipType, rank);

            Text name = UiFactory.CreateArcadeLabel(card.transform, ship.DisplayName, 34, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold, new Vector2(0.46f, 0.51f), new Vector2(0.66f, 0.84f), Vector2.zero);
            UiFactory.ConfigureConstrainedText(name, 22, 34);
            Text attack = UiFactory.CreateArcadeLabel(card.transform, ship.MainWeapon, 23, TextAnchor.MiddleLeft, new Color(0.58f, 1f, 0.96f), FontStyle.Bold, new Vector2(0.46f, 0.22f), new Vector2(0.66f, 0.50f), Vector2.zero);
            UiFactory.ConfigureSingleLine(attack);

            CreateStarRating(card.transform, ship.PowerStars, new Vector2(0.72f, 0.68f), new Vector2(0.94f, 0.88f));
            CreateStatBar(card.transform, "火力", ship.PowerStars, new Vector2(0.66f, 0.52f), new Vector2(0.94f, 0.63f));
            CreateStatBar(card.transform, "攻击", ship.AttackStars, new Vector2(0.66f, 0.39f), new Vector2(0.94f, 0.50f));
            CreateStatBar(card.transform, "防御", ship.DefenseStars, new Vector2(0.66f, 0.26f), new Vector2(0.94f, 0.37f));
            CreateStatBar(card.transform, "速度", ship.SpeedStars, new Vector2(0.66f, 0.13f), new Vector2(0.94f, 0.24f));

            cardButton.onClick.AddListener(() =>
            {
                selectedShip = shipType;
                ShowShipSelect();
            });
        }

        private static void CreateRankBadge(Transform parent, int rank, bool selected, Color accent)
        {
            Color badgeColor = selected ? ArcadeTheme.EnergyYellow : accent;
            Image glow = UiFactory.CreateSpritePanel(parent, "RankBadgeGlow", ShipSelectBadgeFrameResourcePath, new Color(badgeColor.r, badgeColor.g, badgeColor.b, selected ? 0.42f : 0.24f), new Vector2(0.10f, 0.5f), new Vector2(0.10f, 0.5f));
            glow.raycastTarget = false;
            glow.preserveAspect = true;
            glow.rectTransform.sizeDelta = selected ? new Vector2(112f, 112f) : new Vector2(102f, 102f);
            glow.rectTransform.anchoredPosition = Vector2.zero;

            Image outer = UiFactory.CreateSpritePanel(parent, "RankBadgeOuter", ShipSelectBadgeFrameResourcePath, selected ? Color.Lerp(Color.white, ArcadeTheme.EnergyYellow, 0.12f) : Color.Lerp(Color.white, accent, 0.24f), new Vector2(0.10f, 0.5f), new Vector2(0.10f, 0.5f));
            outer.raycastTarget = false;
            outer.preserveAspect = true;
            outer.rectTransform.sizeDelta = new Vector2(92f, 92f);
            outer.rectTransform.anchoredPosition = Vector2.zero;
            Text number = UiFactory.CreateArcadeLabel(outer.transform, rank.ToString(), 44, TextAnchor.MiddleCenter, selected ? ArcadeTheme.EnergyYellow : Color.white, FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            number.raycastTarget = false;
            UiFactory.ConfigureSingleLine(number);
        }

        private static void CreateSelectionChevron(Transform parent, int rank, bool selected, Color accent)
        {
            Color color = selected ? new Color(0.54f, 1f, 0.44f, 0.95f) : (rank < 4 ? new Color(1f, 0.28f, 0.2f, 0.86f) : new Color(0.5f, 1f, 1f, 0.76f));
            Text chevron = UiFactory.CreateArcadeLabel(parent, selected ? "▲" : (rank < 4 ? "▼" : "◆"), 31, TextAnchor.MiddleCenter, Color.Lerp(color, accent, selected ? 0.12f : 0f), FontStyle.Bold, new Vector2(0.035f, 0.66f), new Vector2(0.085f, 0.92f), Vector2.zero);
            chevron.raycastTarget = false;
        }

        private static void CreateStatBar(Transform parent, string label, int value, Vector2 anchorMin, Vector2 anchorMax)
        {
            UiFactory.CreateArcadeLabel(parent, label, 17, TextAnchor.MiddleLeft, new Color(0.86f, 1f, 0.92f), FontStyle.Bold, anchorMin, new Vector2(anchorMin.x + 0.075f, anchorMax.y), Vector2.zero).raycastTarget = false;
            Image track = UiFactory.CreateSpritePanel(parent, label + "Track", ShipSelectStatTrackResourcePath, Color.white, new Vector2(anchorMin.x + 0.085f, anchorMin.y + 0.018f), anchorMax);
            track.raycastTarget = false;
            float fill = Mathf.Clamp01(value / 5f);
            Image bar = UiFactory.CreateSpritePanel(track.transform, label + "Fill", ShipSelectStatFillResourcePath, Color.white, Vector2.zero, new Vector2(fill, 1f));
            bar.raycastTarget = false;
            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
            bar.fillAmount = fill;
        }

        private static void CreateStarRating(Transform parent, int value, Vector2 anchorMin, Vector2 anchorMax)
        {
            Image root = UiFactory.CreatePanel(parent, "StarRating", Color.clear, anchorMin, anchorMax);
            root.raycastTarget = false;
            for (int i = 0; i < 5; i++)
            {
                float x0 = i / 5f;
                float x1 = (i + 0.92f) / 5f;
                Image star = UiFactory.CreateSpritePanel(root.transform, "Star" + (i + 1), i < value ? ShipSelectStarFullResourcePath : ShipSelectStarEmptyResourcePath, Color.white, new Vector2(x0, 0.08f), new Vector2(x1, 0.92f));
                star.raycastTarget = false;
                star.preserveAspect = true;
            }
        }

        private static Button CreateShipSelectActionButton(Transform parent, string label, string resourcePath, Vector2 anchoredPosition)
        {
            Button button = UiFactory.CreateSpriteButton(parent, label, resourcePath, Color.white, new Vector2(360f, 82f), anchoredPosition);
            Text text = button.GetComponentInChildren<Text>();
            text.fontSize = 34;
            text.resizeTextMinSize = 24;
            text.resizeTextMaxSize = 34;
            text.fontStyle = FontStyle.Bold;
            return button;
        }

        private static void ConfigureShipPreview(Image image, PlayerShipType shipType, int rank)
        {
            image.sprite = RuntimeSpriteFactory.GetPlayerShipSprite(shipType);
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            image.rectTransform.sizeDelta = ShipPreviewSize;
            image.rectTransform.anchoredPosition = new Vector2(rank % 2 == 0 ? -8f : 8f, rank == 5 ? -10f : 0f);
            image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rank == 3 ? -8f : rank == 4 ? 7f : rank == 5 ? -14f : 0f);
        }

        private void ShowDifficulty()
        {
            state = MenuUiState.Difficulty;
            Image background = CreateBackground("DifficultyBackground");
            Transform t = background.transform;
            UiFactory.CreateArcadeLabel(t, "选择难度", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            UiFactory.CreateSpritePanel(t, "DiffSprite", "UI/panel/SelectPanel01", new Color(1f, 1f, 1f, 0.10f), new Vector2(0.06f, 0.14f), new Vector2(0.94f, 0.82f));
            CreateDifficultyCard(background.transform, GameDifficulty.Low, "简单", "初始生命 5 / 敌弹较慢 / 适合长流程验收", ArcadeTheme.MilitaryGreen, new Vector2(0.08f, 0.62f), new Vector2(0.92f, 0.78f));
            CreateDifficultyCard(background.transform, GameDifficulty.Medium, "普通", "初始生命 3 / 标准弹幕 / 推荐体验", ArcadeTheme.ElectricBlue, new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.58f));
            CreateDifficultyCard(background.transform, GameDifficulty.High, "困难", "初始生命 2 / 强化火力 / 高压挑战", ArcadeTheme.WarningRed, new Vector2(0.08f, 0.22f), new Vector2(0.92f, 0.38f));
            Button back = UiFactory.CreatePixelButton(background.transform, "返回选机", ArcadeTheme.DimGray, new Vector2(360f, 78f), new Vector2(0f, -780f));
            back.onClick.AddListener(ShowShipSelect);
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
        }

        private void CreateDifficultyCard(Transform parent, GameDifficulty difficulty, string title, string desc, Color edge, Vector2 anchorMin, Vector2 anchorMax)
        {
            Image card = UiFactory.CreateSpritePanel(parent, title + "Card", "UI/panel/MainPanel03", new Color(1f, 1f, 1f, 0.15f), anchorMin, anchorMax);
            UiFactory.CreateArcadeLabel(card.transform, title, ArcadeTheme.TitleSize, TextAnchor.MiddleLeft, edge, FontStyle.Bold, new Vector2(0.08f, 0.5f), new Vector2(0.34f, 0.86f), Vector2.zero);
            UiFactory.CreateArcadeLabel(card.transform, desc, ArcadeTheme.BodySize, TextAnchor.MiddleLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.16f), new Vector2(0.68f, 0.5f), Vector2.zero);
            Button start = UiFactory.CreatePixelButton(card.transform, "出击", edge, new Vector2(180f, 72f), new Vector2(306f, -2f));
            start.onClick.AddListener(() => SceneNavigator.LoadGame(difficulty));
        }

        private void ShowLeaderboard()
        {
            state = MenuUiState.Leaderboard;
            Image background = CreateBackground("LeaderboardBackground");
            Transform t = background.transform;
            Image titleFrame = UiFactory.CreateSpritePanel(t, "LeaderboardTitleFrame", GameOverBootstrap.VictoryTitleFrameResourcePath, Color.white, new Vector2(0.075f, 0.725f), new Vector2(0.925f, 0.86f));
            titleFrame.raycastTarget = false;
            Text titleShadow = UiFactory.CreateArcadeLabel(titleFrame.transform, "本地排行榜", 58, TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.80f), FontStyle.Bold, Vector2.zero, Vector2.one, new Vector2(4f, -5f));
            titleShadow.raycastTarget = false;
            Text title = UiFactory.CreateArcadeLabel(titleFrame.transform, "本地排行榜", 58, TextAnchor.MiddleCenter, ArcadeTheme.ElectricBlue, FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            title.raycastTarget = false;
            Outline titleOutline = title.gameObject.AddComponent<Outline>();
            titleOutline.effectColor = new Color(0.78f, 1f, 0.94f, 0.9f);
            titleOutline.effectDistance = new Vector2(2f, -2f);

            Image board = UiFactory.CreateSpritePanel(t, "LeaderboardPanel", GameOverBootstrap.VictorySupplyPanelFrameResourcePath, Color.white, new Vector2(0.045f, 0.18f), new Vector2(0.955f, 0.665f));
            board.raycastTarget = false;
            Image subtitle = UiFactory.CreateSpritePanel(t, "LeaderboardDifficultyPlate", GameOverBootstrap.VictoryStageFrameResourcePath, Color.white, new Vector2(0.20f, 0.655f), new Vector2(0.80f, 0.71f));
            subtitle.raycastTarget = false;
            UiFactory.CreateArcadeLabel(subtitle.transform, BuildDifficultySettingText(SessionState.SelectedDifficulty) + " / 单机记录", 22, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero).raycastTarget = false;

            CreateLeaderboardHeader(board.transform);
            LeaderboardEntry[] entries = SessionState.GetLeaderboardEntries(SessionState.SelectedDifficulty);
            if (entries.Length == 0)
            {
                Image empty = UiFactory.CreateSpritePanel(board.transform, "LeaderboardEmptyRow", LeaderboardRowFrameResourcePath, Color.white, new Vector2(0.10f, 0.43f), new Vector2(0.90f, 0.55f));
                empty.raycastTarget = false;
                UiFactory.CreateArcadeLabel(empty.transform, "暂无记录  出击后刷新榜单", 26, TextAnchor.MiddleCenter, new Color(0.72f, 0.92f, 1f), FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero).raycastTarget = false;
            }
            else
            {
                int count = Mathf.Min(entries.Length, 9);
                for (int i = 0; i < count; i++)
                {
                    CreateLeaderboardEntryRow(board.transform, entries[i], i);
                }
            }

            Button back = UiFactory.CreateSpriteButton(background.transform, "返回", GameOverBootstrap.VictoryNextButtonResourcePath, Color.white, new Vector2(360f, 82f), new Vector2(0f, -820f));
            back.onClick.AddListener(ShowTitle);
            ConfigureSettingsActionText(back, ArcadeTheme.White);
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
        }

        private static void CreateLeaderboardHeader(Transform parent)
        {
            Image header = UiFactory.CreateSpritePanel(parent, "LeaderboardHeaderRow", GameOverBootstrap.VictorySupplyRowFrameResourcePath, Color.white, new Vector2(0.07f, 0.86f), new Vector2(0.93f, 0.935f));
            header.raycastTarget = false;
            CreateLeaderboardCell(header.transform, "名次", 0.04f, 0.18f, ArcadeTheme.ElectricBlue, TextAnchor.MiddleCenter);
            CreateLeaderboardCell(header.transform, "名号", 0.20f, 0.38f, ArcadeTheme.ElectricBlue, TextAnchor.MiddleCenter);
            CreateLeaderboardCell(header.transform, "分数", 0.38f, 0.64f, ArcadeTheme.ElectricBlue, TextAnchor.MiddleCenter);
            CreateLeaderboardCell(header.transform, "难度", 0.64f, 0.80f, ArcadeTheme.ElectricBlue, TextAnchor.MiddleCenter);
            CreateLeaderboardCell(header.transform, "关卡", 0.80f, 0.96f, ArcadeTheme.ElectricBlue, TextAnchor.MiddleCenter);
        }

        private static void CreateLeaderboardEntryRow(Transform parent, LeaderboardEntry entry, int index)
        {
            float top = 0.835f - index * 0.082f;
            string rowResource = GameOverBootstrap.VictorySupplyRowFrameResourcePath;
            Image row = UiFactory.CreateSpritePanel(parent, "LeaderboardEntryRow" + index, rowResource, Color.white, new Vector2(0.07f, top - 0.064f), new Vector2(0.93f, top));
            if (index < 3)
            {
                Image topWash = UiFactory.CreatePanel(row.transform, "TopRankGoldWash", new Color(0.42f, 0.31f, 0.02f, 0.34f), Vector2.zero, Vector2.one);
                topWash.raycastTarget = false;
            }
            row.raycastTarget = false;
            Color primary = index < 3 ? ArcadeTheme.EnergyYellow : ArcadeTheme.White;
            string rank = (index + 1).ToString("00");
            CreateLeaderboardCell(row.transform, rank, 0.05f, 0.17f, primary, TextAnchor.MiddleCenter);
            if (index < 3)
            {
                string medal = index == 0 ? LeaderboardMedalGoldResourcePath : index == 1 ? LeaderboardMedalSilverResourcePath : LeaderboardMedalBronzeResourcePath;
                Image medalIcon = UiFactory.CreateSpritePanel(row.transform, "MedalIcon", medal, Color.white, new Vector2(0.005f, 0.12f), new Vector2(0.065f, 0.88f));
                medalIcon.preserveAspect = true;
                medalIcon.raycastTarget = false;
            }

            CreateLeaderboardCell(row.transform, entry.Name, 0.20f, 0.36f, new Color(0.80f, 1f, 1f), TextAnchor.MiddleCenter);
            CreateLeaderboardCell(row.transform, entry.Score.ToString("0000000"), 0.37f, 0.64f, primary, TextAnchor.MiddleCenter);
            CreateLeaderboardCell(row.transform, BuildDifficultyShort(entry.Difficulty), 0.66f, 0.78f, ArcadeTheme.ElectricBlue, TextAnchor.MiddleCenter);
            CreateLeaderboardCell(row.transform, entry.LoopNumber + "-" + entry.StageNumber, 0.80f, 0.95f, Color.white, TextAnchor.MiddleCenter);
        }

        private static void CreateLeaderboardCell(Transform parent, string text, float minX, float maxX, Color color, TextAnchor anchor)
        {
            Text label = UiFactory.CreateArcadeLabel(parent, text, 20, anchor, color, FontStyle.Bold, new Vector2(minX, 0f), new Vector2(maxX, 1f), Vector2.zero);
            label.raycastTarget = false;
            UiFactory.ConfigureSingleLine(label);
        }

        private void ShowSettings()
        {
            state = MenuUiState.Settings;
            Image background = CreateBackground("SettingsBackground");
            Transform t = background.transform;
            Text titleGlow = UiFactory.CreateArcadeLabel(t, "设置", 64, TextAnchor.MiddleCenter, new Color(0.36f, 1f, 1f, 0.44f), FontStyle.Bold, new Vector2(0.06f, 0.89f), new Vector2(0.94f, 0.965f), Vector2.zero);
            titleGlow.raycastTarget = false;
            Text title = UiFactory.CreateArcadeLabel(t, "设置", 58, TextAnchor.MiddleCenter, new Color(0.9f, 1f, 0.98f), FontStyle.Bold, new Vector2(0.06f, 0.89f), new Vector2(0.94f, 0.965f), Vector2.zero);
            title.raycastTarget = false;
            UiFactory.CreateDivider(t, "SettingsTitleWingLeft", new Color(0.28f, 0.96f, 1f, 0.64f), new Vector2(0.19f, 0.925f), new Vector2(0.37f, 0.93f));
            UiFactory.CreateDivider(t, "SettingsTitleWingRight", new Color(0.28f, 0.96f, 1f, 0.64f), new Vector2(0.63f, 0.925f), new Vector2(0.81f, 0.93f));

            Image panel = UiFactory.CreateSpritePanel(t, "SettingsPanel", SettingsPanelFrameResourcePath, Color.white, new Vector2(0.035f, 0.16f), new Vector2(0.965f, 0.885f));
            panel.raycastTarget = false;
            audioStatusText = CreateSettingsStatusRow(panel.transform, BuildAudioText(), SessionState.AudioEnabled);
            Button audioToggle = CreateSettingsToggle(panel.transform, "声音", SessionState.AudioEnabled, SettingsIconSoundResourcePath, new Vector2(0.64f, 0.845f), new Vector2(0.91f, 0.925f), () =>
            {
                SessionState.SetAudioEnabled(!SessionState.AudioEnabled);
                ShowSettings();
            });
            audioToggle.gameObject.name = "声音开启Toggle";

            CreateSettingsSlider(panel.transform, "音乐音量", SettingsIconMusicResourcePath, SessionState.MusicVolume, new Vector2(0.065f, 0.70f), new Vector2(0.935f, 0.79f), value =>
            {
                SessionState.SetMusicVolume(value);
                if (musicValueText != null) musicValueText.text = Mathf.RoundToInt(SessionState.MusicVolume * 100f) + "%";
            }, out musicValueText);
            CreateSettingsSlider(panel.transform, "音效音量", SettingsIconSfxResourcePath, SessionState.SfxVolume, new Vector2(0.065f, 0.595f), new Vector2(0.935f, 0.685f), value =>
            {
                SessionState.SetSfxVolume(value);
                if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(SessionState.SfxVolume * 100f) + "%";
            }, out sfxValueText);

            Button defaultShipButton = CreateSettingsOptionButton(panel.transform, "默认战机 " + BuildDefaultShipText(), SettingsIconShipResourcePath, false, new Vector2(0.065f, 0.455f), new Vector2(0.485f, 0.555f), RuntimeSpriteFactory.GetPlayerShipSprite(SessionState.SelectedShip));
            defaultShipButton.onClick.AddListener(CycleDefaultShip);
            Button defaultDifficultyButton = CreateSettingsOptionButton(panel.transform, "默认难度 " + BuildDifficultySettingText(SessionState.SelectedDifficulty), SettingsIconDifficultyResourcePath, true, new Vector2(0.515f, 0.455f), new Vector2(0.935f, 0.555f));
            defaultDifficultyButton.onClick.AddListener(CycleDefaultDifficulty);

            Button sensitivityButton = CreateSettingsOptionButton(panel.transform, "灵敏度 " + BuildSensitivityText(), SettingsIconSensitivityResourcePath, false, new Vector2(0.065f, 0.34f), new Vector2(0.485f, 0.44f));
            sensitivityButton.onClick.AddListener(CycleSensitivity);
            Button nameButton = CreateSettingsOptionButton(panel.transform, "榜名 " + SessionState.LeaderboardName, SettingsIconRankResourcePath, false, new Vector2(0.515f, 0.34f), new Vector2(0.935f, 0.44f));
            nameButton.onClick.AddListener(CycleLeaderboardName);
            Button vibrationButton = CreateSettingsOptionButton(panel.transform, "震动 " + (SessionState.VibrationEnabled ? "开" : "关"), SettingsIconVibrationResourcePath, false, new Vector2(0.065f, 0.225f), new Vector2(0.485f, 0.325f));
            vibrationButton.onClick.AddListener(() =>
            {
                SessionState.SetVibrationEnabled(!SessionState.VibrationEnabled);
                ShowSettings();
            });
            Button damageButton = CreateSettingsOptionButton(panel.transform, "伤害数字 " + (SessionState.DamageNumbersEnabled ? "开" : "关"), SettingsIconDamageResourcePath, false, new Vector2(0.515f, 0.225f), new Vector2(0.935f, 0.325f));
            damageButton.onClick.AddListener(() =>
            {
                SessionState.SetDamageNumbersEnabled(!SessionState.DamageNumbersEnabled);
                ShowSettings();
            });
            Button effectsButton = CreateSettingsOptionButton(panel.transform, "特效质量 " + BuildVisualEffectsQualityText(), SettingsIconEffectsResourcePath, false, new Vector2(0.065f, 0.11f), new Vector2(0.485f, 0.21f));
            effectsButton.onClick.AddListener(CycleVisualEffectsQuality);
            Button resetButton = CreateSettingsOptionButton(panel.transform, "恢复默认", SettingsIconResetResourcePath, false, new Vector2(0.515f, 0.11f), new Vector2(0.935f, 0.21f));
            resetButton.onClick.AddListener(() =>
            {
                SessionState.ResetSettings();
                ShowSettings();
            });

            saveStatusText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 22, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.2f, 0.035f), new Vector2(0.8f, 0.085f), Vector2.zero);
            saveStatusText.gameObject.SetActive(false);

            Button back = UiFactory.CreateSpriteButton(background.transform, "返回标题", SettingsButtonBackResourcePath, Color.white, new Vector2(360f, 82f), new Vector2(-235f, -820f));
            back.onClick.AddListener(ShowTitle);
            ConfigureSettingsActionText(back, ArcadeTheme.White);
            Button save = UiFactory.CreateSpriteButton(background.transform, "保存设置", SettingsButtonSaveResourcePath, Color.white, new Vector2(360f, 82f), new Vector2(235f, -820f));
            save.onClick.AddListener(ShowAutoSaveStatus);
            ConfigureSettingsActionText(save, ArcadeTheme.White);
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
        }

        private static Text CreateSettingsStatusRow(Transform parent, string label, bool enabled)
        {
            Image icon = UiFactory.CreateSpritePanel(parent, "SettingsSoundIcon", SettingsIconSoundResourcePath, Color.white, new Vector2(0.11f, 0.825f), new Vector2(0.22f, 0.935f));
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Text text = UiFactory.CreateArcadeLabel(parent, label, 32, TextAnchor.MiddleLeft, enabled ? ArcadeTheme.EnergyYellow : ArcadeTheme.DimGray, FontStyle.Bold, new Vector2(0.25f, 0.845f), new Vector2(0.58f, 0.92f), Vector2.zero);
            UiFactory.ConfigureSingleLine(text);
            text.raycastTarget = false;
            return text;
        }

        private static void CreateSettingsSlider(Transform parent, string label, string iconPath, float value, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction<float> onChanged, out Text valueText)
        {
            Image row = UiFactory.CreateSpritePanel(parent, label + "Row", SettingsOptionFrameResourcePath, Color.white, anchorMin, anchorMax);
            row.raycastTarget = false;
            Image icon = UiFactory.CreateSpritePanel(row.transform, label + "Icon", iconPath, Color.white, new Vector2(0.035f, 0.18f), new Vector2(0.13f, 0.82f));
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            UiFactory.CreateArcadeLabel(row.transform, label, 19, TextAnchor.MiddleLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.14f, 0f), new Vector2(0.34f, 1f), Vector2.zero).raycastTarget = false;
            valueText = UiFactory.CreateArcadeLabel(row.transform, Mathf.RoundToInt(value * 100f) + "%", 18, TextAnchor.MiddleRight, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.82f, 0f), new Vector2(0.96f, 1f), Vector2.zero);
            valueText.raycastTarget = false;

            GameObject sliderObject = new GameObject(label + "Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(row.transform, false);
            RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.36f, 0.25f);
            sliderRect.anchorMax = new Vector2(0.80f, 0.75f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            Image track = UiFactory.CreateSpritePanel(sliderObject.transform, "Track", SettingsSliderTrackResourcePath, Color.white, Vector2.zero, Vector2.one);
            track.raycastTarget = false;
            Image fill = UiFactory.CreateSpritePanel(track.transform, "Fill", SettingsSliderFillResourcePath, Color.white, Vector2.zero, Vector2.one);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = Mathf.Clamp01(value);
            Image handle = UiFactory.CreatePanel(sliderObject.transform, "Handle", ArcadeTheme.EnergyYellow, new Vector2(0f, 0.02f), new Vector2(0.08f, 0.98f));
            handle.sprite = RuntimeSpriteFactory.GetCircleSprite();

            Slider slider = sliderObject.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.onValueChanged.AddListener(onChanged);
        }

        private static Button CreateSettingsToggle(Transform parent, string label, bool enabled, string iconPath, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
        {
            Button button = UiFactory.CreateButton(parent, enabled ? "ON" : "OFF", Color.clear, Color.white, Vector2.zero, Vector2.zero, anchorMin, anchorMax);
            Image image = button.GetComponent<Image>();
            image.sprite = null;
            image.color = new Color(1f, 1f, 1f, 0.001f);
            image.type = Image.Type.Simple;
            button.targetGraphic = image;
            Image frame = UiFactory.CreateSpritePanel(button.transform, label + "ToggleFrame", enabled ? SettingsToggleOnResourcePath : SettingsToggleOffResourcePath, Color.white, Vector2.zero, Vector2.one);
            frame.raycastTarget = false;
            Text text = button.GetComponentInChildren<Text>();
            text.fontSize = 30;
            text.resizeTextMinSize = 22;
            text.resizeTextMaxSize = 30;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.rectTransform.anchorMin = new Vector2(0.05f, 0f);
            text.rectTransform.anchorMax = new Vector2(0.62f, 1f);
            text.raycastTarget = false;
            button.onClick.AddListener(onClick);
            return button;
        }

        private static Button CreateSettingsOptionButton(Transform parent, string label, string iconPath, bool gold, Vector2 anchorMin, Vector2 anchorMax, Sprite iconSprite = null)
        {
            Button button = UiFactory.CreateButton(parent, string.Empty, Color.clear, Color.white, Vector2.zero, Vector2.zero, anchorMin, anchorMax);
            Image image = button.GetComponent<Image>();
            image.sprite = null;
            image.color = new Color(1f, 1f, 1f, 0.001f);
            image.type = Image.Type.Simple;
            button.targetGraphic = image;
            Image frame = UiFactory.CreateSpritePanel(button.transform, "OptionFrame", gold ? SettingsOptionGoldFrameResourcePath : SettingsOptionFrameResourcePath, Color.white, Vector2.zero, Vector2.one);
            frame.raycastTarget = false;
            bool wideIcon = iconSprite != null;
            Image icon = UiFactory.CreatePanel(button.transform, "OptionIcon", Color.white, new Vector2(0.045f, 0.12f), new Vector2(wideIcon ? 0.34f : 0.28f, 0.88f));
            icon.sprite = iconSprite != null ? iconSprite : LoadTitleIcon(iconPath);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Text emptyText = button.GetComponentInChildren<Text>();
            if (emptyText != null)
            {
                emptyText.raycastTarget = false;
            }

            int split = label.IndexOf(' ');
            float textMinX = wideIcon ? 0.38f : 0.34f;
            if (split > 0)
            {
                Text title = UiFactory.CreateArcadeLabel(button.transform, label.Substring(0, split), 21, TextAnchor.LowerCenter, Color.white, FontStyle.Bold, new Vector2(textMinX, 0.47f), new Vector2(0.94f, 0.92f), Vector2.zero);
                title.raycastTarget = false;
                UiFactory.ConfigureSingleLine(title);
                Text value = UiFactory.CreateArcadeLabel(button.transform, label.Substring(split + 1), 27, TextAnchor.UpperCenter, new Color(0.42f, 1f, 1f), FontStyle.Bold, new Vector2(textMinX, 0.08f), new Vector2(0.94f, 0.52f), Vector2.zero);
                value.raycastTarget = false;
                UiFactory.ConfigureSingleLine(value);
            }
            else
            {
                Text title = UiFactory.CreateArcadeLabel(button.transform, label, 26, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold, new Vector2(0.28f, 0f), new Vector2(0.96f, 1f), Vector2.zero);
                title.raycastTarget = false;
                UiFactory.ConfigureSingleLine(title);
            }
            return button;
        }

        private static void ConfigureSettingsActionText(Button button, Color color)
        {
            Text text = button.GetComponentInChildren<Text>();
            text.fontSize = 32;
            text.resizeTextMinSize = 22;
            text.resizeTextMaxSize = 32;
            text.fontStyle = FontStyle.Bold;
            text.color = color;
            text.raycastTarget = false;
        }

        private static string Stars(int count)
        {
            return new string('■', Mathf.Clamp(count, 1, 5)).PadRight(5, '□');
        }

        private static string BuildStarString(int count)
        {
            return new string('★', Mathf.Clamp(count, 1, 5)).PadRight(5, '☆');
        }

        private static string BuildDifficultyShort(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Medium:
                    return "普";
                case GameDifficulty.High:
                    return "难";
                default:
                    return "易";
            }
        }

        private static string BuildDifficultySettingText(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Low:
                    return "简单";
                case GameDifficulty.High:
                    return "困难";
                default:
                    return "普通";
            }
        }

        private static string BuildDefaultShipText()
        {
            return ShipDefinition.Get(SessionState.SelectedShip).DisplayName;
        }

        private string BuildAudioText()
        {
            return SessionState.AudioEnabled ? "声音开启" : "声音关闭";
        }

        private void ShowAutoSaveStatus()
        {
            PlayerPrefs.Save();
            saveStatusTimer = 2.2f;
            if (saveStatusText != null)
            {
                saveStatusText.text = "已自动保存";
                saveStatusText.gameObject.SetActive(true);
            }
        }

        private void RefreshSaveStatus()
        {
            if (saveStatusText == null || saveStatusTimer <= 0f)
            {
                return;
            }

            saveStatusTimer = Mathf.Max(0f, saveStatusTimer - Time.unscaledDeltaTime);
            if (saveStatusTimer <= 0f)
            {
                saveStatusText.gameObject.SetActive(false);
            }
        }

        private string BuildSensitivityText()
        {
            switch (SessionState.ControlSensitivity)
            {
                case ControlSensitivity.Low:
                    return "低";
                case ControlSensitivity.High:
                    return "高";
                default:
                    return "中";
            }
        }

        private string BuildVisualEffectsQualityText()
        {
            return SessionState.VisualEffectsQuality == VisualEffectsQuality.BatterySaver ? "省电" : "完整";
        }

        private void CycleVisualEffectsQuality()
        {
            VisualEffectsQuality next = SessionState.VisualEffectsQuality == VisualEffectsQuality.Full
                ? VisualEffectsQuality.BatterySaver
                : VisualEffectsQuality.Full;
            SessionState.SetVisualEffectsQuality(next);
            ShowSettings();
        }

        private void CycleSensitivity()
        {
            ControlSensitivity next = SessionState.ControlSensitivity == ControlSensitivity.Low
                ? ControlSensitivity.Medium
                : SessionState.ControlSensitivity == ControlSensitivity.Medium ? ControlSensitivity.High : ControlSensitivity.Low;
            SessionState.SetControlSensitivity(next);
            ShowSettings();
        }

        private void CycleDefaultShip()
        {
            PlayerShipType current = SessionState.SelectedShip;
            int currentIndex = System.Array.IndexOf(ShipSelectRoster, current);
            PlayerShipType next = ShipSelectRoster[(currentIndex + 1 + ShipSelectRoster.Length) % ShipSelectRoster.Length];
            selectedShip = next;
            SessionState.SelectShip(next);
            ShowSettings();
        }

        private void CycleDefaultDifficulty()
        {
            GameDifficulty next;
            switch (SessionState.SelectedDifficulty)
            {
                case GameDifficulty.Low:
                    next = GameDifficulty.Medium;
                    break;
                case GameDifficulty.Medium:
                    next = GameDifficulty.High;
                    break;
                default:
                    next = GameDifficulty.Low;
                    break;
            }

            SessionState.SelectDifficulty(next);
            ShowSettings();
        }

        private void CycleLeaderboardName()
        {
            string current = SessionState.LeaderboardName;
            char[] chars = current.ToCharArray();
            int value = chars[2] - 'A';
            chars[2] = (char)('A' + ((value + 1) % 26));
            SessionState.SetLeaderboardName(new string(chars));
            ShowSettings();
        }
    }
}
