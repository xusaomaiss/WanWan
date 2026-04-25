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
            renderer.color = color;
            renderer.sortingOrder = 14;
            fireballObject.transform.localScale = fromBoss ? new Vector3(0.208f, 0.42f, 1f) : new Vector3(0.176f, 0.368f, 1f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            fireballObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            BoxCollider2D collider = fireballObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = fromBoss ? new Vector2(0.248f, 0.68f) : new Vector2(0.224f, 0.624f);

            Rigidbody2D rigidbody2D = fireballObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            float speed = DifficultyProgression.GetEnemyFireballSpeed(gameManager.ElapsedTime, fromBoss || gameManager.Difficulty == GameDifficulty.High);
            if (fromBoss)
            {
                speed *= gameManager.Difficulty == GameDifficulty.High ? 1.22f : 1.1f;
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
                        new EnemySpawnInstruction(7.6f, EnemyFormationType.SideCutInRight, 4)
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
                        new EnemySpawnInstruction(7.9f, EnemyFormationType.SideCutInLeft, 5),
                        new EnemySpawnInstruction(9.2f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Laser)
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
                        new EnemySpawnInstruction(4.8f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Homing)
                    }),
                new StageWaveConfig(StagePhase.Boss, "危险警报", 999f, true, new EnemySpawnInstruction[0])
            };
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
            float spacing = 1.45f;
            float startX = -((instruction.Count - 1) * spacing * 0.5f);
            for (int i = 0; i < instruction.Count; i++)
            {
                Vector3 position = new Vector3(startX + (i * spacing), spawnY + (i * 0.12f), 0f);
                SpawnEnemy(position, instruction.Elite, Vector2.down, 0f, 0f, i == instruction.Count / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
            }
        }

        private void SpawnSideCutIn(EnemySpawnInstruction instruction, bool fromLeft)
        {
            float x = fromLeft ? leftBound - 0.95f : rightBound + 0.95f;
            Vector2 direction = (fromLeft ? new Vector2(0.62f, -1f) : new Vector2(-0.62f, -1f)).normalized;

            for (int i = 0; i < instruction.Count; i++)
            {
                Vector3 position = new Vector3(x, spawnY - (i * 0.5f), 0f);
                SpawnEnemy(position, instruction.Elite, direction, 0f, 0f, i == instruction.Count / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
            }
        }

        private void SpawnVShape(EnemySpawnInstruction instruction)
        {
            for (int i = 0; i < instruction.Count; i++)
            {
                float offsetIndex = i - ((instruction.Count - 1) * 0.5f);
                float x = offsetIndex * 1.15f;
                float y = spawnY + (Mathf.Abs(offsetIndex) * 0.28f);
                SpawnEnemy(new Vector3(x, y, 0f), instruction.Elite, Vector2.down, 0f, 0f, i == instruction.Count / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
            }
        }

        private void SpawnSnakeSweep(EnemySpawnInstruction instruction)
        {
            float spacing = 1.25f;
            float startX = -((instruction.Count - 1) * spacing * 0.5f);
            for (int i = 0; i < instruction.Count; i++)
            {
                Vector3 position = new Vector3(startX + (i * spacing), spawnY + (i * 0.08f), 0f);
                float swayAmplitude = 0.8f + (i * 0.05f);
                float swayFrequency = 2.4f + (i * 0.12f);
                SpawnEnemy(position, instruction.Elite, Vector2.down, swayAmplitude, swayFrequency, i == instruction.Count / 2 ? instruction.GuaranteedDrop : AmmoPowerupType.None);
            }
        }

        private void SpawnEnemy(Vector3 position, bool elite, Vector2 moveDirection, float swayAmplitude, float swayFrequency, AmmoPowerupType guaranteedDrop)
        {
            bool tough = elite || Random.value <= DifficultyProgression.GetToughChance(gameManager.ElapsedTime);
            int hitPoints = elite ? GetEliteHitPoints() : DifficultyProgression.GetHitPoints(tough, gameManager.ElapsedTime);
            int scoreValue = elite ? 260 : DifficultyProgression.GetScoreValue(tough, hitPoints);
            float speed = elite ? DifficultyProgression.GetBlockSpeed(gameManager.ElapsedTime, true) * 0.82f : DifficultyProgression.GetBlockSpeed(gameManager.ElapsedTime, tough);

            GameObject enemyObject = new GameObject(elite ? "ElitePlane" : (tough ? "ToughPlane" : "Plane"));
            enemyObject.transform.position = position;

            SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
            renderer.sprite = elite ? RuntimeSpriteFactory.GetEliteInterceptorSprite() : RuntimeSpriteFactory.GetEnemyInterceptorSprite();
            renderer.color = elite ? new Color(1f, 0.88f, 0.24f) : (tough ? new Color(1f, 0.38f, 0.52f) : new Color(0.88f, 0.26f, 0.46f));
            renderer.sortingOrder = elite ? 12 : 10;

            float width = elite ? 0.92f : (tough ? 0.82f : 0.72f);
            float height = elite ? 0.9f : (tough ? 0.8f : 0.7f);
            enemyObject.transform.localScale = new Vector3(width, height, 1f);
            enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f);

            BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = elite ? new Vector2(0.62f, 0.72f) : new Vector2(0.56f, 0.66f);

            Rigidbody2D rigidbody2D = enemyObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BlockController block = enemyObject.AddComponent<BlockController>();
            block.Initialize(gameManager, this, effectsController, hitPoints, scoreValue, speed, renderer.color, moveDirection, swayAmplitude, swayFrequency, elite, guaranteedDrop);
            activeEnemyCount++;
        }

        private void SpawnBoss()
        {
            GameObject bossObject = new GameObject("BossFlagship");
            bossObject.transform.position = new Vector3(0f, spawnY + 2.6f, 0f);

            SpriteRenderer renderer = bossObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBossFlagshipSprite();
            renderer.color = new Color(1f, 0.22f, 0.58f);
            renderer.sortingOrder = 13;
            bossObject.transform.localScale = new Vector3(1.72f, 1.42f, 1f);
            bossObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            BoxCollider2D collider = bossObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.08f, 1.12f);

            Rigidbody2D rigidbody2D = bossObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BossController boss = bossObject.AddComponent<BossController>();
            boss.Initialize(gameManager, this, effectsController, GetBossHitPoints(), BuildBossPhases(), gameManager.TopBound - 3.2f);
            bossActive = true;
            activeEnemyCount++;
        }

        private BossPhaseConfig[] BuildBossPhases()
        {
            switch (gameManager.Difficulty)
            {
                case GameDifficulty.High:
                    return new[]
                    {
                        new BossPhaseConfig(0.52f, 1.25f, 5, 50f, true),
                        new BossPhaseConfig(0f, 0.88f, 7, 74f, true)
                    };
                case GameDifficulty.Medium:
                    return new[]
                    {
                        new BossPhaseConfig(0.5f, 1.45f, 5, 42f, true),
                        new BossPhaseConfig(0f, 1.08f, 6, 60f, true)
                    };
                default:
                    return new[]
                    {
                        new BossPhaseConfig(0.5f, 1.68f, 4, 34f, false),
                        new BossPhaseConfig(0f, 1.22f, 5, 48f, true)
                    };
            }
        }

        private int GetBossHitPoints()
        {
            switch (gameManager.Difficulty)
            {
                case GameDifficulty.High:
                    return 78;
                case GameDifficulty.Medium:
                    return 62;
                default:
                    return 48;
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
            return RuntimeSpriteFactory.GetWeaponColor(type);
        }

        private static string GetAmmoPackLabel(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    return "S";
                case AmmoPowerupType.RapidFire:
                    return "R";
                case AmmoPowerupType.Pierce:
                    return "P";
                case AmmoPowerupType.Laser:
                    return "L";
                case AmmoPowerupType.Plasma:
                    return "O";
                case AmmoPowerupType.Burst:
                    return "B";
                case AmmoPowerupType.Homing:
                    return "H";
                case AmmoPowerupType.Wave:
                    return "W";
                case AmmoPowerupType.Guard:
                    return "G";
                default:
                    return "N";
            }
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
