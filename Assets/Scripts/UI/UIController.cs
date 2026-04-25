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
        private Image bossBarRoot;
        private Image bossBarFill;
        private Text bossBarLabel;
        private Image overlay;
        private Text overlayTitle;
        private Text overlayScore;
        private Text overlayBestScore;

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

            scoreText.text = $"得分\n{gameManager.Score:0000000}";
            livesText.text = $"战机 {gameManager.Lives}\n待命";
            highScoreText.text = $"最高分\n{SessionState.HighScore:0000000}";
            difficultyText.text = $"难度 {BuildDifficultyText()}";
            stageProgressText.text = $"第一关 {(gameManager.StageProgress * 100f):0}%\n击落 {gameManager.EnemiesDestroyed}/{gameManager.RequiredKillsToClear}";
            powerupText.text = BuildPowerupHudText();
            pauseHintText.text = gameManager.IsPaused ? "已暂停" : string.Empty;
            stageBannerText.text = gameManager.StageBannerText;
            stageBannerText.gameObject.SetActive(!string.IsNullOrEmpty(gameManager.StageBannerText));
            stageBannerText.color = gameManager.StageBannerText.Contains("警报")
                ? new Color(1f, 0.34f, 0.2f, 0.98f)
                : new Color(0.82f, 0.94f, 1f, 0.98f);
            pauseButtonText.text = gameManager.IsPaused ? "继续" : "暂停";
            pauseOverlay.gameObject.SetActive(gameManager.IsPaused);
            pauseOverlayHighScore.text = $"最高分 {SessionState.HighScore:0000000}";
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
            overlayScore.text = $"本局得分 {score:0000000}";
            overlayBestScore.text = $"最高分 {SessionState.HighScore:0000000}";
            if (title == "游戏胜利")
            {
                overlayBestScore.text = "3秒后进入下一关";
            }
        }

        private void Build()
        {
            Canvas canvas = UiFactory.CreateCanvas("GameCanvas");
            canvas.transform.SetParent(transform, false);

            Image topBar = UiFactory.CreateArcadePanel(canvas.transform, "TopHud", new Color(0.04f, 0.08f, 0.18f, 0.96f), new Color(0.2f, 0.54f, 0.98f, 0.95f), new Vector2(0f, 0.88f), new Vector2(1f, 1f), new Vector2(8f, 8f));
            Image leftCell = UiFactory.CreatePanel(topBar.transform, "ScoreCell", new Color(0.05f, 0.1f, 0.2f, 0.88f), new Vector2(0.015f, 0.1f), new Vector2(0.33f, 0.9f));
            Image centerCell = UiFactory.CreatePanel(topBar.transform, "StageCell", new Color(0.08f, 0.08f, 0.21f, 0.88f), new Vector2(0.345f, 0.1f), new Vector2(0.655f, 0.9f));
            Image rightCell = UiFactory.CreatePanel(topBar.transform, "PlayerCell", new Color(0.07f, 0.06f, 0.18f, 0.88f), new Vector2(0.67f, 0.1f), new Vector2(0.985f, 0.9f));

            scoreText = UiFactory.CreateArcadeLabel(leftCell.transform, "得分\n0000000", 34, TextAnchor.UpperLeft, new Color(0.95f, 0.99f, 1f), FontStyle.Bold, new Vector2(0.05f, 0.28f), new Vector2(0.95f, 0.95f), Vector2.zero);
            highScoreText = UiFactory.CreateArcadeLabel(leftCell.transform, "最高分\n0000000", 26, TextAnchor.LowerLeft, new Color(0.53f, 0.86f, 1f), FontStyle.Bold, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.34f), Vector2.zero);
            difficultyText = UiFactory.CreateArcadeLabel(centerCell.transform, "难度 低级", 24, TextAnchor.UpperCenter, new Color(0.72f, 0.82f, 1f), FontStyle.Bold, new Vector2(0.06f, 0.56f), new Vector2(0.94f, 0.9f), Vector2.zero);
            stageProgressText = UiFactory.CreateArcadeLabel(centerCell.transform, "第一关 0%", 28, TextAnchor.MiddleCenter, new Color(0.96f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.06f, 0.26f), new Vector2(0.94f, 0.6f), Vector2.zero);
            powerupText = UiFactory.CreateArcadeLabel(centerCell.transform, "火力 普通", 24, TextAnchor.LowerCenter, new Color(1f, 0.56f, 0.74f), FontStyle.Bold, new Vector2(0.06f, 0.04f), new Vector2(0.94f, 0.3f), Vector2.zero);
            livesText = UiFactory.CreateArcadeLabel(rightCell.transform, "战机 5\n待命", 26, TextAnchor.UpperCenter, new Color(0.96f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.04f, 0.32f), new Vector2(0.96f, 0.94f), Vector2.zero);
            pauseButton = UiFactory.CreateButton(rightCell.transform, "暂停", new Color(0.98f, 0.42f, 0.34f, 0.96f), Color.white, new Vector2(150f, 72f), new Vector2(-82f, 0f), new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f));
            pauseButton.onClick.AddListener(() => gameManager.TogglePause());
            pauseButtonText = pauseButton.GetComponentInChildren<Text>();
            pauseButtonText.fontStyle = FontStyle.Bold;
            pauseButtonText.fontSize = 30;
            Button bombButton = UiFactory.CreateButton(rightCell.transform, "BOMB", new Color(0.22f, 0.78f, 1f, 0.96f), Color.white, new Vector2(150f, 72f), new Vector2(82f, 0f), new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f));
            bombButton.onClick.AddListener(() => gameManager.TryActivateBomb());
            Text bombButtonText = bombButton.GetComponentInChildren<Text>();
            bombButtonText.fontStyle = FontStyle.Bold;
            bombButtonText.fontSize = 30;

            bossBarRoot = UiFactory.CreateArcadePanel(canvas.transform, "BossBarRoot", new Color(0.14f, 0.03f, 0.08f, 0.92f), new Color(1f, 0.28f, 0.2f, 0.98f), new Vector2(0.05f, 0.825f), new Vector2(0.95f, 0.868f), new Vector2(8f, 8f));
            bossBarRoot.gameObject.SetActive(false);
            bossBarFill = UiFactory.CreatePanel(bossBarRoot.transform, "BossBarFill", new Color(1f, 0.44f, 0.3f, 0.98f), new Vector2(0.012f, 0.14f), new Vector2(0.988f, 0.86f));
            bossBarLabel = UiFactory.CreateArcadeLabel(bossBarRoot.transform, "警报  敌方旗舰", 24, TextAnchor.MiddleCenter, new Color(1f, 0.96f, 0.94f), FontStyle.Bold, new Vector2(0.05f, 0f), new Vector2(0.95f, 1f), Vector2.zero);

            Text helper = UiFactory.CreateArcadeLabel(canvas.transform, "拖动战机  自动开火  收集火力包  BOMB清屏", 26, TextAnchor.LowerCenter, new Color(0.55f, 0.84f, 1f, 0.95f), FontStyle.Bold, new Vector2(0.06f, 0.01f), new Vector2(0.94f, 0.08f), Vector2.zero);
            helper.text = "拖动战机  自动开火  收集火力包  BOMB清屏";
            pauseHintText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 40, TextAnchor.MiddleCenter, new Color(0.6f, 0.84f, 1f, 0.92f), FontStyle.Bold, new Vector2(0.3f, 0.79f), new Vector2(0.7f, 0.84f), Vector2.zero);
            stageBannerText = UiFactory.CreateArcadeLabel(canvas.transform, string.Empty, 86, TextAnchor.MiddleCenter, new Color(1f, 0.34f, 0.2f, 0.98f), FontStyle.Bold, new Vector2(0.08f, 0.63f), new Vector2(0.92f, 0.77f), Vector2.zero);
            stageBannerText.gameObject.SetActive(false);

            pauseOverlay = UiFactory.CreatePanel(canvas.transform, "PauseOverlay", new Color(0.02f, 0.05f, 0.11f, 0.72f), Vector2.zero, Vector2.one);
            pauseOverlay.gameObject.SetActive(false);
            Image pauseCard = UiFactory.CreateArcadePanel(pauseOverlay.transform, "PauseCard", new Color(0.05f, 0.08f, 0.16f, 0.97f), new Color(0.26f, 0.62f, 1f, 0.96f), new Vector2(0.12f, 0.22f), new Vector2(0.88f, 0.7f), new Vector2(12f, 12f));
            pauseOverlayTitle = UiFactory.CreateArcadeLabel(pauseCard.transform, "暂停中", 80, TextAnchor.MiddleCenter, new Color(0.92f, 0.97f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.72f), new Vector2(0.88f, 0.9f), Vector2.zero);
            pauseOverlayHighScore = UiFactory.CreateArcadeLabel(pauseCard.transform, "最高分 0000000", 32, TextAnchor.MiddleCenter, new Color(0.48f, 0.84f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.56f), new Vector2(0.85f, 0.66f), Vector2.zero);
            UiFactory.CreateArcadeLabel(pauseCard.transform, "红色主武器  蓝色追踪/导弹  紫色特殊强化\n重复拾取同类火力包可提升等级", 28, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.34f), new Vector2(0.88f, 0.52f), Vector2.zero);
            Button resumeButton = UiFactory.CreateButton(pauseCard.transform, "继续战斗", new Color(0.22f, 0.64f, 1f), Color.white, new Vector2(340f, 112f), new Vector2(0f, -54f));
            resumeButton.onClick.AddListener(() => gameManager.TogglePause());
            Button restartButton = UiFactory.CreateButton(pauseCard.transform, "重新开始", new Color(1f, 0.42f, 0.34f), Color.white, new Vector2(340f, 112f), new Vector2(0f, -186f));
            restartButton.onClick.AddListener(SceneNavigator.LoadGame);
            Button menuButton = UiFactory.CreateButton(pauseCard.transform, "返回主页", new Color(0.48f, 0.32f, 0.88f), Color.white, new Vector2(340f, 112f), new Vector2(0f, -318f));
            menuButton.onClick.AddListener(SceneNavigator.LoadMenu);

            overlay = UiFactory.CreatePanel(canvas.transform, "Overlay", new Color(0.02f, 0.05f, 0.11f, 0.8f), Vector2.zero, Vector2.one);
            overlay.gameObject.SetActive(false);
            overlayTitle = UiFactory.CreateArcadeLabel(overlay.transform, "任务失败", 84, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold, new Vector2(0.15f, 0.54f), new Vector2(0.85f, 0.7f), Vector2.zero);
            overlayScore = UiFactory.CreateArcadeLabel(overlay.transform, "本局得分 0000000", 46, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.42f), new Vector2(0.85f, 0.52f), Vector2.zero);
            overlayBestScore = UiFactory.CreateArcadeLabel(overlay.transform, "最高分 0000000", 38, TextAnchor.MiddleCenter, new Color(0.54f, 0.85f, 1f), FontStyle.Bold, new Vector2(0.15f, 0.34f), new Vector2(0.85f, 0.42f), Vector2.zero);
        }

        private string BuildPowerupHudText()
        {
            if (!gameManager.HasActivePowerup)
            {
                return $"火力 普通\nBOMB {gameManager.BombCount}";
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

            return $"火力 {name} Lv{gameManager.ActivePowerupLevel} {gameManager.ActivePowerupRemainingSeconds:0.0}s\nBOMB {gameManager.BombCount}";
        }

        private string BuildDifficultyText()
        {
            switch (gameManager.Difficulty)
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
