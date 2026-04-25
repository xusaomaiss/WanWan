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
        "Assets/Resources/RaidenArt/Ships/fighter_jet_128.png",
        "Assets/Scripts/UI/UIController.cs",
        "Assets/Editor/BuildAutomation.cs",
        "scripts/install_unity_editor.sh",
        "scripts/build_android.sh",
        "scripts/build_android_release.sh",
        "scripts/run_editmode_tests.sh",
        "scripts/android_smoke_test.sh",
    ]

    for rel_path in required_files:
        check((ROOT / rel_path).exists(), f"missing required file: {rel_path}")

    build_settings = (ROOT / "ProjectSettings/EditorBuildSettings.asset").read_text(encoding="utf-8")
    for scene in ("Assets/Scenes/Menu.unity", "Assets/Scenes/Game.unity", "Assets/Scenes/GameOver.unity"):
        check(scene in build_settings, f"build settings missing scene: {scene}")

    build_automation = (ROOT / "Assets/Editor/BuildAutomation.cs").read_text(encoding="utf-8")
    for token in ("BuildAndroidDebug", "BuildAndroidRelease", "RunEditModeTests", "SetApplicationIdentifier", "UIOrientation.Portrait"):
        check(token in build_automation, f"BuildAutomation.cs missing token: {token}")

    print("Unity project validation passed.")


if __name__ == "__main__":
    try:
        main()
    except SystemExit:
        raise
    except Exception as exc:  # pragma: no cover
        raise SystemExit(str(exc)) from exc
