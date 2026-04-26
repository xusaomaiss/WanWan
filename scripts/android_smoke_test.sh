#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ADB_BIN="${ADB_BIN:-$HOME/Library/Android/sdk/platform-tools/adb}"
APK_PATH="${APK_PATH:-$ROOT_DIR/Builds/Android/WanwanDropBlaster-release.apk}"
PACKAGE_NAME="${PACKAGE_NAME:-com.mark.wanwan.dropblaster}"
ACTIVITY_NAME="${ACTIVITY_NAME:-com.unity3d.player.UnityPlayerGameActivity}"

if [[ ! -x "$ADB_BIN" ]]; then
  echo "adb not found at $ADB_BIN"
  exit 1
fi

if [[ ! -f "$APK_PATH" ]]; then
  echo "APK not found at $APK_PATH"
  exit 1
fi

"$ADB_BIN" devices -l
"$ADB_BIN" install -r "$APK_PATH"
"$ADB_BIN" shell monkey -p "$PACKAGE_NAME" -c android.intent.category.LAUNCHER 1
sleep 5
"$ADB_BIN" shell dumpsys window | grep -E "mCurrentFocus|topResumedActivity" || true
echo "Smoke test command sequence completed."
