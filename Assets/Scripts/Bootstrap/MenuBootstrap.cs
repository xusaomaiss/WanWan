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
        public static readonly string[] TitlePrimaryLabels = { "开始游戏", "退出" };
        public static readonly Vector2 ModernTitleButtonSize = new Vector2(486f, 96f);
        public static readonly Vector2 ModernTitleIconSize = new Vector2(112f, 112f);
        public static readonly string[] StartScreenHitAreaNames = { "ArcadeStart", "ClassicExit", "Settings", "Leaderboard", "ShipSelect" };
        public static readonly Vector2 TitleStartButtonSliceAnchorMin = new Vector2(0.2232f, 0.2572f);
        public static readonly Vector2 TitleStartButtonSliceAnchorMax = new Vector2(0.7779f, 0.3481f);
        public static readonly Vector2 TitleExitButtonSliceAnchorMin = new Vector2(0.2232f, 0.1507f);
        public static readonly Vector2 TitleExitButtonSliceAnchorMax = new Vector2(0.7779f, 0.2416f);
        public static readonly Vector2 TitleSettingsSliceAnchorMin = new Vector2(0.0797f, 0.0287f);
        public static readonly Vector2 TitleSettingsSliceAnchorMax = new Vector2(0.2922f, 0.1477f);
        public static readonly Vector2 TitleLeaderboardSliceAnchorMin = new Vector2(0.3773f, 0.0383f);
        public static readonly Vector2 TitleLeaderboardSliceAnchorMax = new Vector2(0.6270f, 0.1316f);
        public static readonly Vector2 TitleShipSelectSliceAnchorMin = new Vector2(0.7279f, 0.0287f);
        public static readonly Vector2 TitleShipSelectSliceAnchorMax = new Vector2(0.9405f, 0.1477f);
        public static readonly string[] RightSideMenuLabels = { "CONTROL", "ACHIEVEMENT", "SHOP", "HELP" };
        public static readonly PlayerShipType[] ShipSelectRoster =
        {
            PlayerShipType.Green,
            PlayerShipType.Blue,
            PlayerShipType.Yellow,
            PlayerShipType.Purple,
            PlayerShipType.Azure
        };

        private static readonly Vector2 ShipPreviewSize = new Vector2(205f, 168f);

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

            Sprite backgroundSprite = state == MenuUiState.Title ? LoadTitleBackgroundSprite() : RuntimeSpriteFactory.GetSkyBackgroundSprite();

            backgroundImage = UiFactory.CreatePanel(root.transform, name + "BG", Color.white, Vector2.zero, Vector2.one);
            backgroundImage.sprite = backgroundSprite;
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.preserveAspect = false;
            backgroundImage.raycastTarget = false;

            if (state == MenuUiState.Title)
            {
                return root;
            }

            Image shade = UiFactory.CreatePanel(root.transform, name + "Shade", new Color(0.01f, 0.02f, 0.05f, 0.34f), Vector2.zero, Vector2.one);
            shade.raycastTarget = false;

            Image topVignette = UiFactory.CreatePanel(root.transform, name + "TopVignette", new Color(0f, 0f, 0f, 0.18f), new Vector2(0f, 0.72f), Vector2.one);
            topVignette.raycastTarget = false;

            Image bottomVignette = UiFactory.CreatePanel(root.transform, name + "BottomVignette", new Color(0f, 0f, 0f, 0.58f), Vector2.zero, new Vector2(1f, 0.42f));
            bottomVignette.raycastTarget = false;

            CreateScanlines(root.transform);

            return root;
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
            Image frame = UiFactory.CreatePanel(t, "ShipSelectFrame", new Color(0.04f, 0.36f, 0.28f, 0.96f), new Vector2(0.035f, 0.035f), new Vector2(0.965f, 0.965f));
            frame.raycastTarget = false;
            Image inner = UiFactory.CreatePanel(frame.transform, "ShipSelectInner", new Color(0.02f, 0.18f, 0.14f, 0.88f), new Vector2(0.03f, 0.025f), new Vector2(0.97f, 0.975f));
            inner.raycastTarget = false;
            UiFactory.CreateDivider(frame.transform, "ShipSelectTopEdge", new Color(0.38f, 0.92f, 0.78f, 0.6f), new Vector2(0.02f, 0.965f), new Vector2(0.98f, 0.972f));
            UiFactory.CreateDivider(frame.transform, "ShipSelectBottomEdge", new Color(0.38f, 0.92f, 0.78f, 0.45f), new Vector2(0.02f, 0.028f), new Vector2(0.98f, 0.035f));

            UiFactory.CreateArcadeLabel(t, "FIGHTER SELECT", 34, TextAnchor.MiddleCenter, new Color(0.86f, 1f, 0.93f, 0.9f), FontStyle.Bold, new Vector2(0.08f, 0.90f), new Vector2(0.92f, 0.95f), Vector2.zero);
            for (int i = 0; i < ShipSelectRoster.Length; i++)
            {
                const float top = 0.865f;
                const float rowHeight = 0.152f;
                const float gap = 0.017f;
                float rowTop = top - (i * (rowHeight + gap));
                CreateShipCard(background.transform, ShipSelectRoster[i], i + 1, new Vector2(0.08f, rowTop - rowHeight), new Vector2(0.92f, rowTop));
            }

            Button back = UiFactory.CreatePixelButton(background.transform, "返回", ArcadeTheme.DimGray, new Vector2(220f, 64f), new Vector2(-340f, -846f));
            back.onClick.AddListener(ShowTitle);
            Button next = UiFactory.CreatePixelButton(background.transform, "确认", ArcadeTheme.EnergyYellow, new Vector2(220f, 64f), new Vector2(340f, -846f));
            next.onClick.AddListener(() =>
            {
                SessionState.SelectShip(selectedShip);
                ShowDifficulty();
            });
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
        }

        private void CreateShipCard(Transform parent, PlayerShipType shipType, int rank, Vector2 anchorMin, Vector2 anchorMax)
        {
            ShipDefinition ship = ShipDefinition.Get(shipType);
            bool selected = selectedShip == shipType;
            Color edge = selected ? ArcadeTheme.EnergyYellow : new Color(0.32f, 0.9f, 0.76f, 0.68f);
            Button cardButton = UiFactory.CreateButton(parent, string.Empty, new Color(0.04f, 0.32f, 0.26f, selected ? 0.96f : 0.78f), Color.clear, Vector2.zero, Vector2.zero, anchorMin, anchorMax);
            cardButton.gameObject.name = ship.DisplayName + "ShipRow";
            Image card = cardButton.GetComponent<Image>();
            card.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            card.type = Image.Type.Sliced;
            card.raycastTarget = true;
            RectTransform cardRect = card.rectTransform;
            cardRect.offsetMin = Vector2.zero;
            cardRect.offsetMax = Vector2.zero;
            Text emptyLabel = cardButton.GetComponentInChildren<Text>();
            if (emptyLabel != null)
            {
                emptyLabel.raycastTarget = false;
            }

            Image outline = UiFactory.CreatePanel(card.transform, "RowOutline", edge, Vector2.zero, Vector2.one);
            outline.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            outline.type = Image.Type.Sliced;
            outline.raycastTarget = false;
            outline.rectTransform.offsetMin = new Vector2(2f, 2f);
            outline.rectTransform.offsetMax = new Vector2(-2f, -2f);
            Image fill = UiFactory.CreatePanel(outline.transform, "RowFill", new Color(0.03f, 0.28f, 0.22f, selected ? 0.84f : 0.64f), Vector2.zero, Vector2.one);
            fill.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            fill.type = Image.Type.Sliced;
            fill.raycastTarget = false;
            fill.rectTransform.offsetMin = new Vector2(4f, 4f);
            fill.rectTransform.offsetMax = new Vector2(-4f, -4f);

            CreateRankBadge(card.transform, rank, selected, ship.AccentColor);
            CreateSelectionChevron(card.transform, rank, selected, ship.AccentColor);

            Image shipPanel = UiFactory.CreatePanel(card.transform, "ShipPanel", new Color(0.02f, 0.18f, 0.16f, 0.94f), new Vector2(0.20f, 0.08f), new Vector2(0.42f, 0.92f));
            shipPanel.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
            shipPanel.type = Image.Type.Sliced;
            shipPanel.raycastTarget = false;
            UiFactory.CreateDivider(shipPanel.transform, "ShipPanelGlowTop", new Color(0.3f, 1f, 0.82f, 0.42f), new Vector2(0.02f, 0.96f), new Vector2(0.98f, 0.985f));
            Image shipImage = UiFactory.CreatePanel(shipPanel.transform, "ShipImage", Color.white, Vector2.zero, Vector2.one);
            ConfigureShipPreview(shipImage, ship.AccentColor, rank);

            Text name = UiFactory.CreateArcadeLabel(card.transform, ship.DisplayName, 43, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold, new Vector2(0.47f, 0.46f), new Vector2(0.67f, 0.84f), Vector2.zero);
            UiFactory.ConfigureConstrainedText(name, 24, 43);
            Text attack = UiFactory.CreateArcadeLabel(card.transform, ship.MainWeapon, 24, TextAnchor.MiddleLeft, new Color(0.86f, 1f, 0.92f), FontStyle.Bold, new Vector2(0.47f, 0.19f), new Vector2(0.67f, 0.46f), Vector2.zero);
            UiFactory.ConfigureSingleLine(attack);

            Text stars = UiFactory.CreateArcadeLabel(card.transform, BuildStarString(ship.PowerStars), 30, TextAnchor.MiddleRight, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.77f, 0.70f), new Vector2(0.97f, 0.92f), Vector2.zero);
            UiFactory.ConfigureSingleLine(stars);

            CreateStatBar(card.transform, "POWER", ship.PowerStars, new Vector2(0.68f, 0.54f), new Vector2(0.96f, 0.66f));
            CreateStatBar(card.transform, "ATTACK", ship.AttackStars, new Vector2(0.68f, 0.39f), new Vector2(0.96f, 0.51f));
            CreateStatBar(card.transform, "DEFENCE", ship.DefenseStars, new Vector2(0.68f, 0.24f), new Vector2(0.96f, 0.36f));
            CreateStatBar(card.transform, "SPEED", ship.SpeedStars, new Vector2(0.68f, 0.09f), new Vector2(0.96f, 0.21f));

            cardButton.onClick.AddListener(() =>
            {
                selectedShip = shipType;
                ShowShipSelect();
            });
        }

        private static void CreateRankBadge(Transform parent, int rank, bool selected, Color accent)
        {
            Image outer = UiFactory.CreatePanel(parent, "RankBadgeOuter", selected ? ArcadeTheme.EnergyYellow : Color.Lerp(accent, ArcadeTheme.DimGray, 0.45f), new Vector2(0.045f, 0.18f), new Vector2(0.15f, 0.82f));
            outer.sprite = RuntimeSpriteFactory.GetCircleSprite();
            outer.raycastTarget = false;
            Image inner = UiFactory.CreatePanel(outer.transform, "RankBadgeInner", new Color(0.03f, 0.08f, 0.08f, 0.94f), new Vector2(0.16f, 0.16f), new Vector2(0.84f, 0.84f));
            inner.sprite = RuntimeSpriteFactory.GetCircleSprite();
            inner.raycastTarget = false;
            Text number = UiFactory.CreateArcadeLabel(outer.transform, rank.ToString(), 48, TextAnchor.MiddleCenter, selected ? ArcadeTheme.EnergyYellow : Color.white, FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            number.raycastTarget = false;
            UiFactory.ConfigureSingleLine(number);
        }

        private static void CreateSelectionChevron(Transform parent, int rank, bool selected, Color accent)
        {
            Color color = selected ? new Color(0.54f, 1f, 0.44f, 0.95f) : (rank < 4 ? new Color(1f, 0.28f, 0.2f, 0.86f) : new Color(0.5f, 1f, 1f, 0.76f));
            Text chevron = UiFactory.CreateArcadeLabel(parent, selected ? "▲" : (rank < 4 ? "▼" : "◆"), 36, TextAnchor.MiddleCenter, Color.Lerp(color, accent, selected ? 0.12f : 0f), FontStyle.Bold, new Vector2(0.03f, 0.66f), new Vector2(0.085f, 0.92f), Vector2.zero);
            chevron.raycastTarget = false;
        }

        private static void CreateStatBar(Transform parent, string label, int value, Vector2 anchorMin, Vector2 anchorMax)
        {
            UiFactory.CreateArcadeLabel(parent, label, 17, TextAnchor.MiddleLeft, new Color(0.86f, 1f, 0.92f), FontStyle.Bold, anchorMin, new Vector2(anchorMin.x + 0.095f, anchorMax.y), Vector2.zero).raycastTarget = false;
            Image track = UiFactory.CreatePanel(parent, label + "Track", new Color(0.92f, 0.98f, 1f, 0.92f), new Vector2(anchorMin.x + 0.11f, anchorMin.y + 0.018f), anchorMax);
            track.raycastTarget = false;
            float fill = Mathf.Clamp01(value / 5f);
            Image bar = UiFactory.CreatePanel(track.transform, label + "Fill", new Color(0.46f, 0.88f, 0.32f, 0.98f), Vector2.zero, new Vector2(fill, 1f));
            bar.raycastTarget = false;
            Image cap = UiFactory.CreatePanel(track.transform, label + "Cap", new Color(0.75f, 1f, 0.66f, 0.95f), new Vector2(Mathf.Max(0f, fill - 0.035f), 0f), new Vector2(fill, 1f));
            cap.raycastTarget = false;
        }

        private static void ConfigureShipPreview(Image image, Color tint, int rank)
        {
            image.sprite = RuntimeSpriteFactory.GetRaidenFighterJetSprite();
            image.color = Color.Lerp(Color.white, tint, 0.62f);
            image.preserveAspect = true;
            image.raycastTarget = false;
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
            UiFactory.CreateArcadeLabel(t, "本地排行榜", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            UiFactory.CreateArcadeLabel(t, BuildDifficultySettingText(SessionState.SelectedDifficulty) + " / 单机记录", ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.1f, 0.81f), new Vector2(0.9f, 0.86f), Vector2.zero);
            // Decorative panel background
            UiFactory.CreateSpritePanel(t, "LeaderboardSprite", "UI/panel/MainPanel01", new Color(1f, 1f, 1f, 0.15f), new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f));
            Image board = UiFactory.CreatePixelPanel(t, "LeaderboardPanel", new Color(0.08f, 0.08f, 0.16f, 0.96f), ArcadeTheme.EnergyYellow, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f), new Vector2(8f, 8f));
            UiFactory.CreateArcadeLabel(board.transform, "名次  名号   分数      难度  关卡", ArcadeTheme.BodySize, TextAnchor.UpperLeft, ArcadeTheme.ElectricBlue, FontStyle.Bold, new Vector2(0.08f, 0.9f), new Vector2(0.92f, 0.98f), Vector2.zero);
            LeaderboardEntry[] entries = SessionState.GetLeaderboardEntries(SessionState.SelectedDifficulty);
            if (entries.Length == 0)
            {
                UiFactory.CreateArcadeLabel(board.transform, "暂无记录\n出击后刷新榜单", ArcadeTheme.TitleSize, TextAnchor.MiddleCenter, ArcadeTheme.DimGray, FontStyle.Bold, new Vector2(0.1f, 0.35f), new Vector2(0.9f, 0.62f), Vector2.zero);
            }
            else
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    LeaderboardEntry entry = entries[i];
                    Color color = i < 3 ? ArcadeTheme.EnergyYellow : ArcadeTheme.White;
                    string row = $"{i + 1:00}  {entry.Name,-3}  {entry.Score:0000000}  {BuildDifficultyShort(entry.Difficulty),-2}   {entry.LoopNumber}-{entry.StageNumber}";
                    float top = 0.84f - i * 0.075f;
                    UiFactory.CreateArcadeLabel(board.transform, row, ArcadeTheme.BodySize, TextAnchor.MiddleLeft, color, FontStyle.Bold, new Vector2(0.08f, top - 0.055f), new Vector2(0.92f, top), Vector2.zero);
                }
            }

            Button back = UiFactory.CreatePixelButton(background.transform, "返回标题", ArcadeTheme.DimGray, new Vector2(360f, 78f), new Vector2(0f, -780f));
            back.onClick.AddListener(ShowTitle);
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
        }

        private void ShowSettings()
        {
            state = MenuUiState.Settings;
            Image background = CreateBackground("SettingsBackground");
            Transform t = background.transform;
            UiFactory.CreateArcadeLabel(t, "设置", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.06f, 0.86f), new Vector2(0.94f, 0.94f), Vector2.zero);
            // Decorative panel background
            UiFactory.CreateSpritePanel(t, "SettingsPanelSprite", "UI/panel/MainPanel01", new Color(1f, 1f, 1f, 0.15f), new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f));
            Image panel = UiFactory.CreatePixelPanel(t, "SettingsPanel", new Color(0.08f, 0.08f, 0.16f, 0.96f), ArcadeTheme.ElectricBlue, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.82f), new Vector2(8f, 8f));
            audioStatusText = UiFactory.CreateArcadeLabel(panel.transform, BuildAudioText(), ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.12f, 0.86f), new Vector2(0.88f, 0.96f), Vector2.zero);

            Button audioButton = UiFactory.CreatePixelToggle(panel.transform, "总声音", SessionState.AudioEnabled, new Vector2(0f, 260f), () =>
            {
                SessionState.SetAudioEnabled(!SessionState.AudioEnabled);
                ShowSettings();
            });

            UiFactory.CreatePixelSlider(panel.transform, "音乐音量", SessionState.MusicVolume, new Vector2(0.08f, 0.58f), new Vector2(0.92f, 0.68f), value =>
            {
                SessionState.SetMusicVolume(value);
                if (musicValueText != null) musicValueText.text = Mathf.RoundToInt(SessionState.MusicVolume * 100f) + "%";
            }, out musicValueText);
            UiFactory.CreatePixelSlider(panel.transform, "音效音量", SessionState.SfxVolume, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.55f), value =>
            {
                SessionState.SetSfxVolume(value);
                if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(SessionState.SfxVolume * 100f) + "%";
            }, out sfxValueText);
            UiFactory.CreatePixelSlider(panel.transform, "按键透明", SessionState.VirtualButtonOpacity, new Vector2(0.08f, 0.32f), new Vector2(0.92f, 0.42f), value =>
            {
                SessionState.SetVirtualButtonOpacity(value);
                if (opacityValueText != null) opacityValueText.text = Mathf.RoundToInt(SessionState.VirtualButtonOpacity * 100f) + "%";
            }, out opacityValueText);

            Button defaultShipButton = UiFactory.CreatePixelButton(panel.transform, "默认战机 " + BuildDefaultShipText(), ArcadeTheme.EnergyYellow, new Vector2(420f, 72f), new Vector2(-220f, -136f));
            defaultShipButton.onClick.AddListener(CycleDefaultShip);
            Button defaultDifficultyButton = UiFactory.CreatePixelButton(panel.transform, "默认难度 " + BuildDifficultySettingText(SessionState.SelectedDifficulty), ArcadeTheme.ElectricBlue, new Vector2(420f, 72f), new Vector2(220f, -136f));
            defaultDifficultyButton.onClick.AddListener(CycleDefaultDifficulty);

            Button sensitivityButton = UiFactory.CreatePixelButton(panel.transform, "灵敏度 " + BuildSensitivityText(), ArcadeTheme.EnergyYellow, new Vector2(420f, 72f), new Vector2(-220f, -226f));
            sensitivityButton.onClick.AddListener(CycleSensitivity);
            Button nameButton = UiFactory.CreatePixelButton(panel.transform, "榜名 " + SessionState.LeaderboardName, ArcadeTheme.ElectricBlue, new Vector2(420f, 72f), new Vector2(220f, -226f));
            nameButton.onClick.AddListener(CycleLeaderboardName);
            Button vibrationButton = UiFactory.CreatePixelToggle(panel.transform, "震动", SessionState.VibrationEnabled, new Vector2(-220f, -316f), () =>
            {
                SessionState.SetVibrationEnabled(!SessionState.VibrationEnabled);
                ShowSettings();
            });
            Button damageButton = UiFactory.CreatePixelToggle(panel.transform, "伤害数字", SessionState.DamageNumbersEnabled, new Vector2(220f, -316f), () =>
            {
                SessionState.SetDamageNumbersEnabled(!SessionState.DamageNumbersEnabled);
                ShowSettings();
            });
            Button effectsButton = UiFactory.CreatePixelButton(panel.transform, "特效质量 " + BuildVisualEffectsQualityText(), ArcadeTheme.MilitaryGreen, new Vector2(420f, 72f), new Vector2(-220f, -406f));
            effectsButton.onClick.AddListener(CycleVisualEffectsQuality);
            Button resetButton = UiFactory.CreatePixelButton(panel.transform, "恢复默认", ArcadeTheme.WarningRed, new Vector2(420f, 72f), new Vector2(220f, -406f));
            resetButton.onClick.AddListener(() =>
            {
                SessionState.ResetSettings();
                ShowSettings();
            });

            Button back = UiFactory.CreatePixelButton(background.transform, "返回标题", ArcadeTheme.DimGray, new Vector2(360f, 78f), new Vector2(0f, -780f));
            back.onClick.AddListener(ShowTitle);
            if (transitionController != null) StartCoroutine(transitionController.FadeGroup(background.transform));
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
