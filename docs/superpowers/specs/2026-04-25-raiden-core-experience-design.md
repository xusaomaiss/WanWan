# Raiden Core Experience Enhancement Design

## Goal

Improve the current Unity vertical shooter by absorbing the strongest playable ideas from `/Users/mark/Downloads/仿雷电游戏开发计划.md`: clearer red/blue/purple weapon identity, stronger stage waves, multi-phase Boss pressure, and arcade visual feedback. This is an enhancement to the existing Unity project, not a rewrite to Kotlin or Android Canvas.

## Scope

This pass focuses on one shippable gameplay and visual upgrade:

- Weapon identity becomes easier to understand: red = scatter main weapon, blue = laser/homing missile, purple = special power.
- Ammo packs feel more like Raiden powerups by cycling their visual color/type while drifting.
- Enemy waves gain more varied pressure through existing formation infrastructure.
- Boss combat gains additional health phases and a more dramatic defeat.
- Feedback improves with score popups, stronger explosion debris, and a BOMB flash/shockwave.
- HUD text reports weapon category, level, BOMB count, stage progress, and kill target.

Out of scope for this pass:

- Rebuilding the app in native Android/Kotlin.
- Full 8-stage JSON level authoring.
- New audio/BGM authoring.
- Persistent leaderboard name entry.
- App store release assets.

## Architecture

Keep the existing runtime-generated Unity asset approach. Add small, focused runtime helpers only where current classes are carrying specific behavior:

- `AmmoPackController` owns drifting and cycling pickup identity.
- `BlockSpawner` owns stage wave scripting, enemy spawn type, Boss phase config, and pickup spawning.
- `BossController` owns Boss phase selection and firing patterns.
- `EffectsController` owns visual feedback such as explosions, score labels, BOMB flash, and shockwave.
- `UIController` owns HUD wording and overlay presentation.

The feature should not alter scene files unless necessary. Procedural sprites remain in `RuntimeSpriteFactory`.

## Gameplay Design

Ammo packs will cycle through a small compatible set while they are on screen. Guaranteed drops keep their starting category, but the visible item cycles every 0.8 seconds among related powerups:

- Red pool: `Scatter`, `RapidFire`, `Burst`
- Blue pool: `Laser`, `Homing`, `Pierce`
- Purple pool: `Plasma`, `Wave`, `Guard`

Picking up a pack activates the current displayed type. Repeated pickup of the same type continues to use `ActivePowerupState`'s existing leveling behavior.

Enemy waves remain a single first-stage script, but become denser and more varied. The current phases stay recognizable: preparation, assault, pressure, elite, Boss.

Boss phases increase from two to four bands: above 70%, 70%-40%, 40%-10%, and final 10%. Later phases fire faster and use wider spreads.

## Visual Feedback

Enemy death should feel less flat:

- Existing explosion sprite remains the core burst.
- Additional debris particles fly outward and fall slightly.
- A score popup appears at the destroyed enemy position.

BOMB should feel like a screen-clearing weapon:

- A brief full-screen white/cyan flash.
- A circular shockwave sprite or procedural ring expanding from screen center.
- Existing bullet clear and enemy damage behavior remains.

## Testing

Run the existing static and Unity checks after implementation:

- `python3 scripts/validate_unity_project.py`
- `python3 -m unittest tests/test_automation_files.py -v`
- `/Applications/Unity/Hub/Editor/6000.3.7f1/Unity.app/Contents/MacOS/Unity -batchmode -projectPath /Users/mark/work/wanwan -runTests -testPlatform EditMode -testResults /tmp/wanwan-editmode.xml -quit`

Add focused EditMode tests for any pure logic added, especially ammo pack cycle category selection and Boss phase configuration count.

## Acceptance Criteria

- The game still builds and starts on Android.
- Existing menu, carrier launch, kill clear, and victory flow keep working.
- Pickups visibly cycle and the HUD reflects the activated weapon type and level.
- Boss has four escalating phases.
- Destroying enemies produces stronger explosion/debris feedback and score popup.
- BOMB produces a clear flash/shockwave effect.
- No `.omc/` files are committed.
