using UnityEngine;

namespace Wanwan.Runtime
{
    public class HealthPickupController : MonoBehaviour
    {
        public const int HealAmount = 2;
        private const float BaseVisualScale = 0.78f;

        private GameManager gameManager;
        private BlockSpawner blockSpawner;
        private SpriteRenderer spriteRenderer;
        private float fallSpeed;
        private float bottomDespawnY;
        private float age;
        private bool consumed;

        public void Initialize(GameManager manager, BlockSpawner spawner = null, float speed = 1.8f, float despawnY = -10f)
        {
            gameManager = manager;
            blockSpawner = spawner;
            fallSpeed = Mathf.Max(0.8f, speed);
            bottomDespawnY = despawnY;
            consumed = false;
            age = 0f;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (consumed || gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            age += Time.deltaTime;
            Vector3 playerPosition = gameManager.PlayerPosition;
            if (PickupMagnet.TryMoveTowardPlayer(transform.position, playerPosition, Time.deltaTime, out Vector3 magnetPosition))
            {
                transform.position = magnetPosition;
                if (PickupMagnet.IsInCollectRange(transform.position, playerPosition))
                {
                    Collect();
                    return;
                }
            }
            else
            {
                transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
            }

            float pulse = 1f + (Mathf.Sin(age * 5.8f) * 0.1f);
            transform.localScale = Vector3.one * (BaseVisualScale * pulse);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(age * 2.1f) * 7f);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(new Color(0.54f, 1f, 0.7f), Color.white, Mathf.PingPong(age * 1.9f, 1f));
            }

            if (transform.position.y < bottomDespawnY)
            {
                consumed = true;
                ReturnOrDestroy();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed || other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            Collect();
        }

        private void Collect()
        {
            if (consumed)
            {
                return;
            }

            consumed = true;
            gameManager.HealPlayerFromPickup(transform.position, HealAmount);
            ReturnOrDestroy();
        }

        private void ReturnOrDestroy()
        {
            if (blockSpawner != null)
            {
                blockSpawner.ReturnPickupObject(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
