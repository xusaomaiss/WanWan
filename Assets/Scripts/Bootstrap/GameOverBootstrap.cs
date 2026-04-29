using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class GameOverBootstrap : MonoBehaviour
    {
        public static readonly Vector2 ResultActionButtonSize = new Vector2(360f, 96f);
        public static readonly Vector2 LegacyResultActionButtonSize = new Vector2(390f, 118f);
        public const float VictoryActionButtonY = -660f;
        public const float DefaultActionButtonY = -480f;
        public static readonly Vector2 MountShopAnchorMin = new Vector2(0.05f, 0.20f);
        public static readonly Vector2 MountShopAnchorMax = new Vector2(0.95f, 0.44f);

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
            background.sprite = RuntimeSpriteFactory.GetSciFiBackgroundSprite(victory ? SciFiBackgroundLayerKind.Nebula : SciFiBackgroundLayerKind.Deep);
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

            // Result panel decoration
            Color resultPanelTint = victory ? new Color(0.6f, 0.9f, 1f, 0.20f) : new Color(1f, 0.3f, 0.2f, 0.20f);
            Image resultPanel = UiFactory.CreatePanel(background, "ResultPanelSprite", resultPanelTint, new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.72f));
            resultPanel.sprite = RuntimeSpriteFactory.GetSciFiHudSprite(SciFiHudSpriteKind.ResultPanel);
            resultPanel.raycastTarget = false;
            UiFactory.CreatePanel(background, "ResultDim", new Color(0.01f, 0.01f, 0.03f, victory ? 0.5f : 0.58f), Vector2.zero, Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "ResultTopShade", new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, 0.56f), Vector2.one).raycastTarget = false;

            Text shadow = UiFactory.CreateArcadeLabel(background, titleText, 92, TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.86f), FontStyle.Bold, new Vector2(0.08f, 0.6f), new Vector2(0.92f, 0.74f), new Vector2(6f, -8f));
            UiFactory.ConfigureSingleLine(shadow);
            shadow.raycastTarget = false;
            Text title = UiFactory.CreateArcadeLabel(background, titleText, 92, TextAnchor.MiddleCenter, titleColor, FontStyle.Bold, new Vector2(0.08f, 0.6f), new Vector2(0.92f, 0.74f), Vector2.zero);
            UiFactory.ConfigureSingleLine(title);
            title.raycastTarget = false;
            Outline titleOutline = title.gameObject.AddComponent<Outline>();
            titleOutline.effectColor = outlineColor;
            titleOutline.effectDistance = new Vector2(3f, -3f);

            Text stage = UiFactory.CreateArcadeLabel(background, stageLine, 34, TextAnchor.MiddleCenter, detailColor, FontStyle.Bold, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.59f), Vector2.zero);
            UiFactory.ConfigureSingleLine(stage);
            stage.raycastTarget = false;
            Outline stageOutline = stage.gameObject.AddComponent<Outline>();
            stageOutline.effectColor = new Color(0f, 0f, 0f, 0.78f);
            stageOutline.effectDistance = new Vector2(2f, -2f);

            scoreCounter = new ScoreCounter();
            scoreCounter.Start(SessionState.LastScore);
            scoreCounterText = UiFactory.CreateArcadeLabel(background, "本局得分 0000000", 46, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.48f), new Vector2(0.88f, 0.56f), Vector2.zero);
            Outline scoreOutline = scoreCounterText.gameObject.AddComponent<Outline>();
            scoreOutline.effectColor = new Color(0f, 0f, 0f, 0.72f);
            scoreOutline.effectDistance = new Vector2(2f, -2f);

            if (victory)
            {
                BuildMountShop(background, detailColor);
            }

            string primaryCopy = victory ? "继续下一关" : "重新挑战";
            Vector2 actionButtonSize = victory ? ResultActionButtonSize : LegacyResultActionButtonSize;
            float actionButtonY = victory ? VictoryActionButtonY : -480f;
            Button primaryButton = UiFactory.CreateSciFiWideButton(background, primaryCopy, victory ? ArcadeTheme.ElectricBlue : ArcadeTheme.WarningRed, actionButtonSize, new Vector2(-210f, actionButtonY));
            primaryButton.onClick.AddListener(victory ? SceneNavigator.LoadNextStage : SceneNavigator.LoadGame);
            Button menuButton = UiFactory.CreateSciFiWideButton(background, "返回主页", ArcadeTheme.EnergyYellow, actionButtonSize, new Vector2(210f, actionButtonY));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);

            // Bottom shade must be created after buttons so it renders behind them
            UiFactory.CreatePanel(background, "ResultBottomShade", new Color(0f, 0f, 0f, 0.34f), Vector2.zero, new Vector2(1f, 0.36f)).raycastTarget = false;
        }

        private void BuildMountShop(Transform background, Color detailColor)
        {
            Image panel = UiFactory.CreatePixelPanel(background, "MountShopPanel", new Color(0.025f, 0.035f, 0.08f, 0.86f), ArcadeTheme.EnergyYellow, MountShopAnchorMin, MountShopAnchorMax, new Vector2(5f, 5f));
            UiFactory.CreateArcadeLabel(panel.transform, "挂载补给", 24, TextAnchor.MiddleLeft, detailColor, FontStyle.Bold, new Vector2(0.035f, 0.76f), new Vector2(0.28f, 0.94f), Vector2.zero);
            mountScoreText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 20, TextAnchor.MiddleRight, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.42f, 0.77f), new Vector2(0.96f, 0.93f), Vector2.zero);
            mountStatusText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 16, TextAnchor.MiddleCenter, new Color(0.78f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.04f, 0.02f), new Vector2(0.96f, 0.18f), Vector2.zero);

            MountType[] mounts = MountConfig.GetPlayableMounts();
            for (int i = 0; i < mounts.Length; i++)
            {
                MountType mount = mounts[i];
                MountConfig config = MountConfig.Get(mount);
                float start = 0.03f + (i * 0.315f);
                float end = start + 0.30f;
                // Card background
                // Card dark background for readability
                UiFactory.CreatePanel(panel.transform, config.DisplayName + "DarkBg", new Color(0.02f, 0.02f, 0.05f, 0.75f), new Vector2(start, 0.22f), new Vector2(end, 0.72f)).raycastTarget = false;
                Image card = UiFactory.CreateSpritePanel(panel.transform, config.DisplayName + "Card", "UI/hud/mount_card", new Color(0.9f, 0.9f, 1f, 0.50f), new Vector2(start, 0.22f), new Vector2(end, 0.72f));
                // Mount icon
                Sprite iconSprite = GetMountIconSprite(mount);
                if (iconSprite != null)
                {
                    Image icon = UiFactory.CreatePanel(card.transform, config.DisplayName + "Icon", config.AccentColor, new Vector2(0.25f, 0.58f), new Vector2(0.75f, 0.92f));
                    icon.sprite = iconSprite;
                    icon.preserveAspect = true;
                    icon.raycastTarget = false;
                }
                // Name label
                UiFactory.CreateArcadeLabel(card.transform, config.DisplayName, 20, TextAnchor.MiddleCenter, config.AccentColor, FontStyle.Bold, new Vector2(0.05f, 0.42f), new Vector2(0.95f, 0.58f), Vector2.zero);
                // Cost and amount
                UiFactory.CreateArcadeLabel(card.transform, config.Cost.ToString("00000") + " / +" + config.PurchaseUnits + config.UnitLabel, 14, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.05f, 0.24f), new Vector2(0.95f, 0.40f), Vector2.zero);
                // Larger buy button
                Button buyButton = UiFactory.CreateSpriteButton(card.transform, "购买", "UI/button/Button01", config.AccentColor, new Vector2(160f, 52f), new Vector2(0f, -16f));
                int index = i;
                buyButton.onClick.AddListener(() => PurchaseMount(mounts[index]));
                mountButtons[i] = buyButton;
            }

            RefreshMountShop();
        }

        private static Sprite GetMountIconSprite(MountType mount)
        {
            switch (mount)
            {
                case MountType.MissilePod:
                    return RuntimeSpriteFactory.GetMissileSprite();
                case MountType.DefenseDrone:
                    return RuntimeSpriteFactory.GetRaidenFighterJetSprite();
                case MountType.ShieldEmitter:
                    return RuntimeSpriteFactory.GetCircleSprite();
                default:
                    return null;
            }
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
                bool canBuy = (pendingMount == MountType.None || selected) && SessionState.SpendableScore >= config.Cost;
                mountButtons[i].interactable = canBuy;
                Text text = mountButtons[i].GetComponentInChildren<Text>();
                if (selected)
                {
                    text.text = canBuy ? "加购" : "不足";
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
