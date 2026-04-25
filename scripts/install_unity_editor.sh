#!/usr/bin/env bash
set -euo pipefail

HUB_BIN="${HUB_BIN:-/Applications/Unity Hub.app/Contents/MacOS/Unity Hub}"
UNITY_VERSION="${UNITY_VERSION:-6000.3.7f1}"
UNITY_CHANGESET="${UNITY_CHANGESET:-9b001d489a54}"
UNITY_ARCH="${UNITY_ARCH:-arm64}"
INSTALL_PATH="${INSTALL_PATH:-/Applications/Unity/Hub/Editor}"

if [[ ! -x "$HUB_BIN" ]]; then
  echo "Unity Hub not found at $HUB_BIN"
  echo "Install it first, for example: brew install --cask unity-hub"
  exit 1
fi

mkdir -p "$INSTALL_PATH"

"$HUB_BIN" -- --headless install-path --set "$INSTALL_PATH"
"$HUB_BIN" -- --headless install \
  --version "$UNITY_VERSION" \
  --changeset "$UNITY_CHANGESET" \
  --architecture "$UNITY_ARCH" \
  --module android android-sdk-ndk-tools android-open-jdk-17.0.9+9 \
  --childModules

echo "Unity Editor installation command completed."
