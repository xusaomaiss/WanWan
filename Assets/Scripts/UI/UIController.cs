using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wanwan.Runtime
{
    public class UIController : MonoBehaviour
    {
        public const float TopHudAnchorHeight = 0.1f;
        public const float BottomHudAnchorHeight = 0.105f;
        public const float RightStageProgressAnchorWidth = 0.045f;
        public const int PowerMeterSlotCount = 6;
        public static readonly Vector2 UpgradeButtonSize = new Vector2(176f, 72f);
        public static readonly bool BottomHudTextUsesConstrainedWrapping = true;
        public static readonly string[] TopHudStatLabels = { "SCORE", "COIN", "BOMB", "FIRE" };
        private const float PowerMeterPulseDuration = 0.38f;

        private GameManager gameManager;
        private Text scoreText;
        private Text livesText;
        private Text highScoreText;
        private Text coinCountText;
        private Text bombCountText;
        private Text fireLevelText;
        private Image bombIcon;
        private Text difficultyText;
        private Text stageProgressText;
        private Text stageProgressPercentText;
        private Text comboText;
        private Text comboMultiplierText;
        private Text powerupText;
        private Text powerMeterText;
        private Text pauseHintText;
        private Text stageBannerText;
        private Image weaponIcon;
        private Image playerHealthRoot;
        private Image playerHealthFill;
        private Image playerHealthGlow;
        private Text playerHealthLabel;
        private Button pauseButton;
        private Text pauseButtonText;
        private Button upgradeButton;
        private Text upgradeButtonText;
        private Image pauseOverlay;
        private Text pauseOverlayTitle;
        private Text pauseOverlayHighScore;
        private Text pauseOverlayMission;
        private Text pauseOverlayAudioText;
        private Image stageProgressFill;
        private Image stageProgressGlow;
        private readonly Image[] powerMeterSlotFills = new Image[PowerMeterSlotCount];
        private readonly Image[] powerMeterSlotGlows = new Image[PowerMeterSlotCount];
        private readonly Text[] powerMeterSlotLabels = new Text[PowerMeterSlotCount];
        private int displayedPowerMeterCapsules = -1;
        private int pulsingPowerMeterSlot = -1;
        private float powerMeterPulseTimer;
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
        private Image vignetteTop;
        private Image vignetteBottom;

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

            scoreText.text = $"{TopHudStatLabels[0]} {gameManager.Score:0000000}";
            coinCountText.text = $"{TopHudStatLabels[1]} {gameManager.CoinCount:000}";
            livesText.text = $"装甲 {BuildHealthBar()}";
            UpdateBombDisplay();
            if (fireLevelText != null)
            {
                fireLevelText.text = $"{TopHudStatLabels[3]} LV.{gameManager.FireLevel}";
            }
            RefreshPlayerHealthBar();
            highScoreText.text = $"最高分 {SessionState.HighScore:0000000}";
            difficultyText.text = $"第{gameManager.LoopNumber}轮-{gameManager.StageNumber}关 {gameManager.StageName}";
            comboText.text = gameManager.GetComboDisplayText();
            RefreshComboDisplay();
            stageProgressText.text = $"{gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}";
            if (stageProgressFill != null)
            {
                RectTransform fillRect = stageProgressFill.rectTransform;
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = new Vector2(1f, gameManager.StageProgress);
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
            }
            if (stageProgressGlow != null)
            {
                RectTransform glowRect = stageProgressGlow.rectTransform;
                glowRect.anchorMin = Vector2.zero;
                glowRect.anchorMax = new Vector2(1f, gameManager.StageProgress);
                glowRect.offsetMin = Vector2.zero;
                glowRect.offsetMax = Vector2.zero;
                Color glow = stageProgressGlow.color;
                glow.a = Mathf.Lerp(0.18f, 0.62f, gameManager.StageProgress);
                stageProgressGlow.color = glow;
            }
            if (stageProgressPercentText != null)
            {
                stageProgressPercentText.text = $"{(gameManager.StageProgress * 100f):0}%";
            }
            powerupText.text = BuildPowerupHudText();
            RefreshWeaponIcon();
            if (powerMeterText != null)
            {
                powerMeterText.text = string.Empty;
            }
            RefreshPowerMeterVisuals();
            if (upgradeButton != null)
            {
                upgradeButton.interactable = gameManager.CanActivatePowerMeter;
                upgradeButtonText.text = gameManager.CanActivatePowerMeter ? "升级" : "充能";
            }
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

        private void Update()
        {
            if (powerMeterPulseTimer <= 0f)
            {
                return;
            }

            powerMeterPulseTimer = Mathf.Max(0f, powerMeterPulseTimer - Time.deltaTime);
            RefreshPowerMeterVisuals();
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

            Image topBar = UiFactory.CreatePanel(canvas.transform, "TopHud", new Color(0.015f, 0.018f, 0.035f, 0.65f), new Vector2(0f, 1f - TopHudAnchorHeight), Vector2.one);
            topBar.sprite = RuntimeSpriteFactory.GetSciFiHudSprite(SciFiHudSpriteKind.TopFrame);
            UiFactory.CreateDivider(topBar.transform, "HudBottomLine", new Color(0.14f, 0.88f, 1f, 0.65f), Vector2.zero, new Vector2(1f, 0.03f));

            scoreText = UiFactory.CreateArcadeLabel(topBar.transform, "SCORE 0000000", 25, TextAnchor.MiddleLeft, new Color(0.08f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.018f, 0.54f), new Vector2(0.32f, 0.92f), Vector2.zero);
            highScoreText = UiFactory.CreateArcadeLabel(topBar.transform, "BEST 0000000", 15, TextAnchor.MiddleLeft, new Color(0.3f, 0.65f, 0.95f), FontStyle.Bold, new Vector2(0.018f, 0.16f), new Vector2(0.32f, 0.48f), Vector2.zero);
            coinCountText = UiFactory.CreateArcadeLabel(topBar.transform, "COIN 000", 18, TextAnchor.MiddleLeft, new Color(1f, 0.85f, 0.1f, 0.88f), FontStyle.Bold, new Vector2(0.335f, 0.54f), new Vector2(0.48f, 0.9f), Vector2.zero);

            // Center: combo display
            Image comboBadge = UiFactory.CreateSpritePanel(topBar.transform, "ComboBadge", "UI/hud/combo_badge", new Color(1f, 0.4f, 0.2f, 0.25f), new Vector2(0.38f, 0.6f), new Vector2(0.62f, 0.94f));
            comboBadge.raycastTarget = false;
            comboMultiplierText = UiFactory.CreateArcadeLabel(topBar.transform, "2x", 36, TextAnchor.MiddleCenter, ArcadeTheme.ComboYellow, FontStyle.Bold, new Vector2(0.4f, 0.68f), new Vector2(0.6f, 0.98f), Vector2.zero);
            comboMultiplierText.gameObject.SetActive(false);
            comboText = UiFactory.CreateArcadeLabel(topBar.transform, "连击 0  倍率 1倍", 17, TextAnchor.MiddleCenter, new Color(1f, 0.86f, 0.32f), FontStyle.Bold, new Vector2(0.38f, 0.36f), new Vector2(0.62f, 0.62f), Vector2.zero);

            // Right side: bombs, fire level, shields, difficulty, stage progress
            livesText = UiFactory.CreateArcadeLabel(topBar.transform, "装甲 ■■■■■■■■■■", 16, TextAnchor.MiddleRight, new Color(0.96f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.67f, 0.45f), new Vector2(0.94f, 0.72f), Vector2.zero);
            bombIcon = UiFactory.CreatePanel(topBar.transform, "BombIcon", new Color(1f, 0.8f, 0.2f, 0.9f), new Vector2(0.67f, 0.18f), new Vector2(0.705f, 0.44f));
            bombIcon.sprite = RuntimeSpriteFactory.GetSciFiHudSprite(SciFiHudSpriteKind.IconBomb);
            bombIcon.preserveAspect = true;
            bombIcon.raycastTarget = false;
            bombCountText = UiFactory.CreateArcadeLabel(topBar.transform, "BOMB 03", 17, TextAnchor.MiddleLeft, new Color(1f, 0.7f, 0.12f), FontStyle.Bold, new Vector2(0.708f, 0.17f), new Vector2(0.80f, 0.45f), Vector2.zero);
            fireLevelText = UiFactory.CreateArcadeLabel(topBar.transform, "FIRE LV.1", 17, TextAnchor.MiddleLeft, new Color(0.34f, 1f, 0.86f), FontStyle.Bold, new Vector2(0.80f, 0.17f), new Vector2(0.94f, 0.45f), Vector2.zero);

            difficultyText = UiFactory.CreateArcadeLabel(topBar.transform, "第1轮-1关 乡村", 14, TextAnchor.MiddleRight, new Color(0.78f, 0.88f, 1f), FontStyle.Bold, new Vector2(0.48f, 0.0f), new Vector2(0.78f, 0.18f), Vector2.zero);
            stageProgressText = UiFactory.CreateArcadeLabel(topBar.transform, "击落 0/0", 13, TextAnchor.MiddleRight, new Color(0.9f, 0.92f, 1f, 0.65f), FontStyle.Bold, new Vector2(0.78f, 0.0f), new Vector2(0.94f, 0.18f), Vector2.zero);

            pauseButton = UiFactory.CreateSciFiWideButton(topBar.transform, "Ⅱ", ArcadeTheme.ElectricBlue, new Vector2(54f, 48f), new Vector2(-28f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
            pauseButton.onClick.AddListener(() => gameManager.TogglePause());
            pauseButtonText = pauseButton.GetComponentInChildren<Text>();
            pauseButtonText.fontStyle = FontStyle.Bold;
            pauseButtonText.fontSize = 28;

            // HUD bottom bar with sprite background
            UiFactory.CreateSpritePanel(canvas.transform, "BottomHudBg", "UI/hud/hud_bottom_bar", new Color(1f, 1f, 1f, 0.25f), new Vector2(0f, 0f), new Vector2(1f, BottomHudAnchorHeight));
            Image bottomBar = UiFactory.CreatePixelPanel(canvas.transform, "BottomHud", new Color(0.06f, 0.06f, 0.14f, SessionState.VirtualButtonOpacity + 0.32f), ArcadeTheme.DimGray, new Vector2(0f, 0f), new Vector2(1f, BottomHudAnchorHeight), new Vector2(6f, 6f));
            bottomBar.sprite = RuntimeSpriteFactory.GetHudDecorSprite(1);
            Image weaponSlot = UiFactory.CreatePixelPanel(bottomBar.transform, "WeaponSlot", new Color(0.04f, 0.04f, 0.1f, 0.95f), ArcadeTheme.MilitaryGreen, new Vector2(0.025f, 0.18f), new Vector2(0.25f, 0.86f), new Vector2(4f, 4f));
            weaponIcon = UiFactory.CreateSpritePanel(weaponSlot.transform, "WeaponIcon", "UI/icons/icon_scatter", Color.white, new Vector2(0.12f, 0.18f), new Vector2(0.36f, 0.82f));
            weaponIcon.raycastTarget = false;
            powerupText = UiFactory.CreateArcadeLabel(weaponSlot.transform, "武器 扇形弹 1级", 18, TextAnchor.MiddleCenter, ArcadeTheme.White, FontStyle.Bold, new Vector2(0.03f, 0f), new Vector2(0.97f, 1f), Vector2.zero);
            UiFactory.ConfigureConstrainedText(powerupText, 11, 18);
            Image meterSlot = UiFactory.CreatePixelPanel(bottomBar.transform, "PowerMeterSlot", new Color(0.035f, 0.035f, 0.08f, 0.95f), ArcadeTheme.EnergyYellow, new Vector2(0.27f, 0.2f), new Vector2(0.73f, 0.84f), new Vector2(4f, 4f));
            CreatePowerMeterVisuals(meterSlot.transform);
            for (int i = 1; i < PowerMeterSlotCount; i++)
            {
                float x = i / (float)PowerMeterSlotCount;
                UiFactory.CreateDivider(meterSlot.transform, "PowerMeterDivider" + i, new Color(1f, 0.88f, 0.18f, 0.28f), new Vector2(x - 0.003f, 0.12f), new Vector2(x + 0.003f, 0.88f));
            }
            powerMeterText = UiFactory.CreateArcadeLabel(meterSlot.transform, string.Empty, 1, TextAnchor.MiddleCenter, Color.clear, FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            upgradeButton = UiFactory.CreateSciFiWideButton(bottomBar.transform, "充能", ArcadeTheme.ElectricBlue, UpgradeButtonSize, new Vector2(-112f, 0f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f));
            upgradeButton.onClick.AddListener(() => gameManager.TryActivatePowerMeter());
            upgradeButtonText = upgradeButton.GetComponentInChildren<Text>();
            upgradeButtonText.fontStyle = FontStyle.Bold;
            upgradeButtonText.fontSize = 24;
            CreateStageProgressThermometer(canvas.transform);

            // Boss bar with sprite background
            bossBarRoot = UiFactory.CreateSpritePanel(canvas.transform, "BossBarRoot", "UI/hud/boss_bar_frame", new Color(1f, 0.2f, 0.15f, 0.5f), new Vector2(0.08f, 0.88f), new Vector2(0.92f, 0.92f));
            bossBarRoot.gameObject.SetActive(false);
            UiFactory.CreateDivider(bossBarRoot.transform, "BossBarBorder", new Color(0.14f, 0.88f, 1f, 0.75f), Vector2.zero, Vector2.one);
            Image bossTrack = UiFactory.CreatePanel(bossBarRoot.transform, "BossTrack", new Color(0.01f, 0.01f, 0.02f, 0.95f), new Vector2(0.02f, 0.15f), new Vector2(0.98f, 0.85f));
            bossTrack.raycastTarget = false;
            bossBarFill = UiFactory.CreatePanel(bossTrack.transform, "BossBarFill", new Color(1f, 0.25f, 0.18f, 0.95f), Vector2.zero, Vector2.one);
            bossBarFill.raycastTarget = false;
            bossBarLabel = UiFactory.CreateArcadeLabel(bossBarRoot.transform, "警报  敌方旗舰", 20, TextAnchor.MiddleCenter, new Color(1f, 0.96f, 0.94f), FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            bossBarLabel.raycastTarget = false;

            UiFactory.CreateSpritePanel(canvas.transform, "HealthBarBg", "UI/hud/health_bar_frame", new Color(0.3f, 0.8f, 1f, 0.3f), new Vector2(0.1f, 0.822f), new Vector2(0.9f, 0.852f)).raycastTarget = false;
            playerHealthRoot = UiFactory.CreatePixelPanel(canvas.transform, "PlayerHealthFocusBar", new Color(0.03f, 0.04f, 0.08f, 0.72f), new Color(0.2f, 0.9f, 1f, 0.7f), new Vector2(0.1f, 0.822f), new Vector2(0.9f, 0.852f), new Vector2(4f, 4f));
            Image healthTrack = UiFactory.CreatePanel(playerHealthRoot.transform, "PlayerHealthTrack", new Color(0.02f, 0.015f, 0.018f, 0.96f), new Vector2(0.02f, 0.28f), new Vector2(0.98f, 0.72f));
            playerHealthFill = UiFactory.CreatePanel(healthTrack.transform, "PlayerHealthFill", new Color(0.22f, 1f, 0.62f, 0.98f), Vector2.zero, Vector2.one);
            playerHealthGlow = UiFactory.CreatePanel(playerHealthRoot.transform, "PlayerHealthDamageFlash", new Color(1f, 0.06f, 0.02f, 0f), Vector2.zero, Vector2.one);
            playerHealthGlow.raycastTarget = false;
            playerHealthLabel = UiFactory.CreateArcadeLabel(playerHealthRoot.transform, "装甲 10/10  专注 50%", 17, TextAnchor.MiddleCenter, new Color(0.92f, 0.98f, 1f, 0.85f), FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
            playerHealthRoot.gameObject.SetActive(false);

            vignetteTop = UiFactory.CreatePanel(canvas.transform, "DamageVignetteTop", new Color(1f, 0f, 0f, 0f), new Vector2(0f, 0.89f), Vector2.one);
            vignetteTop.raycastTarget = false;
            vignetteBottom = UiFactory.CreatePanel(canvas.transform, "DamageVignetteBottom", new Color(1f, 0f, 0f, 0f), Vector2.zero, new Vector2(1f, 0.11f));
            vignetteBottom.raycastTarget = false;

            pauseHintText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 34, TextAnchor.MiddleCenter, new Color(0.55f, 0.8f, 1f, 0.85f), FontStyle.Bold, new Vector2(0.32f, 0.72f), new Vector2(0.68f, 0.79f), Vector2.zero);
            stageBannerText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 46, TextAnchor.MiddleCenter, new Color(1f, 0.95f, 0.54f, 0.92f), FontStyle.Bold, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.58f), Vector2.zero);
            Outline stageBannerOutline = stageBannerText.gameObject.AddComponent<Outline>();
            stageBannerOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
            stageBannerOutline.effectDistance = new Vector2(3f, -3f);
            stageBannerText.gameObject.SetActive(false);

            pauseOverlay = UiFactory.CreatePanel(canvas.transform, "PauseOverlay", new Color(0.01f, 0.02f, 0.04f, 0.58f), Vector2.zero, Vector2.one);
            pauseOverlay.raycastTarget = false;
            pauseOverlay.gameObject.SetActive(false);
            Image pauseCard = UiFactory.CreatePanel(pauseOverlay.transform, "PauseFrame", Color.clear, new Vector2(0.2f, 0.34f), new Vector2(0.8f, 0.66f));
            pauseCard.raycastTarget = false;
            Image pauseSprite = UiFactory.CreateSpritePanel(pauseCard.transform, "PauseSpriteBg", "UI/hud/pause_panel", new Color(0.6f, 0.8f, 1f, 0.25f), Vector2.zero, Vector2.one);
            pauseSprite.raycastTarget = false;
            UiFactory.CreateDivider(pauseCard.transform, "PauseTopNeon", ArcadeTheme.ElectricBlue, new Vector2(0f, 0.965f), Vector2.one);
            UiFactory.CreateDivider(pauseCard.transform, "PauseBottomNeon", ArcadeTheme.ElectricBlue, Vector2.zero, new Vector2(1f, 0.035f));
            UiFactory.CreateDivider(pauseCard.transform, "PauseLeftNeon", ArcadeTheme.ElectricBlue, Vector2.zero, new Vector2(0.035f, 1f));
            UiFactory.CreateDivider(pauseCard.transform, "PauseRightNeon", ArcadeTheme.ElectricBlue, new Vector2(0.965f, 0f), Vector2.one);
            Image pauseSelection = UiFactory.CreatePanel(pauseCard.transform, "PauseSelection", new Color(0.85f, 0f, 0.04f, 0.54f), new Vector2(0.18f, 0.55f), new Vector2(0.82f, 0.69f));
            pauseSelection.raycastTarget = false;
            pauseOverlayTitle = UiFactory.CreateArcadeLabel(pauseCard.transform, "暂停菜单", 42, TextAnchor.MiddleCenter, new Color(0.92f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.76f), new Vector2(0.88f, 0.9f), Vector2.zero);
            pauseOverlayHighScore = UiFactory.CreateArcadeLabel(pauseCard.transform, "最高分 0000000", 22, TextAnchor.MiddleCenter, new Color(0.48f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.16f, 0.08f), new Vector2(0.84f, 0.18f), Vector2.zero);
            pauseOverlayMission = UiFactory.CreateArcadeLabel(pauseCard.transform, string.Empty, 18, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 1f, 0.78f), FontStyle.Bold, new Vector2(0.12f, 0.18f), new Vector2(0.88f, 0.28f), Vector2.zero);
            Button resumeButton = UiFactory.CreateSciFiWideButton(pauseCard.transform, "继续游戏", ArcadeTheme.ElectricBlue, new Vector2(420f, 68f), new Vector2(0f, 78f));
            resumeButton.onClick.AddListener(() => gameManager.TogglePause());
            Button restartButton = UiFactory.CreateSciFiWideButton(pauseCard.transform, "重新开始", ArcadeTheme.ElectricBlue, new Vector2(420f, 68f), new Vector2(0f, -12f));
            restartButton.onClick.AddListener(SceneNavigator.LoadGame);
            Button audioButton = UiFactory.CreateSciFiWideButton(pauseCard.transform, "设置", ArcadeTheme.ElectricBlue, new Vector2(420f, 68f), new Vector2(0f, -102f));
            pauseOverlayAudioText = audioButton.GetComponentInChildren<Text>();
            audioButton.onClick.AddListener(() => gameManager.ToggleAudio());
            Button menuButton = UiFactory.CreateSciFiWideButton(pauseCard.transform, "退出", ArcadeTheme.ElectricBlue, new Vector2(420f, 68f), new Vector2(0f, -192f));
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
            overlayScore = UiFactory.CreateArcadeLabel(overlay.transform, "本局得分 0000000", 46, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.38f), new Vector2(0.85f, 0.48f), Vector2.zero);
            overlayBestScore = UiFactory.CreateArcadeLabel(overlay.transform, "最高分 0000000", 38, TextAnchor.MiddleCenter, new Color(0.54f, 0.85f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.28f), new Vector2(0.85f, 0.36f), Vector2.zero);
            overlaySummary = UiFactory.CreateArcadeLabel(overlay.transform, "击落 0/0", 30, TextAnchor.MiddleCenter, new Color(1f, 0.72f, 0.82f), FontStyle.Bold, new Vector2(0.12f, 0.16f), new Vector2(0.88f, 0.26f), Vector2.zero);
        }

        private void RefreshComboDisplay()
        {
            if (comboMultiplierText == null) return;

            int multiplier = gameManager.CurrentComboMultiplier;
            if (multiplier <= 1)
            {
                if (comboMultiplierText.gameObject.activeSelf)
                    comboMultiplierText.gameObject.SetActive(false);
                return;
            }

            comboMultiplierText.gameObject.SetActive(true);
            comboMultiplierText.text = multiplier + "x";
            comboMultiplierText.color = multiplier >= 5 ? ArcadeTheme.ComboRed
                : multiplier >= 3 ? ArcadeTheme.ComboOrange
                : ArcadeTheme.ComboYellow;
            comboMultiplierText.fontSize = multiplier >= 5 ? 44 : multiplier >= 3 ? 36 : 30;
        }

        private void RefreshWeaponIcon()
        {
            if (weaponIcon == null || gameManager == null) return;
            string[] iconPaths = { "UI/icons/icon_scatter", "UI/icons/icon_rapid", "UI/icons/icon_laser",
                                   "UI/icons/icon_homing", "UI/icons/icon_burst", "UI/icons/icon_plasma" };
            int index = (int)gameManager.CurrentWeaponType;
            index = Mathf.Clamp(index, 0, iconPaths.Length - 1);
            Texture2D tex = Resources.Load<Texture2D>(iconPaths[index]);
            if (tex != null)
                weaponIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 128f);
        }

        private string BuildPowerupHudText()
        {
            string weaponLine = $"武器 {WeaponConfig.Get(gameManager.CurrentWeaponType).DisplayName} {gameManager.FireLevel}级";
            string modules = BuildModuleSummary();
            string mount = gameManager.HasActiveMount ? $" 挂载 {gameManager.CurrentMountHudText}" : string.Empty;
            return $"{weaponLine}\n{modules}{mount}".TrimEnd();
        }

        private string BuildModuleSummary()
        {
            WeaponModuleType[] modules = gameManager.CurrentWeaponModules;
            if (modules.Length == 0)
            {
                return "模块 -";
            }

            string text = "模块";
            for (int i = 0; i < modules.Length; i++)
            {
                text += " " + PowerupCycle.GetModuleLabel(modules[i]);
            }

            return text;
        }

        private void CreatePowerMeterVisuals(Transform meterSlot)
        {
            for (int i = 0; i < PowerMeterSlotCount; i++)
            {
                float start = i / (float)PowerMeterSlotCount;
                float end = (i + 1) / (float)PowerMeterSlotCount;
                Vector2 anchorMin = new Vector2(start + 0.012f, 0.18f);
                Vector2 anchorMax = new Vector2(end - 0.012f, 0.82f);

                Image glow = UiFactory.CreatePanel(meterSlot, "PowerMeterGlow" + i, new Color(1f, 0.92f, 0.18f, 0f), anchorMin, anchorMax);
                glow.raycastTarget = false;
                powerMeterSlotGlows[i] = glow;

                Image fill = UiFactory.CreatePanel(meterSlot, "PowerMeterFill" + i, new Color(1f, 0.84f, 0.12f, 0.1f), anchorMin, anchorMax);
                fill.raycastTarget = false;
                powerMeterSlotFills[i] = fill;

                Text label = UiFactory.CreateArcadeLabel(meterSlot, PowerMeterState.SlotLabels[i], 13, TextAnchor.MiddleCenter, ArcadeTheme.EnergyYellow, FontStyle.Bold, anchorMin, anchorMax, Vector2.zero);
                UiFactory.ConfigureConstrainedText(label, 8, 13);
                powerMeterSlotLabels[i] = label;
            }
        }

        private void CreateStageProgressThermometer(Transform canvas)
        {
            // Sprite background for stage progress thermometer
            Image sprRail = UiFactory.CreateSpritePanel(canvas, "StageProgressSprite", "UI/hud/progress_track", new Color(0.5f, 0.8f, 1f, 0.25f), new Vector2(1f - RightStageProgressAnchorWidth - 0.008f, 0.16f), new Vector2(0.985f + 0.008f, 0.80f));
            sprRail.raycastTarget = false;
            Image rail = UiFactory.CreatePixelPanel(canvas, "StageProgressThermometer", new Color(0.02f, 0.025f, 0.045f, 0.65f), new Color(0.22f, 0.86f, 1f, 0.65f), new Vector2(1f - RightStageProgressAnchorWidth, 0.18f), new Vector2(0.985f, 0.78f), new Vector2(3f, 5f));
            rail.raycastTarget = false;

            Image track = UiFactory.CreatePanel(rail.transform, "StageProgressTube", new Color(0.01f, 0.015f, 0.03f, 0.9f), new Vector2(0.3f, 0.08f), new Vector2(0.7f, 0.92f));
            track.raycastTarget = false;
            stageProgressFill = UiFactory.CreatePanel(track.transform, "StageProgressMercury", new Color(1f, 0.86f, 0.2f, 0.9f), Vector2.zero, Vector2.one);
            stageProgressFill.raycastTarget = false;
            stageProgressGlow = UiFactory.CreatePanel(track.transform, "StageProgressGlow", new Color(1f, 0.95f, 0.45f, 0.15f), Vector2.zero, Vector2.one);
            stageProgressGlow.raycastTarget = false;

            stageProgressPercentText = UiFactory.CreateArcadeLabel(rail.transform, "0%", 14, TextAnchor.MiddleCenter, new Color(0.85f, 0.92f, 1f), FontStyle.Bold, Vector2.zero, Vector2.one, Vector2.zero);
        }

        private void RefreshPowerMeterVisuals()
        {
            if (gameManager == null)
            {
                return;
            }

            int collected = Mathf.Clamp(gameManager.PowerMeterCollectedCapsules, 0, PowerMeterSlotCount);
            if (displayedPowerMeterCapsules != collected)
            {
                pulsingPowerMeterSlot = collected > displayedPowerMeterCapsules && collected > 0 ? collected - 1 : -1;
                powerMeterPulseTimer = pulsingPowerMeterSlot >= 0 ? PowerMeterPulseDuration : 0f;
                displayedPowerMeterCapsules = collected;
            }

            float pulse = powerMeterPulseTimer > 0f ? Mathf.Clamp01(powerMeterPulseTimer / PowerMeterPulseDuration) : 0f;
            for (int i = 0; i < PowerMeterSlotCount; i++)
            {
                bool filled = i < collected;
                bool pulsing = i == pulsingPowerMeterSlot && pulse > 0f;
                float pulseWave = pulsing ? Mathf.Sin((1f - pulse) * Mathf.PI) : 0f;

                if (powerMeterSlotFills[i] != null)
                {
                    Color fill = gameManager.CanActivatePowerMeter
                        ? new Color(1f, 0.34f, 0.18f, 0.98f)
                        : new Color(1f, 0.84f, 0.12f, 0.92f);
                    fill.a = filled ? Mathf.Clamp01(fill.a + pulseWave * 0.08f) : 0.08f;
                    powerMeterSlotFills[i].color = fill;
                    powerMeterSlotFills[i].rectTransform.localScale = Vector3.one * (pulsing ? 1f + (pulseWave * 0.08f) : 1f);
                }

                if (powerMeterSlotGlows[i] != null)
                {
                    Color glow = gameManager.CanActivatePowerMeter
                        ? new Color(1f, 0.12f, 0.08f, 0.5f)
                        : new Color(1f, 0.92f, 0.18f, 0.32f);
                    glow.a = filled ? (gameManager.CanActivatePowerMeter ? 0.38f : 0.18f) + (pulseWave * 0.5f) : 0f;
                    powerMeterSlotGlows[i].color = glow;
                }

                if (powerMeterSlotLabels[i] != null)
                {
                    powerMeterSlotLabels[i].text = PowerMeterState.SlotLabels[i];
                    powerMeterSlotLabels[i].color = filled ? ArcadeTheme.InkBlack : new Color(1f, 0.88f, 0.24f, 0.82f);
                }
            }
        }

        private void UpdateBombDisplay()
        {
            if (bombCountText == null || gameManager == null) return;
            int count = Mathf.Clamp(gameManager.BombCount, 0, 99);
            bombCountText.text = $"{TopHudStatLabels[2]} {count:00}";
        }

        private string BuildShieldIcons()
        {
            int active = Mathf.Clamp(gameManager.ShieldCharges, 0, 3);
            char[] icons = { '◇', '◇', '◇' };
            for (int i = 0; i < active; i++)
            {
                icons[i] = '◆';
            }

            return new string(icons);
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
                playerHealthLabel.text = $"装甲 {gameManager.PlayerHealth}/{gameManager.MaxPlayerHealth}  专注 {(gameManager.FocusMeterNormalized * 100f):0}%";
            }

            // Focus bar auto-shows when player is hurt, auto-hides when healthy
            bool shouldShow = playerHealthFill != null && gameManager.PlayerHealth < gameManager.MaxPlayerHealth;
            if (playerHealthRoot != null)
            {
                playerHealthRoot.gameObject.SetActive(shouldShow);
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

            float vignetteAlpha = Mathf.Clamp01(playerDamageFlashTimer / 0.42f) * 0.35f;
            if (vignetteTop != null)
            {
                Color c = vignetteTop.color;
                c.a = vignetteAlpha;
                vignetteTop.color = c;
            }
            if (vignetteBottom != null)
            {
                Color c = vignetteBottom.color;
                c.a = vignetteAlpha;
                vignetteBottom.color = c;
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
