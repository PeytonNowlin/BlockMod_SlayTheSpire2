using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace BlockMod;

[ModInitializer(nameof(ModLoaded))]
public static class BlockModEntry
{
    private const string HarmonyId = "BlockMod";

    public static void ModLoaded()
    {
        try
        {
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(typeof(BlockModEntry).Assembly);

            if (!harmony.GetPatchedMethods().Any())
            {
                Log.Error($"[{HarmonyId}] no patches applied - this game version may be unsupported.");
                return;
            }

            Log.Warn($"[{HarmonyId}] loaded - player block now persists between turns.");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[{HarmonyId}] failed to apply Harmony patches: {ex}");
        }
    }
}
