using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class GameOverBootstrap : MonoBehaviour
    {
        public const string VictorySupplyScreenResourcePath = "RaidenArt/Cinematics/victory_supply_screen_ai";
        public const string VictoryTitleFrameResourcePath = "GameOver/Victory/title_frame";
        public const string VictoryStageFrameResourcePath = "GameOver/Victory/stage_frame";
        public const string VictorySupplyPanelFrameResourcePath = "GameOver/Victory/supply_panel_frame";
        public const string VictorySupplyRowFrameResourcePath = "GameOver/Victory/supply_row_frame";
        public const string VictoryNextButtonResourcePath = "GameOver/Victory/button_next";
        public const string VictoryMenuButtonResourcePath = "GameOver/Victory/button_menu";
        public const string FailureTitleFrameResourcePath = "GameOver/Failure/title_frame";
        public const string FailureStageFrameResourcePath = "GameOver/Failure/stage_frame";
        public const string FailureRetryButtonResourcePath = "GameOver/Failure/button_retry";
        public const string FailureMenuButtonResourcePath = "GameOver/Failure/button_menu";
        public static readonly string[] VictorySlicedResourcePaths =
        {
            VictoryTitleFrameResourcePath,
            VictoryStageFrameResourcePath,
            VictorySupplyPanelFrameResourcePath,
            VictorySupplyRowFrameResourcePath,
            VictoryNextButtonResourcePath,
            VictoryMenuButtonResourcePath
        };
        public static readonly string[] FailureSlicedResourcePaths =
        {
            FailureTitleFrameResourcePath,
            FailureStageFrameResourcePath,
            FailureRetryButtonResourcePath,
            FailureMenuButtonResourcePath
        };
        public static readonly Vector2 ResultActionButtonSize = new Vector2(360f, 96f);
        public static readonly Vector2 LegacyResultActionButtonSize = new Vector2(390f, 118f);
        public const float VictoryActionButtonY = -802f;
        public const float DefaultActionButtonY = -480f;
        public static readonly Vector2 MountShopAnchorMin = new Vector2(0.045f, 0.255f);
        public static readonly Vector2 MountShopAnchorMax = new Vector2(0.955f, 0.565f);

        private Text nameText;
        private Text mountScoreText;
        private Text mountStatusText;
        private readonly Button[] mountButtons = new Button[3];
        private char[] initials;
        private ScoreCounter scoreCounter;
        private Text scoreCounterText;

        private void Awake()
        {
            bool victory = SessionState.LastRunWasVictory;
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera(victory ? new Color(0.03f, 0.06f, 0.14f) : new Color(0.09f, 0.03f, 0.12f));
            BuildGameOver();
        }

        private void Update()
        {
            if (scoreCounter != null && !scoreCounter.IsComplete)
            {
                scoreCounter.Tick(Time.deltaTime);
                if (scoreCounterText != null)
                {
                    scoreCounterText.text = $"本局得分 {scoreCounter.CurrentScore:0000000}";
                }
            }
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

        private void BuildGameOver()
        {
            Canvas canvas = UiFactory.CreateCanvas("GameOverCanvas");
            bool victory = SessionState.LastRunWasVictory;
            Image background = UiFactory.CreatePanel(canvas.transform, "Background", Color.white, Vector2.zero, Vector2.one);
            background.sprite = RuntimeSpriteFactory.GetSciFiBackgroundSprite(SciFiBackgroundLayerKind.Deep);
            background.preserveAspect = false;
            BuildFloatingGameOver(background.transform, victory);
        }

        private void BuildFloatingGameOver(Transform background, bool victory)
        {
            Color titleColor = victory ? ArcadeTheme.ElectricBlue : ArcadeTheme.WarningRed;
            Color outlineColor = victory ? new Color(0.78f, 1f, 0.94f, 0.9f) : new Color(1f, 0.84f, 0.28f, 0.9f);
            Color detailColor = victory ? new Color(0.78f, 1f, 0.92f) : new Color(1f, 0.9f, 0.66f);
            string titleText = victory ? "任务完成" : "任务失败";
            string stageLine = victory
                ? $"第{SessionState.CurrentStageNumber}关突破"
                : $"第{SessionState.CurrentStageNumber}关 {SessionState.CurrentStage.Name}";

            if (!victory)
            {
                UiFactory.CreatePanel(background, "ResultDim", new Color(0.01f, 0.01f, 0.03f, 0.58f), Vector2.zero, Vector2.one).raycastTarget = false;
                UiFactory.CreatePanel(background, "ResultTopShade", new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, 0.56f), Vector2.one).raycastTarget = false;
                UiFactory.CreatePanel(background, "ResultBottomShade", new Color(0f, 0f, 0f, 0.34f), Vector2.zero, new Vector2(1f, 0.36f)).raycastTarget = false;
                Image titleFrame = UiFactory.CreateSpritePanel(background, "FailureTitleFrame", FailureTitleFrameResourcePath, Color.white, new Vector2(0.08f, 0.62f), new Vector2(0.92f, 0.78f));
                titleFrame.raycastTarget = false;
                Image stageFrame = UiFactory.CreateSpritePanel(background, "FailureStageFrame", FailureStageFrameResourcePath, Color.white, new Vector2(0.17f, 0.50f), new Vector2(0.83f, 0.565f));
                stageFrame.raycastTarget = false;
            }

            if (victory)
            {
                Image titleFrame = UiFactory.CreateSpritePanel(background, "VictoryTitleFrame", VictoryTitleFrameResourcePath, Color.white, new Vector2(0.075f, 0.715f), new Vector2(0.925f, 0.86f));
                titleFrame.raycastTarget = false;
                Image stageFrame = UiFactory.CreateSpritePanel(background, "VictoryStageFrame", VictoryStageFrameResourcePath, Color.white, new Vector2(0.17f, 0.61f), new Vector2(0.83f, 0.665f));
                stageFrame.raycastTarget = false;
            }

            Vector2 titleMin = victory ? new Vector2(0.13f, 0.745f) : new Vector2(0.12f, 0.655f);
            Vector2 titleMax = victory ? new Vector2(0.87f, 0.835f) : new Vector2(0.88f, 0.745f);
            Vector2 stageMin = victory ? new Vector2(0.20f, 0.618f) : new Vector2(0.20f, 0.512f);
            Vector2 stageMax = victory ? new Vector2(0.80f, 0.658f) : new Vector2(0.80f, 0.555f);
            Vector2 scoreMin = victory ? new Vector2(0.08f, 0.56f) : new Vector2(0.10f, 0.425f);
            Vector2 scoreMax = victory ? new Vector2(0.92f, 0.61f) : new Vector2(0.90f, 0.505f);
            int titleSize = victory ? 84 : 82;
            int stageSize = victory ? 30 : 30;
            int scoreSize = victory ? 38 : 42;

            Text shadow = UiFactory.CreateArcadeLabel(background, titleText, titleSize, TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.86f), FontStyle.Bold, titleMin, titleMax, new Vector2(6f, -8f));
            UiFactory.ConfigureSingleLine(shadow);
            shadow.raycastTarget = false;
            Text title = UiFactory.CreateArcadeLabel(background, titleText, titleSize, TextAnchor.MiddleCenter, titleColor, FontStyle.Bold, titleMin, titleMax, Vector2.zero);
            UiFactory.ConfigureSingleLine(title);
            title.raycastTarget = false;
            Outline titleOutline = title.gameObject.AddComponent<Outline>();
            titleOutline.effectColor = outlineColor;
            titleOutline.effectDistance = new Vector2(3f, -3f);

            Text stage = UiFactory.CreateArcadeLabel(background, stageLine, stageSize, TextAnchor.MiddleCenter, detailColor, FontStyle.Bold, stageMin, stageMax, Vector2.zero);
            UiFactory.ConfigureSingleLine(stage);
            stage.raycastTarget = false;
            Outline stageOutline = stage.gameObject.AddComponent<Outline>();
            stageOutline.effectColor = new Color(0f, 0f, 0f, 0.78f);
            stageOutline.effectDistance = new Vector2(2f, -2f);

            scoreCounter = new ScoreCounter();
            scoreCounter.Start(SessionState.LastScore);
            scoreCounterText = UiFactory.CreateArcadeLabel(background, "本局得分 0000000", scoreSize, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, scoreMin, scoreMax, Vector2.zero);
            Outline scoreOutline = scoreCounterText.gameObject.AddComponent<Outline>();
            scoreOutline.effectColor = new Color(0f, 0f, 0f, 0.72f);
            scoreOutline.effectDistance = new Vector2(2f, -2f);

            if (victory)
            {
                BuildMountShop(background, detailColor);
            }

            string primaryCopy = victory ? "继续" : "重新挑战";
            float actionButtonY = victory ? VictoryActionButtonY : -480f;
            Button primaryButton = victory
                ? CreateVictoryActionButton(background, primaryCopy, VictoryNextButtonResourcePath, new Vector2(-235f, actionButtonY))
                : CreateFailureActionButton(background, primaryCopy, FailureRetryButtonResourcePath, new Vector2(-210f, actionButtonY));
            primaryButton.onClick.AddListener(victory ? SceneNavigator.LoadNextStage : SceneNavigator.LoadGame);
            Button menuButton = victory
                ? CreateVictoryActionButton(background, "返回", VictoryMenuButtonResourcePath, new Vector2(235f, actionButtonY))
                : CreateFailureActionButton(background, "返回", FailureMenuButtonResourcePath, new Vector2(210f, actionButtonY));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);
        }

        private void BuildMountShop(Transform background, Color detailColor)
        {
            Image panel = UiFactory.CreateSpritePanel(background, "MountShopPanel", VictorySupplyPanelFrameResourcePath, Color.white, MountShopAnchorMin, MountShopAnchorMax);
            panel.raycastTarget = false;
            UiFactory.CreateArcadeLabel(panel.transform, "挂载补给", 24, TextAnchor.MiddleLeft, detailColor, FontStyle.Bold, new Vector2(0.06f, 0.895f), new Vector2(0.32f, 0.98f), Vector2.zero).raycastTarget = false;
            mountScoreText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 18, TextAnchor.MiddleRight, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.44f, 0.895f), new Vector2(0.91f, 0.98f), Vector2.zero);
            mountStatusText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 15, TextAnchor.MiddleCenter, new Color(0.78f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.04f, 0.025f), new Vector2(0.96f, 0.085f), Vector2.zero);

            MountType[] mounts = MountConfig.GetPlayableMounts();
            for (int i = 0; i < mounts.Length; i++)
            {
                MountType mount = mounts[i];
                MountConfig config = MountConfig.Get(mount);
                float top = 0.815f - (i * 0.235f);
                float bottom = top - 0.195f;
                Image row = UiFactory.CreateSpritePanel(panel.transform, config.DisplayName + "MountRow", VictorySupplyRowFrameResourcePath, Color.white, new Vector2(0.06f, bottom), new Vector2(0.94f, top));
                row.raycastTarget = false;

                Image accent = UiFactory.CreatePanel(row.transform, "Accent", config.AccentColor, new Vector2(0.018f, 0.14f), new Vector2(0.032f, 0.86f));
                accent.raycastTarget = false;

                Sprite iconSprite = GetMountIconSprite(mount);
                if (iconSprite != null)
                {
                    Image iconBack = UiFactory.CreatePanel(row.transform, config.DisplayName + "IconBack", new Color(0.02f, 0.08f, 0.12f, 0.66f), new Vector2(0.06f, 0.13f), new Vector2(0.22f, 0.87f));
                    iconBack.sprite = RuntimeSpriteFactory.GetRoundedSquareSprite();
                    iconBack.type = Image.Type.Sliced;
                    iconBack.raycastTarget = false;
                    Image icon = UiFactory.CreatePanel(iconBack.transform, config.DisplayName + "Icon", config.AccentColor, new Vector2(0.14f, 0.10f), new Vector2(0.86f, 0.90f));
                    icon.sprite = iconSprite;
                    icon.preserveAspect = true;
                    icon.raycastTarget = false;
                }

                UiFactory.CreateArcadeLabel(row.transform, config.DisplayName, 23, TextAnchor.MiddleLeft, config.AccentColor, FontStyle.Bold, new Vector2(0.25f, 0.55f), new Vector2(0.49f, 0.88f), Vector2.zero).raycastTarget = false;
                UiFactory.CreateArcadeLabel(row.transform, config.Description, 14, TextAnchor.MiddleLeft, new Color(0.78f, 0.94f, 1f), FontStyle.Bold, new Vector2(0.25f, 0.20f), new Vector2(0.55f, 0.54f), Vector2.zero).raycastTarget = false;
                UiFactory.CreateArcadeLabel(row.transform, config.Cost.ToString("00000") + " / +" + config.PurchaseUnits + config.UnitLabel, 16, TextAnchor.MiddleLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.55f, 0.50f), new Vector2(0.74f, 0.80f), Vector2.zero).raycastTarget = false;
                Text owned = UiFactory.CreateArcadeLabel(row.transform, string.Empty, 14, TextAnchor.MiddleLeft, new Color(0.72f, 1f, 0.72f), FontStyle.Bold, new Vector2(0.55f, 0.20f), new Vector2(0.74f, 0.50f), Vector2.zero);
                owned.gameObject.name = config.DisplayName + "OwnedText";
                owned.raycastTarget = false;

                Button buyButton = UiFactory.CreateSpriteButton(row.transform, "购买", VictoryMenuButtonResourcePath, Color.white, new Vector2(150f, 54f), new Vector2(302f, 0f));
                buyButton.gameObject.name = config.DisplayName + "BuyButton";
                ConfigureVictoryButtonText(buyButton, 25);
                int index = i;
                buyButton.onClick.AddListener(() => PurchaseMount(mounts[index]));
                mountButtons[i] = buyButton;
            }

            RefreshMountShop();
        }

        private static Button CreateVictoryActionButton(Transform parent, string label, string resourcePath, Vector2 anchoredPosition)
        {
            Button button = UiFactory.CreateSpriteButton(parent, label, resourcePath, Color.white, ResultActionButtonSize, anchoredPosition);
            ConfigureVictoryButtonText(button, 42);
            return button;
        }

        private static Button CreateFailureActionButton(Transform parent, string label, string resourcePath, Vector2 anchoredPosition)
        {
            Button button = UiFactory.CreateSpriteButton(parent, label, resourcePath, Color.white, LegacyResultActionButtonSize, anchoredPosition);
            ConfigureVictoryButtonText(button, 40);
            return button;
        }

        private static void ConfigureVictoryButtonText(Button button, int fontSize)
        {
            Text text = button.GetComponentInChildren<Text>();
            text.fontSize = fontSize;
            text.resizeTextMinSize = Mathf.Max(16, fontSize - 10);
            text.resizeTextMaxSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.color = ArcadeTheme.White;
            text.raycastTarget = false;
            Outline outline = text.gameObject.GetComponent<Outline>();
            if (outline == null)
            {
                outline = text.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
            UiFactory.ConfigureSingleLine(text);
        }

        private static Sprite GetMountIconSprite(MountType mount)
        {
            switch (mount)
            {
                case MountType.MissilePod:
                    return RuntimeSpriteFactory.GetMissileSprite();
                case MountType.DefenseDrone:
                    return RuntimeSpriteFactory.GetDefenseDroneSprite();
                case MountType.ShieldEmitter:
                    return RuntimeSpriteFactory.GetShieldEmitterSprite();
                default:
                    return null;
            }
        }

        private static Sprite LoadResourceSprite(string resourcePath, Sprite fallback)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            return texture == null
                ? fallback
                : Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 128f);
        }

        private void PurchaseMount(MountType mount)
        {
            SessionState.TryPurchaseMount(mount);
            RefreshMountShop();
        }

        private void RefreshMountShop()
        {
            if (mountScoreText == null)
            {
                return;
            }

            mountScoreText.text = $"可用积分 {SessionState.SpendableScore:0000000}";
            MountType pendingMount = SessionState.PendingMount;
            mountStatusText.text = pendingMount == MountType.None
                ? "购买弹药或护盾带入下一关"
                : "已补给 " + MountConfig.Get(pendingMount).DisplayName + " " + SessionState.PendingMountUnits + MountConfig.Get(pendingMount).UnitLabel;

            MountType[] mounts = MountConfig.GetPlayableMounts();
            for (int i = 0; i < mounts.Length && i < mountButtons.Length; i++)
            {
                if (mountButtons[i] == null)
                {
                    continue;
                }

                MountConfig config = MountConfig.Get(mounts[i]);
                bool selected = pendingMount == mounts[i];
                bool canBuy = SessionState.CanPurchaseMount(mounts[i]);
                mountButtons[i].interactable = canBuy;
                Text text = mountButtons[i].GetComponentInChildren<Text>();
                Text owned = FindMountOwnedText(mountButtons[i].transform.parent, config.DisplayName);
                if (owned != null)
                {
                    owned.text = selected
                        ? "已购 " + SessionState.PendingMountUnits + config.UnitLabel
                        : "未加购";
                }

                if (selected)
                {
                    text.text = canBuy ? "加购" : "已满";
                }
                else if (pendingMount != MountType.None)
                {
                    text.text = "已选";
                }
                else
                {
                    text.text = canBuy ? "购买" : "不足";
                }
            }
        }

        private static Text FindMountOwnedText(Transform row, string displayName)
        {
            if (row == null)
            {
                return null;
            }

            foreach (Text text in row.GetComponentsInChildren<Text>())
            {
                if (text.gameObject.name == displayName + "OwnedText")
                {
                    return text;
                }
            }

            return null;
        }

        private void CycleInitial(int index, int delta)
        {
            if (initials == null || index < 0 || index >= initials.Length)
            {
                return;
            }

            int value = initials[index] - 'A';
            value = (value + delta + 26) % 26;
            initials[index] = (char)('A' + value);
            string name = new string(initials);
            nameText.text = name;
            SessionState.UpdateLastLeaderboardName(name);
        }

        private static string BuildDifficultyText()
        {
            switch (SessionState.LastRunDifficulty)
            {
                case GameDifficulty.Medium:
                    return "中级";
                case GameDifficulty.High:
                    return "高级";
                default:
                    return "低级";
            }
        }
    }
}
