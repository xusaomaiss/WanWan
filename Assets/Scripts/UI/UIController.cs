using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class UIController : MonoBehaviour
    {
        private GameManager gameManager;
        private Text scoreText;
        private Text livesText;
        private Text highScoreText;
        private Text difficultyText;
        private Text stageProgressText;
        private Text powerupText;
        private Text pauseHintText;
        private Text stageBannerText;
        private Button pauseButton;
        private Text pauseButtonText;
        private Image pauseOverlay;
        private Text pauseOverlayTitle;
        private Text pauseOverlayHighScore;
        private Text pauseOverlayMission;
        private Text pauseOverlayAudioText;
        private Image stageProgressFill;
        private Image introOverlay;
        private Text introTitle;
        private Text introCountdown;
        private Image bossWarningOverlay;
        private Text bossWarningTitle;
        private Image bossBarRoot;
        private Image bossBarFill;
        private Text bossBarLabel;
        private Image overlay;
        private Text overlayTitle;
        private Text overlayStage;
        private Text overlayScore;
        private Text overlayBestScore;
        private Text overlaySummary;

        public void Bind(GameManager manager)
        {
            gameManager = manager;
            Build();
            RefreshHud();
        }

        public void RefreshHud()
        {
            if (gameManager == null)
            {
                return;
            }

            scoreText.text = $"得分 {gameManager.Score:0000000}\n金币 {gameManager.CoinCount:000}";
            livesText.text = $"战机 {gameManager.Lives}\n{BuildBombIcons()}";
            highScoreText.text = $"最高分\n{SessionState.HighScore:0000000}";
            difficultyText.text = $"难度 {BuildDifficultyText()}";
            stageProgressText.text = $"第{gameManager.StageNumber}关 {(gameManager.StageProgress * 100f):0}%\n击落 {gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}";
            if (stageProgressFill != null)
            {
                RectTransform fillRect = stageProgressFill.rectTransform;
                fillRect.anchorMax = new Vector2(gameManager.StageProgress, 1f);
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
            }
            powerupText.text = BuildPowerupHudText();
            pauseHintText.text = gameManager.IsPaused ? "已暂停" : string.Empty;
            stageBannerText.text = gameManager.StageBannerText;
            stageBannerText.gameObject.SetActive(!string.IsNullOrEmpty(gameManager.StageBannerText));
            stageBannerText.color = gameManager.StageBannerText.Contains("警报")
                ? new Color(1f, 0.34f, 0.2f, 0.98f)
                : new Color(0.82f, 0.94f, 1f, 0.98f);
            pauseButtonText.text = gameManager.IsPaused ? "▶" : "Ⅱ";
            pauseOverlay.gameObject.SetActive(gameManager.IsPaused);
            pauseOverlayHighScore.text = $"最高分 {SessionState.HighScore:0000000}";
            pauseOverlayMission.text = $"第{gameManager.StageNumber}关 {gameManager.StageName}\n目标 {gameManager.BossDisplayName}\n击落 {gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}  炸弹 {gameManager.BombCount}";
            pauseOverlayAudioText.text = gameManager.AudioEnabled ? "声音 开" : "声音 关";
            bossBarRoot.gameObject.SetActive(gameManager.HasBoss);
            if (gameManager.HasBoss)
            {
                bossBarLabel.text = $"警报  {gameManager.BossName}";
                RectTransform fillRect = bossBarFill.rectTransform;
                fillRect.anchorMax = new Vector2(gameManager.BossHealthNormalized, 1f);
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
            }
        }

        public void ShowGameOverOverlay(string title, int score)
        {
            overlay.gameObject.SetActive(true);
            overlayTitle.text = title;
            overlayStage.text = $"L{gameManager.LoopNumber}-{gameManager.StageNumber} {gameManager.StageName}  {BuildDifficultyText()}";
            overlayScore.text = $"本局得分 {score:0000000}";
            overlayBestScore.text = $"最高分 {SessionState.HighScore:0000000}";
            overlaySummary.text = $"击落 {gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}  BOMB {gameManager.BombsUsed}  目标 {gameManager.BossDisplayName}";
            if (title == "游戏胜利")
            {
                overlayBestScore.text = $"本关得分 {score:0000000}  击落 {gameManager.EnemiesDestroyed}";
                overlaySummary.text = $"使用BOMB {gameManager.BombsUsed}  下一关 {StageCatalog.GetStage(StageCatalog.GetNextStageIndex(SessionState.CurrentStageIndex)).Name}";
            }
        }

        public void ShowIntroPrompt()
        {
            if (introOverlay == null)
            {
                return;
            }

            introOverlay.gameObject.SetActive(true);
            introTitle.text = "LAUNCH SEQUENCE";
            introCountdown.text = "TAP / ANY KEY SKIP";
        }

        public void HideIntroPrompt()
        {
            if (introOverlay != null)
            {
                introOverlay.gameObject.SetActive(false);
            }
        }

        public void ShowBossWarning(string bossDisplayName)
        {
            if (bossWarningOverlay == null || bossWarningOverlay.gameObject.activeSelf)
            {
                return;
            }

            bossWarningTitle.text = $"WARNING\n{bossDisplayName}";
            StartCoroutine(ShowBossWarningSequence());
        }

        private void Build()
        {
            Canvas canvas = UiFactory.CreateCanvas("GameCanvas");
            canvas.transform.SetParent(transform, false);

            Image topBar = UiFactory.CreatePixelPanel(canvas.transform, "TopHud", ArcadeTheme.PanelBase, ArcadeTheme.ElectricBlue, new Vector2(0f, 0.92f), new Vector2(1f, 1f), new Vector2(6f, 6f));
            Image leftCell = UiFactory.CreatePanel(topBar.transform, "ScoreCell", new Color(0.05f, 0.1f, 0.2f, 0.88f), new Vector2(0.015f, 0.1f), new Vector2(0.33f, 0.9f));
            Image centerCell = UiFactory.CreatePanel(topBar.transform, "StageCell", new Color(0.08f, 0.08f, 0.21f, 0.88f), new Vector2(0.345f, 0.1f), new Vector2(0.655f, 0.9f));
            Image rightCell = UiFactory.CreatePanel(topBar.transform, "PlayerCell", new Color(0.07f, 0.06f, 0.18f, 0.88f), new Vector2(0.67f, 0.1f), new Vector2(0.985f, 0.9f));

            scoreText = UiFactory.CreateArcadeLabel(leftCell.transform, "得分\n0000000", ArcadeTheme.ScoreSize, TextAnchor.UpperLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.05f, 0.28f), new Vector2(0.95f, 0.95f), Vector2.zero);
            highScoreText = UiFactory.CreateArcadeLabel(leftCell.transform, "最高分\n0000000", ArcadeTheme.SmallSize, TextAnchor.LowerLeft, ArcadeTheme.ElectricBlue, FontStyle.Bold, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.34f), Vector2.zero);
            difficultyText = UiFactory.CreateArcadeLabel(centerCell.transform, "难度 低级", 22, TextAnchor.UpperCenter, new Color(0.72f, 0.82f, 1f), FontStyle.Bold, new Vector2(0.04f, 0.62f), new Vector2(0.96f, 0.92f), Vector2.zero);
            stageProgressText = UiFactory.CreateArcadeLabel(centerCell.transform, "第1关 0%", 28, TextAnchor.MiddleCenter, new Color(0.96f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.06f, 0.26f), new Vector2(0.94f, 0.6f), Vector2.zero);
            powerupText = UiFactory.CreateArcadeLabel(centerCell.transform, "火力 普通", 24, TextAnchor.LowerCenter, new Color(1f, 0.56f, 0.74f), FontStyle.Bold, new Vector2(0.06f, 0.04f), new Vector2(0.94f, 0.3f), Vector2.zero);
            livesText = UiFactory.CreateArcadeLabel(rightCell.transform, "战机 5\n待命", 26, TextAnchor.UpperCenter, new Color(0.96f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.04f, 0.32f), new Vector2(0.96f, 0.94f), Vector2.zero);
            pauseButton = UiFactory.CreateButton(rightCell.transform, "Ⅱ", ArcadeTheme.WarningRed, Color.white, new Vector2(64f, 64f), new Vector2(-94f, 0f), new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f));
            pauseButton.onClick.AddListener(() => gameManager.TogglePause());
            pauseButtonText = pauseButton.GetComponentInChildren<Text>();
            pauseButtonText.fontStyle = FontStyle.Bold;
            pauseButtonText.fontSize = 30;

            Image bottomBar = UiFactory.CreatePixelPanel(canvas.transform, "BottomHud", new Color(0.08f, 0.08f, 0.16f, SessionState.VirtualButtonOpacity + 0.25f), ArcadeTheme.DimGray, new Vector2(0f, 0f), new Vector2(1f, 0.065f), new Vector2(6f, 6f));
            Image weaponSlot = UiFactory.CreatePixelPanel(bottomBar.transform, "WeaponSlot", new Color(0.04f, 0.04f, 0.1f, 0.95f), ArcadeTheme.MilitaryGreen, new Vector2(0.04f, 0.18f), new Vector2(0.36f, 0.82f), new Vector2(4f, 4f));
            powerupText = UiFactory.CreateArcadeLabel(weaponSlot.transform, "火力 普通", ArcadeTheme.SmallSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.05f, 0f), new Vector2(0.95f, 1f), Vector2.zero);
            Image progressTrack = UiFactory.CreatePanel(bottomBar.transform, "ProgressTrack", ArcadeTheme.InkBlack, new Vector2(0.39f, 0.36f), new Vector2(0.67f, 0.64f));
            stageProgressFill = UiFactory.CreatePanel(progressTrack.transform, "ProgressFill", ArcadeTheme.EnergyYellow, Vector2.zero, Vector2.one);
            UiFactory.CreateArcadeLabel(bottomBar.transform, "吃炸弹图标清屏", 24, TextAnchor.MiddleCenter, new Color(0.7f, 0.9f, 1f), FontStyle.Bold, new Vector2(0.7f, 0.12f), new Vector2(0.96f, 0.88f), Vector2.zero);

            bossBarRoot = UiFactory.CreatePixelPanel(canvas.transform, "BossBarRoot", new Color(0.14f, 0.03f, 0.08f, 0.92f), ArcadeTheme.WarningRed, new Vector2(0.05f, 0.86f), new Vector2(0.95f, 0.9f), new Vector2(6f, 6f));
            bossBarRoot.gameObject.SetActive(false);
            bossBarFill = UiFactory.CreatePanel(bossBarRoot.transform, "BossBarFill", new Color(1f, 0.44f, 0.3f, 0.98f), new Vector2(0.012f, 0.14f), new Vector2(0.988f, 0.86f));
            bossBarLabel = UiFactory.CreateArcadeLabel(bossBarRoot.transform, "警报  敌方旗舰", 24, TextAnchor.MiddleCenter, new Color(1f, 0.96f, 0.94f), FontStyle.Bold, new Vector2(0.05f, 0f), new Vector2(0.95f, 1f), Vector2.zero);

            pauseHintText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 40, TextAnchor.MiddleCenter, new Color(0.6f, 0.84f, 1f, 0.92f), FontStyle.Bold, new Vector2(0.3f, 0.79f), new Vector2(0.7f, 0.84f), Vector2.zero);
            stageBannerText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 86, TextAnchor.MiddleCenter, new Color(1f, 0.34f, 0.2f, 0.98f), FontStyle.Bold, new Vector2(0.08f, 0.63f), new Vector2(0.92f, 0.77f), Vector2.zero);
            stageBannerText.gameObject.SetActive(false);

            pauseOverlay = UiFactory.CreatePanel(canvas.transform, "PauseOverlay", new Color(0.02f, 0.05f, 0.11f, 0.72f), Vector2.zero, Vector2.one);
            pauseOverlay.raycastTarget = false;
            pauseOverlay.gameObject.SetActive(false);
            Image pauseCard = UiFactory.CreatePixelPanel(pauseOverlay.transform, "PauseCard", ArcadeTheme.PanelBase, ArcadeTheme.ElectricBlue, new Vector2(0.16f, 0.28f), new Vector2(0.84f, 0.68f), new Vector2(10f, 10f));
            pauseCard.raycastTarget = false;
            pauseOverlayTitle = UiFactory.CreateArcadeLabel(pauseCard.transform, "暂停中", 80, TextAnchor.MiddleCenter, new Color(0.92f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.72f), new Vector2(0.88f, 0.9f), Vector2.zero);
            pauseOverlayHighScore = UiFactory.CreateArcadeLabel(pauseCard.transform, "最高分 0000000", 32, TextAnchor.MiddleCenter, new Color(0.48f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.56f), new Vector2(0.85f, 0.66f), Vector2.zero);
            pauseOverlayMission = UiFactory.CreateArcadeLabel(pauseCard.transform, "第1关 乡村\n目标 乡村防卫旗舰", 30, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.1f, 0.38f), new Vector2(0.9f, 0.54f), Vector2.zero);
            UiFactory.CreateArcadeLabel(pauseCard.transform, "红色主武器  蓝色追踪/导弹  紫色特殊强化", 24, TextAnchor.MiddleCenter, new Color(1f, 0.58f, 0.76f), FontStyle.Bold, new Vector2(0.12f, 0.29f), new Vector2(0.88f, 0.36f), Vector2.zero);
            Button resumeButton = UiFactory.CreatePixelButton(pauseCard.transform, "继续游戏", ArcadeTheme.MilitaryGreen, new Vector2(300f, 82f), new Vector2(-170f, -60f));
            resumeButton.onClick.AddListener(() => gameManager.TogglePause());
            Button audioButton = UiFactory.CreatePixelButton(pauseCard.transform, "设置", ArcadeTheme.ElectricBlue, new Vector2(300f, 82f), new Vector2(170f, -60f));
            pauseOverlayAudioText = audioButton.GetComponentInChildren<Text>();
            audioButton.onClick.AddListener(() => gameManager.ToggleAudio());
            Button restartButton = UiFactory.CreatePixelButton(pauseCard.transform, "重新开始", ArcadeTheme.WarningRed, new Vector2(300f, 82f), new Vector2(-170f, -160f));
            restartButton.onClick.AddListener(SceneNavigator.LoadGame);
            Button menuButton = UiFactory.CreatePixelButton(pauseCard.transform, "返回标题", ArcadeTheme.EnergyYellow, new Vector2(300f, 82f), new Vector2(170f, -160f));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);

            introOverlay = UiFactory.CreatePanel(canvas.transform, "LevelIntroOverlay", new Color(0.02f, 0.02f, 0.06f, 0.18f), Vector2.zero, Vector2.one);
            introOverlay.raycastTarget = false;
            introTitle = UiFactory.CreateArcadeLabel(introOverlay.transform, "第1关", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.68f), Vector2.zero);
            UiFactory.CreateArcadeLabel(introOverlay.transform, "CARRIER DECK READY", ArcadeTheme.BodySize, TextAnchor.MiddleCenter, ArcadeTheme.WarningRed, FontStyle.Bold, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.55f), Vector2.zero);
            introCountdown = UiFactory.CreateArcadeLabel(introOverlay.transform, "TAP / ANY KEY SKIP", 34, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.12f, 0.34f), new Vector2(0.88f, 0.44f), Vector2.zero);
            introOverlay.gameObject.SetActive(false);

            bossWarningOverlay = UiFactory.CreatePanel(canvas.transform, "BossWarningOverlay", new Color(0.08f, 0f, 0.02f, 0.82f), Vector2.zero, Vector2.one);
            bossWarningOverlay.gameObject.SetActive(false);
            UiFactory.CreateDivider(bossWarningOverlay.transform, "WarningTop", ArcadeTheme.WarningRed, new Vector2(0f, 0.78f), new Vector2(1f, 0.84f));
            UiFactory.CreateDivider(bossWarningOverlay.transform, "WarningBottom", ArcadeTheme.WarningRed, new Vector2(0f, 0.16f), new Vector2(1f, 0.22f));
            bossWarningTitle = UiFactory.CreateArcadeLabel(bossWarningOverlay.transform, "WARNING", 72, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.38f), new Vector2(0.92f, 0.62f), Vector2.zero);

            overlay = UiFactory.CreatePanel(canvas.transform, "Overlay", new Color(0.02f, 0.05f, 0.11f, 0.8f), Vector2.zero, Vector2.one);
            overlay.gameObject.SetActive(false);
            overlayTitle = UiFactory.CreateArcadeLabel(overlay.transform, "任务失败", 84, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold, new Vector2(0.15f, 0.58f), new Vector2(0.85f, 0.72f), Vector2.zero);
            overlayStage = UiFactory.CreateArcadeLabel(overlay.transform, "L1-1 乡村", 34, TextAnchor.MiddleCenter, new Color(0.7f, 0.88f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.5f), new Vector2(0.88f, 0.57f), Vector2.zero);
            overlayScore = UiFactory.CreateArcadeLabel(overlay.transform, "本局得分 0000000", 46, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.4f), new Vector2(0.85f, 0.49f), Vector2.zero);
            overlayBestScore = UiFactory.CreateArcadeLabel(overlay.transform, "最高分 0000000", 38, TextAnchor.MiddleCenter, new Color(0.54f, 0.85f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.32f), new Vector2(0.85f, 0.4f), Vector2.zero);
            overlaySummary = UiFactory.CreateArcadeLabel(overlay.transform, "击落 0/0", 30, TextAnchor.MiddleCenter, new Color(1f, 0.72f, 0.82f), FontStyle.Bold, new Vector2(0.12f, 0.24f), new Vector2(0.88f, 0.31f), Vector2.zero);
        }

        private string BuildPowerupHudText()
        {
            if (!gameManager.HasActivePowerup)
            {
                return $"火力 Lv{gameManager.FireLevel} 普通\n{BuildBombIcons()}";
            }

            string name;
            switch (gameManager.ActivePowerupType)
            {
                case AmmoPowerupType.Scatter:
                    name = "红色散射";
                    break;
                case AmmoPowerupType.RapidFire:
                    name = "红色连发";
                    break;
                case AmmoPowerupType.Pierce:
                    name = "蓝色穿透";
                    break;
                case AmmoPowerupType.Laser:
                    name = "蓝色激光";
                    break;
                case AmmoPowerupType.Plasma:
                    name = "紫色等离子";
                    break;
                case AmmoPowerupType.Burst:
                    name = "红色爆裂";
                    break;
                case AmmoPowerupType.Homing:
                    name = "蓝色追踪";
                    break;
                case AmmoPowerupType.Wave:
                    name = "紫色波刃";
                    break;
                case AmmoPowerupType.Guard:
                    name = "紫色护航";
                    break;
                default:
                    name = "普通";
                    break;
            }

            return $"火力 Lv{gameManager.FireLevel} {name} {gameManager.ActivePowerupRemainingSeconds:0.0}s\n{BuildBombIcons()}";
        }

        private string BuildBombIcons()
        {
            int active = Mathf.Clamp(gameManager.BombCount, 0, 3);
            char[] icons = { '◇', '◇', '◇' };
            for (int i = 0; i < active; i++)
            {
                icons[i] = '◆';
            }

            return $"炸弹 {new string(icons)}";
        }

        private string BuildDifficultyText()
        {
            switch (gameManager.Difficulty)
            {
                case GameDifficulty.Medium:
                    return $"中级  L{gameManager.LoopNumber}-{gameManager.StageNumber} {gameManager.StageName}";
                case GameDifficulty.High:
                    return $"高级  L{gameManager.LoopNumber}-{gameManager.StageNumber} {gameManager.StageName}";
                default:
                    return $"低级  L{gameManager.LoopNumber}-{gameManager.StageNumber} {gameManager.StageName}";
            }
        }

        private IEnumerator ShowBossWarningSequence()
        {
            bossWarningOverlay.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            bossWarningOverlay.gameObject.SetActive(false);
        }
    }
}
