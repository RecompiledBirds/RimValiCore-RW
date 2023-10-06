using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Plants
{
    [StaticConstructorOnStartup]
    public class RVPHarmony
    {
        static RVPHarmony()
        {
            RVCLog.Log("Starting RVP patches.");
            Harmony harmony = new Harmony("RecompiledBirds.RVC.RVP");
            harmony.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Temperature"), prefix: new HarmonyMethod(typeof(SeasonGrowthRatePrefix), nameof(SeasonGrowthRatePrefix.Prefix)));
            harmony.Patch(AccessTools.Method(typeof(PlantUtility), "GrowthSeasonNow"),prefix: new HarmonyMethod(typeof(CanGrowPrefix),nameof(CanGrowPrefix.Prefix)));
            harmony.Patch(AccessTools.Method(typeof(Plant), "get_LeaflessTemperatureThresh"),prefix: new HarmonyMethod(typeof(TempThreshPrefix),nameof(TempThreshPrefix.Prefix)));
            harmony.Patch(AccessTools.Method(typeof(Zone_Growing), "GetInspectString"),prefix: new HarmonyMethod(typeof(GrowingZonePrefix),nameof(GrowingZonePrefix.Prefix)));
            RVCLog.Log("RVP patches completed.");

        }
    }
}
