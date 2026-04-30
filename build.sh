#!/usr/bin/env bash
# Build BlockMod: compile the C# DLL and export the Godot PCK.
set -euo pipefail

cd "$(dirname "$0")"

# Tool paths
GODOT="${GODOT:-$HOME/godot-stspire/Godot_mono.app/Contents/MacOS/Godot}"
export DOTNET_ROOT="${DOTNET_ROOT:-/opt/homebrew/opt/dotnet@9/libexec}"
export PATH="/opt/homebrew/opt/dotnet@9/bin:$HOME/.dotnet/tools:$PATH"

# Make sure Godot has imported assets at least once (writes .godot/)
if [ ! -d ".godot" ]; then
  echo "[build] First-time Godot import..."
  "$GODOT" --headless --import . >/dev/null 2>&1 || true
fi

echo "[build] Compiling BlockMod.dll (Release)..."
dotnet build -c Release

echo "[build] Exporting BlockMod.pck via Godot..."
"$GODOT" --headless --export-pack "Windows Desktop" BlockMod.pck

DLL="$PWD/.godot/mono/temp/bin/Release/BlockMod.dll"
if [ ! -f "$DLL" ]; then
  DLL="$PWD/bin/Release/net9.0/BlockMod.dll"
fi

echo "[build] Done."
echo "  DLL: $DLL"
echo "  PCK: $PWD/BlockMod.pck"
echo "  JSON: $PWD/BlockMod.json"
