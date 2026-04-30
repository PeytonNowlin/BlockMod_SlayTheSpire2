#!/usr/bin/env bash
# Install BlockMod into the Slay the Spire 2 Mods folder.
set -euo pipefail

cd "$(dirname "$0")"

GAME_MODS="${GAME_MODS:-$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/Mods}"
TARGET="$GAME_MODS/BlockMod"

mkdir -p "$TARGET"

DLL="$PWD/.godot/mono/temp/bin/Release/BlockMod.dll"
if [ ! -f "$DLL" ]; then
  DLL="$PWD/bin/Release/net9.0/BlockMod.dll"
fi

if [ ! -f "$DLL" ]; then
  echo "[install] BlockMod.dll not found - run ./build.sh first" >&2
  exit 1
fi
if [ ! -f "BlockMod.pck" ]; then
  echo "[install] BlockMod.pck not found - run ./build.sh first" >&2
  exit 1
fi

# Wipe any previous install (or stray manual copy of the source tree) so the
# game only ever sees the three real mod files. This prevents the "manifest
# found but assembly missing" error when older clutter is in the folder.
rm -rf "$TARGET"
mkdir -p "$TARGET"

cp "$DLL" "$TARGET/BlockMod.dll"
cp "BlockMod.pck" "$TARGET/BlockMod.pck"
cp "BlockMod.json" "$TARGET/BlockMod.json"

# Also stage a "drag-and-drop" copy next to the project so the user can
# install manually by dropping the folder into the game's Mods directory.
DIST="$PWD/dist/BlockMod"
rm -rf "$DIST"
mkdir -p "$DIST"
cp "$DLL" "$DIST/BlockMod.dll"
cp "BlockMod.pck" "$DIST/BlockMod.pck"
cp "BlockMod.json" "$DIST/BlockMod.json"

echo "[install] Installed to: $TARGET"
ls -la "$TARGET"
echo "[install] Drag-and-drop copy at: $DIST"
