using UnityEngine;

namespace Wanwan.Runtime
{
    public class EnemyFireballController : MonoBehaviour
    {
        private GameManager gameManager;
        private EffectsController effectsController;
        private float speed;
        private float despawnY;
        private float leftDespawnX;
        private float rightDespawnX;
        private Vector2 direction = Vector2.down;
        private Color color;
        private bool resolved;

        public void Initialize(GameManager manager, EffectsController effects, float travelSpeed, Vector2 travelDirection, float minY, float minX, float maxX, Color fireColor)
        {
            gameManager = manager;
            effectsController = effects;
            speed = travelSpeed;
            direction = travelDirection.normalized;
            despawnY = minY;
            leftDespawnX = minX - 1.2f;
            rightDespawnX = maxX + 1.2f;
            color = fireColor;
        }

        private void Update()
        {
            if (resolved || !gameManager.IsPlaying)
            {
                return;
            }

            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
            if (transform.position.y < despawnY || transform.position.x < leftDespawnX || transform.position.x > rightDespawnX)
            {
                resolved = true;
                Destroy(gameObject);
            }
        }

        public void ClearByBomb()
        {
            if (resolved)
            {
                return;
            }

            resolved = true;
            Destroy(gameObject);
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
                effectsController.PlayPlayerPierced(transform.position, color);
                gameManager.DestroyPlayerByPierce();
                Destroy(gameObject);
            }
        }
    }
}
