using UnityEngine;

namespace Wanwan.Runtime
{
    public class BossController : MonoBehaviour
    {
        private GameManager gameManager;
        private BlockSpawner blockSpawner;
        private EffectsController effectsController;
        private SpriteRenderer spriteRenderer;
        private BossPhaseConfig[] phases;
        private int hitPoints;
        private int maxHitPoints;
        private float anchorY;
        private float hoverAmplitude;
        private float hoverSpeed;
        private float fireTimer;
        private float elapsed;
        private bool entering = true;
        private bool resolved;

        public void Initialize(GameManager manager, BlockSpawner spawner, EffectsController effects, int startingHitPoints, BossPhaseConfig[] phaseConfigs, float hoverY)
        {
            gameManager = manager;
            blockSpawner = spawner;
            effectsController = effects;
            phases = phaseConfigs;
            hitPoints = startingHitPoints;
            maxHitPoints = startingHitPoints;
            anchorY = hoverY;
            hoverAmplitude = manager.Difficulty == GameDifficulty.High ? 2f : 1.45f;
            hoverSpeed = manager.Difficulty == GameDifficulty.High ? 1.35f : 1.05f;
            spriteRenderer = GetComponent<SpriteRenderer>();
            gameManager.NotifyBossSpawn(manager.BossDisplayName, hitPoints, maxHitPoints);
            effectsController.PlayBossArrival(transform.position, spriteRenderer.color);
        }

        private void Update()
        {
            if (resolved || !gameManager.IsPlaying)
            {
                return;
            }

            elapsed += Time.deltaTime;

            if (entering)
            {
                Vector3 position = transform.position;
                position.y = Mathf.MoveTowards(position.y, anchorY, Time.deltaTime * 3.8f);
                transform.position = position;
                entering = Mathf.Abs(position.y - anchorY) > 0.02f;
                return;
            }

            Vector3 hoverPosition = transform.position;
            hoverPosition.x = Mathf.Sin(elapsed * hoverSpeed) * hoverAmplitude;
            hoverPosition.y = anchorY + (Mathf.Sin(elapsed * 0.55f) * 0.18f);
            transform.position = hoverPosition;

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                FirePattern();
                fireTimer = GetActivePhase().FireInterval;
            }
        }

        public void ApplyHit(int damage)
        {
            if (resolved)
            {
                return;
            }

            hitPoints -= damage;
            effectsController.PlayHit(transform.position, spriteRenderer.color);
            gameManager.UpdateBossHealth(hitPoints, maxHitPoints);

            if (hitPoints <= 0)
            {
                ResolveDefeat();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (resolved || !gameManager.EnemyCollisionEndsRun)
            {
                return;
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                resolved = true;
                effectsController.PlayPlayerPierced(transform.position, spriteRenderer.color);
                gameManager.DestroyPlayerByCollision();
                blockSpawner.NotifyBossResolved();
                Destroy(gameObject);
            }
        }

        private BossPhaseConfig GetActivePhase()
        {
            float normalized = hitPoints / (float)maxHitPoints;
            for (int i = 0; i < phases.Length; i++)
            {
                if (normalized >= phases[i].TriggerHealthNormalized)
                {
                    return phases[i];
                }
            }

            return phases[phases.Length - 1];
        }

        private void FirePattern()
        {
            BossPhaseConfig active = GetActivePhase();
            int salvoCount = Mathf.Max(1, active.SalvoCount);
            float step = salvoCount == 1 ? 0f : active.SpreadAngle / (salvoCount - 1);
            float startAngle = -active.SpreadAngle * 0.5f;

            for (int i = 0; i < salvoCount; i++)
            {
                float angle = startAngle + (step * i);
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.8f), direction, spriteRenderer.color, true);
            }

            if (active.AimedCoreShot)
            {
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.9f), Vector2.down, new Color(1f, 0.9f, 0.52f), true);
            }

            if (active.ExtraRingShot)
            {
                FireRingBurst(active);
            }
        }

        private void FireRingBurst(BossPhaseConfig active)
        {
            int count = Mathf.Max(6, active.SalvoCount);
            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.Lerp(-120f, 120f, count == 1 ? 0.5f : i / (float)(count - 1));
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
                Color ringColor = i % 2 == 0 ? new Color(0.35f, 0.95f, 1f) : new Color(1f, 0.82f, 0.28f);
                blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.68f), direction, ringColor, true);
            }
        }

        private void ResolveDefeat()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            effectsController.PlayBossDefeat(transform.position, spriteRenderer.color);
            for (int i = 0; i < 3; i++)
            {
                Vector3 burstPosition = transform.position + (Vector3)(Random.insideUnitCircle * 0.7f);
                effectsController.PlayBurst(burstPosition, spriteRenderer.color);
            }

            gameManager.AddScore(900);
            effectsController.PlayScorePopup(transform.position, 900);
            gameManager.MarkStageClear();
            blockSpawner.NotifyBossResolved();
            Destroy(gameObject);
        }
    }
}
