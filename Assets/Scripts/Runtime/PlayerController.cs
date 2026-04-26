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
        }

        private void Update()
        {
            if (!combatEnabled || !gameManager.IsPlaying)
            {
                return;
            }

            UpdateTargetPosition();
            Vector2 clampedTarget = ClampToBounds(targetPosition);
            Vector2 current = transform.position;
            transform.position = Vector2.SmoothDamp(current, clampedTarget, ref moveVelocity, MoveSmoothTime, MaxHorizontalSpeed, Time.deltaTime);

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
            const float shipInset = 0.55f;
            return new Vector2(
                Mathf.Clamp(position.x, leftBound + shipInset, rightBound - shipInset),
                Mathf.Clamp(position.y, bottomBound + shipInset, topBound - shipInset));
        }

        private void Fire()
        {
            AmmoPowerupType type = gameManager.HasActivePowerup ? gameManager.ActivePowerupType : AmmoPowerupType.Normal;
            effectsController.PlayPlayerShot(type);

            PlayerShotSpec[] shots = PlayerFirePattern.GetShots(gameManager.FireLevel);
            bool canPierce = type == AmmoPowerupType.Pierce || type == AmmoPowerupType.Laser;
            int damage = Mathf.Max(1, gameManager.FireLevel >= 4 ? 2 : 1);
            int pierceHits = canPierce ? 2 + gameManager.ActivePowerupLevel : 1;
            BulletMotionType motionType = type == AmmoPowerupType.Homing ? BulletMotionType.Homing : (type == AmmoPowerupType.Wave ? BulletMotionType.Wave : BulletMotionType.Straight);
            float homingStrength = motionType == BulletMotionType.Homing ? 2.8f + gameManager.FireLevel : 0f;
            float waveAmplitude = motionType == BulletMotionType.Wave ? 0.18f + (gameManager.FireLevel * 0.04f) : 0f;
            float waveFrequency = motionType == BulletMotionType.Wave ? 10f : 0f;

            for (int i = 0; i < shots.Length; i++)
            {
                FireOffsetShot(type, shots[i].Offset, shots[i].Direction, shots[i].Width, canPierce, pierceHits, damage, motionType, homingStrength, waveAmplitude, waveFrequency);
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
                    return Mathf.Max(0.08f, DefaultFireCooldown - ((gameManager.FireLevel - 1) * 0.015f));
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
            GameObject bulletObject = new GameObject(canPierce ? type + "PierceShot" : type + "Shot");
            bulletObject.transform.position = transform.position + offset;

            SpriteRenderer renderer = bulletObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBulletSprite(type);
            renderer.color = Color.white;
            renderer.sortingOrder = 15;
            bulletObject.transform.localScale = WeaponShotPresentation.GetPlayerScale(type, canPierce, width);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            bulletObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            BoxCollider2D collider = bulletObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = WeaponShotPresentation.GetPlayerColliderSize(type, canPierce);

            Rigidbody2D rigidbody2D = bulletObject.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0f;
            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            BulletController bullet = bulletObject.AddComponent<BulletController>();
            bullet.Initialize(BulletSpeed, damage, direction, gameManager.TopBound + 1.5f, gameManager.LeftBound, gameManager.RightBound, canPierce, pierceHits, motionType, homingStrength, waveAmplitude, waveFrequency);
        }
    }
}
