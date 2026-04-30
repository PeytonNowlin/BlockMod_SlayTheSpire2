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
            Log.Warn($"[{HarmonyId}] loaded - player block now persists between turns.");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[{HarmonyId}] failed to apply Harmony patches: {ex}");
        }
    }
}
