using UnityEngine;

namespace Wanwan.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        private const float MoveSmoothTime = 0.045f;
        private const float MaxHorizontalSpeed = 27f;
        private const float DefaultFireCooldown = 0.16f;
        private const float BulletSpeed = 21.25f;

        private GameManager gameManager;
        private EffectsController effectsController;
        private Camera worldCamera;
        private SpriteRenderer spriteRenderer;
        private float leftBound;
        private float rightBound;
        private float topBound;
        private float bottomBound;
        private Vector2 targetPosition;
        private Vector2 moveVelocity;
        private float fireTimer;
        private bool combatEnabled = true;

        public void Initialize(GameManager manager, EffectsController effects, Camera camera, float minX, float maxX, float minY, float maxY)
        {
            gameManager = manager;
            effectsController = effects;
            worldCamera = camera;
            leftBound = minX;
            rightBound = maxX;
            bottomBound = minY;
            topBound = maxY;
            targetPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            UpdateInvulnerabilityPresentation();

            if (!combatEnabled || !gameManager.IsPlaying)
            {
                return;
            }

            UpdateTargetPosition();
            Vector2 clampedTarget = ClampToBounds(targetPosition);
            Vector2 current = transform.position;
            transform.position = Vector2.SmoothDamp(current, clampedTarget, ref moveVelocity, MoveSmoothTime, MaxHorizontalSpeed * gameManager.PlayerSpeedMultiplier, Time.deltaTime);

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
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }

        public void ResetForGameplayPosition(Vector3 position)
        {
            transform.position = position;
            targetPosition = ClampToBounds(position);
            moveVelocity = Vector2.zero;
            fireTimer = 0f;
        }

        private void UpdateTargetPosition()
        {
            if (Input.touchCount > 0)
            {
                targetPosition = ScreenToWorldPosition(Input.GetTouch(0).position);
                return;
            }

            if (Input.GetMouseButton(0))
            {
                targetPosition = ScreenToWorldPosition(Input.mousePosition);
            }
        }

        private Vector2 ScreenToWorldPosition(Vector3 screenPosition)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -worldCamera.transform.position.z));
            return ClampToBounds(worldPosition);
        }

        private Vector2 ClampToBounds(Vector2 position)
        {
            const float shipInset = 0.12f;
            return new Vector2(
                Mathf.Clamp(position.x, leftBound + shipInset, rightBound - shipInset),
                Mathf.Clamp(position.y, bottomBound + shipInset, topBound - shipInset));
        }

        public Vector2 ClampToPlayableBoundsForTests(Vector2 position)
        {
            return ClampToBounds(position);
        }

        private void Fire()
        {
            WeaponType type = gameManager.CurrentWeaponType;
            effectsController.PlayPlayerShot(type);

            PlayerShotSpec[] shots = WeaponShotPattern.GetShots(type, gameManager.FireLevel);
            for (int i = 0; i < shots.Length; i++)
            {
                FireOffsetShot(shots[i]);
            }
        }

        private float GetCurrentFireCooldown()
        {
            float configuredInterval = WeaponConfig.Get(gameManager.CurrentWeaponType).FireInterval;
            return Mathf.Max(0.08f, configuredInterval - ((gameManager.FireLevel - 1) * 0.012f));
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
            FireSingleShot(AmmoPowerupType.RapidFire, Vector2.up, 0.17f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            if (level >= 2)
            {
                float sideOffset = WeaponShotPresentation.GetRapidSideOffset();
                FireOffsetShot(AmmoPowerupType.RapidFire, new Vector3(-sideOffset, 0.06f, 0f), Vector2.up, 0.145f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                FireOffsetShot(AmmoPowerupType.RapidFire, new Vector3(sideOffset, 0.06f, 0f), Vector2.up, 0.145f, false, 1, 1, BulletMotionType.Straight, 0f, 0f, 0f);
            }
        }

        private void FireLaser(int level)
        {
            FireSingleShot(AmmoPowerupType.Laser, Vector2.up, 0.14f, true, 2 + level, 1 + level, BulletMotionType.Straight, 0f, 0f, 0f);
            if (level >= 2)
            {
                float sideOffset = WeaponShotPresentation.GetLaserSideOffset();
                FireOffsetShot(AmmoPowerupType.Laser, new Vector3(-sideOffset, 0f, 0f), Vector2.up, 0.12f, true, 2, 1, BulletMotionType.Straight, 0f, 0f, 0f);
                FireOffsetShot(AmmoPowerupType.Laser, new Vector3(sideOffset, 0f, 0f), Vector2.up, 0.12f, true, 2, 1, BulletMotionType.Straight, 0f, 0f, 0f);
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
            FireOffsetShot(new PlayerShotSpec(offset, direction, width, PowerupCycle.ToWeaponType(type), canPierce, pierceHits, damage, motionType, homingStrength, 0f));
        }

        private void FireOffsetShot(PlayerShotSpec shot)
        {
            GameObject bulletObject = new GameObject(shot.CanPierce ? shot.WeaponType + "PierceShot" : shot.WeaponType + "Shot");
            bulletObject.transform.position = transform.position + shot.Offset;

            SpriteRenderer renderer = bulletObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBulletSprite(shot.WeaponType);
            renderer.color = Color.white;
            renderer.sortingOrder = 15;
            bulletObject.transform.localScale = WeaponShotPresentation.GetPlayerScale(shot.WeaponType, shot.CanPierce, shot.Width);
            float angle = Mathf.Atan2(shot.Direction.y, shot.Direction.x) * Mathf.Rad2Deg - 90f;
            bulletObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            BoxCollider2D collider = bulletObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = WeaponShotPresentation.GetPlayerColliderSize(shot.WeaponType, shot.CanPierce);

            Rigidbody2D rigidbody2D = bulletObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BulletController bullet = bulletObject.AddComponent<BulletController>();
            float speed = WeaponConfig.Get(shot.WeaponType).BulletSpeed;
            bullet.Initialize(speed > 0f ? speed : BulletSpeed, shot.Damage, shot.Direction, gameManager.TopBound + 1.5f, gameManager.LeftBound, gameManager.RightBound, shot.CanPierce, shot.PierceHits, shot.MotionType, shot.HomingStrength, 0f, 0f, shot.ExplosionRadius);
        }

        private void UpdateInvulnerabilityPresentation()
        {
            if (spriteRenderer == null || gameManager == null)
            {
                return;
            }

            Color color = spriteRenderer.color;
            color.a = gameManager.PlayerInvulnerabilityFlashAlpha;
            spriteRenderer.color = color;
        }
    }
}
