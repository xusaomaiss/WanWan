using UnityEngine;

namespace Wanwan.Runtime
{
    public class BlockController : MonoBehaviour
    {
        private GameManager gameManager;
        private BlockSpawner blockSpawner;
        private EffectsController effectsController;
        private SpriteRenderer spriteRenderer;
        private TextMesh hitPointText;
        private Color effectColor;
        private int hitPoints;
        private float fallSpeed;
        private Vector2 moveDirection = Vector2.down;
        private Vector3 spawnPosition;
        private float swayAmplitude;
        private float swayFrequency;
        private float flightTime;
        private bool isTough;
        private bool isElite;
        private int scoreValue;
        private float fireTimer;
        private bool resolved;
        private AmmoPowerupType guaranteedDrop;
        private EnemyType enemyType;
        private bool shieldAbsorbedFirstHit;
        private float healTimer;
        private float barrageHoverEndTime;
        private const float HealerInterval = 2f;
        private const float BarrageHoverDuration = 3f;
        public bool IsResolved => resolved;

        public void Initialize(GameManager manager, BlockSpawner spawner, EffectsController effects, int startingHitPoints, int awardedScore, float speed, Color color, Vector2 direction, float swayAmount, float swayRate, bool elite, AmmoPowerupType dropType, EnemyType type = EnemyType.Normal)
        {
            gameManager = manager;
            blockSpawner = spawner;
            effectsController = effects;
            hitPoints = startingHitPoints;
            scoreValue = awardedScore;
            fallSpeed = speed;
            isTough = startingHitPoints > 1;
            isElite = elite;
            moveDirection = direction.normalized;
            swayAmplitude = swayAmount;
            swayFrequency = swayRate;
            guaranteedDrop = dropType;
            spawnPosition = transform.position;
            fireTimer = GetFireInterval() * Random.Range(0.55f, 1.1f);
            spriteRenderer = GetComponent<SpriteRenderer>();
            effectColor = color;
            spriteRenderer.color = elite ? color : Color.white;
            enemyType = type;
            shieldAbsorbedFirstHit = false;
            healTimer = HealerInterval;
            BuildHealthLabel();
            RefreshHealthLabel();
        }

        private void Update()
        {
            if (!gameManager.IsPlaying)
            {
                return;
            }

            flightTime += Time.deltaTime;

            if (enemyType == EnemyType.SelfDestruct)
            {
                UpdateSelfDestructBehavior();
            }

            if (enemyType == EnemyType.Healer)
            {
                UpdateHealerBehavior();
            }

            bool shouldMove = true;
            if (enemyType == EnemyType.Barrage)
            {
                if (barrageHoverEndTime <= 0f && transform.position.y < gameManager.BottomBound + 5f)
                {
                    barrageHoverEndTime = Time.time + BarrageHoverDuration;
                }
                if (barrageHoverEndTime > 0f && Time.time < barrageHoverEndTime)
                {
                    shouldMove = false;
                }
            }

            if (shouldMove)
            {
                Vector2 drift = moveDirection * (fallSpeed * flightTime);
                Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);
                float sway = swayAmplitude > 0f ? Mathf.Sin(flightTime * swayFrequency) * swayAmplitude : 0f;
                transform.position = spawnPosition + new Vector3(drift.x, drift.y, 0f) + ((Vector3)(perpendicular * sway));
            }

            if (transform.position.y < gameManager.BottomBound - 1.2f)
            {
                Escape();
                return;
            }

            UpdateFireTimer();
        }

        private void UpdateSelfDestructBehavior()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist < 4f)
                {
                    Vector2 towardPlayer = (player.transform.position - transform.position).normalized;
                    spawnPosition = transform.position - (Vector3)(towardPlayer * fallSpeed * flightTime);
                    moveDirection = towardPlayer;
                    fallSpeed *= 1.02f;
                }
            }
        }

        private void UpdateHealerBehavior()
        {
            healTimer -= Time.deltaTime;
            if (healTimer <= 0f)
            {
                healTimer = HealerInterval;
                BlockController[] allBlocks = FindObjectsByType<BlockController>(FindObjectsSortMode.None);
                int healed = 0;
                for (int i = 0; i < allBlocks.Length; i++)
                {
                    if (allBlocks[i] == this || allBlocks[i].resolved)
                        continue;
                    if (Vector2.Distance(transform.position, allBlocks[i].transform.position) < 3.5f)
                    {
                        allBlocks[i].Heal(1);
                        healed++;
                        if (healed >= 3)
                            break;
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (resolved)
            {
                return;
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                resolved = true;
                effectsController.PlayPlayerPierced(transform.position, effectColor);
                gameManager.DamagePlayerByCollision();
                blockSpawner.NotifyEnemyResolved();
                if (enemyType == EnemyType.SelfDestruct)
                {
                    effectsController.PlayEliteBurst(transform.position, Color.red);
                }
                blockSpawner.ReturnEnemyObject(gameObject);
            }
        }

        public void ApplyHit(int damage)
        {
            if (resolved)
            {
                return;
            }

            if (enemyType == EnemyType.Shield && !shieldAbsorbedFirstHit)
            {
                shieldAbsorbedFirstHit = true;
                spriteRenderer.color = Color.white;
                Invoke(nameof(RestoreEnemyColor), 0.18f);
                effectsController.PlayHit(transform.position, effectColor);
                return;
            }

            hitPoints -= damage;
            effectsController.PlayHit(transform.position, effectColor);

            if (hitPoints <= 0)
            {
                resolved = true;
                gameManager.RegisterEnemyKillScore(scoreValue, transform.position);
                gameManager.NotifyEnemyDestroyed();
                blockSpawner.SpawnCoinsAtPosition(transform.position);
                blockSpawner.SpawnEnemyAmmoPackDrop(guaranteedDrop, transform.position);
                if (isElite)
                {
                    effectsController.PlayEliteBurst(transform.position, effectColor);
                }
                else if (isTough)
                {
                    effectsController.PlayBurst(transform.position, effectColor);
                }
                else
                {
                    effectsController.PlaySmallBurst(transform.position, effectColor);
                }
                blockSpawner.NotifyEnemyResolved();
                blockSpawner.ReturnEnemyObject(gameObject);
                return;
            }

            RefreshHealthLabel();
            transform.localScale *= 0.96f;
        }

        public void ClearByBomb()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            gameManager.RegisterEnemyKillScore(scoreValue, transform.position);
            gameManager.NotifyEnemyDestroyed();
            blockSpawner.SpawnCoinsAtPosition(transform.position);
            if (isElite)
            {
                effectsController.PlayEliteBurst(transform.position, effectColor);
            }
            else if (isTough)
            {
                effectsController.PlayBurst(transform.position, effectColor);
            }
            else
            {
                effectsController.PlaySmallBurst(transform.position, effectColor);
            }
            blockSpawner.NotifyEnemyResolved();
            blockSpawner.ReturnEnemyObject(gameObject);
        }

        public void ReachBase()
        {
            Escape();
        }

        private void Escape()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            gameManager.NotifyEnemyEscaped(transform.position);
            blockSpawner.NotifyEnemyResolved();
            blockSpawner.ReturnEnemyObject(gameObject);
        }

        public void Heal(int amount)
        {
            if (resolved)
                return;
            hitPoints += amount;
            RefreshHealthLabel();
        }

        private void RestoreEnemyColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isElite ? effectColor : Color.white;
            }
        }

        private void BuildHealthLabel()
        {
            GameObject label = new GameObject("HitPointLabel");
            label.transform.SetParent(transform, false);
            label.transform.localPosition = new Vector3(0f, -0.08f, 0f);
            hitPointText = label.AddComponent<TextMesh>();
            ArcadeFontProvider.ApplyTo(hitPointText);
            hitPointText.anchor = TextAnchor.MiddleCenter;
            hitPointText.alignment = TextAlignment.Center;
            hitPointText.characterSize = 0.11f;
            hitPointText.fontSize = 48;
            hitPointText.color = new Color(0.35f, 0.18f, 0.28f);
        }

        private void RefreshHealthLabel()
        {
            if (hitPointText != null)
            {
                hitPointText.text = hitPoints > 1 ? hitPoints.ToString() : string.Empty;
            }
        }

        private void UpdateFireTimer()
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer > 0f || transform.position.y < gameManager.BottomBound + 4.2f)
            {
                return;
            }

            FireEnemyShot();
            fireTimer = GetFireInterval() * Random.Range(0.88f, 1.16f);
        }

        private void FireEnemyShot()
        {
            if (enemyType == EnemyType.Barrage)
            {
                FireBarrageSpread();
                return;
            }

            if (isElite)
            {
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(-0.16f, -1f), new Color(1f, 0.52f, 0.28f));
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.48f), new Vector2(0.16f, -1f), new Color(1f, 0.52f, 0.28f));
                return;
            }

            if (gameManager.EnemyUsesScatterShot)
            {
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(-0.22f, -1f), effectColor);
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.48f), Vector2.down, effectColor);
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(0.22f, -1f), effectColor);
                return;
            }

            if (gameManager.ElapsedTime > 18f && Random.value < 0.35f)
            {
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(-0.28f, -1f), new Color(1f, 0.42f, 0.34f));
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.46f), Vector2.down, new Color(1f, 0.58f, 0.3f));
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(0.28f, -1f), new Color(1f, 0.42f, 0.34f));
                return;
            }

            blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.45f), Vector2.down, effectColor);
        }

        private float GetFireInterval()
        {
            float interval = DifficultyProgression.GetEnemyFireInterval(gameManager.ElapsedTime, isTough);
            if (isElite)
            {
                interval *= 0.68f;
            }

            return interval / Mathf.Lerp(1f, gameManager.StageDifficultyMultiplier, 0.28f);
        }

        private void FireBarrageSpread()
        {
            float[] angles = { 0f, -0.22f, 0.22f, -0.44f, 0.44f };
            for (int i = 0; i < angles.Length; i++)
            {
                Vector2 dir = new Vector2(angles[i], -1f).normalized;
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.45f), dir, effectColor);
            }
        }
    }
}
