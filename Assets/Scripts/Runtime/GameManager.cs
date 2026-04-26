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
        private readonly PlayerWeaponState weaponState = new PlayerWeaponState();
        private readonly PowerMeterState powerMeter = new PowerMeterState();
        private readonly ComboState comboState = new ComboState();
        private readonly PlayerHealthState playerHealth = new PlayerHealthState();
        private readonly PlayerInvulnerabilityState invulnerabilityState = new PlayerInvulnerabilityState();
        private readonly WaveDirector waveDirector = new WaveDirector();
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
        private int speedUpLevel;
        private int shieldCharges;
        private StagePhase currentStagePhase = StagePhase.Preparation;

        public GameFlowState CurrentState { get; private set; } = GameFlowState.Intro;
        public int Score { get; private set; }
        public int Lives => PlayerHealth;
        public int PlayerHealth => playerHealth.CurrentHealth;
        public int MaxPlayerHealth => playerHealth.MaxHealth;
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
        public int FireLevel => weaponState.FireLevel;
        public WeaponType CurrentWeaponType => weaponState.CurrentWeaponType;
        public WeaponModuleType[] CurrentWeaponModules => weaponState.GetEquippedModules();
        public int CurrentCombo => comboState.CurrentCombo;
        public int CurrentComboMultiplier => comboState.CurrentMultiplier;
        public int MaxCombo => comboState.MaxCombo;
        public int MaxComboMultiplier => comboState.MaxMultiplier;
        public Vector3 PlayerPosition => playerController != null ? playerController.transform.position : Vector3.zero;
        public int BombCount => Mathf.Max(0, rewardState.BombPickupsEarned - bombsUsed);
        public int BombsUsed => bombsUsed;
        public bool HasBomb => BombCount > 0;
        public int ShieldCharges => shieldCharges;
        public float PlayerSpeedMultiplier => 1f + (Mathf.Clamp(speedUpLevel, 0, 3) * 0.18f);
        public bool CanActivatePowerMeter => powerMeter.CanActivate;
        public string PowerMeterHudText => powerMeter.BuildHudText();
        public StagePhase CurrentStagePhase => currentStagePhase;
        public string StageLabel => stageLabel;
        public float StageProgress => stageProgress;
        public bool HasBoss => currentStagePhase == StagePhase.Boss && bossMaxHitPoints > 0;
        public string BossName => bossName;
        public float BossHealthNormalized => bossMaxHitPoints <= 0 ? 0f : Mathf.Clamp01(bossCurrentHitPoints / (float)bossMaxHitPoints);
        public string StageBannerText => stageBannerTimer > 0f ? stageBannerText : string.Empty;
        public bool LastRunWasVictory => stageClear;
        public bool EnemyUsesScatterShot => Difficulty == GameDifficulty.High;
        public int EnemiesDestroyed => enemiesDestroyed;
        public int RequiredKillsToClear => StageClearTarget.GetRequiredKills(Difficulty, SessionState.CurrentStageIndex, SessionState.CurrentLoopIndex);
        public bool AudioEnabled => SessionState.AudioEnabled;
        public bool IsPlayerInvulnerable => invulnerabilityState.IsInvulnerable;
        public float PlayerInvulnerabilityFlashAlpha => invulnerabilityState.GetFlashAlpha();
        public int StageNumber => SessionState.CurrentStageNumber;
        public int LoopNumber => SessionState.CurrentLoopNumber;
        public string StageName => SessionState.CurrentStage.Name;
        public string BossDisplayName => SessionState.CurrentStage.BossName;
        public Color StageAccentColor => SessionState.CurrentStage.AccentColor;
        public StageCombatStyle StageCombatStyle => SessionState.CurrentStage.CombatStyle;
        public BossPatternStyle BossPatternStyle => SessionState.CurrentStage.BossPattern;
        public float StageDifficultyMultiplier => SessionState.CurrentStageDifficultyMultiplier;
        public WavePhase CurrentWavePhase => waveDirector.GetCurrentPhase();
        public float WavePhaseProgress => waveDirector.GetPhaseProgress();

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

            waveDirector.PhaseChanged += HandleWavePhaseChanged;
            uiController.Bind(this);
        }

        public void BeginIntro()
        {
            if (gameEnded)
            {
                return;
            }

            CurrentState = GameFlowState.Intro;
            stageLabel = "起飞准备";
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
            ShowStageBanner("开始出击");
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
                    invulnerabilityState.Tick(Time.deltaTime);
                    stageBannerTimer = Mathf.Max(0f, stageBannerTimer - Time.deltaTime);
                    if (IsPlaying)
                    {
                        waveDirector.UpdatePhase(Time.deltaTime);
                    }
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

        public int AddComboScaledScore(int baseAmount, Vector3 position)
        {
            if (gameEnded)
            {
                return 0;
            }

            int earned = Mathf.Max(0, baseAmount) * comboState.CurrentMultiplier;
            Score += earned;
            if (earned > 0)
            {
                effectsController.PlayScorePopup(position, earned);
            }

            uiController.RefreshHud();
            return earned;
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
                ShowStageBanner("炸弹就绪");
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

        public void RegisterEnemyKillScore(int baseScore, Vector3 position)
        {
            if (gameEnded || stageClear)
            {
                return;
            }

            comboState.RegisterKill();
            AddComboScaledScore(baseScore, position);
        }

        public void RegisterBossPhaseCombo()
        {
            if (gameEnded)
            {
                return;
            }

            comboState.RegisterBossPhaseClear();
            ShowStageBanner("连击倍率 " + comboState.CurrentMultiplier + "倍");
            uiController.RefreshHud();
        }

        public void RegisterBossDefeatedScore(int baseScore, Vector3 position)
        {
            if (gameEnded)
            {
                return;
            }

            comboState.RegisterBossDefeated();
            AddComboScaledScore(baseScore, position);
        }

        public void NotifyEnemyEscaped(Vector3 position)
        {
            if (gameEnded)
            {
                return;
            }

            comboState.BreakCombo();
            int deducted = rewardState.DeductCoins(rewardConfig.CoinsLostPerEscapedEnemy);
            if (deducted > 0)
            {
                effectsController.PlayScorePopup(position, -deducted);
                ShowStageBanner("金币 -" + deducted);
            }

            uiController.RefreshHud();
        }

        public void DamagePlayerByPierce()
        {
            DamagePlayer(1, "战机损毁");
        }

        public void DamagePlayerByCollision()
        {
            DamagePlayer(1, "战机损毁");
        }

        private void DamagePlayer(int amount, string depletedTitle)
        {
            if (gameEnded)
            {
                return;
            }

            if (invulnerabilityState.IsInvulnerable)
            {
                return;
            }

            if (shieldCharges > 0)
            {
                shieldCharges--;
                invulnerabilityState.Trigger(0.45f);
                ShowStageBanner("护盾吸收");
                uiController.RefreshHud();
                return;
            }

            int damageApplied = playerHealth.ApplyDamage(amount);
            if (damageApplied <= 0)
            {
                return;
            }

            comboState.BreakCombo();
            if (!playerHealth.IsDepleted)
            {
                DropRecoveryPowerupsAfterDamage();
            }

            invulnerabilityState.Trigger();
            PlayerDamageFeedback.TriggerVibration(damageApplied);
            effectsController.PlayBaseHit();
            uiController.NotifyPlayerDamaged();
            uiController.RefreshHud();
            if (playerHealth.IsDepleted)
            {
                gameOverTitle = depletedTitle;
                StartCoroutine(EndRun());
            }
        }

        private void DropRecoveryPowerupsAfterDamage()
        {
            WeaponType previousWeapon = weaponState.CurrentWeaponType;
            int previousLevel = weaponState.FireLevel;
            WeaponModuleType[] previousModules = weaponState.GetEquippedModules();
            if (!weaponState.ApplyDeathPenalty())
            {
                return;
            }

            Vector3 origin = PlayerPosition;
            blockSpawner.SpawnAmmoPackAtPosition(previousWeapon, origin + new Vector3(-0.42f, 0.64f, 0f));
            if (previousModules.Length > 0)
            {
                blockSpawner.SpawnAmmoPackAtPosition(PowerupCycle.ToAmmoPowerupType(previousModules[previousModules.Length - 1]), origin + new Vector3(0.42f, 0.64f, 0f));
            }
            else if (previousLevel >= 3)
            {
                WeaponType supportType = previousWeapon == WeaponType.Spread ? WeaponType.Laser : WeaponType.Spread;
                blockSpawner.SpawnAmmoPackAtPosition(supportType, origin + new Vector3(0.42f, 0.64f, 0f));
            }

            ShowStageBanner("火力重置");
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
            int clearBonus = ScoreRewardConfig.GetStageClearBonus(BombCount, shieldCharges, comboState.MaxMultiplier);
            Score += clearBonus;
            effectsController.PlayScorePopup(PlayerPosition, clearBonus);
            ShowStageBanner("任务完成");
            StartCoroutine(EndVictoryRun());
        }

        public void ActivatePowerup(AmmoPowerupType type, float durationSeconds = PowerupDurationSeconds)
        {
            if (gameEnded || type == AmmoPowerupType.None)
            {
                return;
            }

            CollectPowerCapsule();
        }

        public void ApplyWeaponPickup(WeaponType type)
        {
            if (gameEnded)
            {
                return;
            }

            weaponState.ApplyWeaponPickup(type);
            ShowStageBanner(WeaponConfig.Get(type).DisplayName + " " + weaponState.FireLevel + "级");
            uiController.RefreshHud();
        }

        public void ApplyPowerupPickup(AmmoPowerupType type)
        {
            if (gameEnded || type == AmmoPowerupType.None)
            {
                return;
            }

            weaponState.ApplyPowerup(type);
            ShowStageBanner(PowerupCycle.GetLabel(type) + " 火力 " + weaponState.FireLevel + "级");
            uiController.RefreshHud();
        }

        public void CollectPowerCapsule()
        {
            if (gameEnded)
            {
                return;
            }

            powerMeter.CollectCapsule();
            ShowStageBanner("能量胶囊 " + GetPowerMeterUpgradeLabel(powerMeter.HighlightedUpgrade));
            uiController.RefreshHud();
        }

        public bool TryActivatePowerMeter()
        {
            if (gameEnded || paused || !powerMeter.CanActivate)
            {
                uiController.RefreshHud();
                return false;
            }

            PowerMeterUpgrade upgrade = powerMeter.ActivateHighlightedUpgrade();
            ApplyPowerMeterUpgrade(upgrade);
            uiController.RefreshHud();
            return true;
        }

        public string GetCurrentWeaponDisplayText()
        {
            return weaponState.GetCurrentWeaponDisplayText();
        }

        public string GetComboDisplayText()
        {
            return comboState.GetDisplayText();
        }

        public bool TryActivateBomb()
        {
            return TryActivatePowerMeter();
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

            foreach (GroundTargetController groundTarget in FindObjectsByType<GroundTargetController>(FindObjectsSortMode.None))
            {
                groundTarget.ClearByBomb();
            }

            foreach (BossController boss in FindObjectsByType<BossController>(FindObjectsSortMode.None))
            {
                boss.ApplyHit(5);
            }

            effectsController.PlayBombDetonation(origin);
            bombsUsed++;
            ShowStageBanner("炸弹清屏");
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
            SessionState.CommitRunScore(Score, stageClear, BuildRunRating(), BuildRunSummary(), comboState.MaxCombo, comboState.MaxMultiplier);
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
            uiController.ShowGameOverOverlay("任务完成", Score);
            SessionState.CommitRunScore(Score, true, BuildRunRating(), BuildRunSummary(), comboState.MaxCombo, comboState.MaxMultiplier);
            yield return new WaitForSeconds(1.15f);
            SceneNavigator.LoadGameOver();
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

        private void ApplyPowerMeterUpgrade(PowerMeterUpgrade upgrade)
        {
            switch (upgrade)
            {
                case PowerMeterUpgrade.SpeedUp:
                    speedUpLevel = Mathf.Min(3, speedUpLevel + 1);
                    ShowStageBanner("速度提升 " + speedUpLevel);
                    break;
                case PowerMeterUpgrade.Missile:
                    ApplyWeaponPickup(WeaponType.Burst);
                    ShowStageBanner("导弹火力");
                    break;
                case PowerMeterUpgrade.Double:
                    weaponState.SetWeapon(WeaponType.Spread);
                    weaponState.UpgradeFireLevel();
                    ShowStageBanner("双发火力 " + weaponState.FireLevel + "级");
                    break;
                case PowerMeterUpgrade.Laser:
                    ApplyWeaponPickup(WeaponType.Laser);
                    ShowStageBanner("激光贯穿");
                    break;
                case PowerMeterUpgrade.Option:
                    weaponState.UpgradeFireLevel();
                    ShowStageBanner("子机火力 " + weaponState.FireLevel + "级");
                    break;
                case PowerMeterUpgrade.Shield:
                    shieldCharges = Mathf.Min(3, shieldCharges + 1);
                    ShowStageBanner("护盾就绪");
                    break;
            }
        }

        private static string GetPowerMeterUpgradeLabel(PowerMeterUpgrade upgrade)
        {
            switch (upgrade)
            {
                case PowerMeterUpgrade.SpeedUp:
                    return "速度";
                case PowerMeterUpgrade.Missile:
                    return "导弹";
                case PowerMeterUpgrade.Double:
                    return "双发";
                case PowerMeterUpgrade.Laser:
                    return "激光";
                case PowerMeterUpgrade.Option:
                    return "子机";
                case PowerMeterUpgrade.Shield:
                    return "护盾";
                default:
                    return "待充能";
            }
        }

        private void HandleWavePhaseChanged(WavePhase phase)
        {
            if (phase == WavePhase.Burst)
            {
                ShowStageBanner("危险突袭");
            }
            else if (phase == WavePhase.Reward)
            {
                ShowStageBanner("奖励航线");
            }

            uiController.RefreshHud();
        }

        private string BuildRunRating()
        {
            if (!stageClear)
            {
                return "丙";
            }

            if (Difficulty == GameDifficulty.High)
            {
                return "特";
            }

            return Difficulty == GameDifficulty.Medium ? "甲" : "乙";
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
    }
}
