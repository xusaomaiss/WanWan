# 2026-04-25 Sky Arcade Shooter Core Design

## Summary

This design upgrades the current vertical shooter into a stronger classic arcade shooter core with a bright blue-sky flight fantasy. The game should feel like the player is piloting a fighter through a vertically scrolling airspace while enemy waves, denser bullets, weapon pickups, bombs, and a boss fight build pressure over time.

The confirmed direction is the `classic arcade core` option:

- Blue-sky/cloud high-speed flight background
- Higher-quality fighter, enemy, boss, bullet, and ammo-pack visuals
- Ten weapon/powerup types instead of the current three
- Weapon upgrades through repeated pickups
- Basic BOMB support for emergency screen-clearing
- Increasing enemy and bullet density using the current stage-one structure

This is not a full multi-stage rewrite. The existing single-stage flow remains the implementation base.

## Goals

- Make the battlefield immediately read as `a fighter flying through the sky`
- Add background variation through scrolling sky, cloud layers, and color changes
- Replace flat projectile and ammo-pack presentation with richer, distinct visuals
- Expand the weapon system to at least ten readable bullet/powerup types
- Add a basic BOMB mechanic: limited uses, clear enemy bullets, damage enemies, help survival
- Preserve the easy-to-start mobile controls and automatic shooting
- Increase challenge over time with denser enemy waves and bullet patterns

## Non-Goals

- No full multi-stage campaign in this round
- No full audio redesign
- No complex RPG-style weapon tree
- No manual-only shooting mode
- No complete Boss pattern scripting framework beyond targeted additions
- No 1:1 clone of any copyrighted arcade title

## Visual Direction

### Background

The chosen background direction is `blue sky / cloud high-speed flight`.

The battle scene should use layered vertical scrolling:

1. Far blue-sky base: slow scroll, broad color gradient
2. Mid cloud layer: medium scroll, soft cloud masses
3. Near cloud streak/shadow layer: faster scroll, gives speed and altitude

The background can use open web assets as candidates, but implementation should keep a procedural fallback. Candidate references found during design:

- `Sky Backdrop` on OpenGameArt by `bart`: strong painted-cloud quality, available under CC-BY 3.0 / CC-BY-SA / GPL options, requires attribution if used
- `Sky Backgrounds` on OpenGameArt by `Umplix`: CC0, no required credit, but lower resolution and less polished
- `Space Backgrounds 9` by `Rawdanitsu`: public-domain space fallback, not the chosen primary direction

Preferred asset policy:

- Use runtime/procedural generated sky sprites first for reliable builds
- If imported image assets are added later, place them under `Assets/Art/Backgrounds/`
- Record license and source in a credits file before shipping

### Player Fighter

The player craft should become a polished arcade fighter rather than a plain aircraft silhouette:

- Symmetrical top-down fighter shape
- Long nose and strong center fuselage
- Angular wings and tail fins
- Metallic blue/silver shading
- Bright cockpit glint
- Twin engine glow
- Clear outline at mobile scale

### Enemies and Boss

Small enemies should be fast, sharp, and numerous. Large enemies should look heavier and carry visible weapon pods. The boss should read as a wide, imposing heavy aircraft with a central core/cannon and side hardpoints.

### Bullets

Projectiles must be visually distinct enough that the player can read danger and power quickly:

- Player bullets use bright cyan, yellow, green, violet, magenta, or white cores
- Enemy bullets use warmer red/orange/pink tones where possible
- Boss bullets are larger or more saturated than regular enemy bullets
- Bullet sizes remain readable without covering too much play space

### Ammo Packs

Ammo packs should no longer be plain capsules with letters only. Each pack should use:

- A glowing square or hex-tech frame
- Distinct color family
- A compact letter/icon marker
- Small inner projectile symbol or energy shape
- Pulse animation on spawn/fall

## Weapon and Powerup Roster

The roster contains ten weapon states:

1. `Normal`: baseline yellow energy shot
2. `Scatter`: fan-shaped multi-shot
3. `RapidFire`: faster thin shots
4. `Pierce`: blue-white penetrating shot
5. `Laser`: narrow high-speed beam style
6. `Plasma`: larger energy orb shot
7. `Burst`: slower high-damage explosive shot
8. `Homing`: shots that lightly steer toward the nearest enemy
9. `Wave`: side-to-side wave/blade projectiles
10. `Guard`: temporary side drones or escort shots

Existing fire modes map cleanly:

- `Scatter` keeps the current fan pattern but gets richer visuals and upgrades
- `RapidFire` keeps the lower cooldown but gets thinner/brighter bullet visuals
- `Pierce` keeps multi-hit piercing behavior and gets a stronger blue-white sprite

New fire modes should start simple:

- `Laser`: fires tight twin/triple beams with high speed
- `Plasma`: fires larger slower orb projectiles with higher damage
- `Burst`: fires slower, heavier projectiles with visible blast style on hit
- `Homing`: periodically adjusts direction toward the closest active enemy
- `Wave`: applies sinusoidal horizontal motion while moving upward
- `Guard`: spawns two short-lived side shot emitters or side projectiles

## Weapon Upgrade Rules

Powerups should support repeated pickup upgrades without adding a complex equipment system.

Recommended state:

- Active weapon type
- Active weapon level, clamped from 1 to 3
- Remaining duration for temporary enhanced state

Pickup behavior:

- Picking a different weapon switches weapon type and sets level to 1
- Picking the same weapon raises level by 1 up to 3 and refreshes duration
- `Normal` remains the default fallback after powerup expiration

Upgrade examples:

- Scatter level 1: 3 shots
- Scatter level 2: 5 shots
- Scatter level 3: 5 shots plus stronger center shot
- RapidFire levels shorten cooldown and/or add twin shots
- Pierce levels increase pierce count or damage
- Laser levels add beam count
- Plasma/Burst levels increase projectile size or damage
- Homing levels increase steering strength or projectile count
- Wave levels increase blade count
- Guard levels extend side support or add a second side shot

## BOMB Design

The BOMB is a survival mechanic, not a constant damage button.

Behavior:

- Player starts each run with a small bomb count, recommended `3`
- Bomb count is shown in the HUD
- Triggering BOMB clears active enemy bullets
- It damages or destroys small enemies
- It deals limited damage to the boss
- It briefly grants relief but does not replace dodging

Controls:

- Mobile: visible HUD button near the lower-right safe area
- Desktop/testing: keyboard shortcut, recommended `Space`

Visual:

- Brief expanding sky shockwave or white-blue flash
- Enemy bullets fade/pop away
- Small hit text or screen pulse is acceptable

## Difficulty and Stage Flow

The existing stage-one wave flow remains the base. Challenge increases through:

- More frequent enemy fire later in the stage
- Wider bullet spreads from elite enemies
- Boss volleys that mix straight shots and angled patterns
- More tightly timed waves
- Powerup drops placed to encourage route memory

This supports the requested `easy to start, hard to master` structure:

- Movement stays simple
- Shooting remains automatic
- Bomb provides a clear emergency action
- Mastery comes from dodging, bomb timing, pickup routing, and wave memory

## Architecture

### RuntimeSpriteFactory

Keep visual sprite creation centralized here.

Expected additions:

- `GetBulletSprite(WeaponProjectileVisual visual)` or equivalent keyed sprite access
- Ten bullet texture builders or one parameterized builder
- Ten ammo-pack icon builders or one parameterized pack builder
- Upgraded blue-sky background textures and cloud-layer textures
- Updated hero fighter, enemy, elite, and boss silhouettes

The goal is to avoid scattering procedural texture code across controllers.

### PlayerController

The current `Fire()` switch grows into a clearer weapon dispatch.

Expected changes:

- Use weapon type and level from game state
- Spawn one or more projectiles per weapon mode
- Pass projectile behavior data to bullet controllers
- Trigger BOMB input through GameManager or a bomb service

### BulletController

The current bullet controller should support lightweight behavior variants.

Expected additions:

- Damage amount
- Pierce count remains
- Optional homing behavior
- Optional wave motion
- Optional lifetime/width scaling

Keep collision handling simple and avoid a large inheritance hierarchy unless behavior becomes hard to read.

### GameManager / Powerup State

The current `ActivePowerupState` should expand to track:

- Weapon type
- Weapon level
- Remaining duration
- Bomb count, or delegate bomb state to a small dedicated class

### BlockSpawner

Responsibilities:

- Spawn expanded ammo-pack types
- Assign pack colors/icons/labels
- Increase enemy bullet density with elapsed time and difficulty
- Add or tune fixed wave placements to support route memory

### UIController

Add HUD support for:

- Current weapon name/type
- Weapon level
- Remaining powerup duration
- Bomb count
- Bomb button on mobile

The HUD should stay readable over the brighter sky background.

## Data Flow

1. Enemy wave spawns from `BlockSpawner`
2. Enemy death may spawn an ammo pack with a specific weapon type
3. Player collects the pack through `AmmoPackController`
4. `GameManager` updates active weapon type, level, and duration
5. `PlayerController` reads active weapon state each firing tick
6. `PlayerController` spawns bullets with sprite, color, damage, and behavior data
7. `BulletController` moves, steers/waves if needed, and applies collision effects
8. BOMB input tells `GameManager` to consume a bomb and perform clear/damage effects
9. `UIController` reflects weapon, level, timer, score, and bomb count

## Error Handling and Asset Safety

- If imported background assets are unavailable, procedural backgrounds still render
- If an unknown weapon type is encountered, fall back to `Normal`
- If bomb count is zero, BOMB input should do nothing except optionally play a disabled UI feedback
- Sprite cache keys must include weapon/icon variant so sprites do not overwrite each other
- External assets require source/license notes before being considered shippable

## Testing Strategy

### EditMode / Unit Tests

Add focused tests where possible:

- Powerup level behavior: switch type, same-type upgrade, max level, expiration
- Bomb state: initial count, consume, no negative count
- Difficulty progression: enemy fire density values remain bounded

### Project Validation

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

### Manual Unity Smoke

Verify in play mode or Android smoke build:

- Background scrolls continuously and looks like forward flight
- Player fighter is readable over sky
- At least ten ammo-pack visuals can appear
- Repeated same-type pickup raises weapon level
- Each weapon type fires distinct projectiles
- BOMB clears enemy bullets and decrements count
- Boss remains playable and readable with denser bullets

## Implementation Boundaries

Recommended implementation order:

1. Add/adjust state tests for weapon levels and bomb count
2. Expand weapon enum/state without changing visuals yet
3. Add bullet visual variants and ammo-pack icons
4. Wire ten weapon firing modes in small groups
5. Add BOMB state and clear-screen behavior
6. Upgrade background to scrolling sky/cloud layers
7. Tune enemy/boss bullet density
8. Polish HUD readability and pack feedback

Each step should leave the game playable.
