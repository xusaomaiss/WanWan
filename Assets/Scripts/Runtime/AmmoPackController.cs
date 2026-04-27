using UnityEngine;

namespace Wanwan.Runtime
{
    public class AmmoPackController : MonoBehaviour
    {
        private const float DefaultDurationSeconds = 8f;
        private const float SidePadding = 0.28f;
        private const float TopPadding = 0.28f;
        private const float BounceDamping = 0.82f;
        private const float Drag = 0.92f;
        private const float DownwardAcceleration = 1.15f;
        private const float MaxFallSpeed = 2.75f;
        private const float DriftAmplitude = 0.28f;

        private GameManager gameManager;
        private EffectsController effectsController;
        private BlockSpawner blockSpawner;
        private SpriteRenderer spriteRenderer;
        private TextMesh labelText;
        private AmmoPowerupType startingType;
        private AmmoPowerupType powerupType;
        private float fallSpeed;
        private float bottomDespawnY;
        private float animationTime;
        private float cycleTimer;
        private int cycleIndex;
        private float phaseOffset;
        private bool resolved;
        private Vector3 baseScale;
        private Vector2 velocity;
        private float leftBound;
        private float rightBound;
        private float topBound;
        private AmmoPackPickupMode pickupMode;

        public void Initialize(GameManager manager, EffectsController effects, BlockSpawner spawner, AmmoPowerupType type, float speed, float despawnY, float left, float right, float top, Color color, string label, AmmoPackPickupMode mode = AmmoPackPickupMode.Normal)
        {
            gameManager = manager;
            effectsController = effects;
            blockSpawner = spawner;
            startingType = type;
            powerupType = type;
            fallSpeed = speed;
            bottomDespawnY = despawnY;
            leftBound = left;
            rightBound = right;
            topBound = top;
            pickupMode = mode;
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            baseScale = transform.localScale;
            phaseOffset = Random.Range(0f, Mathf.PI * 2f);
            velocity = BuildInitialVelocity(speed);
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
            if (pickupMode == AmmoPackPickupMode.Normal && PowerupCycle.IsPrimaryWeaponPowerup(startingType) && cycleTimer >= 0.8f)
            {
                cycleTimer = 0f;
                cycleIndex++;
                RefreshDisplayedPowerup();
            }

            ApplyMotionStep(Time.deltaTime);
            float pulse = 1f + (Mathf.Sin((animationTime * 6f) + phaseOffset) * 0.08f);
            transform.localScale = baseScale * pulse;
            float tilt = Mathf.Sin((animationTime * 4f) + phaseOffset) * 8f;
            transform.rotation = Quaternion.Euler(0f, 0f, tilt);
        }

        public void Collect()
        {
            if (resolved || !gameManager.IsPlaying)
            {
                return;
            }

            resolved = true;
            if (pickupMode == AmmoPackPickupMode.RecoveryRestore && PowerupCycle.IsPrimaryWeaponPowerup(powerupType))
            {
                gameManager.ApplyWeaponPickup(PowerupCycle.ToWeaponType(powerupType));
            }
            else
            {
                gameManager.ApplyPowerupPickup(powerupType);
            }

            effectsController.PlayPowerupPickup(transform.position, spriteRenderer.color, PowerupCycle.GetLabel(powerupType) + " 火力");
            ReturnOrDispose();
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
            ArcadeFontProvider.ApplyTo(labelText);
            labelText.text = label;
            labelText.anchor = TextAnchor.MiddleCenter;
            labelText.alignment = TextAlignment.Center;
            labelText.characterSize = 0.13f;
            labelText.fontSize = 52;
            labelText.color = Color.white;
        }

        private void RefreshDisplayedPowerup()
        {
            powerupType = pickupMode == AmmoPackPickupMode.Normal && PowerupCycle.IsPrimaryWeaponPowerup(startingType) ? PowerupCycle.GetPrimaryTypeAt(startingType, cycleIndex) : startingType;
            spriteRenderer.sprite = RuntimeSpriteFactory.GetAmmoPackSprite(powerupType);
            spriteRenderer.color = PowerupCycle.GetCategoryColor(powerupType);
            if (labelText != null)
            {
                labelText.text = PowerupCycle.GetPickupLabel(powerupType);
            }
        }

        private static Vector2 BuildInitialVelocity(float speed)
        {
            float horizontal = Random.Range(-1.25f, 1.25f);
            if (Mathf.Abs(horizontal) < 0.35f)
            {
                horizontal = Mathf.Sign(horizontal == 0f ? Random.Range(-1f, 1f) : horizontal) * 0.35f;
            }

            return new Vector2(horizontal, Random.Range(1.15f, 2.05f) + (speed * 0.18f));
        }

        private void ApplyMotionStep(float deltaSeconds)
        {
            float dt = Mathf.Max(0f, deltaSeconds);
            velocity.x *= Mathf.Pow(Drag, dt * 8f);
            velocity.y = Mathf.Max(-MaxFallSpeed, velocity.y - (DownwardAcceleration * dt));

            float drift = Mathf.Sin((animationTime * 2.8f) + phaseOffset) * DriftAmplitude;
            Vector3 position = transform.position;
            position += new Vector3((velocity.x + drift) * dt, velocity.y * dt, 0f);

            float minX = leftBound + SidePadding;
            float maxX = rightBound - SidePadding;
            if (position.x < minX)
            {
                position.x = minX;
                velocity.x = Mathf.Abs(velocity.x) * BounceDamping;
            }
            else if (position.x > maxX)
            {
                position.x = maxX;
                velocity.x = -Mathf.Abs(velocity.x) * BounceDamping;
            }

            float maxY = topBound - TopPadding;
            if (position.y > maxY)
            {
                position.y = maxY;
                velocity.y = -Mathf.Abs(velocity.y) * BounceDamping;
            }

            transform.position = position;
            if (position.y < bottomDespawnY)
            {
                resolved = true;
                ReturnOrDispose();
            }
        }

        private void ReturnOrDispose()
        {
            if (blockSpawner != null)
            {
                blockSpawner.ReturnPickupObject(gameObject);
                return;
            }

            DisposeRuntimeObject(gameObject);
        }

        private static void DisposeRuntimeObject(Object target)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
                return;
            }

            DestroyImmediate(target);
        }

#if UNITY_INCLUDE_TESTS
        public bool IsResolvedForTests => resolved;

        public void ConfigureMotionForTests(Vector2 initialVelocity, float left, float right, float top, float bottom)
        {
            velocity = initialVelocity;
            leftBound = left;
            rightBound = right;
            topBound = top;
            bottomDespawnY = bottom;
            phaseOffset = 0f;
        }

        public void ApplyMotionStepForTests(float deltaSeconds)
        {
            ApplyMotionStep(deltaSeconds);
        }
#endif
    }
}
