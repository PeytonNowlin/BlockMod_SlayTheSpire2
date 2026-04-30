using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace BlockMod.Patches;

// Suppress the start-of-turn block reset for the player only.
//
// Game flow (decompiled from sts2.dll):
//   Creature.ClearBlock() is called at the start of each side's turn.
//   It asks Hook.ShouldClearBlock(...) whether any active power/relic
//   prevents the clear (e.g. BarricadePower.ShouldClearBlock returns
//   false when the owner is the creature being cleared). If nothing
//   prevents it, Block is set to 0.
//
// We short-circuit ClearBlock entirely when the creature is the player,
// so block carries over from the previous turn. Enemies are left alone.
[HarmonyPatch(typeof(Creature), "ClearBlock")]
public static class KeepBlockBetweenTurnsPatch
{
    private static bool Prefix(Creature __instance, ref Task __result)
    {
        if (!__instance.IsPlayer)
        {
            return true;
        }

        __result = Task.CompletedTask;
        return false;
    }
}
