using RimWorld;
using Verse;

namespace RVCRestructured.Plants;

public static class TempThreshPrefix
{
    public static bool Prefix(Plant __instance, ref float __result)
    {
        RVCPlantComp plantComp = __instance.TryGetComp<RVCPlantComp>();
        if (plantComp != null)
        {
            __result = plantComp.Props.MinPreferredTemp - 8;


            return false;
        }
        return true;
    }
}
