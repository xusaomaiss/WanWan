using UnityEngine;

namespace Wanwan.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        private const float MoveSmoothTime = 0.045f;
        private const float MaxHorizontalSpeed = 27f;
        private const float DefaultFireCooldown = 0.16f;
        private const float RapidFireCooldown = 0.07f;
        private const float BulletSpeed = 21.25f;

        private GameManager gameManager;
        private EffectsController effectsController;
        private Camera worldCamera;
        private float leftBound;
        private float rightBound;
        private float targetX;
        private float horizontalVelocity;
        private float fireTimer;
        private bool combatEnabled = true;

        public void Initialize(GameManager manager, EffectsController effects, Camera camera, float minX, float maxX)
        {
            gameManager = manager;
            effectsController = effects;
            worldCamera = camera;
            leftBound = minX;
            rightBound = maxX;
            targetX = transform.position.x;
        }

        private void Update()
        {
            if (!combatEnabled || !gameManager.IsPlaying)
            {
                return;
            }

            UpdateTargetPosition();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameManager.TryActivateBomb();
            }

            float clampedX = Mathf.Clamp(targetX, leftBound, rightBound);
            Vector3 current = transform.position;
            current.x = Mathf.SmoothDamp(current.x, clampedX, ref horizontalVelocity, MoveSmoothTime, MaxHorizontalSpeed, Time.deltaTime);
            transform.position = current;

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Fire();
                fireTimer = GetCurrentFireCooldown();
            }
        }

        public void StopCombat()
        {
            combatEnabled = false;
        }

        private void UpdateTargetPosition()
        {
            if (Input.touchCount > 0)
            {
                targetX = ScreenToWorldX(Input.GetTouch(0).position);
                return;
            }

            if (Input.GetMouseButton(0))
            {
                targetX = ScreenToWorldX(Input.mousePosition);
            }
        }

        private float ScreenToWorldX(Vector3 screenPosition)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -worldCamera.transform.position.z));
            return worldPosition.x;
        }

        private void Fire()
        {
            AmmoPowerupType type = gameManager.HasActivePowerup ? gameManager.ActivePowerupType : AmmoPowerupType.Normal;
            int level = Mathf.Max(1, gameManager.ActivePowerupLevel);
            effectsController.PlayPlayerShot(type);

            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    FireScatterShot(level);
                    break;
                case AmmoPowerupType.Pierce:
                    FireSingleShot(type, Vector2.up, 0.208f, true, level + 2, level, BulletMotionType.Straight, 0f, 0f, 0f);
                    break;
                case AmmoPowerupType.RapidFire:
                    FireRapidShot(level);
                    break;
                case AmmoPowerupType.Laser:
                    FireLaser(level);
                    break;
                case AmmoPowerupType.Plasma:
                    FireSingleShot(type, Vector2.up, 0.28f + (level * 0.04f), false, 1, 2 + level, BulletMotionType.Straight, 0f, 0f, 0f);
                    break;
                case AmmoPowerupType.Burst:
                    FireSingleShot(type, Vector2.up, 0.32f + (level * 0.04f), false, 1, 3 + level, BulletMotionType.Straight, 0f, 0f, 0f);
                    break;
                case AmmoPowerupType.Homing:
                    FireHoming(level);
                    break;
                case AmmoPowerupType.Wave:
                    FireWave(level);
                    break;
                case AmmoPowerupType.Guard:
                    FireGuard(level);
                    break;
                default:
                    FireSingleShot(AmmoPowerupType.Normal, Vector2.up, 0.192f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                    break;
            }
        }

        private float GetCurrentFireCooldown()
        {
            AmmoPowerupType type = gameManager.HasActivePowerup ? gameManager.ActivePowerupType : AmmoPowerupType.Normal;
            int level = Mathf.Max(1, gameManager.ActivePowerupLevel);

            switch (type)
            {
                case AmmoPowerupType.RapidFire:
                    return Mathf.Max(0.045f, RapidFireCooldown - (level * 0.008f));
                case AmmoPowerupType.Laser:
                    return 0.1f;
                case AmmoPowerupType.Plasma:
                case AmmoPowerupType.Burst:
                    return 0.22f;
                default:
                    return DefaultFireCooldown;
            }
        }

        private void FireScatterShot(int level)
        {
            float spread = level >= 2 ? 0.34f : 0.24f;
            FireSingleShot(AmmoPowerupType.Scatter, new Vector2(-spread, 1f), 0.176f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            FireSingleShot(AmmoPowerupType.Scatter, Vector2.up, 0.184f, false, 1, level >= 3 ? 2 : 1, BulletMotionType.Straight, 0f, 0f, 0f);
            FireSingleShot(AmmoPowerupType.Scatter, new Vector2(spread, 1f), 0.176f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);

            if (level >= 2)
            {
                FireSingleShot(AmmoPowerupType.Scatter, new Vector2(-0.16f, 1f), 0.16f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                FireSingleShot(AmmoPowerupType.Scatter, new Vector2(0.16f, 1f), 0.16f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            }
        }

        private void FireRapidShot(int level)
        {
            FireSingleShot(AmmoPowerupType.RapidFire, Vector2.up, 0.14f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            if (level >= 2)
            {
                FireOffsetShot(AmmoPowerupType.RapidFire, new Vector3(-0.18f, 0.06f, 0f), Vector2.up, 0.12f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                FireOffsetShot(AmmoPowerupType.RapidFire, new Vector3(0.18f, 0.06f, 0f), Vector2.up, 0.12f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            }
        }

        private void FireLaser(int level)
        {
            FireSingleShot(AmmoPowerupType.Laser, Vector2.up, 0.12f, true, 2 + level, 1 + level, BulletMotionType.Straight, 0f, 0f, 0f);
            if (level >= 2)
            {
                FireOffsetShot(AmmoPowerupType.Laser, new Vector3(-0.22f, 0f, 0f), Vector2.up, 0.1f, true, 2, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                FireOffsetShot(AmmoPowerupType.Laser, new Vector3(0.22f, 0f, 0f), Vector2.up, 0.1f, true, 2, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            }
        }

        private void FireHoming(int level)
        {
            FireSingleShot(AmmoPowerupType.Homing, Vector2.up, 0.18f, false, 1, 1, BulletMotionType.Homing, 2.8f + level, 0f, 0f);
            if (level >= 2)
            {
                FireOffsetShot(AmmoPowerupType.Homing, new Vector3(-0.24f, 0f, 0f), new Vector2(-0.08f, 1f), 0.16f, false, 1, 1, BulletMotionType.Homing, 2.4f + level, 0f, 0f);
                FireOffsetShot(AmmoPowerupType.Homing, new Vector3(0.24f, 0f, 0f), new Vector2(0.08f, 1f), 0.16f, false, 1, 1, BulletMotionType.Homing, 2.4f + level, 0f, 0f);
            }
        }

        private void FireWave(int level)
        {
            FireSingleShot(AmmoPowerupType.Wave, Vector2.up, 0.22f, false, 1, 1 + level, BulletMotionType.Wave, 0f, 0.28f + (level * 0.08f), 10f);
            if (level >= 2)
            {
                FireOffsetShot(AmmoPowerupType.Wave, new Vector3(-0.28f, 0f, 0f), Vector2.up, 0.18f, false, 1, 1, BulletMotionType.Wave, 0f, 0.18f, 12f);
                FireOffsetShot(AmmoPowerupType.Wave, new Vector3(0.28f, 0f, 0f), Vector2.up, 0.18f, false, 1, 1, BulletMotionType.Wave, 0f, 0.18f, 12f);
            }
        }

        private void FireGuard(int level)
        {
            FireSingleShot(AmmoPowerupType.Guard, Vector2.up, 0.2f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            FireOffsetShot(AmmoPowerupType.Guard, new Vector3(-0.48f, -0.04f, 0f), Vector2.up, 0.16f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            FireOffsetShot(AmmoPowerupType.Guard, new Vector3(0.48f, -0.04f, 0f), Vector2.up, 0.16f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);

            if (level >= 3)
            {
                FireOffsetShot(AmmoPowerupType.Guard, new Vector3(-0.72f, -0.1f, 0f), Vector2.up, 0.14f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                FireOffsetShot(AmmoPowerupType.Guard, new Vector3(0.72f, -0.1f, 0f), Vector2.up, 0.14f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            }
        }

        private void FireSingleShot(AmmoPowerupType type, Vector2 direction, float width, bool canPierce, int pierceHits, int damage, BulletMotionType motionType, float homingStrength, float waveAmplitude, float waveFrequency)
        {
            FireOffsetShot(type, Vector3.up * 0.45f, direction, width, canPierce, pierceHits, damage, motionType, homingStrength, waveAmplitude, waveFrequency);
        }

        private void FireOffsetShot(AmmoPowerupType type, Vector3 offset, Vector2 direction, float width, bool canPierce, int pierceHits, int damage, BulletMotionType motionType, float homingStrength, float waveAmplitude, float waveFrequency)
        {
            GameObject bulletObject = new GameObject(canPierce ? type + "PierceShot" : type + "Shot");
            bulletObject.transform.position = transform.position + offset;

            SpriteRenderer renderer = bulletObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBulletSprite(type);
            renderer.color = RuntimeSpriteFactory.GetWeaponColor(type);
            renderer.sortingOrder = 15;
            bulletObject.transform.localScale = new Vector3(width, canPierce ? 0.48f : 0.36f, 1f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            bulletObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            BoxCollider2D collider = bulletObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.224f, canPierce ? 0.72f : 0.656f);

            Rigidbody2D rigidbody2D = bulletObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BulletController bullet = bulletObject.AddComponent<BulletController>();
            bullet.Initialize(BulletSpeed, damage, direction, gameManager.TopBound + 1.5f, gameManager.LeftBound, gameManager.RightBound, canPierce, pierceHits, motionType, homingStrength, waveAmplitude, waveFrequency);
        }
    }
}
