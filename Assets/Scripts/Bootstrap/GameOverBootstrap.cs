using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class GameOverBootstrap : MonoBehaviour
    {
        private Text nameText;
        private Text mountScoreText;
        private Text mountStatusText;
        private readonly Button[] mountButtons = new Button[3];
        private char[] initials;

        private void Awake()
        {
            bool victory = SessionState.LastRunWasVictory;
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera(victory ? new Color(0.03f, 0.06f, 0.14f) : new Color(0.09f, 0.03f, 0.12f));
            BuildGameOver();
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
            background.sprite = RuntimeSpriteFactory.GetRaidenStageBackgroundSprite(SessionState.CurrentStageNumber);
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
                ? $"第{SessionState.CurrentStageNumber}关突破  得分 {SessionState.LastScore:0000000}"
                : $"第{SessionState.CurrentStageNumber}关 {SessionState.CurrentStage.Name}  得分 {SessionState.LastScore:0000000}";

            UiFactory.CreatePanel(background, "ResultDim", new Color(0.01f, 0.01f, 0.03f, victory ? 0.5f : 0.58f), Vector2.zero, Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "ResultTopShade", new Color(0f, 0f, 0f, 0.24f), new Vector2(0f, 0.56f), Vector2.one).raycastTarget = false;
            UiFactory.CreatePanel(background, "ResultBottomShade", new Color(0f, 0f, 0f, 0.34f), Vector2.zero, new Vector2(1f, 0.36f)).raycastTarget = false;

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

            initials = SessionState.LeaderboardName.ToCharArray();
            Image namePanel = UiFactory.CreatePixelPanel(background, "NameEntryPanel", new Color(0.03f, 0.04f, 0.09f, 0.82f), ArcadeTheme.ElectricBlue, new Vector2(0.2f, 0.31f), new Vector2(0.8f, 0.43f), new Vector2(5f, 5f));
            UiFactory.CreateArcadeLabel(namePanel.transform, "本地榜名", 24, TextAnchor.MiddleLeft, detailColor, FontStyle.Bold, new Vector2(0.07f, 0.52f), new Vector2(0.42f, 0.92f), Vector2.zero);
            nameText = UiFactory.CreateArcadeLabel(namePanel.transform, new string(initials), 46, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.38f, 0.1f), new Vector2(0.62f, 0.9f), Vector2.zero);
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                Button up = UiFactory.CreatePixelButton(namePanel.transform, "+", ArcadeTheme.ElectricBlue, new Vector2(58f, 42f), new Vector2(42f + i * 58f, 34f));
                up.onClick.AddListener(() => CycleInitial(index, 1));
                Button down = UiFactory.CreatePixelButton(namePanel.transform, "-", ArcadeTheme.DimGray, new Vector2(58f, 42f), new Vector2(42f + i * 58f, -34f));
                down.onClick.AddListener(() => CycleInitial(index, -1));
            }

            if (victory)
            {
                BuildMountShop(background, detailColor);
            }

            string primaryCopy = victory ? "继续下一关" : "重新挑战";
            Button primaryButton = UiFactory.CreatePixelButton(background, primaryCopy, victory ? ArcadeTheme.ElectricBlue : ArcadeTheme.WarningRed, new Vector2(390f, 118f), new Vector2(-226f, -610f));
            primaryButton.onClick.AddListener(victory ? SceneNavigator.LoadNextStage : SceneNavigator.LoadGame);
            Button menuButton = UiFactory.CreatePixelButton(background, "返回主页", ArcadeTheme.EnergyYellow, new Vector2(390f, 118f), new Vector2(226f, -610f));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);
        }

        private void BuildMountShop(Transform background, Color detailColor)
        {
            Image panel = UiFactory.CreatePixelPanel(background, "MountShopPanel", new Color(0.025f, 0.035f, 0.08f, 0.86f), ArcadeTheme.EnergyYellow, new Vector2(0.08f, 0.13f), new Vector2(0.92f, 0.29f), new Vector2(5f, 5f));
            UiFactory.CreateArcadeLabel(panel.transform, "挂载补给", 24, TextAnchor.MiddleLeft, detailColor, FontStyle.Bold, new Vector2(0.035f, 0.68f), new Vector2(0.28f, 0.94f), Vector2.zero);
            mountScoreText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 20, TextAnchor.MiddleRight, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.42f, 0.69f), new Vector2(0.96f, 0.93f), Vector2.zero);
            mountStatusText = UiFactory.CreateArcadeLabel(panel.transform, string.Empty, 18, TextAnchor.MiddleCenter, new Color(0.78f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.26f), Vector2.zero);

            MountType[] mounts = MountConfig.GetPlayableMounts();
            for (int i = 0; i < mounts.Length; i++)
            {
                MountType mount = mounts[i];
                MountConfig config = MountConfig.Get(mount);
                float start = 0.04f + (i * 0.31f);
                float end = start + 0.29f;
                Image card = UiFactory.CreatePixelPanel(panel.transform, config.DisplayName + "Card", new Color(0.02f, 0.025f, 0.055f, 0.92f), config.AccentColor, new Vector2(start, 0.29f), new Vector2(end, 0.66f), new Vector2(4f, 4f));
                UiFactory.CreateArcadeLabel(card.transform, config.DisplayName, 18, TextAnchor.MiddleCenter, config.AccentColor, FontStyle.Bold, new Vector2(0.04f, 0.5f), new Vector2(0.96f, 0.9f), Vector2.zero);
                UiFactory.CreateArcadeLabel(card.transform, config.Cost.ToString("00000"), 16, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.46f), Vector2.zero);
                Button buyButton = UiFactory.CreateButton(card.transform, "购买", config.AccentColor, Color.white, new Vector2(128f, 42f), new Vector2(0f, -38f));
                int index = i;
                buyButton.onClick.AddListener(() => PurchaseMount(mounts[index]));
                mountButtons[i] = buyButton;
            }

            RefreshMountShop();
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
                ? "选择一个挂载带入下一关"
                : "已装备 " + MountConfig.Get(pendingMount).DisplayName;

            MountType[] mounts = MountConfig.GetPlayableMounts();
            for (int i = 0; i < mounts.Length && i < mountButtons.Length; i++)
            {
                if (mountButtons[i] == null)
                {
                    continue;
                }

                MountConfig config = MountConfig.Get(mounts[i]);
                bool selected = pendingMount == mounts[i];
                bool canBuy = pendingMount == MountType.None && SessionState.SpendableScore >= config.Cost;
                mountButtons[i].interactable = canBuy;
                Text text = mountButtons[i].GetComponentInChildren<Text>();
                text.text = selected ? "已装" : canBuy ? "购买" : "不足";
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
