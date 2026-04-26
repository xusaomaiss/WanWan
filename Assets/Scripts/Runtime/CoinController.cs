using UnityEngine;

namespace Wanwan.Runtime
{
    public class CoinController : MonoBehaviour
    {
        public const float BaseVisualScale = 0.36f;

        private GameManager gameManager;
        private SpriteRenderer spriteRenderer;
        private Vector3 driftVelocity;
        private float age;
        private bool collected;

        public void Initialize(GameManager manager, Vector2 initialDrift)
        {
            gameManager = manager;
            driftVelocity = initialDrift;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (collected || gameManager == null || !gameManager.IsPlaying)
            {
                return;
            }

            age += Time.deltaTime;
            Vector3 playerPosition = gameManager.PlayerPosition;
            float distance = Vector2.Distance(transform.position, playerPosition);

            if (distance <= gameManager.RewardConfig.CoinCollectRadiusWorld)
            {
                collected = true;
                gameManager.CollectCoin(transform.position);
                Destroy(gameObject);
                return;
            }

            if (distance <= gameManager.RewardConfig.CoinMagnetRadiusWorld)
            {
                float magnetStrength = Mathf.InverseLerp(gameManager.RewardConfig.CoinMagnetRadiusWorld, gameManager.RewardConfig.CoinCollectRadiusWorld, distance);
                transform.position = Vector3.Lerp(transform.position, playerPosition, Time.deltaTime * Mathf.Lerp(5f, 13f, magnetStrength));
            }
            else
            {
                transform.position += driftVelocity * Time.deltaTime;
                driftVelocity = Vector3.Lerp(driftVelocity, Vector3.down * 0.42f, Time.deltaTime * 1.4f);
            }

            float pulse = 1f + (Mathf.Sin(age * 9f) * 0.12f);
            transform.localScale = Vector3.one * (BaseVisualScale * pulse);
            transform.rotation = Quaternion.Euler(0f, 0f, age * 180f);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = age > 8f ? Mathf.Clamp01(10f - age) : 1f;
                spriteRenderer.color = color;
            }

            if (age > 10f || transform.position.y < gameManager.BottomBound - 1.2f)
            {
                Destroy(gameObject);
            }
        }
    }
}
