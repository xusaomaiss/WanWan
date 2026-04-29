using UnityEngine;

namespace Wanwan.Runtime
{
    public class EnemyFireballController : MonoBehaviour
    {
        private GameManager gameManager;
        private EffectsController effectsController;
        private BlockSpawner blockSpawner;
        private float speed;
        private float despawnY;
        private float leftDespawnX;
        private float rightDespawnX;
        private Vector2 direction = Vector2.down;
        private Color color;
        private bool resolved;

        public void Initialize(GameManager manager, EffectsController effects, BlockSpawner spawner, float travelSpeed, Vector2 travelDirection, float minY, float minX, float maxX, Color fireColor)
        {
            gameManager = manager;
            effectsController = effects;
            blockSpawner = spawner;
            speed = travelSpeed;
            direction = travelDirection.normalized;
            despawnY = minY;
            leftDespawnX = minX - 1.2f;
            rightDespawnX = maxX + 1.2f;
            color = fireColor;
            resolved = false;
        }

        private void Update()
        {
            if (resolved || gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
            if (transform.position.y < despawnY || transform.position.x < leftDespawnX || transform.position.x > rightDespawnX)
            {
                resolved = true;
                ReturnOrDestroy();
            }
        }

        public void ClearByBomb()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            ReturnOrDestroy();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (resolved || gameManager == null)
            {
                return;
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                resolved = true;
                if (effectsController != null)
                {
                    effectsController.PlayPlayerPierced(transform.position, color);
                }
                gameManager.DamagePlayerByPierce();
                ReturnOrDestroy();
            }
        }

        private void ReturnOrDestroy()
        {
            if (blockSpawner != null)
            {
                blockSpawner.ReturnFireballObject(gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}
