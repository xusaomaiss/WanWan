# 2026-04-25 Raiden UI Refresh Design

## Summary

This design refreshes the game's presentation to strongly evoke the classic 1990s `Raiden` arcade feel while keeping the current vertical Android shooter structure and stage-one flow intact. The focus is on `battle HUD first`, then matching the player craft, enemy craft, boss craft, title screen, and result screen to the same visual language.

The new style direction is:

- `90s arcade metal panel + blue/purple neon`
- `heavy Raiden homage`
- `sci-fi fighter silhouettes instead of modern realistic carrier aircraft`
- `short, arcade-style labels instead of soft explanatory UI copy`

This is a UI and presentation redesign. Core gameplay rules remain mostly unchanged unless a visual change requires a small wiring update.

## Goals

- Make the combat screen feel immediately closer to classic `Raiden`
- Prioritize `HUD score strip`, `boss health bar`, and `warning text`
- Replace current realistic aircraft silhouettes with `retro sci-fi shooter` craft
- Unify menu, battle, and result screens into one coherent arcade presentation
- Preserve readability on a vertical Android screen

## Non-Goals

- No full gameplay rewrite
- No new stage structure beyond the already implemented stage-one flow
- No new input system
- No attempt to clone a specific copyrighted `Raiden` screen 1:1

## Visual Direction

### Core Palette

- Background metal: deep navy, midnight blue, charcoal
- Neon accents: electric blue, violet, cyan-white
- Alert accents: orange-red, magenta-red
- Text highlights: white, pale cyan

### Material Language

- HUD panels should look like arcade machine overlays or cockpit plating
- Strong framed compartments, bevel-like borders, glowing edges
- Minimal soft pastel surfaces
- Larger contrast between neutral metal base and hot alert colors

### Typography Feel

- Labels should use short arcade terms: `1P SCORE`, `HI SCORE`, `STAGE 1`, `WARNING`, `MISSION CLEAR`
- Numbers should feel more prominent than helper copy
- Existing Unity text can remain, but styling and phrasing should emulate arcade information density

## Scope

The redesign covers:

1. In-battle HUD
2. Boss warning presentation
3. Boss health UI
4. Player fighter sprite
5. Standard enemy fighter sprite
6. Elite enemy fighter sprite
7. Boss sprite presentation
8. Title/menu screen
9. Result screen

## Screen-Level Design

### 1. Battle HUD

The battle HUD becomes the main expression of the `Raiden` homage.

#### Layout

- Replace the current soft top card with a full-width segmented metal information strip
- Left compartment:
  - `1P SCORE`
  - current score in large digits
  - `HI SCORE`
- Center compartment:
  - `STAGE 1`
  - current power-up state
  - difficulty
- Right compartment:
  - player status
  - run status text when relevant
- Boss UI sits directly below the top information strip, not floating like a generic progress bar

#### Style

- Dark navy base panel
- Cyan and violet edge lighting
- Thin mechanical separators between compartments
- Score numbers brighter and larger than surrounding labels
- Reduced use of long Chinese helper phrases in the main HUD

#### Copy Changes

- `Score` -> `1P SCORE`
- `Best` -> `HI SCORE`
- `强化` can remain Chinese if desired, but should be paired with arcade-style short formatting
- `首关进度` should feel like a stage indicator rather than a soft progress note

### 2. Boss Warning Sequence

The boss entrance should feel more like an arcade danger event.

#### Behavior

- When boss phase begins, show a large center-top `WARNING` banner before or while the boss bar appears
- Secondary line can show `BOSS APPROACH`
- Existing stage banner system should be restyled to support this stronger warning mode

#### Style

- Large block text
- Orange-red core with bright outline
- Flashing or pulsing appearance
- Higher contrast and more aggressive scale than ordinary phase text

### 3. Boss Health Bar

The boss bar should become a dedicated danger target UI rather than a plain fill strip.

#### Layout

- Wide framed boss slot directly under the top HUD
- Boss codename centered in the frame
- Health energy fill uses orange-red or magenta-red
- Mechanical left/right end caps to make it look locked-in

#### Behavior

- Hidden when no boss is active
- Shown only in boss phase
- Transitions from `WARNING` state to live boss bar state

### 4. Player Fighter Style

The player craft should move away from realistic carrier aviation into arcade sci-fi hero fighter design.

#### Shape Language

- Long pointed nose
- Strong center fuselage
- Wide angular wings
- Symmetrical silhouette
- Twin engine glow at the rear
- Bright cockpit highlight at center

#### Color Direction

- Silver-blue body
- Cyan-white highlights
- Small red accent near nose or body center

#### Motion/Readability

- Craft must remain readable at small vertical-mobile scale
- Outer wing shape should clearly separate it from enemies even in motion

### 5. Standard Enemy Fighter

Standard enemies should read as mass-produced hostile sci-fi interceptors.

#### Shape Language

- Smaller, sharper, more aggressive silhouette
- Shorter body than player craft
- Pronounced forward points or wing claws

#### Color Direction

- Red-gray or purple-gray body
- Hot accents around cockpit or weapon ports

### 6. Elite Enemy Fighter

Elite enemies should look like an upgraded class, not just recolored standard enemies.

#### Shape Language

- Broader chassis
- Thicker wings or secondary fins
- More obvious weapon pods

#### Color Direction

- Dark violet or gunmetal base
- Brighter core highlights than standard enemies

### 7. Boss Craft

The boss should read as a classic stage-one sci-fi heavy assault craft.

#### Shape Language

- Very wide front silhouette
- Strong bilateral symmetry
- Large center cannon/core
- Clear side weapon structures

#### Style

- Dark metallic body with glowing core sections
- Bigger, heavier, more imposing than all other craft
- Must feel like a `stage boss`, not a scaled-up enemy

### 8. Menu Screen

The menu should become more like an arcade title/start screen.

#### Layout

- Strong logo/title area at top
- Hero composition featuring player fighter and enemy threat presence
- Difficulty selection as glowing framed buttons
- Less soft card layout, more machine-panel composition

#### Copy Direction

- `START MISSION`
- `DIFFICULTY SELECT`
- retain Chinese support where useful, but keep phrases shorter and more arcade-like

### 9. Result Screen

The result screen should split clearly by outcome.

#### Failure

- Headline: `MISSION FAILED`
- Red-purple warning atmosphere
- Score and retry framed like arcade result board

#### Victory

- Headline: `MISSION CLEAR`
- Blue-white celebratory glow
- Rating and next-stage teaser shown as arcade mission debrief

## Assets and Implementation Approach

### Sprite Strategy

Keep the current `RuntimeSpriteFactory` procedural approach, but redraw the ship silhouettes and UI motifs to fit the new direction. This avoids introducing a full external art pipeline while still allowing a dramatic visual shift.

### UI Strategy

Keep the existing Unity runtime-built UI approach through `UiFactory`, but recompose panels, labels, colors, and sizing rather than lightly recoloring the current widgets.

### Animation/Effects

Use restrained animation only where it serves the arcade feel:

- `WARNING` flash/pulse
- boss bar entrance emphasis
- stage banner glow
- optional HUD shimmer or blinking alert state

Avoid excessive motion that harms readability.

## Data / Interface Impact

### Existing Systems to Reuse

- `UIController` remains the central battle HUD builder
- `MenuBootstrap` remains the menu screen builder
- `GameOverBootstrap` remains the result screen builder
- `RuntimeSpriteFactory` remains the procedural visual factory
- `GameManager` remains the source of boss, score, difficulty, power-up, and stage state

### Likely Code Changes

- `UIController`
  - rebuild layout and visual styling of the in-battle HUD
  - add dedicated warning presentation styling
  - rework boss bar structure
- `RuntimeSpriteFactory`
  - replace player, enemy, elite, boss craft silhouettes
  - update projectile visuals if needed to better match the new fighter set
- `MenuBootstrap`
  - rebuild title composition and difficulty panel styling
- `GameOverBootstrap`
  - rebuild failed/clear result board styling

## Constraints

- Must remain legible on portrait mobile screens
- Must not hide too much active play area
- Must avoid exact copyrighted screen copying
- Must fit the current runtime-generated UI and sprite architecture

## Testing Plan

### Visual Validation

- Battle HUD reads clearly on phone screen
- Score, boss bar, and warning text are visible during active play
- Player fighter is visually distinct from enemy fighters
- Boss reads as a boss immediately on entry
- Menu/result pages feel visually consistent with battle HUD

### Functional Regression

- Score, high score, stage progress, and power-up text still update correctly
- Boss bar shows and hides at proper times
- Warning banner appears during boss approach
- Difficulty buttons still load the correct mode
- Result screen still reflects victory vs failure correctly

### Device Validation

- Rebuild Android APK
- Install on connected phone
- Verify combat readability under real touch play

## Risks

- Over-styling the HUD may reduce usable play space
- Strong homage may drift too close to direct imitation if not abstracted into original silhouettes
- Large warning text could obscure gameplay if timing is too long

## Recommendation

Implement in this order:

1. Rebuild `RuntimeSpriteFactory` ship silhouettes
2. Rebuild battle HUD in `UIController`
3. Restyle boss warning and boss bar
4. Restyle `MenuBootstrap`
5. Restyle `GameOverBootstrap`
6. Rebuild and validate on Android device

## Open Decisions Already Resolved

- Style target: `90s arcade metal panel + blue/purple neon`
- Priority area: `battle HUD, boss bar, warning text`
- Aircraft direction: `sci-fi fighter silhouettes`
- Homage intensity: `heavy Raiden homage`

## Explicit Assumptions

- The redesign should reference the classic `Raiden` feel strongly, but not duplicate an exact copyrighted interface layout
- Chinese labels may remain in some places, but core battle UI should lean on short arcade-form labels
- Existing gameplay systems stay in place unless small supporting changes are needed for the visual presentation
