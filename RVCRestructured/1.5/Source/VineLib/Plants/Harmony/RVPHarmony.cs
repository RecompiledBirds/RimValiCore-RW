using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.Plants;

[StaticConstructorOnStartup]
public class RVPHarmony
{
    static RVPHarmony()
    {
        RVCLog.Log("Starting RVP patches.");
        Harmony harmony = new("RecompiledBirds.RVC.RVP");
        harmony.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Temperature"), prefix: new HarmonyMethod(typeof(SeasonGrowthRatePrefix), nameof(SeasonGrowthRatePrefix.Prefix)));
        harmony.Patch(AccessTools.Method(typeof(PlantUtility), "GrowthSeasonNow", [typeof(IntVec3),typeof(Map), typeof(ThingDef)]),prefix: new HarmonyMethod(typeof(CanGrowPrefix),nameof(CanGrowPrefix.Prefix_IntVec)));
        harmony.Patch(AccessTools.Method(typeof(PlantUtility), "GrowthSeasonNow", [typeof(Map), typeof(ThingDef)]), prefix: new HarmonyMethod(typeof(CanGrowPrefix), nameof(CanGrowPrefix.Prefix_MapOnly)));
        harmony.Patch(AccessTools.Method(typeof(Plant), "get_LeaflessTemperatureThresh"),prefix: new HarmonyMethod(typeof(TempThreshPrefix),nameof(TempThreshPrefix.Prefix)));
        harmony.Patch(AccessTools.Method(typeof(Zone_Growing), "GetInspectString"),prefix: new HarmonyMethod(typeof(GrowingZonePrefix),nameof(GrowingZonePrefix.Prefix)));
        RVCLog.Log("RVP patches completed.");

    }
}
