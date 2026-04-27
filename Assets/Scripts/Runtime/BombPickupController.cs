using UnityEngine;

namespace Wanwan.Runtime
{
    public class BombPickupController : MonoBehaviour
    {
        private GameManager gameManager;
        private SpriteRenderer spriteRenderer;
        private float age;
        private bool consumed;

        public void Initialize(GameManager manager)
        {
            gameManager = manager;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (consumed || gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            age += Time.deltaTime;
            float pulse = 1f + (Mathf.Sin(age * 5.5f) * 0.14f);
            transform.localScale = Vector3.one * (PickupPresentation.BombVisualScale * pulse);
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(age * 2.4f) * 8f);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(new Color(0.82f, 0.98f, 1f), Color.white, Mathf.PingPong(age * 2.4f, 1f));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed || other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            consumed = true;
            gameManager.ActivateBombFromPickup(transform.position);
            Destroy(gameObject);
        }
    }
}
