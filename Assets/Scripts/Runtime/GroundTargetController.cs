using UnityEngine;

namespace Wanwan.Runtime
{
    public class GroundTargetController : MonoBehaviour
    {
        private GameManager gameManager;
        private BlockSpawner blockSpawner;
        private EffectsController effectsController;
        private SpriteRenderer spriteRenderer;
        private GroundTargetProfile profile;
        private int hitPoints;
        private float scrollSpeed;
        private float fireTimer;
        private bool resolved;

        public bool IsResolved => resolved;

        public void Initialize(GameManager manager, BlockSpawner spawner, EffectsController effects, GroundTargetProfile targetProfile, float stageScrollSpeed)
        {
            gameManager = manager;
            blockSpawner = spawner;
            effectsController = effects;
            profile = targetProfile;
            hitPoints = targetProfile.HitPoints;
            scrollSpeed = Mathf.Max(0.2f, stageScrollSpeed);
            fireTimer = targetProfile.FireInterval * Random.Range(0.45f, 0.9f);
            resolved = false;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (resolved || gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            transform.position += Vector3.down * (scrollSpeed * Time.deltaTime);
            if (transform.position.y < gameManager.BottomBound - 1.4f)
            {
                ResolveWithoutPenalty();
                return;
            }

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                FireAtPlayer();
                fireTimer = profile.FireInterval * Random.Range(0.86f, 1.14f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (resolved)
            {
                return;
            }

            if (other.GetComponent<PlayerController>() != null && gameManager != null)
            {
                if (effectsController != null)
                {
                    effectsController.PlayPlayerPierced(transform.position, profile.AccentColor);
                }
                gameManager.DamagePlayerByCollision();
                ResolveWithoutPenalty();
            }
        }

        public void ApplyHit(int damage)
        {
            if (resolved || gameManager == null)
            {
                return;
            }

            hitPoints -= Mathf.Max(1, damage);
            if (effectsController != null)
            {
                effectsController.PlayHit(transform.position, profile.AccentColor);
            }
            if (hitPoints > 0)
            {
                transform.localScale *= 0.97f;
                return;
            }

            resolved = true;
            gameManager.RegisterEnemyKillScore(profile.ScoreValue, transform.position);
            gameManager.NotifyEnemyDestroyed();
            if (blockSpawner != null)
            {
                blockSpawner.SpawnCoinsAtPosition(transform.position);
                blockSpawner.SpawnEnemyAmmoPackDrop(AmmoPowerupType.None, transform.position);
                blockSpawner.NotifyEnemyResolved();
                blockSpawner.ReturnEnemyObject(gameObject);
            }
            if (effectsController != null)
            {
                effectsController.PlayGroundTargetDestroyed(transform.position, profile.AccentColor);
            }
            if (blockSpawner == null)
            {
                Destroy(gameObject);
            }
        }

        public void ClearByBomb()
        {
            if (resolved || gameManager == null)
            {
                return;
            }

            resolved = true;
            gameManager.RegisterEnemyKillScore(profile.ScoreValue, transform.position);
            gameManager.NotifyEnemyDestroyed();
            if (blockSpawner != null)
            {
                blockSpawner.SpawnCoinsAtPosition(transform.position);
                blockSpawner.NotifyEnemyResolved();
                blockSpawner.ReturnEnemyObject(gameObject);
            }
            if (effectsController != null)
            {
                effectsController.PlayGroundTargetDestroyed(transform.position, profile.AccentColor);
            }
            if (blockSpawner == null)
            {
                Destroy(gameObject);
            }
        }

        private void FireAtPlayer()
        {
            if (gameManager == null || blockSpawner == null)
            {
                return;
            }

            Vector2 direction = ((Vector2)(gameManager.PlayerPosition - transform.position)).normalized;
            if (direction == Vector2.zero)
            {
                direction = Vector2.down;
            }

            Color shotColor = Color.Lerp(profile.AccentColor, gameManager.StageAccentColor, 0.3f);
            blockSpawner.SpawnEnemyMissile(transform.position + ((Vector3)direction * 0.32f), direction, shotColor);
        }

        private void ResolveWithoutPenalty()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            if (blockSpawner != null)
            {
                blockSpawner.NotifyEnemyResolved();
                blockSpawner.ReturnEnemyObject(gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}
