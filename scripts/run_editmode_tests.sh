#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_BIN="${UNITY_BIN:-}"

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

"$UNITY_BIN" \
  -batchmode \
  -projectPath "$ROOT_DIR" \
  -executeMethod Wanwan.Editor.BuildAutomation.RunEditModeTests \
  -logFile -

test -f "$ROOT_DIR/Builds/editmode-results.xml"
