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
        private float age;
        private bool consumed;

        public void Initialize(GameManager manager, BlockSpawner spawner = null)
        {
            gameManager = manager;
            blockSpawner = spawner;
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
            float pulse = 1f + (Mathf.Sin(age * 5.8f) * 0.1f);
            transform.localScale = Vector3.one * (BaseVisualScale * pulse);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(age * 2.1f) * 7f);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(new Color(0.54f, 1f, 0.7f), Color.white, Mathf.PingPong(age * 1.9f, 1f));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed || other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            consumed = true;
            gameManager.HealPlayerFromPickup(transform.position, HealAmount);
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
