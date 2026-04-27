using UnityEngine;
using Wanwan.Runtime.Pools;

namespace Wanwan.Runtime
{
    public class BlockSpawner : MonoBehaviour
    {
        private GameManager gameManager;
        private EffectsController effectsController;
        private PoolCollection pools;
        private StageGameplayProfile gameplayProfile;
        private float leftBound;
        private float rightBound;
        private float spawnY;
        private bool spawningEnabled = true;
        private bool bossActive;
        private int activeEnemyCount;
        private int groundTargetSpawnSequence;
        private float phaseTimer;
        private int currentWaveIndex = -1;
        private int nextInstructionIndex;
        private StageWaveConfig[] stageScript;
        private StageWaveConfig currentWave;
        private WavePhase observedWavePhase = WavePhase.Calm;
        private bool rewardGuaranteedPowerupGranted;
        private int powerCapsulesSpawned;
        private int guaranteedAmmoPackIndex;
        private int medicalPacksSpawned;

        public void Initialize(GameManager manager, EffectsController effects, Camera camera, float minX, float maxX, float topY)
        {
            gameManager = manager;
            effectsController = effects;
            pools = FindObjectOfType<GameBootstrap>()?.Pools;
            leftBound = minX;
            rightBound = maxX;
            spawnY = topY;
            gameplayProfile = StageCatalog.GetGameplayProfile(manager.StageNumber - 1);
            stageScript = BuildStageScript();
            AdvanceToNextWave();
        }

        private void Update()
        {
            if (!spawningEnabled || !gameManager.IsPlaying || currentWave == null)
            {
                return;
            }

            UpdateObservedWavePhase();
            phaseTimer += Time.deltaTime;
            float duration = Mathf.Max(0.01f, currentWave.DurationSeconds);
            gameManager.UpdateStageProgress(Mathf.Clamp01(phaseTimer / duration));

            while (nextInstructionIndex < currentWave.Instructions.Length && phaseTimer >= currentWave.Instructions[nextInstructionIndex].Time)
            {
                SpawnFormation(currentWave.Instructions[nextInstructionIndex]);
                nextInstructionIndex++;
            }

            bool allInstructionsSpawned = nextInstructionIndex >= currentWave.Instructions.Length;
            bool phaseExpired = phaseTimer >= currentWave.DurationSeconds;
            bool phaseCleared = activeEnemyCount <= 0 && !bossActive;

            if (currentWave.Phase == StagePhase.Boss)
            {
                return;
            }

            if (allInstructionsSpawned && phaseExpired && (!currentWave.WaitForClear || phaseCleared))
            {
                AdvanceToNextWave();
            }
        }

        public void StopSpawning()
        {
            spawningEnabled = false;
        }

        public void NotifyEnemyResolved()
        {
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        }

        public void NotifyBossResolved()
        {
            bossActive = false;
            activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        }

        public void ReturnEnemyObject(GameObject obj)
        {
            if (pools != null)
            {
                pools.ReturnEnemy(obj);
                return;
            }

            Destroy(obj);
        }

        public void ReturnPickupObject(GameObject obj)
        {
            if (pools != null)
            {
                pools.ReturnPickup(obj);
                return;
            }

            Destroy(obj);
        }

        public void ReturnFireballObject(GameObject obj)
        {
            if (pools != null)
            {
                pools.ReturnFireball(obj);
                return;
            }

            Destroy(obj);
        }

        public void SpawnEnemyMissile(Vector3 origin, Vector2 direction, Color color, bool fromBoss = false)
        {
            GameObject fireballObject = pools != null ? pools.RentFireball() : new GameObject(fromBoss ? "BossMissile" : "EnemyMissile");
            fireballObject.transform.position = origin;

            SpriteRenderer renderer = fireballObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBulletSprite(fromBoss ? AmmoPowerupType.Burst : AmmoPowerupType.Normal);
            renderer.color = fromBoss ? Color.Lerp(color, gameManager.StageAccentColor, 0.26f) : color;
            renderer.sortingOrder = 14;
            fireballObject.transform.localScale = WeaponShotPresentation.GetEnemyScale(fromBoss);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            fireballObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            BoxCollider2D collider = fireballObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = WeaponShotPresentation.GetEnemyColliderSize(fromBoss);

            Rigidbody2D rigidbody2D = fireballObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            float speed = DifficultyProgression.GetEnemyFireballSpeed(gameManager.ElapsedTime, fromBoss || gameManager.Difficulty == GameDifficulty.High);
            if (fromBoss)
            {
                speed *= gameManager.Difficulty == GameDifficulty.High ? 1.22f : 1.1f;
                speed *= GetBossPatternSpeedMultiplier(gameManager.BossPatternStyle);
            }

            EnemyFireballController fireball = fireballObject.AddComponent<EnemyFireballController>();
            fireball.Initialize(gameManager, effectsController, this, speed, direction, gameManager.BottomBound - 1.2f, gameManager.LeftBound, gameManager.RightBound, renderer.color);
        }

        public void SpawnAmmoPackAtPosition(AmmoPowerupType type, Vector3 position, AmmoPackPickupMode pickupMode = AmmoPackPickupMode.Normal)
        {
            if (type == AmmoPowerupType.None)
            {
                return;
            }

            GameObject packObject = pools != null ? pools.RentPickup() : new GameObject(type + "Pack");
            packObject.transform.position = position;
            packObject.transform.localScale = Vector3.one * PickupPresentation.AmmoPackVisualScale;

            SpriteRenderer renderer = packObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetAmmoPackSprite(type);
            renderer.color = GetAmmoPackColor(type);
            renderer.sortingOrder = 11;

            CircleCollider2D collider = packObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = PickupPresentation.AmmoPackColliderRadius;

            Rigidbody2D rigidbody2D = packObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            AmmoPackController packController = packObject.AddComponent<AmmoPackController>();
            packController.Initialize(gameManager, effectsController, this, type, DifficultyProgression.GetAmmoPackSpeed(gameManager.ElapsedTime), gameManager.BottomBound - 1.25f, gameManager.LeftBound, gameManager.RightBound, gameManager.TopBound, renderer.color, GetAmmoPackLabel(type), pickupMode);
        }

        public void SpawnAmmoPackAtPosition(WeaponType type, Vector3 position, AmmoPackPickupMode pickupMode = AmmoPackPickupMode.Normal)
        {
            SpawnAmmoPackAtPosition(PowerupCycle.ToAmmoPowerupType(type), position, pickupMode);
        }

        public void SpawnCoinsAtPosition(Vector3 position)
        {
            int count = Mathf.CeilToInt(gameManager.RewardConfig.CoinsPerEnemy * GetCoinDropMultiplier());
            for (int i = 0; i < count; i++)
            {
                GameObject coinObject = pools != null ? pools.RentPickup() : new GameObject("Coin");
                coinObject.transform.position = position + new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(-0.14f, 0.2f), 0f);
                coinObject.transform.localScale = Vector3.one * CoinController.BaseVisualScale;

                SpriteRenderer renderer = coinObject.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.GetCoinSprite();
                renderer.color = Color.white;
                renderer.sortingOrder = 18;

            CoinController coin = coinObject.AddComponent<CoinController>();
            float angle = ((Mathf.PI * 2f) / Mathf.Max(1, count)) * i;
            Vector2 drift = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(0.55f, 1.05f);
            drift.y = Mathf.Abs(drift.y) * 0.65f;
            coin.Initialize(gameManager, this, drift);
            }
        }

        public void SpawnEnemyAmmoPackDrop(AmmoPowerupType guaranteedDrop, Vector3 position)
        {
            TrySpawnPowerCapsule(position);
            if (TrySpawnGuaranteedStagePickup(position))
            {
                return;
            }

            if (guaranteedDrop != AmmoPowerupType.None)
            {
                SpawnAmmoPackAtPosition(guaranteedDrop, position);
                return;
            }

            if (gameManager.CurrentWavePhase != WavePhase.Reward)
            {
                float pressureChance = gameManager.CurrentWavePhase == WavePhase.Burst ? 0.16f : 0.08f;
                if (gameManager.CurrentWavePhase == WavePhase.Pressure || gameManager.CurrentWavePhase == WavePhase.Burst)
                {
                    if (Random.value <= pressureChance)
                    {
                        SpawnAmmoPackAtPosition(GetRandomPowerupType(), position);
                    }
                }

                return;
            }

            if (!rewardGuaranteedPowerupGranted)
            {
                rewardGuaranteedPowerupGranted = true;
                SpawnAmmoPackAtPosition(GetRandomPowerupType(), position);
                return;
            }

            if (Random.value <= 0.36f)
            {
                SpawnAmmoPackAtPosition(GetRandomPowerupType(), position);
            }
        }

        public void SpawnBombPickupAtRandomReachablePosition()
        {
            SpawnArea area = GetBombPickupSpawnArea(leftBound, rightBound, gameManager.BottomBound, gameManager.TopBound);
            Vector3 position = new Vector3(
                Random.Range(area.MinX, area.MaxX),
                Random.Range(area.MinY, area.MaxY),
                0f);
            SpawnBombPickupAtPosition(position);
        }

        public static SpawnArea GetBombPickupSpawnArea(float minX, float maxX, float bottomY, float topY)
        {
            float horizontalPadding = 0.7f;
            float minY = bottomY + 2f;
            float maxY = Mathf.Lerp(bottomY, topY, 0.4f);
            if (maxY < minY)
            {
                maxY = minY;
            }

            return new SpawnArea(minX + horizontalPadding, maxX - horizontalPadding, minY, maxY);
        }

        public void SpawnBombPickupAtPosition(Vector3 position)
        {
            GameObject bombObject = pools != null ? pools.RentPickup() : new GameObject("BombPickup");
            bombObject.transform.position = position;
            bombObject.transform.localScale = Vector3.one * PickupPresentation.BombVisualScale;

            SpriteRenderer renderer = bombObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBombPickupSprite();
            renderer.sortingOrder = 19;

            CircleCollider2D collider = bombObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = PickupPresentation.BombColliderRadius;

            Rigidbody2D rigidbody2D = bombObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BombPickupController pickup = bombObject.AddComponent<BombPickupController>();
            pickup.Initialize(gameManager, this);
            effectsController.PlayPowerupSpawn(position, new Color(0.45f, 0.86f, 1f));
        }

        public void SpawnHealthPickupAtPosition(Vector3 position)
        {
            GameObject healthObject = pools != null ? pools.RentPickup() : new GameObject("HealthPickup");
            healthObject.transform.position = position + new Vector3(-0.28f, 0.22f, 0f);
            healthObject.transform.localScale = Vector3.one * PickupPresentation.HealthPickupVisualScale;

            SpriteRenderer renderer = healthObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetHealthPickupSprite();
            renderer.sortingOrder = 19;

            CircleCollider2D collider = healthObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = PickupPresentation.HealthPickupColliderRadius;

            Rigidbody2D rigidbody2D = healthObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            HealthPickupController pickup = healthObject.AddComponent<HealthPickupController>();
            pickup.Initialize(gameManager, this);
            effectsController.PlayPowerupSpawn(position, new Color(0.35f, 1f, 0.62f));
        }

        private void SpawnPowerCapsuleAtPosition(Vector3 position)
        {
            GameObject capsuleObject = pools != null ? pools.RentPickup() : new GameObject("PowerCapsule");
            capsuleObject.transform.position = position + new Vector3(0.28f, 0.22f, 0f);
            capsuleObject.transform.localScale = Vector3.one * PickupPresentation.PowerCapsuleVisualScale;

            SpriteRenderer renderer = capsuleObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetCapsuleSprite();
            renderer.color = new Color(0.42f, 0.9f, 1f);
            renderer.sortingOrder = 19;

            CircleCollider2D collider = capsuleObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = PickupPresentation.PowerCapsuleColliderRadius;

            Rigidbody2D rigidbody2D = capsuleObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            PowerCapsuleController capsule = capsuleObject.AddComponent<PowerCapsuleController>();
            capsule.Initialize(gameManager, effectsController, this, DifficultyProgression.GetAmmoPackSpeed(gameManager.ElapsedTime) * 0.86f, gameManager.BottomBound - 1.25f);
        }

        private void TrySpawnPowerCapsule(Vector3 position)
        {
            if (gameplayProfile == null || powerCapsulesSpawned >= gameplayProfile.PowerMeterCapsuleBudget)
            {
                return;
            }

            if (gameManager.CurrentStagePhase == StagePhase.Preparation || gameManager.CurrentStagePhase == StagePhase.Boss)
            {
                return;
            }

            if (gameManager.StageProgress < gameplayProfile.GetPowerCapsuleUnlockProgress(powerCapsulesSpawned))
            {
                return;
            }

            powerCapsulesSpawned++;
            SpawnPowerCapsuleAtPosition(position);
        }

        private bool TrySpawnGuaranteedStagePickup(Vector3 position)
        {
            if (gameplayProfile == null || gameManager.CurrentStagePhase == StagePhase.Preparation || gameManager.CurrentStagePhase == StagePhase.Boss)
            {
                return false;
            }

            int nextGuaranteedPickupIndex = medicalPacksSpawned + guaranteedAmmoPackIndex;
            if (gameManager.StageProgress < gameplayProfile.GetGuaranteedPickupUnlockProgress(nextGuaranteedPickupIndex))
            {
                return false;
            }

            if (medicalPacksSpawned < gameplayProfile.GuaranteedMedicalPacks)
            {
                medicalPacksSpawned++;
                SpawnHealthPickupAtPosition(position);
                return true;
            }

            AmmoPowerupType[] guaranteedPacks = gameplayProfile.GuaranteedAmmoPacks;
            if (guaranteedPacks != null && guaranteedAmmoPackIndex < guaranteedPacks.Length)
            {
                SpawnAmmoPackAtPosition(guaranteedPacks[guaranteedAmmoPackIndex], position);
                guaranteedAmmoPackIndex++;
                return true;
            }

            return false;
        }

        private void AdvanceToNextWave()
        {
            currentWaveIndex++;
            if (currentWaveIndex >= stageScript.Length)
            {
                currentWave = null;
                return;
            }

            currentWave = stageScript[currentWaveIndex];
            nextInstructionIndex = 0;
            phaseTimer = 0f;
            gameManager.SetStageState(currentWave.Phase, currentWave.Banner);

            if (currentWave.Phase == StagePhase.Boss)
            {
                SpawnBoss();
            }
        }

        private StageWaveConfig[] BuildStageScript()
        {
            StageCombatStyle style = gameManager.StageCombatStyle;
            StageWaveConfig[] script;
            if (style == StageCombatStyle.Flanking)
            {
                script = BuildFlankingStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            if (style == StageCombatStyle.Swarm)
            {
                script = BuildSwarmStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            if (style == StageCombatStyle.Sniper)
            {
                script = BuildSniperStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            if (style == StageCombatStyle.Heavy)
            {
                script = BuildHeavyStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            if (style == StageCombatStyle.Agile)
            {
                script = BuildAgileStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            if (style == StageCombatStyle.Spiral)
            {
                script = BuildSpiralStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            if (style == StageCombatStyle.Finale)
            {
                script = BuildFinaleStageScript();
                return ApplyStageDurationMultiplier(script);
            }

            script = BuildBalancedStageScript();
            return ApplyStageDurationMultiplier(script);
        }

        private StageWaveConfig[] ApplyStageDurationMultiplier(StageWaveConfig[] script)
        {
            float multiplier = StageCatalog.GetStageDurationMultiplier(gameManager.StageNumber - 1);
            if (Mathf.Approximately(multiplier, 1f))
            {
                return script;
            }

            StageWaveConfig[] adjusted = new StageWaveConfig[script.Length];
            for (int i = 0; i < script.Length; i++)
            {
                StageWaveConfig wave = script[i];
                if (wave.Phase == StagePhase.Boss)
                {
                    adjusted[i] = wave;
                    continue;
                }

                EnemySpawnInstruction[] instructions = new EnemySpawnInstruction[wave.Instructions.Length];
                for (int j = 0; j < instructions.Length; j++)
                {
                    EnemySpawnInstruction instruction = wave.Instructions[j];
                    instructions[j] = new EnemySpawnInstruction(instruction.Time * multiplier, instruction.Formation, instruction.Count, instruction.Elite, instruction.GuaranteedDrop, instruction.EnemyType);
                }

                adjusted[i] = new StageWaveConfig(wave.Phase, wave.Banner, wave.DurationSeconds * multiplier, wave.WaitForClear, instructions);
            }

            return adjusted;
        }

        private StageWaveConfig[] BuildBalancedStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "敌机来袭", 1.15f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(
                    StagePhase.Assault,
                    "编队突袭",
                    9.6f,
                    true,
                    new[]
                    {
                        new EnemySpawnInstruction(0.3f, EnemyFormationType.DiveLine, 3),
                        new EnemySpawnInstruction(2.0f, EnemyFormationType.SideCutInLeft, 4),
                        new EnemySpawnInstruction(3.7f, EnemyFormationType.VShape, 5),
                        new EnemySpawnInstruction(5.8f, EnemyFormationType.SnakeSweep, 5, false, AmmoPowerupType.Scatter),
                        new EnemySpawnInstruction(7.1f, EnemyFormationType.DiveLine, 4),
                        new EnemySpawnInstruction(8.2f, EnemyFormationType.SideCutInRight, 4)
                    }),
                new StageWaveConfig(
                    StagePhase.Pressure,
                    "火力压制",
                    10.8f,
                    true,
                    new[]
                    {
                        new EnemySpawnInstruction(0.2f, EnemyFormationType.SideCutInRight, 5),
                        new EnemySpawnInstruction(1.8f, EnemyFormationType.DiveLine, 4),
                        new EnemySpawnInstruction(3.4f, EnemyFormationType.SnakeSweep, 6),
                        new EnemySpawnInstruction(5.5f, EnemyFormationType.VShape, 5, false, AmmoPowerupType.RapidFire),
                        new EnemySpawnInstruction(6.8f, EnemyFormationType.DiveLine, 2, enemyType: EnemyType.SelfDestruct),
                        new EnemySpawnInstruction(7.2f, EnemyFormationType.SideCutInLeft, 5),
                        new EnemySpawnInstruction(8.4f, EnemyFormationType.DiveLine, 5, false, AmmoPowerupType.Laser),
                        new EnemySpawnInstruction(9.5f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Laser)
                    }),
                new StageWaveConfig(
                    StagePhase.Elite,
                    "精英来袭",
                    7.5f,
                    true,
                    new[]
                    {
                        new EnemySpawnInstruction(0.4f, EnemyFormationType.VShape, 2),
                        new EnemySpawnInstruction(1.2f, EnemyFormationType.DiveLine, 1, true, AmmoPowerupType.Pierce),
                        new EnemySpawnInstruction(2.8f, EnemyFormationType.SideCutInRight, 2, enemyType: EnemyType.Shield),
                        new EnemySpawnInstruction(4.2f, EnemyFormationType.SnakeSweep, 2, true, AmmoPowerupType.Plasma),
                        new EnemySpawnInstruction(5.6f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Homing),
                        new EnemySpawnInstruction(6.2f, EnemyFormationType.DiveLine, 1, enemyType: EnemyType.Healer),
                        new EnemySpawnInstruction(6.8f, EnemyFormationType.SnakeSweep, 2, enemyType: EnemyType.Barrage)
                    }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildFlankingStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "海峡低空接敌", 1.05f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "双翼包夹", 10.2f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.SideCutInLeft, 4),
                    new EnemySpawnInstruction(1.4f, EnemyFormationType.SideCutInRight, 4),
                    new EnemySpawnInstruction(2.8f, EnemyFormationType.DiveLine, 3),
                    new EnemySpawnInstruction(4.4f, EnemyFormationType.SideCutInLeft, 5, false, AmmoPowerupType.Wave),
                    new EnemySpawnInstruction(6.4f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(8.1f, EnemyFormationType.VShape, 4, false, AmmoPowerupType.RapidFire)
                }),
                new StageWaveConfig(StagePhase.Pressure, "交叉火线", 11.2f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(1.2f, EnemyFormationType.SideCutInLeft, 5),
                    new EnemySpawnInstruction(3.0f, EnemyFormationType.SnakeSweep, 5),
                    new EnemySpawnInstruction(5.1f, EnemyFormationType.SideCutInLeft, 4, true, AmmoPowerupType.Pierce),
                    new EnemySpawnInstruction(7.0f, EnemyFormationType.SideCutInRight, 4, true, AmmoPowerupType.Laser),
                    new EnemySpawnInstruction(9.1f, EnemyFormationType.DiveLine, 5)
                }),
                new StageWaveConfig(StagePhase.Elite, "拦截中队", 7.2f, true, new[]
                {
                    new EnemySpawnInstruction(0.4f, EnemyFormationType.SideCutInLeft, 2, true),
                    new EnemySpawnInstruction(1.8f, EnemyFormationType.SideCutInRight, 2, true),
                    new EnemySpawnInstruction(3.4f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Homing),
                    new EnemySpawnInstruction(5.2f, EnemyFormationType.SnakeSweep, 3, true, AmmoPowerupType.Scatter)
                }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildSwarmStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "林区伏击", 0.95f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "密集蜂群", 10.6f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.DiveLine, 5),
                    new EnemySpawnInstruction(1.6f, EnemyFormationType.VShape, 6, false, AmmoPowerupType.Scatter),
                    new EnemySpawnInstruction(3.1f, EnemyFormationType.SnakeSweep, 7),
                    new EnemySpawnInstruction(5.0f, EnemyFormationType.DiveLine, 6),
                    new EnemySpawnInstruction(6.6f, EnemyFormationType.VShape, 7, false, AmmoPowerupType.RapidFire),
                    new EnemySpawnInstruction(8.5f, EnemyFormationType.SnakeSweep, 6)
                }),
                new StageWaveConfig(StagePhase.Pressure, "连续突入", 11.4f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.DiveLine, 6),
                    new EnemySpawnInstruction(1.4f, EnemyFormationType.DiveLine, 6),
                    new EnemySpawnInstruction(2.8f, EnemyFormationType.SnakeSweep, 7),
                    new EnemySpawnInstruction(4.7f, EnemyFormationType.VShape, 6),
                    new EnemySpawnInstruction(6.5f, EnemyFormationType.SideCutInLeft, 5),
                    new EnemySpawnInstruction(7.5f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(9.3f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Plasma)
                }),
                new StageWaveConfig(StagePhase.Elite, "林冠精英", 7.4f, true, new[]
                {
                    new EnemySpawnInstruction(0.3f, EnemyFormationType.VShape, 3, true),
                    new EnemySpawnInstruction(2.2f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Wave),
                    new EnemySpawnInstruction(4.2f, EnemyFormationType.DiveLine, 3, true, AmmoPowerupType.Homing)
                }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildSniperStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "沙暴锁定", 1.25f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "远距点射", 10.8f, true, new[]
                {
                    new EnemySpawnInstruction(0.4f, EnemyFormationType.DiveLine, 3),
                    new EnemySpawnInstruction(2.0f, EnemyFormationType.VShape, 4, false, AmmoPowerupType.Pierce),
                    new EnemySpawnInstruction(3.8f, EnemyFormationType.SideCutInLeft, 3),
                    new EnemySpawnInstruction(5.4f, EnemyFormationType.SideCutInRight, 3),
                    new EnemySpawnInstruction(7.0f, EnemyFormationType.DiveLine, 4),
                    new EnemySpawnInstruction(8.8f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Laser)
                }),
                new StageWaveConfig(StagePhase.Pressure, "炮线封锁", 11.6f, true, new[]
                {
                    new EnemySpawnInstruction(0.3f, EnemyFormationType.VShape, 4),
                    new EnemySpawnInstruction(2.1f, EnemyFormationType.DiveLine, 4),
                    new EnemySpawnInstruction(4.1f, EnemyFormationType.SideCutInLeft, 4),
                    new EnemySpawnInstruction(5.9f, EnemyFormationType.SideCutInRight, 4),
                    new EnemySpawnInstruction(7.5f, EnemyFormationType.DiveLine, 2, true, AmmoPowerupType.Plasma),
                    new EnemySpawnInstruction(9.4f, EnemyFormationType.VShape, 5)
                }),
                new StageWaveConfig(StagePhase.Elite, "沙暴炮手", 7.8f, true, new[]
                {
                    new EnemySpawnInstruction(0.5f, EnemyFormationType.DiveLine, 1, true),
                    new EnemySpawnInstruction(2.2f, EnemyFormationType.SideCutInLeft, 2, true, AmmoPowerupType.Burst),
                    new EnemySpawnInstruction(4.3f, EnemyFormationType.SideCutInRight, 2, true, AmmoPowerupType.Homing)
                }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildHeavyStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "重装推进", 1.2f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "装甲梯队", 10.8f, true, new[]
                {
                    new EnemySpawnInstruction(0.4f, EnemyFormationType.VShape, 4),
                    new EnemySpawnInstruction(2.2f, EnemyFormationType.DiveLine, 3, true, AmmoPowerupType.Plasma),
                    new EnemySpawnInstruction(4.2f, EnemyFormationType.SnakeSweep, 4),
                    new EnemySpawnInstruction(6.0f, EnemyFormationType.VShape, 5, true, AmmoPowerupType.Burst),
                    new EnemySpawnInstruction(8.4f, EnemyFormationType.SideCutInLeft, 4)
                }),
                new StageWaveConfig(StagePhase.Pressure, "钢铁压制", 12f, true, new[]
                {
                    new EnemySpawnInstruction(0.3f, EnemyFormationType.DiveLine, 4, true),
                    new EnemySpawnInstruction(2.4f, EnemyFormationType.SideCutInRight, 4),
                    new EnemySpawnInstruction(4.2f, EnemyFormationType.VShape, 5, true, AmmoPowerupType.Guard),
                    new EnemySpawnInstruction(5.8f, EnemyFormationType.DiveLine, 3, enemyType: EnemyType.SelfDestruct),
                    new EnemySpawnInstruction(6.6f, EnemyFormationType.SnakeSweep, 5),
                    new EnemySpawnInstruction(8.8f, EnemyFormationType.DiveLine, 3, true, AmmoPowerupType.Laser),
                    new EnemySpawnInstruction(10.1f, EnemyFormationType.VShape, 2, enemyType: EnemyType.Shield)
                }),
                new StageWaveConfig(StagePhase.Elite, "重型护卫", 8f, true, new[]
                {
                    new EnemySpawnInstruction(0.5f, EnemyFormationType.VShape, 2, true),
                    new EnemySpawnInstruction(2.6f, EnemyFormationType.DiveLine, 2, true, AmmoPowerupType.Pierce),
                    new EnemySpawnInstruction(4.8f, EnemyFormationType.SnakeSweep, 3, true, AmmoPowerupType.Homing),
                    new EnemySpawnInstruction(5.8f, EnemyFormationType.VShape, 2, enemyType: EnemyType.Barrage),
                    new EnemySpawnInstruction(6.8f, EnemyFormationType.DiveLine, 1, enemyType: EnemyType.Healer)
                }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildAgileStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "云层穿梭", 0.9f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "高速俯冲", 9.8f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.DiveLine, 4),
                    new EnemySpawnInstruction(1.3f, EnemyFormationType.SnakeSweep, 5, false, AmmoPowerupType.RapidFire),
                    new EnemySpawnInstruction(2.8f, EnemyFormationType.SideCutInLeft, 4),
                    new EnemySpawnInstruction(4.0f, EnemyFormationType.SideCutInRight, 4),
                    new EnemySpawnInstruction(5.4f, EnemyFormationType.SnakeSweep, 6),
                    new EnemySpawnInstruction(7.2f, EnemyFormationType.VShape, 5, false, AmmoPowerupType.Wave)
                }),
                new StageWaveConfig(StagePhase.Pressure, "云间穿插", 10.6f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.SnakeSweep, 6),
                    new EnemySpawnInstruction(1.8f, EnemyFormationType.SideCutInLeft, 5),
                    new EnemySpawnInstruction(3.0f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(4.7f, EnemyFormationType.DiveLine, 5),
                    new EnemySpawnInstruction(6.5f, EnemyFormationType.SnakeSweep, 5, true, AmmoPowerupType.Homing),
                    new EnemySpawnInstruction(8.5f, EnemyFormationType.VShape, 4)
                }),
                new StageWaveConfig(StagePhase.Elite, "高空王牌", 7.2f, true, new[]
                {
                    new EnemySpawnInstruction(0.3f, EnemyFormationType.SnakeSweep, 3, true),
                    new EnemySpawnInstruction(2.4f, EnemyFormationType.SideCutInLeft, 2, true, AmmoPowerupType.Laser),
                    new EnemySpawnInstruction(4.3f, EnemyFormationType.SideCutInRight, 2, true, AmmoPowerupType.Scatter)
                }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildSpiralStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "轨道封锁", 1.05f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "轨道螺旋", 10.4f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.SnakeSweep, 5),
                    new EnemySpawnInstruction(1.7f, EnemyFormationType.VShape, 5, false, AmmoPowerupType.Wave),
                    new EnemySpawnInstruction(3.3f, EnemyFormationType.SnakeSweep, 6),
                    new EnemySpawnInstruction(5.2f, EnemyFormationType.SideCutInLeft, 4),
                    new EnemySpawnInstruction(6.4f, EnemyFormationType.SideCutInRight, 4),
                    new EnemySpawnInstruction(8.1f, EnemyFormationType.SnakeSweep, 5, true, AmmoPowerupType.Plasma)
                }),
                new StageWaveConfig(StagePhase.Pressure, "环形锁网", 11.4f, true, new[]
                {
                    new EnemySpawnInstruction(0.3f, EnemyFormationType.VShape, 6),
                    new EnemySpawnInstruction(2.0f, EnemyFormationType.SnakeSweep, 7),
                    new EnemySpawnInstruction(4.2f, EnemyFormationType.SideCutInLeft, 5),
                    new EnemySpawnInstruction(5.3f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(7.4f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Guard),
                    new EnemySpawnInstruction(9.2f, EnemyFormationType.VShape, 5, true, AmmoPowerupType.Homing)
                }),
                new StageWaveConfig(StagePhase.Elite, "轨道守卫", 7.8f, true, new[]
                {
                    new EnemySpawnInstruction(0.5f, EnemyFormationType.SnakeSweep, 3, true),
                    new EnemySpawnInstruction(2.8f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Burst),
                    new EnemySpawnInstruction(5.1f, EnemyFormationType.DiveLine, 2, true, AmmoPowerupType.Laser)
                }),
                BuildBossWave()
            };
        }

        private StageWaveConfig[] BuildFinaleStageScript()
        {
            return new[]
            {
                new StageWaveConfig(StagePhase.Preparation, "核心接触", 1f, false, new EnemySpawnInstruction[0]),
                new StageWaveConfig(StagePhase.Assault, "母舰外环", 10.6f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.VShape, 6),
                    new EnemySpawnInstruction(1.5f, EnemyFormationType.SideCutInLeft, 5),
                    new EnemySpawnInstruction(2.5f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(4.0f, EnemyFormationType.SnakeSweep, 7, false, AmmoPowerupType.Plasma),
                    new EnemySpawnInstruction(6.2f, EnemyFormationType.DiveLine, 6),
                    new EnemySpawnInstruction(8.0f, EnemyFormationType.VShape, 5, true, AmmoPowerupType.Burst)
                }),
                new StageWaveConfig(StagePhase.Pressure, "核心防卫圈", 12f, true, new[]
                {
                    new EnemySpawnInstruction(0.2f, EnemyFormationType.SnakeSweep, 7),
                    new EnemySpawnInstruction(1.8f, EnemyFormationType.VShape, 6, true, AmmoPowerupType.Laser),
                    new EnemySpawnInstruction(3.8f, EnemyFormationType.SideCutInLeft, 5),
                    new EnemySpawnInstruction(4.8f, EnemyFormationType.SideCutInRight, 5),
                    new EnemySpawnInstruction(5.6f, EnemyFormationType.DiveLine, 3, enemyType: EnemyType.SelfDestruct),
                    new EnemySpawnInstruction(6.8f, EnemyFormationType.DiveLine, 5, true, AmmoPowerupType.Homing),
                    new EnemySpawnInstruction(8.8f, EnemyFormationType.SnakeSweep, 6),
                    new EnemySpawnInstruction(9.6f, EnemyFormationType.DiveLine, 2, enemyType: EnemyType.Barrage),
                    new EnemySpawnInstruction(10.2f, EnemyFormationType.VShape, 4, true, AmmoPowerupType.Guard)
                }),
                new StageWaveConfig(StagePhase.Elite, "核心护卫队", 8.2f, true, new[]
                {
                    new EnemySpawnInstruction(0.4f, EnemyFormationType.VShape, 3, true),
                    new EnemySpawnInstruction(2.2f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Wave),
                    new EnemySpawnInstruction(4.4f, EnemyFormationType.SideCutInLeft, 2, true, AmmoPowerupType.Pierce),
                    new EnemySpawnInstruction(5.2f, EnemyFormationType.DiveLine, 2, enemyType: EnemyType.Shield),
                    new EnemySpawnInstruction(5.8f, EnemyFormationType.SideCutInRight, 3, true, AmmoPowerupType.Scatter),
                    new EnemySpawnInstruction(6.8f, EnemyFormationType.DiveLine, 1, enemyType: EnemyType.Healer)
                }),
                BuildBossWave()
            };
        }

        private static StageWaveConfig BuildBossWave()
        {
            return new StageWaveConfig(StagePhase.Boss, "危险警报", 999f, true, new EnemySpawnInstruction[0]);
        }

        private void SpawnFormation(EnemySpawnInstruction instruction)
        {
            EnemyFormationType formation = GetPacedFormation(instruction.Formation);
            switch (formation)
            {
                case EnemyFormationType.SideCutInLeft:
                    SpawnSideCutIn(instruction, true);
                    break;
                case EnemyFormationType.SideCutInRight:
                    SpawnSideCutIn(instruction, false);
                    break;
                case EnemyFormationType.VShape:
                    SpawnVShape(instruction);
                    break;
                case EnemyFormationType.SnakeSweep:
                    SpawnSnakeSweep(instruction);
                    break;
                default:
                    SpawnDiveLine(instruction);
                    break;
            }

            SpawnGroundSupportIfNeeded(instruction, formation);
        }

        private void SpawnDiveLine(EnemySpawnInstruction instruction)
        {
            int spawnCount = GetSpawnCount(instruction);
            float spacing = 1.45f;
            float startX = -((spawnCount - 1) * spacing * 0.5f);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 position = new Vector3(startX + (i * spacing), spawnY + (i * 0.12f), 0f);
                SpawnEnemy(position, instruction.Elite, Vector2.down, 0f, 0f, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None, instruction.EnemyType);
            }
        }

        private void SpawnSideCutIn(EnemySpawnInstruction instruction, bool fromLeft)
        {
            int spawnCount = GetSpawnCount(instruction);
            float x = fromLeft ? leftBound - 0.95f : rightBound + 0.95f;
            Vector2 direction = (fromLeft ? new Vector2(0.62f, -1f) : new Vector2(-0.62f, -1f)).normalized;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 position = new Vector3(x, spawnY - (i * 0.5f), 0f);
                SpawnEnemy(position, instruction.Elite, direction, 0f, 0f, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None, instruction.EnemyType);
            }
        }

        private void SpawnVShape(EnemySpawnInstruction instruction)
        {
            int spawnCount = GetSpawnCount(instruction);
            for (int i = 0; i < spawnCount; i++)
            {
                float offsetIndex = i - ((spawnCount - 1) * 0.5f);
                float x = offsetIndex * 1.15f;
                float y = spawnY + (Mathf.Abs(offsetIndex) * 0.28f);
                SpawnEnemy(new Vector3(x, y, 0f), instruction.Elite, Vector2.down, 0f, 0f, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None, instruction.EnemyType);
            }
        }

        private void SpawnSnakeSweep(EnemySpawnInstruction instruction)
        {
            int spawnCount = GetSpawnCount(instruction);
            float spacing = 1.25f;
            float startX = -((spawnCount - 1) * spacing * 0.5f);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 position = new Vector3(startX + (i * spacing), spawnY + (i * 0.08f), 0f);
                float swayAmplitude = 0.8f + (i * 0.05f);
                float swayFrequency = 2.4f + (i * 0.12f);
                SpawnEnemy(position, instruction.Elite, Vector2.down, swayAmplitude, swayFrequency, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None, instruction.EnemyType);
            }
        }

        private int GetSpawnCount(EnemySpawnInstruction instruction)
        {
            int adjustedCount = EnemySpawnBudget.GetAdjustedCount(instruction.Count);
            float multiplier = GetSpawnCountMultiplier(gameManager.CurrentWavePhase);
            return Mathf.Max(1, Mathf.FloorToInt(adjustedCount * multiplier));
        }

        private void SpawnEnemy(Vector3 position, bool elite, Vector2 moveDirection, float swayAmplitude, float swayFrequency, AmmoPowerupType guaranteedDrop, EnemyType enemyType = EnemyType.Normal)
        {
            bool pacedElite = ShouldSpawnElite(elite);
            bool tough = pacedElite || ShouldSpawnTough();
            int hitPoints = pacedElite ? GetEliteHitPoints() : DifficultyProgression.GetHitPoints(tough, gameManager.ElapsedTime);
            hitPoints += Mathf.FloorToInt((gameManager.StageDifficultyMultiplier - 1f) * (pacedElite ? 3f : 1.25f));
            int scoreValue = pacedElite ? 260 : DifficultyProgression.GetScoreValue(tough, hitPoints);
            float speed = pacedElite ? DifficultyProgression.GetBlockSpeed(gameManager.ElapsedTime, true) * 0.82f : DifficultyProgression.GetBlockSpeed(gameManager.ElapsedTime, tough);
            speed *= Mathf.Lerp(1f, gameManager.StageDifficultyMultiplier, 0.32f);

            GameObject enemyObject = pools != null ? pools.RentEnemy() : new GameObject(pacedElite ? "ElitePlane" : (tough ? "ToughPlane" : "Plane"));
            enemyObject.transform.position = position;

            SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
            if (pacedElite)
            {
                renderer.sprite = RuntimeSpriteFactory.GetEliteInterceptorSprite();
            }
            else if (tough)
            {
                renderer.sprite = RuntimeSpriteFactory.GetToughInterceptorSprite();
            }
            else
            {
                renderer.sprite = RuntimeSpriteFactory.GetEnemyInterceptorSprite();
            }
            Color accent = gameManager.StageAccentColor;
            Color normalColor = Color.Lerp(new Color(0.88f, 0.26f, 0.46f), accent, 0.24f);
            Color toughColor = Color.Lerp(new Color(1f, 0.38f, 0.52f), accent, 0.18f);
            Color eliteColor = Color.Lerp(new Color(1f, 0.88f, 0.24f), accent, 0.22f);
            renderer.color = pacedElite ? eliteColor : Color.white;
            renderer.sortingOrder = pacedElite ? 12 : 10;

            enemyObject.transform.localScale = GetEnemyVisualScale(tough, pacedElite);
            enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f);

            BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = GetEnemyColliderSize(tough, pacedElite);

            Rigidbody2D rigidbody2D = enemyObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BlockController block = enemyObject.AddComponent<BlockController>();
            Color effectColor = pacedElite ? eliteColor : (tough ? toughColor : normalColor);
            block.Initialize(gameManager, this, effectsController, hitPoints, scoreValue, speed, effectColor, moveDirection, swayAmplitude, swayFrequency, pacedElite, guaranteedDrop, enemyType);
            activeEnemyCount++;
        }

        public static Vector3 GetEnemyVisualScale(bool tough, bool elite)
        {
            if (elite)
            {
                return new Vector3(0.74f, 0.72f, 1f);
            }

            if (tough)
            {
                return new Vector3(0.68f, 0.66f, 1f);
            }

            return new Vector3(0.72f, 0.7f, 1f);
        }

        public static Vector2 GetEnemyColliderSize(bool tough, bool elite)
        {
            if (elite)
            {
                return new Vector2(0.4f, 0.46f);
            }

            if (tough)
            {
                return new Vector2(0.38f, 0.45f);
            }

            return new Vector2(0.44f, 0.52f);
        }

        private void SpawnGroundSupportIfNeeded(EnemySpawnInstruction instruction, EnemyFormationType formation)
        {
            if (!ShouldSpawnGroundSupport(instruction, formation))
            {
                return;
            }

            int count = gameManager.StageCombatStyle == StageCombatStyle.Heavy || gameManager.StageCombatStyle == StageCombatStyle.Finale ? 2 : 1;
            for (int i = 0; i < count; i++)
            {
                GroundTargetType type = (groundTargetSpawnSequence + i) % 3 == 0 ? GroundTargetType.Turret : GroundTargetType.Tank;
                float laneT = count == 1 ? 0.5f : (i + 1f) / (count + 1f);
                float x = Mathf.Lerp(leftBound + 0.85f, rightBound - 0.85f, laneT);
                float y = spawnY + 0.85f + (i * 0.55f);
                SpawnGroundTarget(type, new Vector3(x, y, 0f));
            }

            groundTargetSpawnSequence += count;
        }

        private bool ShouldSpawnGroundSupport(EnemySpawnInstruction instruction, EnemyFormationType formation)
        {
            if (gameManager.CurrentWavePhase == WavePhase.Calm || gameManager.CurrentWavePhase == WavePhase.Reward)
            {
                return false;
            }

            float groundBias = gameplayProfile != null ? gameplayProfile.GroundThreatBias : 1f;
            if (gameManager.StageCombatStyle == StageCombatStyle.Heavy || gameManager.StageCombatStyle == StageCombatStyle.Finale)
            {
                return instruction.Count >= (groundBias > 1.3f ? 3 : 4);
            }

            if (groundBias > 1.25f && instruction.Count >= 4)
            {
                return true;
            }

            return formation == EnemyFormationType.VShape && (gameManager.StageCombatStyle == StageCombatStyle.Sniper || gameManager.StageCombatStyle == StageCombatStyle.Balanced);
        }

        private void SpawnGroundTarget(GroundTargetType type, Vector3 position)
        {
            GroundTargetProfile profile = GroundTargetProfile.Get(type, gameManager.StageDifficultyMultiplier);
            GameObject targetObject = pools != null ? pools.RentEnemy() : new GameObject(type == GroundTargetType.Turret ? "GroundTurret" : "GroundTank");
            targetObject.transform.position = position;
            targetObject.transform.localScale = type == GroundTargetType.Turret ? new Vector3(0.68f, 0.68f, 1f) : new Vector3(0.82f, 0.68f, 1f);

            SpriteRenderer renderer = targetObject.AddComponent<SpriteRenderer>();
            renderer.sprite = type == GroundTargetType.Turret ? RuntimeSpriteFactory.GetGroundTurretSprite() : RuntimeSpriteFactory.GetGroundTankSprite();
            renderer.color = Color.Lerp(profile.AccentColor, gameManager.StageAccentColor, 0.18f);
            renderer.sortingOrder = 8;

            BoxCollider2D collider = targetObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = type == GroundTargetType.Turret ? new Vector2(0.54f, 0.54f) : new Vector2(0.62f, 0.46f);

            Rigidbody2D rigidbody2D = targetObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            float scrollSpeed = 0.68f * gameManager.StageDifficultyMultiplier;
            GroundTargetController target = targetObject.AddComponent<GroundTargetController>();
            target.Initialize(gameManager, this, effectsController, profile, scrollSpeed);
            activeEnemyCount++;
        }

        private void UpdateObservedWavePhase()
        {
            WavePhase phase = gameManager.CurrentWavePhase;
            if (phase == observedWavePhase)
            {
                return;
            }

            observedWavePhase = phase;
            if (phase == WavePhase.Reward)
            {
                rewardGuaranteedPowerupGranted = false;
            }
        }

        private static float GetSpawnCountMultiplier(WavePhase phase)
        {
            switch (phase)
            {
                case WavePhase.Calm:
                    return 0.75f;
                case WavePhase.Burst:
                    return 1.35f;
                case WavePhase.Reward:
                    return 0.35f;
                default:
                    return 1f;
            }
        }

        private EnemyFormationType GetPacedFormation(EnemyFormationType formation)
        {
            WavePhase phase = gameManager.CurrentWavePhase;
            if (phase == WavePhase.Calm || phase == WavePhase.Reward)
            {
                return EnemyFormationType.DiveLine;
            }

            if (phase == WavePhase.Pressure && formation != EnemyFormationType.DiveLine && formation != EnemyFormationType.SideCutInLeft && formation != EnemyFormationType.SideCutInRight)
            {
                return Random.value < 0.5f ? EnemyFormationType.SideCutInLeft : EnemyFormationType.SideCutInRight;
            }

            return formation;
        }

        private bool ShouldSpawnElite(bool scriptedElite)
        {
            WavePhase phase = gameManager.CurrentWavePhase;
            if (phase == WavePhase.Reward)
            {
                return false;
            }

            if (scriptedElite)
            {
                return true;
            }

            if (phase == WavePhase.Burst)
            {
                return Random.value <= 0.12f;
            }

            return phase == WavePhase.Pressure && Random.value <= 0.04f;
        }

        private bool ShouldSpawnTough()
        {
            WavePhase phase = gameManager.CurrentWavePhase;
            if (phase == WavePhase.Reward)
            {
                return false;
            }

            float chance = DifficultyProgression.GetToughChance(gameManager.ElapsedTime);
            if (phase == WavePhase.Pressure)
            {
                chance += 0.08f;
            }
            else if (phase == WavePhase.Burst)
            {
                chance += 0.18f;
            }

            return Random.value <= Mathf.Clamp01(chance);
        }

        private float GetCoinDropMultiplier()
        {
            return gameManager.CurrentWavePhase == WavePhase.Reward ? 1.5f : 1f;
        }

        private void SpawnBoss()
        {
            GameObject bossObject = pools != null ? pools.RentEnemy() : new GameObject("BossFlagship");
            bossObject.transform.position = new Vector3(0f, spawnY + 2.6f, 0f);

            SpriteRenderer renderer = bossObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBossFlagshipSprite();
            renderer.color = Color.Lerp(new Color(1f, 0.22f, 0.58f), gameManager.StageAccentColor, 0.32f);
            renderer.sortingOrder = 13;
            bossObject.transform.localScale = new Vector3(1.72f, 1.42f, 1f);
            bossObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            BoxCollider2D collider = bossObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.88f, 0.9f);

            Rigidbody2D rigidbody2D = bossObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BossController boss = bossObject.AddComponent<BossController>();
            boss.Initialize(gameManager, this, effectsController, GetBossHitPoints(), BuildBossPhases(gameManager.Difficulty, gameManager.BossPatternStyle), gameManager.TopBound - 3.2f);
            bossActive = true;
            activeEnemyCount++;
        }

        public static BossPhaseConfig[] BuildBossPhases(GameDifficulty difficulty)
        {
            return BuildBossPhases(difficulty, BossPatternStyle.Standard);
        }

        public static BossPhaseConfig[] BuildBossPhases(GameDifficulty difficulty, BossPatternStyle style)
        {
            float intervalMultiplier = GetBossPatternIntervalMultiplier(style);
            int salvoBonus = GetBossPatternSalvoBonus(style);
            float spreadBonus = GetBossPatternSpreadBonus(style);
            bool forceAimed = style == BossPatternStyle.Needle || style == BossPatternStyle.Core;
            bool forceRing = style == BossPatternStyle.Orbit || style == BossPatternStyle.Core || style == BossPatternStyle.Barrage;

            switch (difficulty)
            {
                case GameDifficulty.High:
                    return new[]
                    {
                        BuildBossPhase(0.7f, 1.38f, 5, 46f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1f, false),
                        BuildBossPhase(0.4f, 1.12f, 6, 62f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.2f, false),
                        BuildBossPhase(0.1f, 0.9f, 7, 82f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.4f, true),
                        BuildBossPhase(0f, 0.74f, 9, 102f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.6f, true)
                    };
                case GameDifficulty.Medium:
                    return new[]
                    {
                        BuildBossPhase(0.7f, 1.58f, 4, 38f, false, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1f, false),
                        BuildBossPhase(0.4f, 1.32f, 5, 52f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.15f, false),
                        BuildBossPhase(0.1f, 1.08f, 6, 70f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.35f, true),
                        BuildBossPhase(0f, 0.9f, 7, 86f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.5f, true)
                    };
                default:
                    return new[]
                    {
                        BuildBossPhase(0.7f, 1.82f, 3, 28f, false, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1f, false),
                        BuildBossPhase(0.4f, 1.5f, 4, 40f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.1f, false),
                        BuildBossPhase(0.1f, 1.2f, 5, 54f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.25f, false),
                        BuildBossPhase(0f, 1f, 6, 66f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing, 1.4f, true)
                    };
            }
        }

        private static BossPhaseConfig BuildBossPhase(float triggerHealthNormalized, float fireInterval, int salvoCount, float spreadAngle, bool aimedCoreShot, bool extraRingShot, float intervalMultiplier, int salvoBonus, float spreadBonus, bool forceAimed, bool forceRing, float movementSpeedMultiplier = 1f, bool isEnraged = false)
        {
            return new BossPhaseConfig(
                triggerHealthNormalized,
                fireInterval * intervalMultiplier,
                Mathf.Max(1, salvoCount + salvoBonus),
                Mathf.Max(18f, spreadAngle + spreadBonus),
                aimedCoreShot || forceAimed,
                extraRingShot || forceRing,
                movementSpeedMultiplier,
                isEnraged);
        }

        private static float GetBossPatternIntervalMultiplier(BossPatternStyle style)
        {
            switch (style)
            {
                case BossPatternStyle.Crossfire:
                    return 1.04f;
                case BossPatternStyle.Barrage:
                    return 0.96f;
                case BossPatternStyle.Needle:
                    return 1.12f;
                case BossPatternStyle.Crusher:
                    return 1.18f;
                case BossPatternStyle.Drift:
                    return 0.92f;
                case BossPatternStyle.Orbit:
                    return 1.08f;
                case BossPatternStyle.Core:
                    return 0.98f;
                default:
                    return 1f;
            }
        }

        private static int GetBossPatternSalvoBonus(BossPatternStyle style)
        {
            switch (style)
            {
                case BossPatternStyle.Barrage:
                case BossPatternStyle.Orbit:
                    return 2;
                case BossPatternStyle.Crossfire:
                case BossPatternStyle.Drift:
                case BossPatternStyle.Core:
                    return 1;
                case BossPatternStyle.Crusher:
                    return -1;
                default:
                    return 0;
            }
        }

        private static float GetBossPatternSpreadBonus(BossPatternStyle style)
        {
            switch (style)
            {
                case BossPatternStyle.Crossfire:
                    return 18f;
                case BossPatternStyle.Barrage:
                    return 12f;
                case BossPatternStyle.Needle:
                    return -14f;
                case BossPatternStyle.Crusher:
                    return -20f;
                case BossPatternStyle.Drift:
                    return 24f;
                case BossPatternStyle.Orbit:
                    return 34f;
                case BossPatternStyle.Core:
                    return 28f;
                default:
                    return 0f;
            }
        }

        private static float GetBossPatternSpeedMultiplier(BossPatternStyle style)
        {
            switch (style)
            {
                case BossPatternStyle.Needle:
                    return 1.16f;
                case BossPatternStyle.Drift:
                    return 1.08f;
                case BossPatternStyle.Crusher:
                    return 0.92f;
                case BossPatternStyle.Core:
                    return 1.12f;
                default:
                    return 1f;
            }
        }

        private int GetBossHitPoints()
        {
            switch (gameManager.Difficulty)
            {
                case GameDifficulty.High:
                    return Mathf.RoundToInt(78 * gameManager.StageDifficultyMultiplier);
                case GameDifficulty.Medium:
                    return Mathf.RoundToInt(62 * gameManager.StageDifficultyMultiplier);
                default:
                    return Mathf.RoundToInt(48 * gameManager.StageDifficultyMultiplier);
            }
        }

        private int GetEliteHitPoints()
        {
            switch (gameManager.Difficulty)
            {
                case GameDifficulty.High:
                    return 10;
                case GameDifficulty.Medium:
                    return 8;
                default:
                    return 6;
            }
        }

        private Color GetAmmoPackColor(AmmoPowerupType type)
        {
            return PowerupCycle.GetCategoryColor(type);
        }

        private static string GetAmmoPackLabel(AmmoPowerupType type)
        {
            return PowerupCycle.GetPickupLabel(type);
        }

        private AmmoPowerupType GetRandomPowerupType()
        {
            if (gameplayProfile != null)
            {
                bool rewardPhase = gameManager.CurrentWavePhase == WavePhase.Reward;
                if (rewardPhase || Random.value <= gameplayProfile.RewardBias)
                {
                    return gameplayProfile.ChooseThemedPowerup(Random.Range(0, 1024), rewardPhase);
                }
            }

            AmmoPowerupType[] earlyPool =
            {
                AmmoPowerupType.Scatter,
                AmmoPowerupType.RapidFire,
                AmmoPowerupType.Pierce,
                AmmoPowerupType.Burst
            };

            AmmoPowerupType[] fullPool =
            {
                AmmoPowerupType.Scatter,
                AmmoPowerupType.RapidFire,
                AmmoPowerupType.Pierce,
                AmmoPowerupType.Laser,
                AmmoPowerupType.Burst,
                AmmoPowerupType.Homing,
                AmmoPowerupType.Wave,
                AmmoPowerupType.Plasma,
                AmmoPowerupType.Guard
            };

            AmmoPowerupType[] pool = gameManager.ElapsedTime < 32f && gameManager.CurrentStagePhase != StagePhase.Boss ? earlyPool : fullPool;
            return pool[Random.Range(0, pool.Length)];
        }
    }
}
