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
        public bool IsResolved => resolved;

        public void Initialize(GameManager manager, BlockSpawner spawner, EffectsController effects, int startingHitPoints, int awardedScore, float speed, Color color, Vector2 direction, float swayAmount, float swayRate, bool elite, AmmoPowerupType dropType)
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
            Vector2 drift = moveDirection * (fallSpeed * flightTime);
            Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);
            float sway = swayAmplitude > 0f ? Mathf.Sin(flightTime * swayFrequency) * swayAmplitude : 0f;
            transform.position = spawnPosition + new Vector3(drift.x, drift.y, 0f) + ((Vector3)(perpendicular * sway));
            if (transform.position.y < gameManager.BottomBound - 1.2f)
            {
                Escape();
                return;
            }

            UpdateFireTimer();
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
                Destroy(gameObject);
            }
        }

        public void ApplyHit(int damage)
        {
            if (resolved)
            {
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
                effectsController.PlayBurst(transform.position, effectColor);
                blockSpawner.NotifyEnemyResolved();
                Destroy(gameObject);
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
            effectsController.PlayBurst(transform.position, effectColor);
            blockSpawner.NotifyEnemyResolved();
            Destroy(gameObject);
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
            Destroy(gameObject);
        }

        private void BuildHealthLabel()
        {
            GameObject label = new GameObject("HitPointLabel");
            label.transform.SetParent(transform, false);
            label.transform.localPosition = new Vector3(0f, -0.08f, 0f);
            hitPointText = label.AddComponent<TextMesh>();
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
    }
}
