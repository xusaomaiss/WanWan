# Raiden Core Experience Enhancement Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make the existing Unity shooter feel closer to a Raiden-style arcade shooter through cycling powerups, stronger waves, four-phase Boss pressure, and punchier visual feedback.

**Architecture:** Keep procedural runtime assets and existing scene bootstraps. Add small pure helpers for powerup cycling and Boss phase definitions so EditMode tests can cover behavior without launching scenes. Keep visual effects in `EffectsController`.

**Tech Stack:** Unity 6000.3.7f1, C#, NUnit EditMode tests, existing Python validation scripts.

---

## File Structure

- Modify `Assets/Scripts/Runtime/AmmoPackController.cs`: cycle visible pickup type and collect the currently displayed type.
- Create `Assets/Scripts/Runtime/PowerupCycle.cs`: pure category/pool helper for red, blue, and purple pickup cycling.
- Modify `Assets/Scripts/Runtime/BlockSpawner.cs`: use cycling pack labels/colors, denser stage script, and four Boss phase configs.
- Modify `Assets/Scripts/Runtime/BossController.cs`: support phase-specific extra ring bursts in later phases.
- Modify `Assets/Scripts/Runtime/EffectsController.cs`: add score popup, debris burst, BOMB flash, and shockwave.
- Modify `Assets/Scripts/Runtime/GameManager.cs`: trigger BOMB visual and score popup on enemy/Boss events through existing calls.
- Modify `Assets/Scripts/UI/UIController.cs`: refine HUD copy for Raiden-like red/blue/purple identity.
- Create `Assets/Tests/EditMode/PowerupCycleTests.cs`: verify cycle pools and labels.
- Create `Assets/Tests/EditMode/BossPhaseConfigTests.cs`: verify each difficulty has four phases.

### Task 1: Powerup Cycle Logic

**Files:**
- Create: `Assets/Scripts/Runtime/PowerupCycle.cs`
- Create: `Assets/Scripts/Runtime/PowerupCycle.cs.meta`
- Create: `Assets/Tests/EditMode/PowerupCycleTests.cs`
- Create: `Assets/Tests/EditMode/PowerupCycleTests.cs.meta`
- Modify: `Assets/Scripts/Runtime/AmmoPackController.cs`
- Modify: `Assets/Scripts/Runtime/BlockSpawner.cs`

- [x] **Step 1: Write failing tests**

Add tests that assert `Scatter` cycles through `Scatter`, `RapidFire`, `Burst`; `Laser` cycles through `Laser`, `Homing`, `Pierce`; `Plasma` cycles through `Plasma`, `Wave`, `Guard`; labels are `S/R/B`, `L/H/P`, and `O/W/G`.

- [x] **Step 2: Implement pure helper**

Create `PowerupCycle` with:

```csharp
public static AmmoPowerupType GetTypeAt(AmmoPowerupType startingType, int index)
public static string GetLabel(AmmoPowerupType type)
public static Color GetCategoryColor(AmmoPowerupType type)
```

- [x] **Step 3: Wire pickup controller**

`AmmoPackController` stores a cycle index, advances every `0.8f` seconds, refreshes sprite/color/label, and calls `gameManager.ActivatePowerup(powerupType, DefaultDurationSeconds)` with the current type.

- [x] **Step 4: Run tests**

Run: `python3 scripts/validate_unity_project.py` and Unity EditMode after Task 2 or later.

### Task 2: Boss Four-Phase Pressure

**Files:**
- Create: `Assets/Tests/EditMode/BossPhaseConfigTests.cs`
- Create: `Assets/Tests/EditMode/BossPhaseConfigTests.cs.meta`
- Modify: `Assets/Scripts/Runtime/BlockSpawner.cs`
- Modify: `Assets/Scripts/Runtime/BossController.cs`
- Modify: `Assets/Scripts/Runtime/BossPhaseConfig.cs`

- [x] **Step 1: Write failing tests**

Expose Boss phase construction via an internal static method and test each difficulty returns four phases with trigger thresholds `0.7f`, `0.4f`, `0.1f`, `0f`.

- [x] **Step 2: Extend Boss phase config**

Add `ExtraRingShot` to `BossPhaseConfig` so late phases can add ring-style bursts.

- [x] **Step 3: Update phase arrays**

Low, medium, and high all return four phases. Later phases reduce fire interval, increase salvo count/spread, enable aimed core shot, and enable ring burst for the last two phases.

- [x] **Step 4: Update Boss firing**

When `ExtraRingShot` is true, Boss fires an additional short ring fan using `SpawnEnemyMissile` with warm yellow/cyan colors.

### Task 3: Arcade Feedback

**Files:**
- Modify: `Assets/Scripts/Runtime/EffectsController.cs`
- Modify: `Assets/Scripts/Runtime/BlockController.cs`
- Modify: `Assets/Scripts/Runtime/BossController.cs`
- Modify: `Assets/Scripts/Runtime/GameManager.cs`

- [x] **Step 1: Add score popup effect**

Create a `TextMesh` at the world position, animate it upward, fade it, and destroy after about `0.65f`.

- [x] **Step 2: Add explosion debris**

Extend `PlayBurst` to spawn small colored fragments with random velocity and fade.

- [x] **Step 3: Add BOMB screen effect**

Add `PlayBombDetonation(Vector3 center)` that creates a translucent flash plane plus expanding ring/shockwave and shakes the screen.

- [x] **Step 4: Wire gameplay calls**

`BlockController.ApplyHit` calls score popup when destroyed. `BossController.ResolveDefeat` calls score popup for Boss score. `GameManager.TryActivateBomb` calls `PlayBombDetonation`.

### Task 4: Wave/HUD Polish and Verification

**Files:**
- Modify: `Assets/Scripts/Runtime/BlockSpawner.cs`
- Modify: `Assets/Scripts/UI/UIController.cs`

- [x] **Step 1: Increase stage pressure carefully**

Add one extra formation in assault and pressure phases, and one extra elite drop in elite phase. Keep total stage length similar and maintain low difficulty playability.

- [x] **Step 2: Refine HUD copy**

Use red/blue/purple identity in powerup HUD text: `红色散射`, `蓝色激光`, `蓝色追踪`, `紫色波刃`, etc.

- [x] **Step 3: Run validation**

Run:

```bash
python3 scripts/validate_unity_project.py
python3 -m unittest tests/test_automation_files.py -v
/Applications/Unity/Hub/Editor/6000.3.7f1/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit
```

Result: static validation, Python automation, C# script compilation, Android build, install, launch, and gameplay screenshot passed. Unity EditMode CLI did not produce a test result XML under Unity 6000.3.7f1 in this environment, so it is recorded as runner-inconclusive rather than a passing test result.

- [x] **Step 4: Build and smoke test if validation passes**

Run:

```bash
scripts/build_android.sh
scripts/android_smoke_test.sh
```

## Self-Review

- Spec coverage: cycling pickups, Boss phases, feedback, HUD, and wave polish are all represented.
- Placeholder scan: no intentionally deferred implementation details remain.
- Type consistency: planned helper names match existing `AmmoPowerupType`, `BossPhaseConfig`, `BlockSpawner`, and `EffectsController` boundaries.
