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
        private WeaponType startingType;
        private WeaponType weaponType;
        private float fallSpeed;
        private float bottomDespawnY;
        private float animationTime;
        private float cycleTimer;
        private int cycleIndex;
        private float phaseOffset;
        private bool resolved;
        private Vector3 baseScale;

        public void Initialize(GameManager manager, EffectsController effects, AmmoPowerupType type, float speed, float despawnY, Color color, string label)
        {
            Initialize(manager, effects, PowerupCycle.ToWeaponType(type), speed, despawnY, color, label);
        }

        public void Initialize(GameManager manager, EffectsController effects, WeaponType type, float speed, float despawnY, Color color, string label)
        {
            gameManager = manager;
            effectsController = effects;
            startingType = type;
            weaponType = type;
            fallSpeed = speed;
            bottomDespawnY = despawnY;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            baseScale = transform.localScale;
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
            BuildLabel(label);
            RefreshDisplayedPowerup();
            effectsController.PlayPowerupSpawn(transform.position, spriteRenderer.color);
        }

        private void Update()
        {
            if (!gameManager.IsPlaying || resolved)
            {
                return;
            }

            animationTime += Time.deltaTime;
            cycleTimer += Time.deltaTime;
            if (cycleTimer >= 0.8f)
            {
                cycleTimer = 0f;
                cycleIndex++;
                RefreshDisplayedPowerup();
            }

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
            gameManager.ApplyWeaponPickup(weaponType);
            effectsController.PlayPowerupPickup(transform.position, spriteRenderer.color, GetDisplayName(weaponType));
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

        private void RefreshDisplayedPowerup()
        {
            weaponType = PowerupCycle.GetWeaponTypeAt(startingType, cycleIndex);
            spriteRenderer.sprite = RuntimeSpriteFactory.GetAmmoPackSprite(weaponType);
            spriteRenderer.color = PowerupCycle.GetCategoryColor(weaponType);
            if (labelText != null)
            {
                labelText.text = PowerupCycle.GetLabel(weaponType);
            }
        }

        private static string GetDisplayName(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Laser:
                    return "激光";
                case WeaponType.Homing:
                    return "追踪";
                case WeaponType.Burst:
                    return "爆裂";
                default:
                    return "扇形";
            }
        }
    }
}
