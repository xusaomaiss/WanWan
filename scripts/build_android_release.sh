#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_BIN="${UNITY_BIN:-}"
KEYSTORE_PATH="${WANWAN_ANDROID_KEYSTORE:-$ROOT_DIR/Builds/Android/wanwan-release.keystore}"
KEYSTORE_PASS="${WANWAN_ANDROID_KEYSTORE_PASS:-wanwan-release-2026}"
KEY_ALIAS="${WANWAN_ANDROID_KEYALIAS:-wanwan-release}"
KEY_ALIAS_PASS="${WANWAN_ANDROID_KEYALIAS_PASS:-$KEYSTORE_PASS}"

if [[ -z "${UNITY_BIN}" ]]; then
  CANDIDATES=(
    "/Applications/Unity/Hub/Editor/6000.3.7f1/Unity.app/Contents/MacOS/Unity"
    "/Applications/Unity/Hub/Editor/6000.0.0f1/Unity.app/Contents/MacOS/Unity"
    "/Applications/Unity/Hub/Editor/2022.3.62f1/Unity.app/Contents/MacOS/Unity"
    "$HOME/Applications/Unity/Hub/Editor/6000.3.7f1/Unity.app/Contents/MacOS/Unity"
    "$HOME/Applications/Unity/Hub/Editor/6000.0.0f1/Unity.app/Contents/MacOS/Unity"
  )

  for candidate in "${CANDIDATES[@]}"; do
    if [[ -x "$candidate" ]]; then
      UNITY_BIN="$candidate"
      break
    fi
  done
fi

if [[ -z "${UNITY_BIN}" || ! -x "${UNITY_BIN}" ]]; then
  echo "Unity binary not found. Set UNITY_BIN=/path/to/Unity.app/Contents/MacOS/Unity"
  exit 1
fi

repair_android_sdk_layout() {
  local unity_contents sdk_root legacy_tools modern_tools ndk_root
  local canonical_path duplicate_path version_dir

  unity_contents="$(cd "$(dirname "$UNITY_BIN")/.." && pwd)"
  sdk_root="$unity_contents/PlaybackEngines/AndroidPlayer/SDK"
  legacy_tools="$sdk_root/tools"
  modern_tools="$sdk_root/cmdline-tools/latest"
  ndk_root="$unity_contents/PlaybackEngines/AndroidPlayer/NDK"

  if [[ -x "$legacy_tools/bin/sdkmanager" && ! -e "$modern_tools/bin/sdkmanager" ]]; then
    mkdir -p "$sdk_root/cmdline-tools"
    ln -sfn "$legacy_tools" "$modern_tools"
  fi

  if [[ -d "$sdk_root/build-tools" ]]; then
    for version_dir in "$sdk_root"/build-tools/*-2; do
      [[ -d "$version_dir" ]] || continue
      canonical_path="${version_dir%-2}"
      if [[ -d "$canonical_path" ]]; then
        rm -rf "$version_dir"
      fi
    done
  fi

  duplicate_path="$sdk_root/platform-tools-2"
  canonical_path="$sdk_root/platform-tools"
  if [[ -d "$duplicate_path" && -d "$canonical_path" ]]; then
    rm -rf "$duplicate_path"
  fi

  if [[ -d "$sdk_root/ndk" ]]; then
    version_dir="$(find "$sdk_root/ndk" -mindepth 1 -maxdepth 1 -type d | sort | tail -n 1)"
    if [[ -n "${version_dir:-}" && ! -e "$ndk_root/source.properties" ]]; then
      if [[ -L "$ndk_root" || -f "$ndk_root" ]]; then
        rm -f "$ndk_root"
      elif [[ -d "$ndk_root" ]]; then
        rmdir "$ndk_root" 2>/dev/null || rm -rf "$ndk_root"
      fi

      if [[ ! -e "$ndk_root" ]]; then
        ln -s "$version_dir" "$ndk_root"
      fi
    fi
  fi
}

find_keytool() {
  local unity_contents unity_jdk_keytool
  unity_contents="$(cd "$(dirname "$UNITY_BIN")/.." && pwd)"
  unity_jdk_keytool="$unity_contents/PlaybackEngines/AndroidPlayer/OpenJDK/bin/keytool"

  if [[ -x "$unity_jdk_keytool" ]]; then
    echo "$unity_jdk_keytool"
    return
  fi

  if command -v keytool >/dev/null 2>&1; then
    command -v keytool
    return
  fi

  echo "keytool not found. Install a JDK or Unity Android OpenJDK module." >&2
  exit 1
}

clean_corrupt_gradle_artifacts() {
  local gradle_module_root
  gradle_module_root="$HOME/.gradle/caches/modules-2/files-2.1/com.android.tools.external.com-intellij"

  rm -rf "$gradle_module_root/intellij-core/31.10.0"
  rm -rf "$gradle_module_root/kotlin-compiler/31.10.0"
}

repair_android_sdk_layout
clean_corrupt_gradle_artifacts
mkdir -p "$(dirname "$KEYSTORE_PATH")"

if [[ ! -f "$KEYSTORE_PATH" ]]; then
  KEYTOOL_BIN="$(find_keytool)"
  "$KEYTOOL_BIN" \
    -genkeypair \
    -v \
    -keystore "$KEYSTORE_PATH" \
    -storepass "$KEYSTORE_PASS" \
    -alias "$KEY_ALIAS" \
    -keypass "$KEY_ALIAS_PASS" \
    -keyalg RSA \
    -keysize 2048 \
    -validity 10000 \
    -dname "CN=Wanwan Drop Blaster, OU=Release, O=Mark, L=Shanghai, S=Shanghai, C=CN"
fi

export WANWAN_ANDROID_KEYSTORE="$KEYSTORE_PATH"
export WANWAN_ANDROID_KEYSTORE_PASS="$KEYSTORE_PASS"
export WANWAN_ANDROID_KEYALIAS="$KEY_ALIAS"
export WANWAN_ANDROID_KEYALIAS_PASS="$KEY_ALIAS_PASS"

python3 "$ROOT_DIR/scripts/validate_unity_project.py"

"$UNITY_BIN" \
  -batchmode \
  -quit \
  -projectPath "$ROOT_DIR" \
  -executeMethod Wanwan.Editor.BuildAutomation.BuildAndroidRelease \
  -logFile -
