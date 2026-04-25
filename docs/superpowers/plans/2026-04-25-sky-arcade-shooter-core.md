# Sky Arcade Shooter Core Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade the current Unity vertical shooter into a blue-sky arcade shooter core with scrolling clouds, ten weapon types, weapon levels, BOMB, richer projectile/ammo-pack visuals, and stronger enemy pressure.

**Architecture:** Keep the existing runtime-generated Unity structure and add small focused state classes before wiring visuals and gameplay. `ActivePowerupState` owns weapon type/level/duration, a new `BombState` owns bomb count, `BulletController` gains optional homing/wave behavior, and `RuntimeSpriteFactory` remains the central sprite cache for generated art.

**Tech Stack:** Unity 6000 C#, `UnityEngine` 2D sprites/colliders/UI, NUnit EditMode tests, existing Python validation scripts.

---

## File Structure

- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPowerupType.cs`
  - Expand weapon enum to ten weapon states.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/ActivePowerupState.cs`
  - Track active weapon level and same-type upgrade behavior.
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BombState.cs`
  - Small testable class for bomb count.
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BulletMotionType.cs`
  - Enum for straight, homing, and wave bullet motion.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BulletController.cs`
  - Add damage, homing, and wave parameters while preserving current collision behavior.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/PlayerController.cs`
  - Dispatch all ten weapon modes and BOMB input.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/GameManager.cs`
  - Expose weapon level, bomb count, and bomb activation.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/EnemyFireballController.cs`
  - Add `ClearByBomb()` so bombs can remove enemy bullets cleanly.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
  - Add bullet, ammo-pack, sky, and cloud sprite variants.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockSpawner.cs`
  - Spawn expanded powerups and stronger enemy/boss bullet patterns.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPackController.cs`
  - Display improved pack icon/label colors.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Bootstrap/GameBootstrap.cs`
  - Replace static background with layered scrolling sky objects.
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/ScrollingBackgroundLayer.cs`
  - Moves a background layer downward and wraps it.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`
  - Show weapon level and bomb count/button.
- Test: `/Users/mark/work/wanwan/Assets/Tests/EditMode/ActivePowerupStateTests.cs`
- Create: `/Users/mark/work/wanwan/Assets/Tests/EditMode/BombStateTests.cs`
- Test: `/Users/mark/work/wanwan/tests/test_automation_files.py`

Because `/Users/mark/work/wanwan` is not currently a git repository, all commit steps below are conditional. If `git status` returns `fatal: not a git repository`, skip the commit step and continue.

### Task 1: Expand Powerup State With Weapon Levels

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPowerupType.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/ActivePowerupState.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Tests/EditMode/ActivePowerupStateTests.cs`

- [ ] **Step 1: Write failing tests for weapon switching and same-type upgrades**

Append these tests inside `ActivePowerupStateTests`:

```csharp
[Test]
public void Activate_SameType_IncreasesLevelAndRefreshesDuration()
{
    ActivePowerupState state = new ActivePowerupState();
    state.Activate(AmmoPowerupType.Laser, 8f);
    state.Tick(3f);

    state.Activate(AmmoPowerupType.Laser, 8f);

    Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.Laser));
    Assert.That(state.Level, Is.EqualTo(2));
    Assert.That(state.RemainingSeconds, Is.EqualTo(8f));
}

[Test]
public void Activate_SameType_ClampsLevelAtThree()
{
    ActivePowerupState state = new ActivePowerupState();

    state.Activate(AmmoPowerupType.Homing, 8f);
    state.Activate(AmmoPowerupType.Homing, 8f);
    state.Activate(AmmoPowerupType.Homing, 8f);
    state.Activate(AmmoPowerupType.Homing, 8f);

    Assert.That(state.Level, Is.EqualTo(3));
}

[Test]
public void Activate_DifferentType_ResetsLevelToOne()
{
    ActivePowerupState state = new ActivePowerupState();
    state.Activate(AmmoPowerupType.Scatter, 8f);
    state.Activate(AmmoPowerupType.Scatter, 8f);

    state.Activate(AmmoPowerupType.Plasma, 8f);

    Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.Plasma));
    Assert.That(state.Level, Is.EqualTo(1));
}

[Test]
public void Tick_ClearsLevel_WhenDurationExpires()
{
    ActivePowerupState state = new ActivePowerupState();
    state.Activate(AmmoPowerupType.Burst, 1f);
    state.Activate(AmmoPowerupType.Burst, 1f);

    state.Tick(1.1f);

    Assert.That(state.Type, Is.EqualTo(AmmoPowerupType.None));
    Assert.That(state.Level, Is.EqualTo(0));
    Assert.That(state.HasActivePowerup, Is.False);
}
```

- [ ] **Step 2: Run tests and verify they fail**

Run:

```bash
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: compile/test failure because `AmmoPowerupType.Laser`, `Plasma`, `Homing`, `Burst`, and `ActivePowerupState.Level` do not exist.

- [ ] **Step 3: Expand `AmmoPowerupType`**

Replace `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPowerupType.cs` with:

```csharp
namespace Wanwan.Runtime
{
    public enum AmmoPowerupType
    {
        None = 0,
        Normal = 1,
        Scatter = 2,
        RapidFire = 3,
        Pierce = 4,
        Laser = 5,
        Plasma = 6,
        Burst = 7,
        Homing = 8,
        Wave = 9,
        Guard = 10
    }
}
```

- [ ] **Step 4: Implement level behavior in `ActivePowerupState`**

Replace `/Users/mark/work/wanwan/Assets/Scripts/Runtime/ActivePowerupState.cs` with:

```csharp
using UnityEngine;

namespace Wanwan.Runtime
{
    public class ActivePowerupState
    {
        private const int MaxLevel = 3;

        public AmmoPowerupType Type { get; private set; }
        public int Level { get; private set; }
        public float RemainingSeconds { get; private set; }
        public bool HasActivePowerup => Type != AmmoPowerupType.None && RemainingSeconds > 0f;

        public void Activate(AmmoPowerupType type, float durationSeconds)
        {
            if (type == AmmoPowerupType.None || type == AmmoPowerupType.Normal)
            {
                Clear();
                return;
            }

            Level = Type == type && HasActivePowerup ? Mathf.Min(MaxLevel, Level + 1) : 1;
            Type = type;
            RemainingSeconds = Mathf.Max(0f, durationSeconds);
        }

        public void Tick(float deltaSeconds)
        {
            if (!HasActivePowerup)
            {
                return;
            }

            RemainingSeconds = Mathf.Max(0f, RemainingSeconds - deltaSeconds);
            if (RemainingSeconds <= 0f)
            {
                Clear();
            }
        }

        public void Clear()
        {
            Type = AmmoPowerupType.None;
            Level = 0;
            RemainingSeconds = 0f;
        }
    }
}
```

- [ ] **Step 5: Run EditMode tests**

Run:

```bash
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: `ActivePowerupStateTests` pass.

- [ ] **Step 6: Run project validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

Expected: both pass.

- [ ] **Step 7: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/AmmoPowerupType.cs Assets/Scripts/Runtime/ActivePowerupState.cs Assets/Tests/EditMode/ActivePowerupStateTests.cs
git commit -m "feat: expand weapon powerup state"
```

Skip if `git status` reports this is not a repository.

### Task 2: Add Bomb State and GameManager Bomb API

**Files:**
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BombState.cs`
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BombState.cs.meta` only if Unity does not auto-generate it
- Create: `/Users/mark/work/wanwan/Assets/Tests/EditMode/BombStateTests.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/GameManager.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/EnemyFireballController.cs`

- [ ] **Step 1: Add BombState tests**

Create `/Users/mark/work/wanwan/Assets/Tests/EditMode/BombStateTests.cs`:

```csharp
using NUnit.Framework;
using Wanwan.Runtime;

namespace Wanwan.Tests.EditMode
{
    public class BombStateTests
    {
        [Test]
        public void Constructor_SetsInitialCount()
        {
            BombState state = new BombState(3);

            Assert.That(state.Count, Is.EqualTo(3));
            Assert.That(state.HasBomb, Is.True);
        }

        [Test]
        public void TryConsume_DecrementsCount()
        {
            BombState state = new BombState(2);

            bool consumed = state.TryConsume();

            Assert.That(consumed, Is.True);
            Assert.That(state.Count, Is.EqualTo(1));
        }

        [Test]
        public void TryConsume_WhenEmpty_ReturnsFalseAndDoesNotGoNegative()
        {
            BombState state = new BombState(0);

            bool consumed = state.TryConsume();

            Assert.That(consumed, Is.False);
            Assert.That(state.Count, Is.EqualTo(0));
        }
    }
}
```

- [ ] **Step 2: Run tests and verify they fail**

Run:

```bash
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: compile failure because `BombState` does not exist.

- [ ] **Step 3: Implement `BombState`**

Create `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BombState.cs`:

```csharp
using UnityEngine;

namespace Wanwan.Runtime
{
    public class BombState
    {
        public BombState(int initialCount)
        {
            Count = Mathf.Max(0, initialCount);
        }

        public int Count { get; private set; }
        public bool HasBomb => Count > 0;

        public bool TryConsume()
        {
            if (!HasBomb)
            {
                return false;
            }

            Count--;
            return true;
        }
    }
}
```

- [ ] **Step 4: Add bomb API to `GameManager`**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/GameManager.cs`:

Add a field near `activePowerup`:

```csharp
private readonly BombState bombState = new BombState(3);
```

Add public properties near the existing powerup properties:

```csharp
public int ActivePowerupLevel => activePowerup.Level;
public int BombCount => bombState.Count;
public bool HasBomb => bombState.HasBomb;
```

Add this method near `ActivatePowerup`:

```csharp
public bool TryActivateBomb()
{
    if (gameEnded || paused || !bombState.TryConsume())
    {
        uiController.RefreshHud();
        return false;
    }

    foreach (EnemyFireballController fireball in FindObjectsByType<EnemyFireballController>(FindObjectsSortMode.None))
    {
        fireball.ClearByBomb();
    }

    foreach (BlockController block in FindObjectsByType<BlockController>(FindObjectsSortMode.None))
    {
        block.ApplyHit(4);
    }

    foreach (BossController boss in FindObjectsByType<BossController>(FindObjectsSortMode.None))
    {
        boss.ApplyHit(5);
    }

    ShowStageBanner("BOMB");
    uiController.RefreshHud();
    return true;
}
```

- [ ] **Step 5: Add bomb clearing to `EnemyFireballController`**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/EnemyFireballController.cs`, add this public method before `OnTriggerEnter2D`:

```csharp
public void ClearByBomb()
{
    if (resolved)
    {
        return;
    }

    resolved = true;
    Destroy(gameObject);
}
```

- [ ] **Step 6: Run EditMode tests**

Run:

```bash
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: `BombStateTests` and existing tests pass.

- [ ] **Step 7: Run validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

Expected: both pass.

- [ ] **Step 8: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/BombState.cs Assets/Tests/EditMode/BombStateTests.cs Assets/Scripts/Runtime/GameManager.cs Assets/Scripts/Runtime/EnemyFireballController.cs
git commit -m "feat: add bomb state and activation"
```

Skip if this is not a git repository.

### Task 3: Add Bullet Motion Variants

**Files:**
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BulletMotionType.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BulletController.cs`

- [ ] **Step 1: Create bullet motion enum**

Create `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BulletMotionType.cs`:

```csharp
namespace Wanwan.Runtime
{
    public enum BulletMotionType
    {
        Straight = 0,
        Homing = 1,
        Wave = 2
    }
}
```

- [ ] **Step 2: Replace `BulletController` with configurable motion**

Replace `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BulletController.cs` with:

```csharp
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
        private float age;
        private Vector3 origin;

        public void Initialize(float travelSpeed, int bulletDamage, Vector2 travelDirection, float maxY, float minX, float maxX, bool canPierce, int pierceHits)
        {
            Initialize(travelSpeed, bulletDamage, travelDirection, maxY, minX, maxX, canPierce, pierceHits, BulletMotionType.Straight, 0f, 0f, 0f);
        }

        public void Initialize(float travelSpeed, int bulletDamage, Vector2 travelDirection, float maxY, float minX, float maxX, bool canPierce, int pierceHits, BulletMotionType bulletMotionType, float homingStrength, float sideAmplitude, float sideFrequency)
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
                ResolveHit();
                return;
            }

            if (other.TryGetComponent(out AmmoPackController ammoPack))
            {
                if (!hitTargets.Add(ammoPack.GetInstanceID()))
                {
                    return;
                }

                ammoPack.Collect();
                ResolveHit();
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
    }
}
```

- [ ] **Step 3: Run validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

Expected: both pass.

- [ ] **Step 4: Run EditMode tests**

Run:

```bash
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: tests pass.

- [ ] **Step 5: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/BulletMotionType.cs Assets/Scripts/Runtime/BulletController.cs
git commit -m "feat: add bullet motion variants"
```

Skip if this is not a git repository.

### Task 4: Add Ten Bullet and Ammo-Pack Sprite Variants

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockSpawner.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPackController.cs`

- [ ] **Step 1: Add sprite accessor methods to `RuntimeSpriteFactory`**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`, add these public methods after `GetMissileSprite()`:

```csharp
public static Sprite GetBulletSprite(AmmoPowerupType type)
{
    return GetOrCreate("bullet-" + type, () => BuildBulletTexture(type));
}

public static Sprite GetAmmoPackSprite(AmmoPowerupType type)
{
    return GetOrCreate("ammo-pack-" + type, () => BuildAmmoPackTexture(type));
}
```

- [ ] **Step 2: Add bullet and pack texture builders**

Add these methods before `BuildHeroFighterTexture()`:

```csharp
private static Texture2D BuildBulletTexture(AmmoPowerupType type)
{
    const int width = 96;
    const int height = 144;
    Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
    Color core = GetWeaponColor(type);
    Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            float dx = Mathf.Abs(x - center.x);
            float dy = Mathf.Abs(y - center.y);
            bool filled = false;
            Color pixel = core;

            switch (type)
            {
                case AmmoPowerupType.Laser:
                    filled = dx <= 5f && y >= 8f && y <= 136f;
                    pixel = dx <= 2f ? Color.white : core;
                    break;
                case AmmoPowerupType.Plasma:
                    filled = Vector2.Distance(new Vector2(x, y), center) <= 30f;
                    pixel = Color.Lerp(core, Color.white, Mathf.InverseLerp(30f, 0f, Vector2.Distance(new Vector2(x, y), center)) * 0.7f);
                    break;
                case AmmoPowerupType.Burst:
                    filled = dy + (dx * 0.8f) <= 42f;
                    pixel = dx <= 8f ? Color.white : core;
                    break;
                case AmmoPowerupType.Homing:
                    filled = (dx <= 8f && y >= 20f && y <= 124f) || (dx >= 10f && dx <= 28f && y >= 26f && y <= 58f);
                    break;
                case AmmoPowerupType.Wave:
                    filled = dy <= 45f - (dx * 0.75f) && dx <= 44f;
                    break;
                case AmmoPowerupType.Guard:
                    filled = Mathf.Abs(Vector2.Distance(new Vector2(x, y), center) - 24f) <= 5f || (dx <= 5f && dy <= 36f);
                    break;
                case AmmoPowerupType.Scatter:
                    filled = dx <= Mathf.Lerp(3f, 14f, Mathf.InverseLerp(136f, 16f, y)) && y >= 16f && y <= 136f;
                    break;
                case AmmoPowerupType.RapidFire:
                    filled = dx <= 4f && y >= 10f && y <= 134f;
                    pixel = Color.Lerp(core, Color.white, dx <= 2f ? 0.75f : 0.1f);
                    break;
                case AmmoPowerupType.Pierce:
                    filled = dx <= 7f && y >= 10f && y <= 134f;
                    pixel = dx <= 3f ? Color.white : core;
                    break;
                default:
                    filled = dx <= 8f && y >= 18f && y <= 126f;
                    break;
            }

            texture.SetPixel(x, y, filled ? pixel : Color.clear);
        }
    }

    texture.Apply();
    return texture;
}

private static Texture2D BuildAmmoPackTexture(AmmoPowerupType type)
{
    const int size = 128;
    Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
    Color core = GetWeaponColor(type);
    Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size; x++)
        {
            float dx = Mathf.Abs(x - center.x);
            float dy = Mathf.Abs(y - center.y);
            bool frame = Mathf.Max(dx, dy) <= 54f && Mathf.Max(dx, dy) >= 42f;
            bool inner = Mathf.Max(dx, dy) < 42f;
            bool glyph = false;

            switch (type)
            {
                case AmmoPowerupType.Scatter:
                    glyph = (dy <= 22f && dx <= 5f) || (Mathf.Abs(dx - dy) <= 4f && dy <= 26f);
                    break;
                case AmmoPowerupType.RapidFire:
                    glyph = (dx <= 5f && dy <= 30f) || (dx >= 14f && dx <= 22f && dy <= 30f);
                    break;
                case AmmoPowerupType.Pierce:
                    glyph = dx <= 8f && dy <= 34f;
                    break;
                case AmmoPowerupType.Laser:
                    glyph = dx <= 4f && dy <= 38f;
                    break;
                case AmmoPowerupType.Plasma:
                    glyph = Vector2.Distance(new Vector2(x, y), center) <= 22f;
                    break;
                case AmmoPowerupType.Burst:
                    glyph = dx + dy <= 32f;
                    break;
                case AmmoPowerupType.Homing:
                    glyph = dy <= 28f && dx <= 16f + (Mathf.Sin(y * 0.18f) * 8f);
                    break;
                case AmmoPowerupType.Wave:
                    glyph = Mathf.Abs(y - center.y - (Mathf.Sin((x - center.x) * 0.12f) * 14f)) <= 5f && dx <= 34f;
                    break;
                case AmmoPowerupType.Guard:
                    glyph = Mathf.Abs(Vector2.Distance(new Vector2(x, y), center) - 22f) <= 4f;
                    break;
                default:
                    glyph = dx <= 8f && dy <= 24f;
                    break;
            }

            if (glyph)
            {
                texture.SetPixel(x, y, Color.white);
            }
            else if (frame)
            {
                texture.SetPixel(x, y, core);
            }
            else if (inner)
            {
                texture.SetPixel(x, y, Color.Lerp(new Color(0.03f, 0.08f, 0.16f), core, 0.22f));
            }
            else
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
    }

    texture.Apply();
    return texture;
}

public static Color GetWeaponColor(AmmoPowerupType type)
{
    switch (type)
    {
        case AmmoPowerupType.Scatter:
            return new Color(1f, 0.48f, 0.32f);
        case AmmoPowerupType.RapidFire:
            return new Color(1f, 0.9f, 0.28f);
        case AmmoPowerupType.Pierce:
            return new Color(0.45f, 0.95f, 1f);
        case AmmoPowerupType.Laser:
            return new Color(0.35f, 1f, 0.55f);
        case AmmoPowerupType.Plasma:
            return new Color(0.7f, 0.38f, 1f);
        case AmmoPowerupType.Burst:
            return new Color(1f, 0.22f, 0.34f);
        case AmmoPowerupType.Homing:
            return new Color(0.28f, 0.72f, 1f);
        case AmmoPowerupType.Wave:
            return new Color(1f, 0.42f, 0.82f);
        case AmmoPowerupType.Guard:
            return new Color(0.58f, 1f, 0.78f);
        default:
            return new Color(1f, 0.86f, 0.32f);
    }
}
```

- [ ] **Step 3: Use ammo-pack sprites in `BlockSpawner`**

In `SpawnAmmoPackAtPosition`, replace:

```csharp
renderer.sprite = RuntimeSpriteFactory.GetCircleSprite();
```

with:

```csharp
renderer.sprite = RuntimeSpriteFactory.GetAmmoPackSprite(type);
```

Replace `GetAmmoPackColor` body with:

```csharp
private Color GetAmmoPackColor(AmmoPowerupType type)
{
    return RuntimeSpriteFactory.GetWeaponColor(type);
}
```

Replace `GetAmmoPackLabel` body with:

```csharp
private static string GetAmmoPackLabel(AmmoPowerupType type)
{
    switch (type)
    {
        case AmmoPowerupType.Scatter:
            return "S";
        case AmmoPowerupType.RapidFire:
            return "R";
        case AmmoPowerupType.Pierce:
            return "P";
        case AmmoPowerupType.Laser:
            return "L";
        case AmmoPowerupType.Plasma:
            return "O";
        case AmmoPowerupType.Burst:
            return "B";
        case AmmoPowerupType.Homing:
            return "H";
        case AmmoPowerupType.Wave:
            return "W";
        case AmmoPowerupType.Guard:
            return "G";
        default:
            return "N";
    }
}
```

- [ ] **Step 4: Make pack label readable over icons**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPackController.cs`, replace:

```csharp
labelText.color = new Color(0.31f, 0.18f, 0.22f);
```

with:

```csharp
labelText.color = Color.white;
```

- [ ] **Step 5: Run validation and EditMode tests**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: all pass.

- [ ] **Step 6: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/RuntimeSpriteFactory.cs Assets/Scripts/Runtime/BlockSpawner.cs Assets/Scripts/Runtime/AmmoPackController.cs
git commit -m "feat: add arcade bullet and ammo pack sprites"
```

Skip if this is not a git repository.

### Task 5: Wire Ten Player Weapon Modes

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/PlayerController.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/GameManager.cs`

- [ ] **Step 1: Expose weapon level from `GameManager`**

If Task 2 did not already add this property, add it near `ActivePowerupType`:

```csharp
public int ActivePowerupLevel => activePowerup.Level;
```

- [ ] **Step 2: Replace `PlayerController` firing logic**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/PlayerController.cs`, replace the methods from `Fire()` through `FireSingleShot(...)` with:

```csharp
private void Fire()
{
    AmmoPowerupType type = gameManager.HasActivePowerup ? gameManager.ActivePowerupType : AmmoPowerupType.Normal;
    int level = Mathf.Max(1, gameManager.ActivePowerupLevel);

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
```

- [ ] **Step 3: Wire BOMB keyboard input**

In `PlayerController.Update()`, after `UpdateTargetPosition();`, add:

```csharp
if (Input.GetKeyDown(KeyCode.Space))
{
    gameManager.TryActivateBomb();
}
```

- [ ] **Step 4: Run validation and EditMode tests**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: all pass.

- [ ] **Step 5: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/PlayerController.cs Assets/Scripts/Runtime/GameManager.cs
git commit -m "feat: add ten player weapon modes"
```

Skip if this is not a git repository.

### Task 6: Expand Powerup Drops and HUD Text

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockSpawner.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPackController.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`

- [ ] **Step 1: Update guaranteed drops in `BuildStageScript`**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockSpawner.cs`, replace the three existing guaranteed drops so each wave introduces new weapons:

```csharp
new EnemySpawnInstruction(5.8f, EnemyFormationType.SnakeSweep, 5, false, AmmoPowerupType.Scatter),
```

keep as Scatter.

```csharp
new EnemySpawnInstruction(5.5f, EnemyFormationType.VShape, 5, false, AmmoPowerupType.RapidFire),
```

keep as RapidFire.

```csharp
new EnemySpawnInstruction(1.2f, EnemyFormationType.DiveLine, 1, true, AmmoPowerupType.Pierce),
```

keep as Pierce.

Then add two more guaranteed drops in the Pressure and Elite arrays:

```csharp
new EnemySpawnInstruction(9.2f, EnemyFormationType.SnakeSweep, 4, true, AmmoPowerupType.Laser)
```

inside the Pressure wave instruction array after the `7.9f` instruction, with a comma after the prior entry.

```csharp
new EnemySpawnInstruction(4.8f, EnemyFormationType.VShape, 3, true, AmmoPowerupType.Homing)
```

inside the Elite wave instruction array after the `2.8f` instruction, with a comma after the prior entry.

- [ ] **Step 2: Add random drop pool**

In `BlockSpawner`, add this helper near `GetAmmoPackLabel`:

```csharp
private static AmmoPowerupType GetRandomPowerupType()
{
    AmmoPowerupType[] pool =
    {
        AmmoPowerupType.Scatter,
        AmmoPowerupType.RapidFire,
        AmmoPowerupType.Pierce,
        AmmoPowerupType.Laser,
        AmmoPowerupType.Plasma,
        AmmoPowerupType.Burst,
        AmmoPowerupType.Homing,
        AmmoPowerupType.Wave,
        AmmoPowerupType.Guard
    };

    return pool[Random.Range(0, pool.Length)];
}
```

Use this helper only if this same task adds random drops. The current code path uses guaranteed drops through `EnemySpawnInstruction`, so this task does not need to modify `BlockController` drop ownership.

- [ ] **Step 3: Update pack display names**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/AmmoPackController.cs`, replace `GetDisplayName` with:

```csharp
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
```

- [ ] **Step 4: Update HUD powerup text and bomb display**

In `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`, replace `BuildPowerupHudText()` with:

```csharp
private string BuildPowerupHudText()
{
    if (!gameManager.HasActivePowerup)
    {
        return $"火力 普通\nBOMB {gameManager.BombCount}";
    }

    string name;
    switch (gameManager.ActivePowerupType)
    {
        case AmmoPowerupType.Scatter:
            name = "散射";
            break;
        case AmmoPowerupType.RapidFire:
            name = "连发";
            break;
        case AmmoPowerupType.Pierce:
            name = "穿透";
            break;
        case AmmoPowerupType.Laser:
            name = "激光";
            break;
        case AmmoPowerupType.Plasma:
            name = "等离子";
            break;
        case AmmoPowerupType.Burst:
            name = "爆裂";
            break;
        case AmmoPowerupType.Homing:
            name = "追踪";
            break;
        case AmmoPowerupType.Wave:
            name = "波刃";
            break;
        case AmmoPowerupType.Guard:
            name = "护航";
            break;
        default:
            name = "普通";
            break;
    }

    return $"火力 {name} Lv{gameManager.ActivePowerupLevel} {gameManager.ActivePowerupRemainingSeconds:0.0}s\nBOMB {gameManager.BombCount}";
}
```

- [ ] **Step 5: Add BOMB button**

In `UIController.Build()`, after the pause button setup, add:

```csharp
Button bombButton = UiFactory.CreateButton(rightCell.transform, "BOMB", new Color(0.22f, 0.78f, 1f, 0.96f), Color.white, new Vector2(200f, 72f), new Vector2(0f, -74f), new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f));
bombButton.onClick.AddListener(() => gameManager.TryActivateBomb());
Text bombButtonText = bombButton.GetComponentInChildren<Text>();
bombButtonText.fontStyle = FontStyle.Bold;
bombButtonText.fontSize = 30;
```

If this overlaps pause at runtime, reduce `pauseButton` width to `150f` and place BOMB at `new Vector2(78f, 0f)` with anchor `new Vector2(0.28f, 0.16f)`.

- [ ] **Step 6: Run validation and EditMode tests**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: all pass.

- [ ] **Step 7: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/BlockSpawner.cs Assets/Scripts/Runtime/AmmoPackController.cs Assets/Scripts/UI/UIController.cs
git commit -m "feat: expand weapon drops and hud"
```

Skip if this is not a git repository.

### Task 7: Add Scrolling Blue-Sky Background Layers

**Files:**
- Create: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/ScrollingBackgroundLayer.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Bootstrap/GameBootstrap.cs`

- [ ] **Step 1: Create scrolling layer component**

Create `/Users/mark/work/wanwan/Assets/Scripts/Runtime/ScrollingBackgroundLayer.cs`:

```csharp
using UnityEngine;

namespace Wanwan.Runtime
{
    public class ScrollingBackgroundLayer : MonoBehaviour
    {
        private float speed;
        private float wrapHeight;
        private Vector3 startPosition;

        public void Initialize(float scrollSpeed, float layerHeight)
        {
            speed = scrollSpeed;
            wrapHeight = layerHeight;
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.position += Vector3.down * (speed * Time.deltaTime);
            if (transform.position.y <= startPosition.y - wrapHeight)
            {
                transform.position += Vector3.up * wrapHeight;
            }
        }
    }
}
```

- [ ] **Step 2: Add sky/cloud sprite accessors**

In `RuntimeSpriteFactory`, add:

```csharp
public static Sprite GetSkyBackgroundSprite()
{
    return GetOrCreate("sky-background", BuildSkyBackgroundTexture);
}

public static Sprite GetCloudLayerSprite()
{
    return GetOrCreate("cloud-layer", BuildCloudLayerTexture);
}

public static Sprite GetCloudStreakSprite()
{
    return GetOrCreate("cloud-streak-layer", BuildCloudStreakTexture);
}
```

- [ ] **Step 3: Add sky/cloud texture builders**

Add these methods before `BuildBattlefieldBackgroundTexture()`:

```csharp
private static Texture2D BuildSkyBackgroundTexture()
{
    const int width = 512;
    const int height = 1024;
    Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

    for (int y = 0; y < height; y++)
    {
        float v = y / (float)(height - 1);
        Color row = Color.Lerp(new Color(0.43f, 0.78f, 1f), new Color(0.1f, 0.42f, 0.82f), v);
        for (int x = 0; x < width; x++)
        {
            float u = x / (float)(width - 1);
            float glow = Mathf.PerlinNoise(u * 2.4f, v * 3.2f) * 0.08f;
            texture.SetPixel(x, y, Color.Lerp(row, Color.white, glow));
        }
    }

    texture.Apply();
    return texture;
}

private static Texture2D BuildCloudLayerTexture()
{
    const int width = 512;
    const int height = 1024;
    Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

    for (int y = 0; y < height; y++)
    {
        float v = y / (float)(height - 1);
        for (int x = 0; x < width; x++)
        {
            float u = x / (float)(width - 1);
            float noise = Mathf.PerlinNoise((u * 4.5f) + 1.7f, (v * 8.2f) + 0.4f);
            float alpha = Mathf.InverseLerp(0.54f, 0.86f, noise) * 0.58f;
            texture.SetPixel(x, y, new Color(0.92f, 0.97f, 1f, alpha));
        }
    }

    texture.Apply();
    return texture;
}

private static Texture2D BuildCloudStreakTexture()
{
    const int width = 512;
    const int height = 1024;
    Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

    for (int y = 0; y < height; y++)
    {
        float v = y / (float)(height - 1);
        for (int x = 0; x < width; x++)
        {
            float u = x / (float)(width - 1);
            float stripe = Mathf.Abs(Mathf.Sin((v * 34f) + (u * 4f)));
            float noise = Mathf.PerlinNoise((u * 8f) + 2.1f, (v * 18f) + 3.5f);
            float alpha = stripe > 0.93f && noise > 0.38f ? 0.22f : 0f;
            texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
        }
    }

    texture.Apply();
    return texture;
}
```

- [ ] **Step 4: Replace backdrop creation in `GameBootstrap`**

Replace `CreateBattlefieldBackdrop(orthographicSize, horizontalExtent);` with:

```csharp
CreateScrollingSkyBackdrop(orthographicSize, horizontalExtent);
```

Replace the entire `CreateBattlefieldBackdrop` method with:

```csharp
private static void CreateScrollingSkyBackdrop(float orthographicSize, float horizontalExtent)
{
    float targetWidth = (horizontalExtent * 2f) + 3f;
    float targetHeight = (orthographicSize * 2f) + 3f;
    CreateBackgroundLayer("SkyBaseA", RuntimeSpriteFactory.GetSkyBackgroundSprite(), -60, 0.35f, targetWidth, targetHeight, 0f);
    CreateBackgroundLayer("SkyBaseB", RuntimeSpriteFactory.GetSkyBackgroundSprite(), -60, 0.35f, targetWidth, targetHeight, targetHeight);
    CreateBackgroundLayer("CloudLayerA", RuntimeSpriteFactory.GetCloudLayerSprite(), -55, 0.9f, targetWidth, targetHeight, 0f);
    CreateBackgroundLayer("CloudLayerB", RuntimeSpriteFactory.GetCloudLayerSprite(), -55, 0.9f, targetWidth, targetHeight, targetHeight);
    CreateBackgroundLayer("CloudStreakA", RuntimeSpriteFactory.GetCloudStreakSprite(), -54, 1.8f, targetWidth, targetHeight, 0f);
    CreateBackgroundLayer("CloudStreakB", RuntimeSpriteFactory.GetCloudStreakSprite(), -54, 1.8f, targetWidth, targetHeight, targetHeight);
}

private static void CreateBackgroundLayer(string name, Sprite sprite, int sortingOrder, float speed, float targetWidth, float targetHeight, float yOffset)
{
    GameObject backgroundObject = new GameObject(name);
    backgroundObject.transform.position = new Vector3(0f, yOffset, 8f);

    SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
    renderer.sprite = sprite;
    renderer.sortingOrder = sortingOrder;

    Vector2 spriteSize = renderer.sprite.bounds.size;
    backgroundObject.transform.localScale = new Vector3(targetWidth / spriteSize.x, targetHeight / spriteSize.y, 1f);
    backgroundObject.AddComponent<ScrollingBackgroundLayer>().Initialize(speed, targetHeight);
}
```

- [ ] **Step 5: Update camera background**

In `Awake`, change:

```csharp
Camera cameraComponent = EnsureCamera(new Color(0.03f, 0.06f, 0.14f));
```

to:

```csharp
Camera cameraComponent = EnsureCamera(new Color(0.38f, 0.72f, 1f));
```

- [ ] **Step 6: Run validation and EditMode tests**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected: all pass.

- [ ] **Step 7: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/ScrollingBackgroundLayer.cs Assets/Scripts/Runtime/RuntimeSpriteFactory.cs Assets/Scripts/Bootstrap/GameBootstrap.cs
git commit -m "feat: add scrolling sky background"
```

Skip if this is not a git repository.

### Task 8: Tune Enemy Pressure and Final Verification

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockController.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockSpawner.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`

- [ ] **Step 1: Increase enemy fire variety in `BlockController`**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockController.cs`, find the enemy firing method that calls `SpawnEnemyMissile`. Keep the existing branches and add this elapsed-time pressure branch before the final single-shot fallback:

```csharp
if (gameManager.ElapsedTime > 18f && Random.value < 0.35f)
{
    blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(-0.28f, -1f), new Color(1f, 0.42f, 0.34f));
    blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.46f), Vector2.down, new Color(1f, 0.58f, 0.3f));
    blockSpawner.SpawnEnemyMissile(transform.position + (Vector3.down * 0.42f), new Vector2(0.28f, -1f), new Color(1f, 0.42f, 0.34f));
    return;
}
```

- [ ] **Step 2: Make boss bullets visually distinct**

In `/Users/mark/work/wanwan/Assets/Scripts/Runtime/BlockSpawner.cs`, inside `SpawnEnemyMissile`, replace:

```csharp
renderer.sprite = RuntimeSpriteFactory.GetMissileSprite();
```

with:

```csharp
renderer.sprite = RuntimeSpriteFactory.GetBulletSprite(fromBoss ? AmmoPowerupType.Burst : AmmoPowerupType.Normal);
```

Keep enemy missile colors warm by leaving the existing `renderer.color = color;`.

- [ ] **Step 3: Update pause helper copy**

In `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`, replace:

```csharp
helper.text = "左右拖动战机  自动开火  接取强化弹药包";
```

with:

```csharp
helper.text = "拖动战机  自动开火  收集火力包  BOMB清屏";
```

Replace the pause overlay powerup label text:

```csharp
UiFactory.CreateArcadeLabel(pauseCard.transform, "散射  连发  穿透\n弹药包可以直接吃到，也能用子弹打中触发", 28, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.34f), new Vector2(0.88f, 0.52f), Vector2.zero);
```

with:

```csharp
UiFactory.CreateArcadeLabel(pauseCard.transform, "红色主武器  蓝色追踪/导弹  紫色特殊强化\n重复拾取同类火力包可提升等级", 28, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 1f), FontStyle.Bold, new Vector2(0.12f, 0.34f), new Vector2(0.88f, 0.52f), Vector2.zero);
```

- [ ] **Step 4: Run full validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
/Applications/Unity/Hub/Editor/6000.0.58f2/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Expected:

- `Unity project validation passed.`
- Python unittest passes
- EditMode test XML reports no failures

- [ ] **Step 5: Build Android smoke if local Unity build tools are available**

Run:

```bash
/Users/mark/work/wanwan/scripts/android_smoke_test.sh
```

Expected: script completes without build errors. If Android build tools or Unity modules are missing, record the exact missing tool/module and continue with EditMode validation as the blocking check.

- [ ] **Step 6: Manual play-mode checklist**

Open the Unity project and verify:

```text
1. The battle background scrolls downward continuously with multiple cloud speeds.
2. The player ship remains readable over the bright sky.
3. At least ten weapon pack icons can be spawned or observed through wave/random drops.
4. Repeating the same pickup raises the HUD to Lv2/Lv3.
5. Space and the BOMB button consume one bomb and clear enemy bullets.
6. Boss bullets are larger/warmer than player bullets.
7. Later waves feel denser than early waves without becoming unreadable.
```

- [ ] **Step 7: Commit if available**

```bash
cd /Users/mark/work/wanwan
git status
git add Assets/Scripts/Runtime/BlockController.cs Assets/Scripts/Runtime/BlockSpawner.cs Assets/Scripts/UI/UIController.cs
git commit -m "feat: tune arcade shooter pressure"
```

Skip if this is not a git repository.
