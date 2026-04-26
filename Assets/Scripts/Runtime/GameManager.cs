using System.Collections;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class GameManager : MonoBehaviour
    {
        private const float PowerupDurationSeconds = 8f;

        private UIController uiController;
        private EffectsController effectsController;
        private BlockSpawner blockSpawner;
        private PlayerController playerController;
        private readonly ActivePowerupState activePowerup = new ActivePowerupState();
        private readonly GameplayRewardConfig rewardConfig = GameplayRewardConfig.Default;
        private readonly GameplayRewardState rewardState = new GameplayRewardState(GameplayRewardConfig.Default);
        private readonly FireLevelState fireLevelState = new FireLevelState();
        private bool gameEnded;
        private bool paused;
        private bool stageClear;
        private float elapsedTime;
        private float stageProgress;
        private string stageBannerText = string.Empty;
        private float stageBannerTimer;
        private string gameOverTitle = "任务失败";
        private string stageLabel = "准备出击";
        private string bossName = "敌方旗舰";
        private int bossCurrentHitPoints;
        private int bossMaxHitPoints;
        private int enemiesDestroyed;
        private int bombsUsed;
        private StagePhase currentStagePhase = StagePhase.Preparation;

        public GameFlowState CurrentState { get; private set; } = GameFlowState.Intro;
        public int Score { get; private set; }
        public int Lives { get; private set; } = 5;
        public float LeftBound { get; private set; }
        public float RightBound { get; private set; }
        public float TopBound { get; private set; }
        public float BottomBound { get; private set; }
        public bool IsPlaying => CurrentState == GameFlowState.Playing && !gameEnded && !paused;
        public bool IsPaused => paused;
        public float ElapsedTime => elapsedTime;
        public GameDifficulty Difficulty => SessionState.SelectedDifficulty;
        public AmmoPowerupType ActivePowerupType => activePowerup.Type;
        public int ActivePowerupLevel => activePowerup.Level;
        public float ActivePowerupRemainingSeconds => activePowerup.RemainingSeconds;
        public bool HasActivePowerup => activePowerup.HasActivePowerup;
        public GameplayRewardConfig RewardConfig => rewardConfig;
        public int CoinCount => rewardState.CoinsCollected;
        public int FireLevel => fireLevelState.Level;
        public Vector3 PlayerPosition => playerController != null ? playerController.transform.position : Vector3.zero;
        public int BombCount => Mathf.Max(0, rewardState.BombPickupsEarned - bombsUsed);
        public int BombsUsed => bombsUsed;
        public bool HasBomb => BombCount > 0;
        public StagePhase CurrentStagePhase => currentStagePhase;
        public string StageLabel => stageLabel;
        public float StageProgress => stageProgress;
        public bool HasBoss => currentStagePhase == StagePhase.Boss && bossMaxHitPoints > 0;
        public string BossName => bossName;
        public float BossHealthNormalized => bossMaxHitPoints <= 0 ? 0f : Mathf.Clamp01(bossCurrentHitPoints / (float)bossMaxHitPoints);
        public string StageBannerText => stageBannerTimer > 0f ? stageBannerText : string.Empty;
        public bool LastRunWasVictory => stageClear;
        public bool EnemyCollisionEndsRun => Difficulty != GameDifficulty.Low;
        public bool EnemyUsesScatterShot => Difficulty == GameDifficulty.High;
        public int EnemiesDestroyed => enemiesDestroyed;
        public int RequiredKillsToClear => StageClearTarget.GetRequiredKills(Difficulty, SessionState.CurrentStageIndex, SessionState.CurrentLoopIndex);
        public bool AudioEnabled => SessionState.AudioEnabled;
        public int StageNumber => SessionState.CurrentStageNumber;
        public int LoopNumber => SessionState.CurrentLoopNumber;
        public string StageName => SessionState.CurrentStage.Name;
        public string BossDisplayName => SessionState.CurrentStage.BossName;
        public Color StageAccentColor => SessionState.CurrentStage.AccentColor;
        public StageCombatStyle StageCombatStyle => SessionState.CurrentStage.CombatStyle;
        public BossPatternStyle BossPatternStyle => SessionState.CurrentStage.BossPattern;
        public float StageDifficultyMultiplier => SessionState.CurrentStageDifficultyMultiplier;

        public void Initialize(UIController ui, EffectsController effects, BlockSpawner spawner, PlayerController player, float leftBound, float rightBound, float topBound, float bottomBound)
        {
            uiController = ui;
            effectsController = effects;
            blockSpawner = spawner;
            playerController = player;
            LeftBound = leftBound;
            RightBound = rightBound;
            TopBound = topBound;
            BottomBound = bottomBound;
            Lives = GetInitialLives(Difficulty);

            uiController.Bind(this);
        }

        public void BeginIntro()
        {
            if (gameEnded)
            {
                return;
            }

            CurrentState = GameFlowState.Intro;
            stageLabel = "航母起飞";
            ShowStageBanner("航母起飞");
            if (uiController != null)
            {
                uiController.ShowIntroPrompt();
                uiController.RefreshHud();
            }
        }

        public void CompleteIntro()
        {
            if (gameEnded)
            {
                return;
            }

            CurrentState = GameFlowState.Playing;
            stageLabel = "敌机来袭";
            ShowStageBanner("START");
            if (uiController != null)
            {
                uiController.HideIntroPrompt();
                uiController.RefreshHud();
            }
        }

        private void Update()
        {
            if (!gameEnded)
            {
                if (!paused)
                {
                    elapsedTime += Time.deltaTime;
                    activePowerup.Tick(Time.deltaTime);
                    stageBannerTimer = Mathf.Max(0f, stageBannerTimer - Time.deltaTime);
                }

                uiController.RefreshHud();
            }
        }

        public void AddScore(int amount)
        {
            if (gameEnded)
            {
                return;
            }

            Score += amount;
            uiController.RefreshHud();
        }

        public void CollectCoin(Vector3 position)
        {
            if (gameEnded)
            {
                return;
            }

            int earnedBombPickups = rewardState.CollectCoins(1);
            Score += rewardConfig.ScorePerCoin;
            effectsController.PlayScorePopup(position, rewardConfig.ScorePerCoin);

            for (int i = 0; i < earnedBombPickups; i++)
            {
                blockSpawner.SpawnBombPickupAtRandomReachablePosition();
                ShowStageBanner("BOMB READY");
            }

            uiController.RefreshHud();
        }

        public void NotifyEnemyDestroyed()
        {
            if (gameEnded || stageClear)
            {
                return;
            }

            enemiesDestroyed++;
            uiController.RefreshHud();
            if (enemiesDestroyed >= RequiredKillsToClear)
            {
                MarkStageClear();
            }
        }

        public void DamageBase(int amount)
        {
            if (gameEnded)
            {
                return;
            }

            Lives = Mathf.Max(0, Lives - amount);
            uiController.RefreshHud();
            effectsController.PlayBaseHit();
            gameOverTitle = "基地失守";

            if (Lives <= 0)
            {
                StartCoroutine(EndRun());
            }
        }

        public void DestroyPlayerByPierce()
        {
            if (gameEnded)
            {
                return;
            }

            gameOverTitle = "战机被击穿";
            StartCoroutine(EndRun());
        }

        public void DestroyPlayerByCollision()
        {
            if (gameEnded)
            {
                return;
            }

            gameOverTitle = "战机被撞毁";
            StartCoroutine(EndRun());
        }

        public void SetStageState(StagePhase phase, string label)
        {
            currentStagePhase = phase;
            stageLabel = label;
            stageProgress = Mathf.Max(stageProgress, GetBaselineProgress(phase));
            ShowStageBanner(label);
            uiController.RefreshHud();
        }

        public void UpdateStageProgress(float normalizedProgress)
        {
            float baseline = GetBaselineProgress(currentStagePhase);
            float span = GetProgressSpan(currentStagePhase);
            stageProgress = Mathf.Clamp01(baseline + (Mathf.Clamp01(normalizedProgress) * span));
        }

        public void NotifyBossSpawn(string displayName, int currentHitPoints, int maxHitPoints)
        {
            bossName = displayName;
            bossCurrentHitPoints = currentHitPoints;
            bossMaxHitPoints = maxHitPoints;
            currentStagePhase = StagePhase.Boss;
            stageLabel = "旗舰逼近";
            ShowStageBanner("危险警报");
            uiController.ShowBossWarning(displayName);
            uiController.RefreshHud();
        }

        public void UpdateBossHealth(int currentHitPoints, int maxHitPoints)
        {
            bossCurrentHitPoints = Mathf.Max(0, currentHitPoints);
            bossMaxHitPoints = Mathf.Max(1, maxHitPoints);
            uiController.RefreshHud();
        }

        public void MarkStageClear()
        {
            if (gameEnded)
            {
                return;
            }

            stageClear = true;
            currentStagePhase = StagePhase.Clear;
            stageLabel = "任务完成";
            stageProgress = 1f;
            bossCurrentHitPoints = 0;
            bossMaxHitPoints = 0;
            gameOverTitle = "任务完成";
            ShowStageBanner("游戏胜利");
            StartCoroutine(EndVictoryRun());
        }

        public void ActivatePowerup(AmmoPowerupType type, float durationSeconds = PowerupDurationSeconds)
        {
            if (gameEnded || type == AmmoPowerupType.None)
            {
                return;
            }

            activePowerup.Activate(type, durationSeconds);
            fireLevelState.Increase();
            effectsController.PlayPowerupPickup(PlayerPosition, PowerupCycle.GetCategoryColor(type), "火力 Lv" + fireLevelState.Level);
            uiController.RefreshHud();
        }

        public bool TryActivateBomb()
        {
            return false;
        }

        public bool ActivateBombFromPickup(Vector3 origin)
        {
            if (gameEnded || paused || !IsPlaying)
            {
                uiController.RefreshHud();
                return false;
            }

            foreach (EnemyFireballController fireball in FindObjectsByType<EnemyFireballController>(FindObjectsSortMode.None))
            {
                fireball.ClearByBomb();
            }

            foreach (BlockController block in FindObjectsByType<BlockController>(FindObjectsSortMode.None))
            {
                block.ClearByBomb();
            }

            foreach (BossController boss in FindObjectsByType<BossController>(FindObjectsSortMode.None))
            {
                boss.ApplyHit(5);
            }

            effectsController.PlayBombDetonation(origin);
            bombsUsed++;
            ShowStageBanner("BOMB");
            uiController.RefreshHud();
            return true;
        }

        public void TogglePause()
        {
            if (gameEnded || CurrentState != GameFlowState.Playing)
            {
                return;
            }

            SetPaused(!paused);
        }

        public void ToggleAudio()
        {
            SessionState.SetAudioEnabled(!SessionState.AudioEnabled);
            effectsController.RefreshAudioSettings();
            uiController.RefreshHud();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && !gameEnded && !paused)
            {
                SetPaused(true);
            }
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        private IEnumerator EndRun()
        {
            if (gameEnded)
            {
                yield break;
            }

            CurrentState = GameFlowState.GameOver;
            gameEnded = true;
            SetPaused(false);
            activePowerup.Clear();
            blockSpawner.StopSpawning();
            playerController.StopCombat();
            uiController.ShowGameOverOverlay(gameOverTitle, Score);
            SessionState.CommitRunScore(Score, stageClear, BuildRunRating(), BuildRunSummary());
            yield return new WaitForSeconds(1.15f);
            SceneNavigator.LoadGameOver();
        }

        private IEnumerator EndVictoryRun()
        {
            if (gameEnded)
            {
                yield break;
            }

            CurrentState = GameFlowState.GameOver;
            gameEnded = true;
            SetPaused(false);
            activePowerup.Clear();
            blockSpawner.StopSpawning();
            playerController.StopCombat();
            uiController.ShowGameOverOverlay("游戏胜利", Score);
            SessionState.CommitRunScore(Score, true, BuildRunRating(), BuildRunSummary());
            yield return new WaitForSeconds(3f);
            SceneNavigator.LoadNextStage();
        }

        private void SetPaused(bool value)
        {
            paused = value;
            Time.timeScale = paused ? 0f : 1f;
            uiController.RefreshHud();
        }

        private void ShowStageBanner(string content)
        {
            stageBannerText = content;
            stageBannerTimer = 1.9f;
        }

        private string BuildRunRating()
        {
            if (!stageClear)
            {
                return "C";
            }

            if (Difficulty == GameDifficulty.High)
            {
                return "S";
            }

            return Difficulty == GameDifficulty.Medium ? "A" : "B";
        }

        private string BuildRunSummary()
        {
            if (stageClear)
            {
                return $"第{StageNumber}关 {StageName} 已肃清。{SessionState.CurrentStage.VictorySummary}";
            }

            return $"第{StageNumber}关 {StageName} 作战中断，{BossDisplayName}仍在压制空域。保持走位和火力节奏再试一次。";
        }

        private static float GetBaselineProgress(StagePhase phase)
        {
            switch (phase)
            {
                case StagePhase.Assault:
                    return 0.08f;
                case StagePhase.Pressure:
                    return 0.32f;
                case StagePhase.Elite:
                    return 0.58f;
                case StagePhase.Boss:
                    return 0.76f;
                case StagePhase.Clear:
                    return 1f;
                default:
                    return 0f;
            }
        }

        private static float GetProgressSpan(StagePhase phase)
        {
            switch (phase)
            {
                case StagePhase.Assault:
                    return 0.24f;
                case StagePhase.Pressure:
                    return 0.26f;
                case StagePhase.Elite:
                    return 0.18f;
                case StagePhase.Boss:
                    return 0.24f;
                default:
                    return 0.08f;
            }
        }

        private static int GetInitialLives(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.High:
                    return 2;
                case GameDifficulty.Medium:
                    return 3;
                default:
                    return 5;
            }
        }
    }
}
