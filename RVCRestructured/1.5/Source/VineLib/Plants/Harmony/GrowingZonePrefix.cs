using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.Plants;

public static class GrowingZonePrefix
{
    public static bool Prefix(Zone_Growing __instance, ref string __result)
    {
        string text = "";
        if (__instance.Cells.NullOrEmpty())
        {
            __result = text;
            return true;
        }
       
        if (__instance.Cells.First().UsesOutdoorTemperature(__instance.Map))
        {
            text += "OutdoorGrowingPeriod".Translate() + ": " + Zone_Growing.GrowingQuadrumsDescription(__instance.Map.Tile) + "\n";
        }
        if (IsSeason(__instance))
        {
            text += "GrowSeasonHereNow".Translate();
        }
        else
        {
            text += "CannotGrowBadSeasonTemperature".Translate();
        }
        __result = text;
        return false;
    }

    public static bool IsSeason(Zone_Growing grower)
    {
        return CanGrowPrefix.CanGrowWithDefault(grower.PlantDefToGrow,grower.Position, grower.Map, true);
    }
}
