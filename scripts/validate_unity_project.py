#!/usr/bin/env python3
from __future__ import annotations

import json
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]


def check(condition: bool, message: str) -> None:
    if not condition:
        raise SystemExit(message)


def main() -> None:
    manifest_path = ROOT / "Packages/manifest.json"
    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    deps = manifest.get("dependencies", {})
    check("com.unity.test-framework" in deps, "manifest.json missing test framework dependency")

    required_files = [
        "Assets/Scenes/Menu.unity",
        "Assets/Scenes/Game.unity",
        "Assets/Scenes/GameOver.unity",
        "Assets/Scripts/Bootstrap/MenuBootstrap.cs",
        "Assets/Scripts/Bootstrap/GameBootstrap.cs",
        "Assets/Scripts/Bootstrap/GameOverBootstrap.cs",
        "Assets/Scripts/Runtime/GameManager.cs",
        "Assets/Scripts/Runtime/PlayerController.cs",
        "Assets/Scripts/Runtime/BlockSpawner.cs",
        "Assets/Scripts/Runtime/BlockController.cs",
        "Assets/Scripts/Runtime/BulletController.cs",
        "Assets/Scripts/Runtime/PerformanceBudget.cs",
        "Assets/Resources/RaidenArt/Backgrounds/stage_01_countryside.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_02_city.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_03_coastline.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_04_ruins.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_05_wasteland.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_06_floating_continent.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_07_space_station.png",
        "Assets/Resources/RaidenArt/Backgrounds/stage_08_alien_base.png",
        "Assets/Resources/RaidenArt/Cinematics/menu_storm_title_ai.png",
        "Assets/Resources/MainMenu/Backgrounds/bg_start_screen_ai.png",
        "Assets/Resources/MainMenu/Buttons/button_start_game.png",
        "Assets/Resources/MainMenu/Buttons/button_exit.png",
        "Assets/Resources/MainMenu/Icons/icon_settings_large.png",
        "Assets/Resources/MainMenu/Icons/icon_leaderboard_large.png",
        "Assets/Resources/MainMenu/Icons/icon_ship_select_large.png",
        "Assets/Resources/RaidenArt/Cinematics/victory_supply_screen_ai.png",
        "Assets/Resources/RaidenArt/Cinematics/launch_weather_intro_ai.png",
        "Assets/Resources/RaidenArt/Ships/fighter_jet_128.png",
        "Assets/Resources/RaidenArt/Mounts/mount_missile_pod_ai.png",
        "Assets/Resources/RaidenArt/Mounts/mount_shield_emitter_ai.png",
        "Assets/Resources/RaidenArt/Effects/bullet_spread_arcade.png",
        "Assets/Resources/RaidenArt/Effects/bullet_laser_arcade.png",
        "Assets/Resources/RaidenArt/Effects/bullet_homing_arcade.png",
        "Assets/Resources/RaidenArt/Effects/bullet_burst_arcade.png",
        "Assets/Resources/RaidenArt/Effects/explosion_arcade.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_scatter_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_rapid_fire_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_pierce_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_laser_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_homing_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_burst_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_wave_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_plasma_ai.png",
        "Assets/Resources/RaidenArt/Pickups/ammo_pack_guard_ai.png",
        "Assets/Resources/RaidenArt/HUD/hud_top_frame.png",
        "Assets/Resources/RaidenArt/HUD/hud_bottom_frame.png",
        "Assets/Resources/RaidenArt/HUD/hud_hp_frame.png",
        "Assets/Resources/RaidenArt/HUD/hud_power_slot.png",
        "Assets/Resources/RaidenArt/HUD/hud_power_slot_active.png",
        "Assets/Resources/RaidenArt/HUD/hud_boss_warning.png",
        "Assets/Resources/RaidenArt/HUD/hud_warning_edge.png",
        "Assets/Resources/RaidenArt/HUD/hud_meter_glow.png",
        "Assets/Scripts/UI/UIController.cs",
        "Assets/Editor/BuildAutomation.cs",
        "scripts/install_unity_editor.sh",
        "scripts/build_android.sh",
        "scripts/build_android_release.sh",
        "scripts/build_ios.sh",
        "scripts/run_editmode_tests.sh",
        "scripts/android_smoke_test.sh",
    ]

    for rel_path in required_files:
        check((ROOT / rel_path).exists(), f"missing required file: {rel_path}")

    for stage in range(1, 9):
        for tile in range(3):
            rel_path = f"Assets/Resources/RaidenArt/GroundDetails/stage_{stage:02d}_detail_{tile:02d}.png"
            check((ROOT / rel_path).exists(), f"missing required file: {rel_path}")

    for frame in range(12):
        rel_path = f"Assets/Resources/RaidenArt/Effects/Explosions/explosion_frame_{frame:02d}.png"
        check((ROOT / rel_path).exists(), f"missing required file: {rel_path}")

    build_settings = (ROOT / "ProjectSettings/EditorBuildSettings.asset").read_text(encoding="utf-8")
    for scene in ("Assets/Scenes/Menu.unity", "Assets/Scenes/Game.unity", "Assets/Scenes/GameOver.unity"):
        check(scene in build_settings, f"build settings missing scene: {scene}")

    build_automation = (ROOT / "Assets/Editor/BuildAutomation.cs").read_text(encoding="utf-8")
    for token in ("BuildAndroidDebug", "BuildAndroidRelease", "BuildIOSXcodeProject", "RunEditModeTests", "SetApplicationIdentifier", "UIOrientation.Portrait"):
        check(token in build_automation, f"BuildAutomation.cs missing token: {token}")

    runtime_sprite_factory = (ROOT / "Assets/Scripts/Runtime/RuntimeSpriteFactory.cs").read_text(encoding="utf-8")
    for token in ("MenuStormTitleResourcePath", "RaidenFighterJetResourcePath", "MountMissilePodResourcePath", "MountShieldEmitterResourcePath", "GetGroundDetailResourcePaths", "GetArcadeExplosionFrameSprites", "HudDecorResourcePaths"):
        check(token in runtime_sprite_factory, f"RuntimeSpriteFactory.cs missing token: {token}")

    session_state = (ROOT / "Assets/Scripts/Runtime/SessionState.cs").read_text(encoding="utf-8")
    for token in ("VisualEffectsQuality", "SetVisualEffectsQuality", "wanwan.visual_effects_quality"):
        check(token in session_state, f"SessionState.cs missing token: {token}")

    game_over_bootstrap = (ROOT / "Assets/Scripts/Bootstrap/GameOverBootstrap.cs").read_text(encoding="utf-8")
    for token in ("VictorySupplyScreenResourcePath", "victory_supply_screen_ai"):
        check(token in game_over_bootstrap, f"GameOverBootstrap.cs missing token: {token}")

    print("Unity project validation passed.")


if __name__ == "__main__":
    try:
        main()
    except SystemExit:
        raise
    except Exception as exc:  # pragma: no cover
        raise SystemExit(str(exc)) from exc
