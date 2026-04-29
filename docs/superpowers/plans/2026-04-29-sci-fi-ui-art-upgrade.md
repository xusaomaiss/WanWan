# Sci-Fi UI Art Upgrade Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade the Unity 2D portrait shooter presentation to a unified blue/cyan sci-fi HUD style while preserving gameplay logic.

**Architecture:** Keep the current code-driven scene construction. Add a small sci-fi asset path layer, reuse `Resources.Load` fallback behavior, and update UI/bootstrap/effects scripts to consume unified resources and helpers.

**Tech Stack:** Unity 6000.3, C#, UGUI, `Resources` assets, EditMode NUnit tests.

---

### Task 1: Sci-Fi Resource Contract

**Files:**
- Create: `Assets/Scripts/Runtime/Visual/SciFiResourcePaths.cs`
- Modify: `Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
- Test: `Assets/Tests/EditMode/SciFiResourcePathsTests.cs`

- [ ] Write tests that assert required sci-fi resource path constants are non-empty and that bullet, pickup, explosion, HUD, and background sprites can be resolved with fallback.
- [ ] Add `SciFiResourcePaths` constants for `Resources/SciFi/...` keys and fallback keys already present under `Resources/UI` and `Resources/RaidenArt`.
- [ ] Add `RuntimeSpriteFactory` methods for sci-fi background, HUD, projectile, pickup, bomb, and explosion sprites.
- [ ] Run the focused EditMode tests and verify they pass.

### Task 2: Unified Buttons And Main Menu

**Files:**
- Modify: `Assets/Scripts/UI/UiFactory.cs`
- Modify: `Assets/Scripts/Bootstrap/MenuBootstrap.cs`
- Test: `Assets/Tests/EditMode/MenuBootstrapTests.cs`

- [ ] Add or reuse sci-fi wide/icon button helpers with sprite target graphics and non-blocking decoration children.
- [ ] Convert menu primary and side buttons to the same sci-fi helper.
- [ ] Assert `START`, `MAP`, and `EXIT` are interactable and have onClick listeners.

### Task 3: Game HUD And Pause Overlay

**Files:**
- Modify: `Assets/Scripts/UI/UIController.cs`
- Test: `Assets/Tests/EditMode/UIControllerLayoutTests.cs`

- [ ] Update the top HUD to show score, coins, bomb count, and fire level as first-class labels.
- [ ] Replace pause and upgrade buttons with the same sci-fi button helper.
- [ ] Keep existing health, combo, boss, and stage progress behavior intact.

### Task 4: Space Parallax Background

**Files:**
- Modify: `Assets/Scripts/Bootstrap/GameBootstrap.cs`
- Create: `Assets/Scripts/Runtime/Visual/ParallaxSpaceBackground.cs`
- Test: `Assets/Tests/EditMode/SciFiBackgroundTests.cs`

- [ ] Replace stage ground/cloud visual layer selection with space, nebula, near-star, and dust layers.
- [ ] Reuse `ScrollingBackgroundLayer` behavior so pause/intro speed multipliers continue to work.
- [ ] Assert at least three background layers are created and use sci-fi sprite resources.

### Task 5: Effects Polish

**Files:**
- Modify: `Assets/Scripts/Runtime/EffectsController.cs`
- Create: `Assets/Scripts/Runtime/Visual/ScreenShakeController.cs`
- Create: `Assets/Scripts/Runtime/Visual/ExplosionFlashController.cs`
- Create: `Assets/Scripts/Runtime/Visual/CoinMagnetEffect.cs`
- Test: `Assets/Tests/EditMode/SciFiEffectsTests.cs`

- [ ] Route shake through `ScreenShakeController` while preserving existing durations and quality budgets.
- [ ] Add explosion flash sprite emission to explosion and bomb effects.
- [ ] Add a lightweight coin magnet component that can be attached to coin pickup objects without changing reward accounting.

### Task 6: Game Over Styling And Full Verification

**Files:**
- Modify: `Assets/Scripts/Bootstrap/GameOverBootstrap.cs`
- Test: `Assets/Tests/EditMode/GameOverBootstrapTests.cs`

- [ ] Convert result screen background, panel, and buttons to sci-fi resources.
- [ ] Verify victory and failure flows still bind their existing actions.
- [ ] Run `python3 scripts/validate_unity_project.py`, `python3 -m unittest tests/test_automation_files.py`, and `./scripts/run_editmode_tests.sh`.

### Self-Review

- Covers requested UI surfaces: main menu, HUD, pause, result.
- Covers resource replacement: paths and factory methods for Resources now, Addressables-compatible keys later.
- Covers background and effects: parallax layers, shake, flash, coin magnet component.
- Scope remains presentation-only; scoring, combat, collision, stage progression, and purchases are not redesigned.
