using UnityEngine;

namespace Wanwan.Runtime
{
    public class PowerCapsuleController : MonoBehaviour
    {
        private GameManager gameManager;
        private EffectsController effectsController;
        private SpriteRenderer spriteRenderer;
        private float fallSpeed;
        private float bottomDespawnY;
        private float age;
        private bool consumed;
        private Vector3 baseScale;

        public void Initialize(GameManager manager, EffectsController effects, float speed, float despawnY)
        {
            gameManager = manager;
            effectsController = effects;
            fallSpeed = Mathf.Max(0.8f, speed);
            bottomDespawnY = despawnY;
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseScale = transform.localScale;
            if (effectsController != null)
            {
                effectsController.PlayPowerupSpawn(transform.position, new Color(0.42f, 0.9f, 1f));
            }
        }

        private void Update()
        {
            if (consumed || gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            age += Time.deltaTime;
            transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
            float pulse = 1f + (Mathf.Sin(age * 7.5f) * 0.1f);
            transform.localScale = baseScale * pulse;
            transform.rotation = Quaternion.Euler(0f, 0f, age * 90f);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(new Color(0.25f, 0.72f, 1f), Color.white, Mathf.PingPong(age * 2.2f, 1f));
            }

            if (transform.position.y < bottomDespawnY)
            {
                consumed = true;
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed || other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            consumed = true;
            gameManager.CollectPowerCapsule();
            if (effectsController != null)
            {
                effectsController.PlayPowerupPickup(transform.position, new Color(0.42f, 0.9f, 1f), "能量胶囊");
            }

            Destroy(gameObject);
        }
    }
}
