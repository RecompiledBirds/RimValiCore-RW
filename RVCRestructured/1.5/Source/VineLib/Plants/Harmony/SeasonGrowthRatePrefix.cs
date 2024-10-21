using RimWorld;
using UnityEngine;
using Verse;

namespace RVCRestructured.Plants;

public static class SeasonGrowthRatePrefix
{
    public static bool Prefix(ref float __result, Plant __instance)
    {
        RVCPlantComp plantComp = __instance.TryGetComp<RVCPlantComp>();
        if (plantComp == null)
        {
            return true;
        }
        IntVec3 vec = __instance.Position;
        Map map = __instance.Map;
        float temperature = GridsUtility.GetTemperature(vec, map);
        if (temperature < 6f)
        {
            __result = Mathf.InverseLerp(plantComp.Props.MinPreferredTemp, plantComp.Props.MaxPreferredTemp + 6f, temperature);
        }
        if (temperature > 42f)
        {
            __result = Mathf.InverseLerp(plantComp.Props.MaxPreferredTemp, plantComp.Props.MaxPreferredTemp + 42f, temperature);
        }
        return false;
    }
}
