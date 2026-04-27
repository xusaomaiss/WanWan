using System.Collections.Generic;
using UnityEngine;

namespace Wanwan.Runtime
{
    public class BulletController : MonoBehaviour
    {
        private readonly HashSet<int> hitTargets = new HashSet<int>();
        private float speed;
        private int damage;
        private float despawnY;
        private Vector2 direction = Vector2.up;
        private float leftBound;
        private float rightBound;
        private bool piercesTargets;
        private int remainingPierceHits;
        private BulletMotionType motionType;
        private float steeringStrength;
        private float waveAmplitude;
        private float waveFrequency;
        private float explosionRadius;
        private float age;
        private Vector3 origin;

        public void Initialize(float travelSpeed, int bulletDamage, Vector2 travelDirection, float maxY, float minX, float maxX, bool canPierce, int pierceHits)
        {
            Initialize(travelSpeed, bulletDamage, travelDirection, maxY, minX, maxX, canPierce, pierceHits, BulletMotionType.Straight, 0f, 0f, 0f);
        }

        public void Initialize(float travelSpeed, int bulletDamage, Vector2 travelDirection, float maxY, float minX, float maxX, bool canPierce, int pierceHits, BulletMotionType bulletMotionType, float homingStrength, float sideAmplitude, float sideFrequency)
        {
            Initialize(travelSpeed, bulletDamage, travelDirection, maxY, minX, maxX, canPierce, pierceHits, bulletMotionType, homingStrength, sideAmplitude, sideFrequency, 0f);
        }

        public void Initialize(float travelSpeed, int bulletDamage, Vector2 travelDirection, float maxY, float minX, float maxX, bool canPierce, int pierceHits, BulletMotionType bulletMotionType, float homingStrength, float sideAmplitude, float sideFrequency, float blastRadius)
        {
            speed = travelSpeed;
            damage = Mathf.Max(1, bulletDamage);
            direction = travelDirection.normalized;
            despawnY = maxY;
            leftBound = minX - 1f;
            rightBound = maxX + 1f;
            piercesTargets = canPierce;
            remainingPierceHits = Mathf.Max(1, pierceHits);
            motionType = bulletMotionType;
            steeringStrength = Mathf.Max(0f, homingStrength);
            waveAmplitude = Mathf.Max(0f, sideAmplitude);
            waveFrequency = Mathf.Max(0f, sideFrequency);
            explosionRadius = Mathf.Max(0f, blastRadius);
            origin = transform.position;
        }

        private void Update()
        {
            age += Time.deltaTime;

            if (motionType == BulletMotionType.Homing)
            {
                ApplyHoming();
            }

            transform.position += (Vector3)(direction * (speed * Time.deltaTime));

            if (motionType == BulletMotionType.Wave)
            {
                Vector3 position = transform.position;
                position.x = origin.x + (Mathf.Sin(age * waveFrequency) * waveAmplitude);
                transform.position = position;
            }

            if (transform.position.y > despawnY || transform.position.x < leftBound || transform.position.x > rightBound)
            {
                Destroy(gameObject);
            }
        }

        private void ApplyHoming()
        {
            Transform target = FindClosestTarget();
            if (target == null)
            {
                return;
            }

            Vector2 desired = ((Vector2)(target.position - transform.position)).normalized;
            direction = Vector2.Lerp(direction, desired, Time.deltaTime * steeringStrength).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private Transform FindClosestTarget()
        {
            Transform closest = null;
            float closestDistance = float.MaxValue;

            foreach (BlockController block in FindObjectsByType<BlockController>(FindObjectsSortMode.None))
            {
                float distance = Vector2.SqrMagnitude(block.transform.position - transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = block.transform;
                }
            }

            foreach (BossController boss in FindObjectsByType<BossController>(FindObjectsSortMode.None))
            {
                float distance = Vector2.SqrMagnitude(boss.transform.position - transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = boss.transform;
                }
            }

            foreach (GroundTargetController groundTarget in FindObjectsByType<GroundTargetController>(FindObjectsSortMode.None))
            {
                float distance = Vector2.SqrMagnitude(groundTarget.transform.position - transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = groundTarget.transform;
                }
            }

            return closest;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BlockController block))
            {
                if (!hitTargets.Add(block.GetInstanceID()))
                {
                    return;
                }

                block.ApplyHit(damage);
                ResolveExplosion();
                ResolveHit();
                return;
            }

            if (other.TryGetComponent(out BossController boss))
            {
                if (!hitTargets.Add(boss.GetInstanceID()))
                {
                    return;
                }

                boss.ApplyHit(damage);
                ResolveExplosion();
                ResolveHit();
                return;
            }

            if (other.TryGetComponent(out GroundTargetController groundTarget))
            {
                if (!hitTargets.Add(groundTarget.GetInstanceID()))
                {
                    return;
                }

                groundTarget.ApplyHit(damage);
                ResolveExplosion();
                ResolveHit();
                return;
            }

        }

        private void ResolveHit()
        {
            if (!piercesTargets)
            {
                Destroy(gameObject);
                return;
            }

            remainingPierceHits--;
            if (remainingPierceHits <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void ResolveExplosion()
        {
            if (explosionRadius <= 0f)
            {
                return;
            }

            Vector3 center = transform.position;
            float radiusSqr = explosionRadius * explosionRadius;

            foreach (BlockController block in FindObjectsByType<BlockController>(FindObjectsSortMode.None))
            {
                if (Vector2.SqrMagnitude(block.transform.position - center) <= radiusSqr)
                {
                    if (!hitTargets.Add(block.GetInstanceID()))
                    {
                        continue;
                    }

                    block.ApplyHit(damage);
                }
            }

            foreach (BossController boss in FindObjectsByType<BossController>(FindObjectsSortMode.None))
            {
                if (Vector2.SqrMagnitude(boss.transform.position - center) <= radiusSqr)
                {
                    if (!hitTargets.Add(boss.GetInstanceID()))
                    {
                        continue;
                    }

                    boss.ApplyHit(damage);
                }
            }

            foreach (GroundTargetController groundTarget in FindObjectsByType<GroundTargetController>(FindObjectsSortMode.None))
            {
                if (Vector2.SqrMagnitude(groundTarget.transform.position - center) <= radiusSqr)
                {
                    if (!hitTargets.Add(groundTarget.GetInstanceID()))
                    {
                        continue;
                    }

                    groundTarget.ApplyHit(damage);
                }
            }

            GameObject blast = new GameObject("BurstExplosion");
            blast.transform.position = center;
            blast.transform.localScale = Vector3.one * Mathf.Max(0.45f, explosionRadius);
            SpriteRenderer renderer = blast.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.GetExplosionSprite();
            renderer.color = new Color(1f, 0.5f, 0.18f, 0.82f);
            renderer.sortingOrder = 16;
            DisposeRuntimeObject(blast, 0.18f);
        }

        private static void DisposeRuntimeObject(Object target, float delay = 0f)
        {
            if (Application.isPlaying)
            {
                Destroy(target, delay);
                return;
            }

            DestroyImmediate(target);
        }
    }
}
