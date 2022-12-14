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
    [HarmonyPatch(typeof(Zone_Growing), "GetInspectString")]
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
            ThingDef def = grower.GetPlantDefToGrow();
            return CanGrowPrefix.CanGrow(def, grower.Position, grower.Map, true);
        }
    }
}
