using UnityEngine;

namespace Wanwan.Runtime
{
    public class BlockSpawner : MonoBehaviour
    {
        private GameManager gameManager;
        private EffectsController effectsController;
        private float leftBound;
        private float rightBound;
        private float spawnY;
        private bool spawningEnabled = true;
        private bool bossActive;
        private int activeEnemyCount;
        private float phaseTimer;
        private int currentWaveIndex = -1;
        private int nextInstructionIndex;
        private StageWaveConfig[] stageScript;
        private StageWaveConfig currentWave;

        public void Initialize(GameManager manager, EffectsController effects, Camera camera, float minX, float maxX, float topY)
        {
            gameManager = manager;
            effectsController = effects;
            leftBound = minX;
            rightBound = maxX;
            spawnY = topY;
            stageScript = BuildStageScript();
            AdvanceToNextWave();
        }

        private void Update()
        {
            if (!spawningEnabled || !gameManager.IsPlaying || currentWave == null)
            {
                return;
            }

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

        public void SpawnEnemyMissile(Vector3 origin, Vector2 direction, Color color, bool fromBoss = false)
        {
            GameObject fireballObject = new GameObject(fromBoss ? "BossMissile" : "EnemyMissile");
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
            fireball.Initialize(gameManager, effectsController, speed, direction, gameManager.BottomBound - 1.2f, gameManager.LeftBound, gameManager.RightBound, renderer.color);
        }

        public void SpawnAmmoPackAtPosition(AmmoPowerupType type, Vector3 position)
        {
            if (type == AmmoPowerupType.None)
            {
                return;
            }

            GameObject packObject = new GameObject(type + "Pack");
            packObject.transform.position = position;
            packObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            SpriteRenderer renderer = packObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetAmmoPackSprite(type);
            renderer.color = GetAmmoPackColor(type);
            renderer.sortingOrder = 11;

            CircleCollider2D collider = packObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            Rigidbody2D rigidbody2D = packObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            AmmoPackController packController = packObject.AddComponent<AmmoPackController>();
            packController.Initialize(gameManager, effectsController, type, DifficultyProgression.GetAmmoPackSpeed(gameManager.ElapsedTime), gameManager.BottomBound - 1.25f, renderer.color, GetAmmoPackLabel(type));
        }

        public void SpawnCoinsAtPosition(Vector3 position)
        {
            int count = gameManager.RewardConfig.CoinsPerEnemy;
            for (int i = 0; i < count; i++)
            {
                GameObject coinObject = new GameObject("Coin");
                coinObject.transform.position = position + new Vector3(Random.Range(-0.18f, 0.18f), Random.Range(-0.14f, 0.2f), 0f);
                coinObject.transform.localScale = Vector3.one * 0.18f;

                SpriteRenderer renderer = coinObject.AddComponent<SpriteRenderer>();
                renderer.sprite = RuntimeSpriteFactory.GetCoinSprite();
                renderer.color = Color.white;
                renderer.sortingOrder = 18;

                CoinController coin = coinObject.AddComponent<CoinController>();
                float angle = ((Mathf.PI * 2f) / Mathf.Max(1, count)) * i;
                Vector2 drift = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(0.55f, 1.05f);
                drift.y = Mathf.Abs(drift.y) * 0.65f;
                coin.Initialize(gameManager, drift);
            }
        }

        public void SpawnBombPickupAtRandomReachablePosition()
        {
            Vector3 position = new Vector3(
                Random.Range(leftBound + 0.7f, rightBound - 0.7f),
                Random.Range(gameManager.BottomBound + 2.1f, gameManager.TopBound - 2.6f),
                0f);
            SpawnBombPickupAtPosition(position);
        }

        public void SpawnBombPickupAtPosition(Vector3 position)
        {
            GameObject bombObject = new GameObject("BombPickup");
            bombObject.transform.position = position;
            bombObject.transform.localScale = Vector3.one * 0.56f;

            SpriteRenderer renderer = bombObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBombPickupSprite();
            renderer.sortingOrder = 19;

            CircleCollider2D collider = bombObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.52f;

            Rigidbody2D rigidbody2D = bombObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BombPickupController pickup = bombObject.AddComponent<BombPickupController>();
            pickup.Initialize(gameManager);
            effectsController.PlayPowerupSpawn(position, new Color(0.45f, 0.86f, 1f));
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
            if (style == StageCombatStyle.Flanking)
            {
                return BuildFlankingStageScript();
            }

            if (style == StageCombatStyle.Swarm)
            {
                return BuildSwarmStageScript();
            }

            if (style == StageCombatStyle.Sniper)
            {
                return BuildSniperStageScript();
            }

            if (style == StageCombatStyle.Heavy)
            {
                return BuildHeavyStageScript();
            }

            if (style == StageCombatStyle.Agile)
            {
                return BuildAgileStageScript();
            }

            if (style == StageCombatStyle.Spiral)
            {
                return BuildSpiralStageScript();
            }

            if (style == StageCombatStyle.Finale)
            {
                return BuildFinaleStageScript();
            }

            return BuildBalancedStageScript();
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
                        new EnemySpawnInstruction(2.8f, EnemyFormationType.SideCutInRight, 2),
                        new EnemySpawnInstruction(4.2f, EnemyFormationType.SnakeSweep, 2, true, AmmoPowerupType.Plasma),
                        new EnemySpawnInstruction(5.6f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Homing)
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
                    new EnemySpawnInstruction(6.6f, EnemyFormationType.SnakeSweep, 5),
                    new EnemySpawnInstruction(8.8f, EnemyFormationType.DiveLine, 3, true, AmmoPowerupType.Laser),
                    new EnemySpawnInstruction(10.1f, EnemyFormationType.VShape, 4)
                }),
                new StageWaveConfig(StagePhase.Elite, "重型护卫", 8f, true, new[]
                {
                    new EnemySpawnInstruction(0.5f, EnemyFormationType.VShape, 2, true),
                    new EnemySpawnInstruction(2.6f, EnemyFormationType.DiveLine, 2, true, AmmoPowerupType.Pierce),
                    new EnemySpawnInstruction(4.8f, EnemyFormationType.SnakeSweep, 3, true, AmmoPowerupType.Homing)
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
                    new EnemySpawnInstruction(6.8f, EnemyFormationType.DiveLine, 5, true, AmmoPowerupType.Homing),
                    new EnemySpawnInstruction(8.8f, EnemyFormationType.SnakeSweep, 6),
                    new EnemySpawnInstruction(10.2f, EnemyFormationType.VShape, 4, true, AmmoPowerupType.Guard)
                }),
                new StageWaveConfig(StagePhase.Elite, "核心护卫队", 8.2f, true, new[]
                {
                    new EnemySpawnInstruction(0.4f, EnemyFormationType.VShape, 3, true),
                    new EnemySpawnInstruction(2.2f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Wave),
                    new EnemySpawnInstruction(4.4f, EnemyFormationType.SideCutInLeft, 3, true, AmmoPowerupType.Pierce),
                    new EnemySpawnInstruction(5.8f, EnemyFormationType.SideCutInRight, 3, true, AmmoPowerupType.Scatter)
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
            switch (instruction.Formation)
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
        }

        private void SpawnDiveLine(EnemySpawnInstruction instruction)
        {
            int spawnCount = GetSpawnCount(instruction);
            float spacing = 1.45f;
            float startX = -((spawnCount - 1) * spacing * 0.5f);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 position = new Vector3(startX + (i * spacing), spawnY + (i * 0.12f), 0f);
                SpawnEnemy(position, instruction.Elite, Vector2.down, 0f, 0f, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
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
                SpawnEnemy(position, instruction.Elite, direction, 0f, 0f, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
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
                SpawnEnemy(new Vector3(x, y, 0f), instruction.Elite, Vector2.down, 0f, 0f, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
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
                SpawnEnemy(position, instruction.Elite, Vector2.down, swayAmplitude, swayFrequency, i == spawnCount / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
            }
        }

        private static int GetSpawnCount(EnemySpawnInstruction instruction)
        {
            return EnemySpawnBudget.GetAdjustedCount(instruction.Count);
        }

        private void SpawnEnemy(Vector3 position, bool elite, Vector2 moveDirection, float swayAmplitude, float swayFrequency, AmmoPowerupType guaranteedDrop)
        {
            bool tough = elite || Random.value <= DifficultyProgression.GetToughChance(gameManager.ElapsedTime);
            int hitPoints = elite ? GetEliteHitPoints() : DifficultyProgression.GetHitPoints(tough, gameManager.ElapsedTime);
            hitPoints += Mathf.FloorToInt((gameManager.StageDifficultyMultiplier - 1f) * (elite ? 3f : 1.25f));
            int scoreValue = elite ? 260 : DifficultyProgression.GetScoreValue(tough, hitPoints);
            float speed = elite ? DifficultyProgression.GetBlockSpeed(gameManager.ElapsedTime, true) * 0.82f : DifficultyProgression.GetBlockSpeed(gameManager.ElapsedTime, tough);
            speed *= Mathf.Lerp(1f, gameManager.StageDifficultyMultiplier, 0.32f);

            GameObject enemyObject = new GameObject(elite ? "ElitePlane" : (tough ? "ToughPlane" : "Plane"));
            enemyObject.transform.position = position;

            SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
            renderer.sprite = elite ? RuntimeSpriteFactory.GetEliteInterceptorSprite() : RuntimeSpriteFactory.GetEnemyInterceptorSprite();
            Color accent = gameManager.StageAccentColor;
            Color normalColor = Color.Lerp(new Color(0.88f, 0.26f, 0.46f), accent, 0.24f);
            Color toughColor = Color.Lerp(new Color(1f, 0.38f, 0.52f), accent, 0.18f);
            Color eliteColor = Color.Lerp(new Color(1f, 0.88f, 0.24f), accent, 0.22f);
            renderer.color = elite ? eliteColor : Color.white;
            renderer.sortingOrder = elite ? 12 : 10;

            float width = elite ? 0.92f : (tough ? 0.82f : 0.72f);
            float height = elite ? 0.9f : (tough ? 0.8f : 0.7f);
            enemyObject.transform.localScale = new Vector3(width, height, 1f);
            enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f);

            BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = elite ? new Vector2(0.5f, 0.58f) : new Vector2(0.44f, 0.52f);

            Rigidbody2D rigidbody2D = enemyObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BlockController block = enemyObject.AddComponent<BlockController>();
            Color effectColor = elite ? eliteColor : (tough ? toughColor : normalColor);
            block.Initialize(gameManager, this, effectsController, hitPoints, scoreValue, speed, effectColor, moveDirection, swayAmplitude, swayFrequency, elite, guaranteedDrop);
            activeEnemyCount++;
        }

        private void SpawnBoss()
        {
            GameObject bossObject = new GameObject("BossFlagship");
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
                        BuildBossPhase(0.7f, 1.38f, 5, 46f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0.4f, 1.12f, 6, 62f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0.1f, 0.9f, 7, 82f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0f, 0.74f, 9, 102f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing)
                    };
                case GameDifficulty.Medium:
                    return new[]
                    {
                        BuildBossPhase(0.7f, 1.58f, 4, 38f, false, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0.4f, 1.32f, 5, 52f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0.1f, 1.08f, 6, 70f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0f, 0.9f, 7, 86f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing)
                    };
                default:
                    return new[]
                    {
                        BuildBossPhase(0.7f, 1.82f, 3, 28f, false, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0.4f, 1.5f, 4, 40f, true, false, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0.1f, 1.2f, 5, 54f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing),
                        BuildBossPhase(0f, 1f, 6, 66f, true, true, intervalMultiplier, salvoBonus, spreadBonus, forceAimed, forceRing)
                    };
            }
        }

        private static BossPhaseConfig BuildBossPhase(float triggerHealthNormalized, float fireInterval, int salvoCount, float spreadAngle, bool aimedCoreShot, bool extraRingShot, float intervalMultiplier, int salvoBonus, float spreadBonus, bool forceAimed, bool forceRing)
        {
            return new BossPhaseConfig(
                triggerHealthNormalized,
                fireInterval * intervalMultiplier,
                Mathf.Max(1, salvoCount + salvoBonus),
                Mathf.Max(18f, spreadAngle + spreadBonus),
                aimedCoreShot || forceAimed,
                extraRingShot || forceRing);
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
            return PowerupCycle.GetLabel(type);
        }

        private static AmmoPowerupType GetRandomPowerupType()
        {
            AmmoPowerupType[] pool =
            {
                AmmoPowerupType.Scatter,
                AmmoPowerupType.RapidFire,
                AmmoPowerupType.Pierce,
                AmmoPowerupType.Laser,
                AmmoPowerupType.Plasma,
                AmmoPowerupType.Burst,
                AmmoPowerupType.Homing,
                AmmoPowerupType.Wave,
                AmmoPowerupType.Guard
            };

            return pool[Random.Range(0, pool.Length)];
        }
    }
}
