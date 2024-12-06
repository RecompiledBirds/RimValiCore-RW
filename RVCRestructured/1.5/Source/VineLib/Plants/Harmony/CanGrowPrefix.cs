using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.Plants;


public static class CanGrowPrefix
{
    public static void PostFix(IntVec3 c, Map map, ref bool __result, bool forSowing)
    {
        Plant plant = (Plant)map.thingGrid.ThingAt(c, ThingCategory.Plant);
        __result = CanGrow(plant.def, c, map, forSowing)||__result;
    }
    
    public static bool CanGrow(ThingDef plant, IntVec3 c, Map map, bool forSowing = false)
    {
        if(plant == null) return false;
        RVCPlantCompProperties props = (RVCPlantCompProperties)plant.comps.Find(x => x.GetType() == typeof(RVCPlantCompProperties));
        float temperature = c.GetTemperature(map);
        return props!=null && temperature > props.MinPreferredTemp && temperature < props.MaxPreferredTemp;
    }
    public static bool CanGrowWithDefault(ThingDef plant, IntVec3 c, Map map, bool forSowing = false)
    {
        if (plant == null) return PlantUtility.GrowthSeasonNow(c,map,forSowing);
        RVCPlantCompProperties props = (RVCPlantCompProperties)plant.comps.Find(x => x.GetType() == typeof(RVCPlantCompProperties));
        float temperature = c.GetTemperature(map);
        return props != null && temperature > props.MinPreferredTemp && temperature < props.MaxPreferredTemp;
    }
}
