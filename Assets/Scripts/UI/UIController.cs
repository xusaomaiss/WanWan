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
        private Text comboText;
        private Text powerupText;
        private Text pauseHintText;
        private Text stageBannerText;
        private Image playerHealthRoot;
        private Image playerHealthFill;
        private Image playerHealthGlow;
        private Text playerHealthLabel;
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
        private float playerDamageFlashTimer;

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
            livesText.text = $"装甲 {BuildHealthBar()}\n{BuildBombIcons()}";
            RefreshPlayerHealthBar();
            highScoreText.text = $"最高分 {SessionState.HighScore:0000000}";
            difficultyText.text = $"第{gameManager.LoopNumber}轮-{gameManager.StageNumber}关 {gameManager.StageName}";
            comboText.text = gameManager.GetComboDisplayText();
            stageProgressText.text = $"{(gameManager.StageProgress * 100f):0}%  击落 {gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}";
            if (stageProgressFill != null)
            {
                RectTransform fillRect = stageProgressFill.rectTransform;
                fillRect.anchorMax = new Vector2(gameManager.StageProgress, 1f);
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
            }
            powerupText.text = BuildPowerupHudText();
            pauseHintText.text = gameManager.IsPaused ? "已暂停" : string.Empty;
            string banner = gameManager.StageBannerText ?? string.Empty;
            stageBannerText.text = banner;
            bool hasBanner = !string.IsNullOrEmpty(banner);
            stageBannerText.gameObject.SetActive(hasBanner);
            stageBannerText.color = banner.Contains("警报")
                ? new Color(1f, 0.2f, 0.12f, 0.98f)
                : new Color(1f, 0.95f, 0.54f, 0.96f);
            pauseButtonText.text = gameManager.IsPaused ? "▶" : "Ⅱ";
            pauseOverlay.gameObject.SetActive(gameManager.IsPaused);
            pauseOverlayHighScore.text = $"最高分 {SessionState.HighScore:0000000}";
            pauseOverlayMission.text = $"第{gameManager.StageNumber}关 {gameManager.StageName}";
            pauseOverlayAudioText.text = "设置";
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

        public void NotifyPlayerDamaged()
        {
            playerDamageFlashTimer = 0.42f;
        }

        public void ShowGameOverOverlay(string title, int score)
        {
            overlay.gameObject.SetActive(true);
            overlayTitle.text = title;
            overlayStage.text = $"第{gameManager.LoopNumber}轮-{gameManager.StageNumber}关 {gameManager.StageName}  {BuildDifficultyText()}";
            overlayScore.text = $"本局得分 {score:0000000}";
            overlayBestScore.text = $"最高分 {SessionState.HighScore:0000000}";
            overlaySummary.text = $"击落 {gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}  炸弹 {gameManager.BombsUsed}  目标 {gameManager.BossDisplayName}\n最高连击 {gameManager.MaxCombo}  最高倍率 {gameManager.MaxComboMultiplier}倍";
            if (title == "任务完成")
            {
                overlayBestScore.text = $"本关得分 {score:0000000}  击落 {gameManager.EnemiesDestroyed}";
                overlaySummary.text = $"使用炸弹 {gameManager.BombsUsed}  下一关 {StageCatalog.GetStage(StageCatalog.GetNextStageIndex(SessionState.CurrentStageIndex)).Name}\n最高连击 {gameManager.MaxCombo}  最高倍率 {gameManager.MaxComboMultiplier}倍";
            }
        }

        public void ShowIntroPrompt()
        {
            if (introOverlay == null)
            {
                return;
            }

            introOverlay.gameObject.SetActive(true);
            introTitle.text = "起飞准备";
            introCountdown.text = "点击跳过";
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

            bossWarningTitle.text = $"警报\n{bossDisplayName} 来袭";
            StartCoroutine(ShowBossWarningSequence());
        }

        private void Build()
        {
            Canvas canvas = UiFactory.CreateCanvas("GameCanvas");
            canvas.transform.SetParent(transform, false);

            Image topBar = UiFactory.CreatePanel(canvas.transform, "TopHud", new Color(0.02f, 0.02f, 0.04f, 0.42f), new Vector2(0f, 0.94f), Vector2.one);
            UiFactory.CreateDivider(topBar.transform, "HudBottomLine", new Color(0.14f, 0.88f, 1f, 0.58f), Vector2.zero, new Vector2(1f, 0.045f));

            scoreText = UiFactory.CreateArcadeLabel(topBar.transform, "得分 0000000\n金币 000", 30, TextAnchor.UpperLeft, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.03f, 0.1f), new Vector2(0.32f, 0.96f), Vector2.zero);
            highScoreText = UiFactory.CreateArcadeLabel(topBar.transform, "最高分 0000000", 28, TextAnchor.UpperCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.34f, 0.58f), new Vector2(0.66f, 0.98f), Vector2.zero);
            difficultyText = UiFactory.CreateArcadeLabel(topBar.transform, "第1轮-1关 乡村", 20, TextAnchor.MiddleCenter, new Color(0.78f, 0.88f, 1f), FontStyle.Bold, new Vector2(0.33f, 0.24f), new Vector2(0.67f, 0.62f), Vector2.zero);
            stageProgressText = UiFactory.CreateArcadeLabel(topBar.transform, "第1关 0%  击落 0/0", 20, TextAnchor.LowerCenter, new Color(0.96f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.32f, 0.04f), new Vector2(0.68f, 0.28f), Vector2.zero);
            comboText = UiFactory.CreateArcadeLabel(topBar.transform, "连击 0  倍率 1倍", 22, TextAnchor.LowerCenter, new Color(1f, 0.86f, 0.32f), FontStyle.Bold, new Vector2(0.36f, -0.42f), new Vector2(0.64f, -0.08f), Vector2.zero);
            livesText = UiFactory.CreateArcadeLabel(topBar.transform, "装甲 ■■■■■■■■■■\n炸弹 ◇◇◇", 26, TextAnchor.UpperRight, new Color(0.96f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.68f, 0.1f), new Vector2(0.91f, 0.96f), Vector2.zero);
            pauseButton = UiFactory.CreateButton(topBar.transform, "Ⅱ", ArcadeTheme.WarningRed, Color.white, new Vector2(54f, 54f), new Vector2(-34f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
            pauseButton.onClick.AddListener(() => gameManager.TogglePause());
            pauseButtonText = pauseButton.GetComponentInChildren<Text>();
            pauseButtonText.fontStyle = FontStyle.Bold;
            pauseButtonText.fontSize = 30;

            Image bottomBar = UiFactory.CreatePixelPanel(canvas.transform, "BottomHud", new Color(0.08f, 0.08f, 0.16f, SessionState.VirtualButtonOpacity + 0.25f), ArcadeTheme.DimGray, new Vector2(0f, 0f), new Vector2(1f, 0.065f), new Vector2(6f, 6f));
            Image weaponSlot = UiFactory.CreatePixelPanel(bottomBar.transform, "WeaponSlot", new Color(0.04f, 0.04f, 0.1f, 0.95f), ArcadeTheme.MilitaryGreen, new Vector2(0.04f, 0.18f), new Vector2(0.36f, 0.82f), new Vector2(4f, 4f));
            powerupText = UiFactory.CreateArcadeLabel(weaponSlot.transform, "武器 扇形弹 1级", 22, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.03f, 0f), new Vector2(0.97f, 1f), Vector2.zero);
            Image progressTrack = UiFactory.CreatePanel(bottomBar.transform, "ProgressTrack", ArcadeTheme.InkBlack, new Vector2(0.39f, 0.36f), new Vector2(0.67f, 0.64f));
            stageProgressFill = UiFactory.CreatePanel(progressTrack.transform, "ProgressFill", ArcadeTheme.EnergyYellow, Vector2.zero, Vector2.one);
            UiFactory.CreateArcadeLabel(bottomBar.transform, "吃炸弹图标清屏", 24, TextAnchor.MiddleCenter, new Color(0.7f, 0.9f, 1f), FontStyle.Bold, new Vector2(0.7f, 0.12f), new Vector2(0.96f, 0.88f), Vector2.zero);

            bossBarRoot = UiFactory.CreatePixelPanel(canvas.transform, "BossBarRoot", new Color(0.14f, 0.03f, 0.08f, 0.92f), ArcadeTheme.WarningRed, new Vector2(0.05f, 0.86f), new Vector2(0.95f, 0.9f), new Vector2(6f, 6f));
            bossBarRoot.gameObject.SetActive(false);
            bossBarFill = UiFactory.CreatePanel(bossBarRoot.transform, "BossBarFill", new Color(1f, 0.44f, 0.3f, 0.98f), new Vector2(0.012f, 0.14f), new Vector2(0.988f, 0.86f));
            bossBarLabel = UiFactory.CreateArcadeLabel(bossBarRoot.transform, "警报  敌方旗舰", 24, TextAnchor.MiddleCenter, new Color(1f, 0.96f, 0.94f), FontStyle.Bold, new Vector2(0.05f, 0f), new Vector2(0.95f, 1f), Vector2.zero);

            playerHealthRoot = UiFactory.CreatePixelPanel(canvas.transform, "PlayerHealthFocusBar", new Color(0.03f, 0.04f, 0.08f, 0.88f), new Color(0.2f, 0.9f, 1f, 0.95f), new Vector2(0.14f, 0.815f), new Vector2(0.86f, 0.855f), new Vector2(5f, 5f));
            Image healthTrack = UiFactory.CreatePanel(playerHealthRoot.transform, "PlayerHealthTrack", new Color(0.02f, 0.015f, 0.018f, 0.96f), new Vector2(0.02f, 0.22f), new Vector2(0.98f, 0.78f));
            playerHealthFill = UiFactory.CreatePanel(healthTrack.transform, "PlayerHealthFill", new Color(0.22f, 1f, 0.62f, 0.98f), Vector2.zero, Vector2.one);
            playerHealthGlow = UiFactory.CreatePanel(playerHealthRoot.transform, "PlayerHealthDamageFlash", new Color(1f, 0.06f, 0.02f, 0f), Vector2.zero, Vector2.one);
            playerHealthGlow.raycastTarget = false;
            playerHealthLabel = UiFactory.CreateArcadeLabel(playerHealthRoot.transform, "装甲 10/10", 22, TextAnchor.MiddleCenter, new Color(0.98f, 1f, 1f), FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            playerHealthRoot.gameObject.SetActive(false);

            pauseHintText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 40, TextAnchor.MiddleCenter, new Color(0.6f, 0.84f, 1f, 0.92f), FontStyle.Bold, new Vector2(0.3f, 0.79f), new Vector2(0.7f, 0.84f), Vector2.zero);
            stageBannerText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 48, TextAnchor.MiddleCenter, new Color(1f, 0.95f, 0.54f, 0.94f), FontStyle.Bold, new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.61f), Vector2.zero);
            Outline stageBannerOutline = stageBannerText.gameObject.AddComponent<Outline>();
            stageBannerOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            stageBannerOutline.effectDistance = new Vector2(3f, -3f);
            stageBannerText.gameObject.SetActive(false);

            pauseOverlay = UiFactory.CreatePanel(canvas.transform, "PauseOverlay", new Color(0.01f, 0.02f, 0.04f, 0.58f), Vector2.zero, Vector2.one);
            pauseOverlay.raycastTarget = false;
            pauseOverlay.gameObject.SetActive(false);
            Image pauseCard = UiFactory.CreatePanel(pauseOverlay.transform, "PauseFrame", Color.clear, new Vector2(0.2f, 0.34f), new Vector2(0.8f, 0.66f));
            pauseCard.raycastTarget = false;
            UiFactory.CreateDivider(pauseCard.transform, "PauseTopNeon", ArcadeTheme.ElectricBlue, new Vector2(0f, 0.965f), Vector2.one);
            UiFactory.CreateDivider(pauseCard.transform, "PauseBottomNeon", ArcadeTheme.ElectricBlue, Vector2.zero, new Vector2(1f, 0.035f));
            UiFactory.CreateDivider(pauseCard.transform, "PauseLeftNeon", ArcadeTheme.ElectricBlue, Vector2.zero, new Vector2(0.035f, 1f));
            UiFactory.CreateDivider(pauseCard.transform, "PauseRightNeon", ArcadeTheme.ElectricBlue, new Vector2(0.965f, 0f), Vector2.one);
            Image pauseSelection = UiFactory.CreatePanel(pauseCard.transform, "PauseSelection", new Color(0.85f, 0f, 0.04f, 0.54f), new Vector2(0.18f, 0.55f), new Vector2(0.82f, 0.69f));
            pauseSelection.raycastTarget = false;
            pauseOverlayTitle = UiFactory.CreateArcadeLabel(pauseCard.transform, "暂停菜单", 42, TextAnchor.MiddleCenter, new Color(0.92f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.76f), new Vector2(0.88f, 0.9f), Vector2.zero);
            pauseOverlayHighScore = UiFactory.CreateArcadeLabel(pauseCard.transform, "最高分 0000000", 22, TextAnchor.MiddleCenter, new Color(0.48f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.16f, 0.08f), new Vector2(0.84f, 0.18f), Vector2.zero);
            pauseOverlayMission = UiFactory.CreateArcadeLabel(pauseCard.transform, string.Empty, 18, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 1f, 0.78f), FontStyle.Bold, new Vector2(0.12f, 0.18f), new Vector2(0.88f, 0.28f), Vector2.zero);
            Button resumeButton = UiFactory.CreateButton(pauseCard.transform, "继续游戏", Color.clear, ArcadeTheme.White, new Vector2(420f, 68f), new Vector2(0f, 78f));
            resumeButton.onClick.AddListener(() => gameManager.TogglePause());
            Button audioButton = UiFactory.CreateButton(pauseCard.transform, "设置", Color.clear, ArcadeTheme.White, new Vector2(420f, 68f), new Vector2(0f, -12f));
            pauseOverlayAudioText = audioButton.GetComponentInChildren<Text>();
            audioButton.onClick.AddListener(() => gameManager.ToggleAudio());
            Button menuButton = UiFactory.CreateButton(pauseCard.transform, "退出", Color.clear, ArcadeTheme.White, new Vector2(420f, 68f), new Vector2(0f, -102f));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);

            introOverlay = UiFactory.CreatePanel(canvas.transform, "LevelIntroOverlay", new Color(0.02f, 0.02f, 0.06f, 0.18f), Vector2.zero, Vector2.one);
            introOverlay.raycastTarget = false;
            introTitle = UiFactory.CreateArcadeLabel(introOverlay.transform, "第1关", ArcadeTheme.ScreenTitleSize, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.08f, 0.55f), new Vector2(0.92f, 0.68f), Vector2.zero);
            introCountdown = UiFactory.CreateArcadeLabel(introOverlay.transform, "点击跳过", 34, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, new Vector2(0.12f, 0.34f), new Vector2(0.88f, 0.44f), Vector2.zero);
            introOverlay.gameObject.SetActive(false);

            bossWarningOverlay = UiFactory.CreatePanel(canvas.transform, "BossWarningOverlay", Color.clear, Vector2.zero, Vector2.one);
            bossWarningOverlay.raycastTarget = false;
            bossWarningOverlay.gameObject.SetActive(false);
            bossWarningTitle = UiFactory.CreateArcadeLabel(bossWarningOverlay.transform, "警报", 64, TextAnchor.MiddleCenter, ArcadeTheme.WarningRed, FontStyle.Bold, new Vector2(0.06f, 0.58f), new Vector2(0.94f, 0.7f), Vector2.zero);
            Outline bossWarningOutline = bossWarningTitle.gameObject.AddComponent<Outline>();
            bossWarningOutline.effectColor = new Color(0f, 0f, 0f, 0.92f);
            bossWarningOutline.effectDistance = new Vector2(4f, -4f);

            overlay = UiFactory.CreatePanel(canvas.transform, "Overlay", new Color(0.02f, 0.05f, 0.11f, 0.8f), Vector2.zero, Vector2.one);
            overlay.gameObject.SetActive(false);
            overlayTitle = UiFactory.CreateArcadeLabel(overlay.transform, "任务失败", 84, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold, new Vector2(0.15f, 0.58f), new Vector2(0.85f, 0.72f), Vector2.zero);
            overlayStage = UiFactory.CreateArcadeLabel(overlay.transform, "第1轮-1关 乡村", 34, TextAnchor.MiddleCenter, new Color(0.7f, 0.88f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.5f), new Vector2(0.88f, 0.57f), Vector2.zero);
            overlayScore = UiFactory.CreateArcadeLabel(overlay.transform, "本局得分 0000000", 46, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.4f), new Vector2(0.85f, 0.49f), Vector2.zero);
            overlayBestScore = UiFactory.CreateArcadeLabel(overlay.transform, "最高分 0000000", 38, TextAnchor.MiddleCenter, new Color(0.54f, 0.85f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.32f), new Vector2(0.85f, 0.4f), Vector2.zero);
            overlaySummary = UiFactory.CreateArcadeLabel(overlay.transform, "击落 0/0", 30, TextAnchor.MiddleCenter, new Color(1f, 0.72f, 0.82f), FontStyle.Bold, new Vector2(0.12f, 0.22f), new Vector2(0.88f, 0.31f), Vector2.zero);
        }

        private string BuildPowerupHudText()
        {
            return $"{gameManager.GetCurrentWeaponDisplayText()}\n{BuildBombIcons()}";
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

        private void RefreshPlayerHealthBar()
        {
            if (playerHealthFill == null || gameManager == null)
            {
                return;
            }

            float fillAmount = PlayerHealthHudPresentation.GetFillAmount(gameManager.PlayerHealth, gameManager.MaxPlayerHealth);
            RectTransform fillRect = playerHealthFill.rectTransform;
            fillRect.anchorMax = new Vector2(fillAmount, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            playerHealthFill.color = PlayerHealthHudPresentation.GetFillColor(fillAmount);

            if (playerHealthLabel != null)
            {
                playerHealthLabel.text = $"装甲 {gameManager.PlayerHealth}/{gameManager.MaxPlayerHealth}";
            }

            if (playerDamageFlashTimer > 0f)
            {
                playerDamageFlashTimer = Mathf.Max(0f, playerDamageFlashTimer - Time.deltaTime);
            }

            if (playerHealthGlow != null)
            {
                Color flash = playerHealthGlow.color;
                flash.a = Mathf.Clamp01(playerDamageFlashTimer / 0.42f) * 0.58f;
                playerHealthGlow.color = flash;
            }
        }

        private string BuildHealthBar()
        {
            int max = Mathf.Max(1, gameManager.MaxPlayerHealth);
            int current = Mathf.Clamp(gameManager.PlayerHealth, 0, max);
            char[] segments = new char[max];
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = i < current ? '■' : '□';
            }

            return new string(segments);
        }

        private string BuildDifficultyText()
        {
            switch (gameManager.Difficulty)
            {
                case GameDifficulty.Medium:
                    return $"中级  第{gameManager.LoopNumber}轮-{gameManager.StageNumber}关 {gameManager.StageName}";
                case GameDifficulty.High:
                    return $"高级  第{gameManager.LoopNumber}轮-{gameManager.StageNumber}关 {gameManager.StageName}";
                default:
                    return $"低级  第{gameManager.LoopNumber}轮-{gameManager.StageNumber}关 {gameManager.StageName}";
            }
        }

        private IEnumerator ShowBossWarningSequence()
        {
            bossWarningOverlay.gameObject.SetActive(true);
            RectTransform warningRect = bossWarningTitle.rectTransform;
            Vector3 startScale = Vector3.one;
            float elapsed = 0f;
            const float duration = 1.35f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float pulse = 1f + Mathf.Sin(t * Mathf.PI * 5f) * 0.06f;
                warningRect.localScale = startScale * pulse;
                Color color = bossWarningTitle.color;
                color.a = Mathf.Lerp(1f, 0f, Mathf.Clamp01((t - 0.72f) / 0.28f));
                bossWarningTitle.color = color;
                yield return null;
            }

            warningRect.localScale = startScale;
            Color reset = bossWarningTitle.color;
            reset.a = 1f;
            bossWarningTitle.color = reset;
            bossWarningOverlay.gameObject.SetActive(false);
        }
    }
}
