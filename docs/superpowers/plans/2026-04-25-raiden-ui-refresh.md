# Raiden UI Refresh Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Rebuild the game's presentation so the battle HUD, boss UI, warning text, ships, menu, and result screen feel like a strong 1990s `Raiden`-style arcade homage while preserving the current stage-one gameplay flow.

**Architecture:** Keep the current runtime-generated UI and procedural sprite pipeline, but replace the soft card layouts and realistic aircraft silhouettes with segmented arcade HUD panels and sci-fi fighter shapes. Implement the visual refresh in small vertical slices so battle readability stays intact while menu and result pages converge on the same metal-and-neon language.

**Tech Stack:** Unity 6000 C#, runtime-generated `UnityEngine.UI` layouts, procedural textures in `Texture2D`, Android build scripts, existing Python project validation scripts.

---

## File Structure

- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs`
  - Add a few reusable arcade-style panel/text helpers so the HUD, menu, and result screen can share one visual system.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`
  - Rebuild the in-battle HUD, warning text, and boss bar around the new arcade layout.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
  - Replace player, enemy, elite, boss, and projectile silhouettes with sci-fi arcade sprites and add reusable UI textures if needed.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Bootstrap/MenuBootstrap.cs`
  - Replace the current soft title card with a classic arcade title/start composition.
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Bootstrap/GameOverBootstrap.cs`
  - Restyle the result screen into mission-failed / mission-clear boards.
- Modify: `/Users/mark/work/wanwan/scripts/validate_unity_project.py`
  - Only if needed to account for any new files or validations; otherwise leave untouched.
- Test: `/Users/mark/work/wanwan/tests/test_automation_files.py`
  - Keep the existing automation smoke tests green.

### Task 1: Add Shared Arcade UI Building Blocks

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs`
- Test: `/Users/mark/work/wanwan/tests/test_automation_files.py`

- [ ] **Step 1: Write the failing usage target in the plan**

Add these helper signatures to `UiFactory` so later tasks can reuse them without duplicating panel styling logic:

```csharp
public static Image CreateArcadePanel(
    Transform parent,
    string name,
    Color fillColor,
    Color edgeColor,
    Vector2 anchorMin,
    Vector2 anchorMax,
    Vector2 inset);

public static Text CreateArcadeLabel(
    Transform parent,
    string content,
    int fontSize,
    TextAnchor alignment,
    Color color,
    FontStyle fontStyle,
    Vector2 anchorMin,
    Vector2 anchorMax,
    Vector2 anchoredPosition);
```

- [ ] **Step 2: Run the repo-level validation before changes**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

Expected: both commands pass, establishing the current baseline.

- [ ] **Step 3: Write the minimal helper implementation**

Add focused wrappers in `UiFactory.cs` that still build on the current `Image` and `Text` pipeline:

```csharp
public static Image CreateArcadePanel(
    Transform parent,
    string name,
    Color fillColor,
    Color edgeColor,
    Vector2 anchorMin,
    Vector2 anchorMax,
    Vector2 inset)
{
    Image root = CreatePanel(parent, name, edgeColor, anchorMin, anchorMax);
    RectTransform rootRect = root.rectTransform;
    rootRect.offsetMin = Vector2.zero;
    rootRect.offsetMax = Vector2.zero;

    Image inner = CreatePanel(root.transform, name + "Inner", fillColor, Vector2.zero, Vector2.one);
    inner.rectTransform.offsetMin = inset;
    inner.rectTransform.offsetMax = -inset;
    return root;
}

public static Text CreateArcadeLabel(
    Transform parent,
    string content,
    int fontSize,
    TextAnchor alignment,
    Color color,
    FontStyle fontStyle,
    Vector2 anchorMin,
    Vector2 anchorMax,
    Vector2 anchoredPosition)
{
    Text text = CreateText(parent, content, fontSize, alignment, color, anchorMin, anchorMax, anchoredPosition);
    text.fontStyle = fontStyle;
    return text;
}
```

- [ ] **Step 4: Re-run baseline validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

Expected: both commands pass with no new validation failures.

- [ ] **Step 5: Commit**

```bash
git add /Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs
git commit -m "feat: add arcade ui factory helpers"
```

If `git` is unavailable in this workspace, skip the commit and record that the workspace is not a repository.

### Task 2: Rebuild Player, Enemy, Elite, Boss, and Projectile Sprites

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
- Test: `/Users/mark/work/wanwan/scripts/validate_unity_project.py`

- [ ] **Step 1: Identify the current sprite entry points**

Keep these public methods intact so the rest of the game does not need invasive rewiring:

```csharp
public static Sprite GetMissileSprite()
public static Sprite GetFighterJetSprite()
public static Sprite GetPlaneSprite()
```

Add new internal builders if the existing enemy/boss callers already tint by role and can share one silhouette.

- [ ] **Step 2: Replace the hero fighter silhouette**

Update `BuildFighterJetTexture()` to use a sharper arcade hero craft silhouette:

```csharp
bool fuselage = dx <= 14f && y >= 24f && y <= 170f;
bool nose = y > 170f && y <= 206f && dx <= Mathf.Lerp(2f, 14f, Mathf.InverseLerp(206f, 170f, y));
bool centerBlade = dx <= 6f && y >= 48f && y <= 182f;
bool mainWing = y >= 88f && y <= 128f && dx <= 84f;
bool wingTip = y >= 68f && y <= 132f && dx > 84f && dx <= 112f && y >= 68f + ((dx - 84f) * 0.8f);
bool engineGlow = dx >= 10f && dx <= 24f && y >= 18f && y <= 34f;
bool cockpit = dx <= 10f && y >= 126f && y <= 154f;
```

Color the body in cool silver-blue shades and keep the cockpit brighter than the hull.

- [ ] **Step 3: Replace the missile silhouette**

Update `BuildMissileTexture()` so projectiles feel like arcade sci-fi shots rather than modern missiles:

```csharp
bool core = dx <= 10f && y >= 36f && y <= 194f;
bool nose = y > 194f && y <= 228f && dx <= Mathf.Lerp(1f, 10f, Mathf.InverseLerp(228f, 194f, y));
bool fins = y >= 72f && y <= 126f && dx >= 18f && dx <= 42f;
bool tailFlare = dx <= 8f && y >= 12f && y <= 30f;
```

Use brighter tip and engine colors so bullets read clearly against the battlefield background.

- [ ] **Step 4: Add role-specific silhouette builders if needed**

If the current codebase already relies on tinting alone, add internal helpers without breaking public callers:

```csharp
private static Texture2D BuildEnemyInterceptorTexture()
private static Texture2D BuildEliteInterceptorTexture()
private static Texture2D BuildBossFlagshipTexture()
```

If there are already role-based methods later in the file, update those bodies instead of adding duplicate public entry points.

- [ ] **Step 5: Re-run project validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
```

Expected: `Unity project validation passed.`

- [ ] **Step 6: Commit**

```bash
git add /Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs
git commit -m "feat: redraw sprites with raiden-style silhouettes"
```

If `git` is unavailable in this workspace, skip the commit and record that the workspace is not a repository.

### Task 3: Rebuild the In-Battle HUD and Boss UI

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs`
- Test: `/Users/mark/work/wanwan/scripts/validate_unity_project.py`

- [ ] **Step 1: Preserve the current data contract**

Keep these `GameManager` reads intact so the visual pass does not break the battle state wiring:

```csharp
gameManager.Score
gameManager.Lives
SessionState.HighScore
gameManager.StageProgress
gameManager.HasBoss
gameManager.BossName
gameManager.BossHealthNormalized
gameManager.StageBannerText
gameManager.HasActivePowerup
gameManager.ActivePowerupType
gameManager.ActivePowerupRemainingSeconds
```

- [ ] **Step 2: Replace the top HUD layout with segmented arcade compartments**

Restructure `Build()` so the top of the screen uses a single metal strip with three compartments:

```csharp
Image topHud = UiFactory.CreateArcadePanel(
    canvas.transform,
    "TopHud",
    new Color(0.05f, 0.09f, 0.2f, 0.94f),
    new Color(0.16f, 0.55f, 0.95f, 0.95f),
    new Vector2(0f, 0.87f),
    new Vector2(1f, 1f),
    new Vector2(10f, 10f));

Image leftCell = UiFactory.CreatePanel(topHud.transform, "LeftCell", new Color(0.08f, 0.1f, 0.2f, 0.82f), new Vector2(0.01f, 0.08f), new Vector2(0.32f, 0.92f));
Image centerCell = UiFactory.CreatePanel(topHud.transform, "CenterCell", new Color(0.08f, 0.1f, 0.2f, 0.82f), new Vector2(0.34f, 0.08f), new Vector2(0.66f, 0.92f));
Image rightCell = UiFactory.CreatePanel(topHud.transform, "RightCell", new Color(0.08f, 0.1f, 0.2f, 0.82f), new Vector2(0.68f, 0.08f), new Vector2(0.99f, 0.92f));
```

- [ ] **Step 3: Update battle copy to arcade labels**

Replace the existing labels with short arcade-style text:

```csharp
scoreText.text = $"1P SCORE\n{gameManager.Score:0000000}";
highScoreText.text = $"HI SCORE\n{SessionState.HighScore:0000000}";
difficultyText.text = $"LEVEL {BuildDifficultyText()}";
stageProgressText.text = $"STAGE 1  {(gameManager.StageProgress * 100f):0}%";
pauseHintText.text = gameManager.IsPaused ? "PAUSE" : string.Empty;
```

Keep the power-up line short and compressed, for example:

```csharp
return $"POWER {name} {gameManager.ActivePowerupRemainingSeconds:0.0}s";
```

- [ ] **Step 4: Convert the boss bar into a danger-target frame**

Rebuild the boss bar so it is wider, framed, and mechanically segmented:

```csharp
bossBarRoot = UiFactory.CreateArcadePanel(
    canvas.transform,
    "BossBarRoot",
    new Color(0.14f, 0.03f, 0.08f, 0.92f),
    new Color(1f, 0.32f, 0.22f, 0.95f),
    new Vector2(0.06f, 0.81f),
    new Vector2(0.94f, 0.855f),
    new Vector2(8f, 8f));
```

Use a bright orange-red fill and ensure `RefreshHud()` still updates `bossBarFill.rectTransform.anchorMax` from `gameManager.BossHealthNormalized`.

- [ ] **Step 5: Restyle stage warning text**

Use the existing `stageBannerText` object, but make it read like a `WARNING` / `BOSS APPROACH` alert:

```csharp
stageBannerText = UiFactory.CreateArcadeLabel(
    canvas.transform,
    string.Empty,
    86,
    TextAnchor.MiddleCenter,
    new Color(1f, 0.38f, 0.22f, 0.98f),
    FontStyle.Bold,
    new Vector2(0.1f, 0.64f),
    new Vector2(0.9f, 0.76f),
    Vector2.zero);
```

If the banner text contains `WARNING`, give it a stronger alert color in `RefreshHud()`.

- [ ] **Step 6: Re-run validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
```

Expected: validation passes and the scene still references valid scripts.

- [ ] **Step 7: Commit**

```bash
git add /Users/mark/work/wanwan/Assets/Scripts/UI/UIController.cs /Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs
git commit -m "feat: rebuild battle hud with arcade boss ui"
```

If `git` is unavailable in this workspace, skip the commit and record that the workspace is not a repository.

### Task 4: Restyle the Menu Screen into an Arcade Title Screen

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Bootstrap/MenuBootstrap.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`
- Test: `/Users/mark/work/wanwan/scripts/validate_unity_project.py`

- [ ] **Step 1: Keep the existing difficulty entry wiring**

Do not change the scene-loading actions:

```csharp
lowButton.onClick.AddListener(() => SceneNavigator.LoadGame(GameDifficulty.Low));
mediumButton.onClick.AddListener(() => SceneNavigator.LoadGame(GameDifficulty.Medium));
highButton.onClick.AddListener(() => SceneNavigator.LoadGame(GameDifficulty.High));
```

- [ ] **Step 2: Replace the soft hero card with an arcade title composition**

Update `BuildMenu()` to use a darker sci-fi background and title framing:

```csharp
Image background = UiFactory.CreatePanel(canvas.transform, "Background", new Color(0.02f, 0.04f, 0.11f), Vector2.zero, Vector2.one);
Image titleFrame = UiFactory.CreateArcadePanel(background.transform, "TitleFrame", new Color(0.05f, 0.08f, 0.16f, 0.92f), new Color(0.24f, 0.62f, 1f, 0.96f), new Vector2(0.05f, 0.58f), new Vector2(0.95f, 0.92f), new Vector2(10f, 10f));
```

Change the headline to arcade-flavored copy such as:

```csharp
"WANWAN RAIDEN"
"START MISSION"
"DIFFICULTY SELECT"
```

- [ ] **Step 3: Add ship-vs-enemy composition**

Use the refreshed fighter sprite(s) as a title-screen composition:

```csharp
Image playerShip = UiFactory.CreatePanel(background.transform, "PlayerShipArt", Color.white, new Vector2(0.16f, 0.34f), new Vector2(0.46f, 0.58f));
playerShip.sprite = RuntimeSpriteFactory.GetFighterJetSprite();
playerShip.preserveAspect = true;

Image enemyShip = UiFactory.CreatePanel(background.transform, "EnemyShipArt", new Color(0.95f, 0.45f, 0.55f, 1f), new Vector2(0.56f, 0.34f), new Vector2(0.86f, 0.58f));
enemyShip.sprite = RuntimeSpriteFactory.GetPlaneSprite();
enemyShip.preserveAspect = true;
enemyShip.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
```

If separate enemy sprite methods already exist, use those instead of tinting the shared sprite.

- [ ] **Step 4: Rebuild difficulty buttons as glowing arcade selections**

Keep the buttons large, but use darker metal fills and brighter outlines via `UiFactory`.

- [ ] **Step 5: Re-run validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
```

Expected: validation passes.

- [ ] **Step 6: Commit**

```bash
git add /Users/mark/work/wanwan/Assets/Scripts/Bootstrap/MenuBootstrap.cs /Users/mark/work/wanwan/Assets/Scripts/Runtime/RuntimeSpriteFactory.cs
git commit -m "feat: restyle menu as arcade title screen"
```

If `git` is unavailable in this workspace, skip the commit and record that the workspace is not a repository.

### Task 5: Restyle the Result Screen into Mission Boards

**Files:**
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/Bootstrap/GameOverBootstrap.cs`
- Modify: `/Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs`
- Test: `/Users/mark/work/wanwan/scripts/validate_unity_project.py`

- [ ] **Step 1: Preserve the victory/failure branching**

Keep these current result inputs intact:

```csharp
bool victory = SessionState.LastRunWasVictory;
string title = victory ? "首关通关" : "本局结束";
string actionCopy = victory ? "再次出击" : "再来一局";
```

You may change the displayed strings, but keep the branch semantics unchanged.

- [ ] **Step 2: Replace the soft result card with mission status panels**

Rebuild `BuildGameOver()` so failure and victory use different palettes:

```csharp
Color edge = victory
    ? new Color(0.28f, 0.74f, 1f, 0.96f)
    : new Color(1f, 0.32f, 0.42f, 0.96f);

Image resultFrame = UiFactory.CreateArcadePanel(
    background.transform,
    "ResultFrame",
    new Color(0.05f, 0.07f, 0.15f, 0.94f),
    edge,
    new Vector2(0.07f, 0.14f),
    new Vector2(0.93f, 0.88f),
    new Vector2(12f, 12f));
```

- [ ] **Step 3: Replace result copy with arcade mission language**

Use:

```csharp
string title = victory ? "MISSION CLEAR" : "MISSION FAILED";
```

Retain score, high score, difficulty, and summary, but compress them into result-board style labels.

- [ ] **Step 4: Rebuild the action buttons**

Keep the same navigation wiring, but style the buttons like arcade control pads instead of soft rounded cards.

- [ ] **Step 5: Re-run validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
```

Expected: validation passes.

- [ ] **Step 6: Commit**

```bash
git add /Users/mark/work/wanwan/Assets/Scripts/Bootstrap/GameOverBootstrap.cs /Users/mark/work/wanwan/Assets/Scripts/UI/UiFactory.cs
git commit -m "feat: restyle result screen as mission board"
```

If `git` is unavailable in this workspace, skip the commit and record that the workspace is not a repository.

### Task 6: Verify the Full UI Refresh on Device

**Files:**
- Modify: `/Users/mark/work/wanwan/Builds/Android/WanwanDropBlaster.apk`
- Test: `/Users/mark/work/wanwan/scripts/build_android.sh`
- Test: `/Users/mark/work/wanwan/scripts/android_smoke_test.sh`

- [ ] **Step 1: Re-run local validation**

Run:

```bash
python3 /Users/mark/work/wanwan/scripts/validate_unity_project.py
python3 -m unittest /Users/mark/work/wanwan/tests/test_automation_files.py -v
```

Expected: all checks pass.

- [ ] **Step 2: Build the Android APK**

Run:

```bash
UNITY_BIN='/Applications/Unity/Hub/Editor/6000.3.7f1/Unity.app/Contents/MacOS/Unity' /Users/mark/work/wanwan/scripts/build_android.sh
```

Expected: Unity reports `Build Finished, Result: Success` and updates `/Users/mark/work/wanwan/Builds/Android/WanwanDropBlaster.apk`.

- [ ] **Step 3: Install and smoke-test on the connected phone**

Run:

```bash
/Users/mark/work/wanwan/scripts/android_smoke_test.sh
```

Expected: install succeeds, the app launches, and the resumed activity is `com.mark.wanwan.dropblaster/com.unity3d.player.UnityPlayerGameActivity`.

- [ ] **Step 4: Manual visual spot-check on device**

Confirm all of the following visually:

```text
1. The top HUD now looks like a segmented arcade control strip.
2. Boss appearance shows stronger WARNING-style text before or during boss UI reveal.
3. Boss bar feels framed and dangerous rather than generic.
4. Player and enemy ships now read as sci-fi arcade fighters.
5. Menu and result screens match the same metal + neon language.
```

- [ ] **Step 5: Commit**

```bash
git add /Users/mark/work/wanwan/Assets/Scripts /Users/mark/work/wanwan/Builds/Android/WanwanDropBlaster.apk
git commit -m "feat: ship raiden-inspired ui refresh"
```

If `git` is unavailable in this workspace, skip the commit and record that the workspace is not a repository.

## Self-Review

### Spec Coverage

- Battle HUD redesign: covered by Task 3
- Boss warning and boss bar redesign: covered by Task 3
- Sci-fi fighter silhouettes: covered by Task 2
- Arcade menu screen: covered by Task 4
- Arcade result screen: covered by Task 5
- Android rebuild and phone validation: covered by Task 6

### Placeholder Scan

This plan intentionally avoids `TODO`, `TBD`, and “implement later” placeholders. Every task includes exact file paths and concrete commands.

### Type Consistency

- `UiFactory.CreateArcadePanel` and `UiFactory.CreateArcadeLabel` are defined in Task 1 before later tasks use them.
- `GameManager` data reads referenced in Task 3 match the current property names already used by `UIController`.
- The plan preserves existing scene-loading and result-branch semantics while changing presentation only.
