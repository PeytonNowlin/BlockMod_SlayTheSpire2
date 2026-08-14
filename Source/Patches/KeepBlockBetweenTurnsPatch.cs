using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;

namespace BlockMod.Patches;

// Keep player block between turns using the same public hook Barricade uses.
//
// Game flow (sts2.dll through at least v0.110.x):
//   Creature.ClearBlock() asks Hook.ShouldClearBlock(...) whether any
//   power/relic prevents the wipe. BarricadePower.ShouldClearBlock returns
//   false when the owner is the creature being cleared. If nothing prevents
//   it, Block is set to 0.
//
// Patching the private ClearBlock method broke across Early Access updates.
// Hook.ShouldClearBlock is the public extension point, so we force it to
// false for player creatures. Enemies still lose block normally.
[HarmonyPatch(typeof(Hook), nameof(Hook.ShouldClearBlock))]
public static class KeepBlockBetweenTurnsPatch
{
    private static bool Prepare()
    {
        var method = AccessTools.DeclaredMethod(typeof(Hook), nameof(Hook.ShouldClearBlock));
        if (method != null)
        {
            return true;
        }

        Log.Error("[BlockMod] Hook.ShouldClearBlock was not found. This game version is not supported.");
        return false;
    }

    private static void Postfix(Creature creature, ref bool __result)
    {
        if (creature.IsPlayer)
        {
            __result = false;
        }
    }
}
