using UnityEngine;
using Wanwan.Runtime.Pools;

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
        private readonly SpriteRenderer[] mountRenderers = new SpriteRenderer[2];
        private float leftBound;
        private float rightBound;
        private float topBound;
        private float bottomBound;
        private Vector2 targetPosition;
        private Vector2 moveVelocity;
        private float fireTimer;
        private float mountFireTimer;
        private bool combatEnabled = true;
        private PoolCollection pools;

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
            mountFireTimer = 0f;
            pools = FindObjectOfType<GameBootstrap>()?.Pools;
            RefreshMountPresentation();
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

            mountFireTimer -= Time.deltaTime;
            if (mountFireTimer <= 0f)
            {
                FireMountSupport();
                mountFireTimer = MountConfig.Get(gameManager.CurrentMount).FireInterval;
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
            mountFireTimer = 0f;
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

            PlayerShotSpec[] shots = WeaponShotPattern.GetShots(type, gameManager.FireLevel, gameManager.CurrentWeaponModules);
            for (int i = 0; i < shots.Length; i++)
            {
                FireOffsetShot(shots[i]);
            }
        }

        private void FireMountSupport()
        {
            MountType mount = gameManager.CurrentMount;
            if (mount == MountType.None || mount == MountType.ShieldEmitter)
            {
                return;
            }

            PlayerShotSpec[] shots = MountShotPattern.GetShots(mount);
            if (!gameManager.TryConsumeMountUnits(shots.Length))
            {
                return;
            }

            for (int i = 0; i < shots.Length; i++)
            {
                FireOffsetShot(shots[i]);
            }
        }

        private float GetCurrentFireCooldown()
        {
            float configuredInterval = WeaponConfig.Get(gameManager.CurrentWeaponType).FireInterval;
            float moduleBonus = HasModule(WeaponModuleType.RapidFire) ? 0.034f : 0f;
            float interval = Mathf.Max(0.065f, configuredInterval - moduleBonus - ((gameManager.FireLevel - 1) * 0.012f));
            return interval / gameManager.FocusFireRateMultiplier;
        }

        private bool HasModule(WeaponModuleType module)
        {
            WeaponModuleType[] modules = gameManager.CurrentWeaponModules;
            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i] == module)
                {
                    return true;
                }
            }

            return false;
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
            GameObject bulletObject;
            if (pools != null)
            {
                bulletObject = pools.RentBullet();
                if (bulletObject == null)
                {
                    return;
                }
            }
            else
            {
                bulletObject = new GameObject(shot.CanPierce ? shot.WeaponType + "PierceShot" : shot.WeaponType + "Shot");
            }
            bulletObject.transform.position = transform.position + shot.Offset;

            SpriteRenderer renderer = bulletObject.GetComponent<SpriteRenderer>();
            if (renderer == null)
                renderer = bulletObject.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetBulletSprite(shot.WeaponType);
            renderer.color = Color.white;
            renderer.sortingOrder = 15;
            bulletObject.transform.localScale = WeaponShotPresentation.GetPlayerScale(shot.WeaponType, shot.CanPierce, shot.Width);
            float angle = Mathf.Atan2(shot.Direction.y, shot.Direction.x) * Mathf.Rad2Deg - 90f;
            bulletObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            BoxCollider2D collider = bulletObject.GetComponent<BoxCollider2D>();
            if (collider == null)
                collider = bulletObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = WeaponShotPresentation.GetPlayerColliderSize(shot.WeaponType, shot.CanPierce);

            Rigidbody2D rb = bulletObject.GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = bulletObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            BulletController bullet = bulletObject.GetComponent<BulletController>();
            if (bullet == null)
                bullet = bulletObject.AddComponent<BulletController>();
            if (pools != null)
            {
                bullet.SetPool(pools);
            }
            float speed = WeaponConfig.Get(shot.WeaponType).BulletSpeed;
            bullet.Initialize((speed > 0f ? speed : BulletSpeed) * shot.SpeedMultiplier, shot.Damage, shot.Direction, gameManager.TopBound + 1.5f, gameManager.LeftBound, gameManager.RightBound, shot.CanPierce, shot.PierceHits, shot.MotionType, shot.HomingStrength, shot.WaveAmplitude, shot.WaveFrequency, shot.ExplosionRadius);
        }

        private void UpdateInvulnerabilityPresentation()
        {
            if (spriteRenderer == null || gameManager == null)
            {
                return;
            }

            Color color = spriteRenderer.color;
            color.a = gameManager.PlayerInvulnerabilityFlashAlpha;

            if (gameManager.IsPlayerInvulnerable && color.a > 0f)
            {
                float flashT = color.a;
                if (flashT > 0.6f)
                {
                    color.r = Mathf.Lerp(color.r, 1f, (flashT - 0.6f) / 0.4f);
                    color.g = Mathf.Lerp(color.g, 1f, (flashT - 0.6f) / 0.4f);
                    color.b = Mathf.Lerp(color.b, 1f, (flashT - 0.6f) / 0.4f);
                }
                else if (flashT > 0.3f)
                {
                    color.r = Mathf.Lerp(color.r, 0.3f, (flashT - 0.3f) / 0.3f);
                    color.g = Mathf.Lerp(color.g, 0.6f, (flashT - 0.3f) / 0.3f);
                    color.b = Mathf.Lerp(color.b, 1f, (flashT - 0.3f) / 0.3f);
                }
            }

            spriteRenderer.color = color;
            for (int i = 0; i < mountRenderers.Length; i++)
            {
                if (mountRenderers[i] != null)
                {
                    Color mountColor = mountRenderers[i].color;
                    mountColor.a = color.a;
                    mountRenderers[i].color = mountColor;
                }
            }
        }

        private void RefreshMountPresentation()
        {
            ClearMountPresentation();
            if (gameManager == null || gameManager.CurrentMount == MountType.None || gameManager.CurrentMount == MountType.ShieldEmitter)
            {
                return;
            }

            Sprite sprite = gameManager.CurrentMount == MountType.MissilePod
                ? RuntimeSpriteFactory.GetMissileSprite()
                : RuntimeSpriteFactory.GetRaidenFighterJetSprite();
            Color color = MountConfig.Get(gameManager.CurrentMount).AccentColor;
            CreateMountVisual(0, "LeftMount", new Vector3(-0.72f, -0.08f, 0f), sprite, color);
            CreateMountVisual(1, "RightMount", new Vector3(0.72f, -0.08f, 0f), sprite, color);
        }

        private void CreateMountVisual(int index, string name, Vector3 localPosition, Sprite sprite, Color color)
        {
            GameObject mountObject = new GameObject(name);
            mountObject.transform.SetParent(transform, false);
            mountObject.transform.localPosition = localPosition;
            mountObject.transform.localRotation = Quaternion.identity;

            SpriteRenderer renderer = mountObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.Lerp(Color.white, color, 0.64f);
            renderer.sortingOrder = 11;
            Vector2 size = renderer.sprite.bounds.size;
            float targetWidth = gameManager.CurrentMount == MountType.MissilePod ? 0.32f : 0.54f;
            float targetHeight = gameManager.CurrentMount == MountType.MissilePod ? 0.68f : 0.54f;
            mountObject.transform.localScale = new Vector3(targetWidth / size.x, targetHeight / size.y, 1f);
            mountRenderers[index] = renderer;
        }

        private void ClearMountPresentation()
        {
            for (int i = 0; i < mountRenderers.Length; i++)
            {
                if (mountRenderers[i] == null)
                {
                    continue;
                }

                GameObject mountObject = mountRenderers[i].gameObject;
                mountRenderers[i] = null;
                if (Application.isPlaying)
                {
                    Destroy(mountObject);
                }
                else
                {
                    DestroyImmediate(mountObject);
                }
            }
        }
    }
}
