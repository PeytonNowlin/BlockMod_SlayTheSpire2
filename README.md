# Block Mod

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Slay the Spire 2](https://img.shields.io/badge/Slay%20the%20Spire%202-v0.110.1-7a1a1a)](https://store.steampowered.com/app/2868840)
[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Block%20Mod-c08b4d)](https://www.nexusmods.com/slaythespire2)

An innate Barricade for every Slay the Spire 2 character.

Your block is **never** lost at the start of your turn — whatever you didn't spend defending stays on you and stacks with whatever you gain next turn. Enemies are unaffected; they still lose block normally between their turns. Tiny standalone mod (under 10 KB) that hooks one method in the base game with Harmony.

---

## Install (players)

1. Grab `BlockMod-X.Y.Z.zip` from the latest [release](../../releases) (or [Nexus Mods](https://www.nexusmods.com/slaythespire2)).
2. Extract it. You should see a `BlockMod/` folder containing exactly three files: `BlockMod.dll`, `BlockMod.json`, `BlockMod.pck`.
3. Drop that `BlockMod` folder into the game's `mods` directory:
   - **Windows**: `<Steam>\steamapps\common\Slay the Spire 2\mods\`
   - **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/`
   - **Linux**: `~/.steam/steam/steamapps/common/Slay the Spire 2/mods/`
4. Launch the game from Steam (use **Play with Mods** if Steam offers that option).
5. Open **Settings → Mod Settings**, enable **Block Mod**, and restart if the game asks you to.

To uninstall, disable it in Mod Settings and delete the `BlockMod` folder from `mods/`.

### Verify it's working

Open the game's log:

- Windows: `%APPDATA%\SlayTheSpire2\logs\godot.log`
- macOS: `~/Library/Application Support/SlayTheSpire2/logs/godot.log`

You should see:

```
[BlockMod] loaded - player block now persists between turns.
```

---

## How it works

Decompiling `sts2.dll` shows that block is wiped at the start of each side's turn only if the public hook allows it:

```csharp
// MegaCrit.Sts2.Core.Entities.Creatures.Creature.ClearBlock
private async Task ClearBlock()
{
    if (Hook.ShouldClearBlock(CombatState, this, out AbstractModel preventer))
    {
        Block = 0;
    }
    else
    {
        await Hook.AfterPreventingBlockClear(CombatState, preventer, this);
    }
}
```

Barricade, Blur, and similar effects already return `false` from `ShouldClearBlock`. Block Mod uses the same public hook instead of replacing the private `ClearBlock` method (that private patch broke across Early Access updates):

```csharp
[HarmonyPatch(typeof(Hook), nameof(Hook.ShouldClearBlock))]
public static class KeepBlockBetweenTurnsPatch
{
    private static void Postfix(Creature creature, ref bool __result)
    {
        if (creature.IsPlayer)
        {
            __result = false;
        }
    }
}
```

That's the entire mechanic. Monsters fall through to the original hook and lose block normally.

---

## Build (developers)

### Prerequisites

- [Godot 4.5.1 .NET (mono)](https://godotengine.org/download/archive/4.5.1-stable/)
- [.NET SDK 9](https://dotnet.microsoft.com/en-us/download)
- A local install of Slay the Spire 2 (the build references the game's `sts2.dll` and `0Harmony.dll`)

### Setup

```bash
git clone https://github.com/<you>/BlockMod.git
cd BlockMod

# Copy the game's DLLs into the project root for the build reference.
# (They are gitignored - never commit them.)
GAME_RES="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64"
cp "$GAME_RES/sts2.dll" .
cp "$GAME_RES/0Harmony.dll" .
```

On Windows the source path is `<Steam>\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\` (or wherever your install puts `sts2.dll`).

### Build

```bash
./build.sh        # compiles BlockMod.dll and exports BlockMod.pck
./install.sh      # copies the 3 mod files into your local mods/ folder
```

The Godot binary path is configurable via the `GODOT` env var (defaults to `~/godot-stspire/Godot_mono.app/Contents/MacOS/Godot`).

### Project layout

```
BlockMod/
├── BlockMod.csproj                       # Godot.NET.Sdk 4.5.1, net9.0
├── BlockMod.json                         # mod manifest (id, version, etc.)
├── project.godot                         # Godot project shell
├── export_presets.cfg                    # headless PCK export config
├── icon.svg                              # source for mod_image.png
├── BlockMod/
│   └── mod_image.png                     # 256x256 in-game mod icon
├── Source/
│   ├── BlockModEntry.cs                  # [ModInitializer] entry point
│   └── Patches/
│       └── KeepBlockBetweenTurnsPatch.cs # the Harmony Postfix
├── build.sh
└── install.sh
```

---

## Compatibility

- Updated for Slay the Spire 2 **v0.107.x–v0.110.1** (current Early Access as of August 2026).
- Manifest `min_game_version` is `0.107.0` (Steam Workshop / modern mod-loader era).
- Compatible with character mods (Ryoshu, The Watcher, The Reaper, etc.).
- Patches the public `Hook.ShouldClearBlock` API that Barricade already uses, so conflicts are unlikely.
- Save-safe — keeps no per-run state of its own.

---

## Releasing

1. Bump `version` in `BlockMod.json`.
2. `./build.sh && ./install.sh` (also stages `dist/BlockMod/` for the zip).
3. `cd dist && zip -r BlockMod-X.Y.Z.zip BlockMod` and attach to a new GitHub Release / upload to Nexus.

---

## Acknowledgements

- [MegaCrit](https://www.megacrit.com/) for Slay the Spire 2.
- [doctornoodlearms](https://www.reddit.com/r/slaythespire/comments/1rm5gvg/sts2_early_access_mod_guide/) for the original early-access mod guide.
- [jiegec/STS2FirstMod](https://github.com/jiegec/STS2FirstMod) — used as the project skeleton.
- [Alchyr/ModTemplate-StS2](https://github.com/Alchyr/ModTemplate-StS2) — reference template.
- [pardeike/Harmony](https://github.com/pardeike/Harmony) — runtime patching that makes a one-method mod possible in ~10 lines.

---

## License

MIT — see [LICENSE](LICENSE).
