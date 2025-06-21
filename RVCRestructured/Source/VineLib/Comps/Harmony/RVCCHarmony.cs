using HarmonyLib;
using Verse;

namespace RVCRestructured.Comps.HarmonyPatches;

[StaticConstructorOnStartup]
public static class RVCCHarmony
{
    static RVCCHarmony()
    {
        Harmony harmony = new("RecompiledBirds.RVC.RVCC");
        VineLog.Log("Starting RVCComps patches.");
        harmony.Patch(AccessTools.Method(typeof(Thing), "DeSpawn"),postfix:new HarmonyMethod(typeof(HealingDespawnPatch),nameof(HealingDespawnPatch.Postfix)));
        harmony.Patch(AccessTools.Method(typeof(Thing), "SpawnSetup"), postfix: new HarmonyMethod(typeof(HealingSpawnPatch), nameof(HealingDespawnPatch.Postfix)));

        VineLog.Log("Finished RVCC patches.");
    }
}
