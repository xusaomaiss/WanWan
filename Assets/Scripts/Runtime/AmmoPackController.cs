using UnityEngine;

namespace Wanwan.Runtime
{
    public class AmmoPackController : MonoBehaviour
    {
        private const float DefaultDurationSeconds = 8f;

        private GameManager gameManager;
        private EffectsController effectsController;
        private SpriteRenderer spriteRenderer;
        private TextMesh labelText;
        private AmmoPowerupType powerupType;
        private float fallSpeed;
        private float bottomDespawnY;
        private float animationTime;
        private float phaseOffset;
        private bool resolved;
        private Vector3 baseScale;

        public void Initialize(GameManager manager, EffectsController effects, AmmoPowerupType type, float speed, float despawnY, Color color, string label)
        {
            gameManager = manager;
            effectsController = effects;
            powerupType = type;
            fallSpeed = speed;
            bottomDespawnY = despawnY;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            baseScale = transform.localScale;
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
            BuildLabel(label);
            effectsController.PlayPowerupSpawn(transform.position, color);
        }

        private void Update()
        {
            if (!gameManager.IsPlaying || resolved)
            {
                return;
            }

            animationTime += Time.deltaTime;
            transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
            float pulse = 1f + (Mathf.Sin((animationTime * 6f) + phaseOffset) * 0.08f);
            transform.localScale = baseScale * pulse;
            float tilt = Mathf.Sin((animationTime * 4f) + phaseOffset) * 8f;
            transform.rotation = Quaternion.Euler(0f, 0f, tilt);

            if (transform.position.y < bottomDespawnY)
            {
                resolved = true;
                Destroy(gameObject);
            }
        }

        public void Collect()
        {
            if (resolved || !gameManager.IsPlaying)
            {
                return;
            }

            resolved = true;
            gameManager.ActivatePowerup(powerupType, DefaultDurationSeconds);
            effectsController.PlayPowerupPickup(transform.position, spriteRenderer.color, GetDisplayName(powerupType));
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                Collect();
            }
        }

        private void BuildLabel(string label)
        {
            GameObject labelObject = new GameObject("PackLabel");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.localPosition = Vector3.zero;
            labelText = labelObject.AddComponent<TextMesh>();
            labelText.text = label;
            labelText.anchor = TextAnchor.MiddleCenter;
            labelText.alignment = TextAlignment.Center;
            labelText.characterSize = 0.13f;
            labelText.fontSize = 52;
            labelText.color = Color.white;
        }

        private static string GetDisplayName(AmmoPowerupType type)
        {
            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    return "散射";
                case AmmoPowerupType.RapidFire:
                    return "连发";
                case AmmoPowerupType.Pierce:
                    return "穿透";
                case AmmoPowerupType.Laser:
                    return "激光";
                case AmmoPowerupType.Plasma:
                    return "等离子";
                case AmmoPowerupType.Burst:
                    return "爆裂";
                case AmmoPowerupType.Homing:
                    return "追踪";
                case AmmoPowerupType.Wave:
                    return "波刃";
                case AmmoPowerupType.Guard:
                    return "护航";
                default:
                    return "强化";
            }
        }
    }
}
