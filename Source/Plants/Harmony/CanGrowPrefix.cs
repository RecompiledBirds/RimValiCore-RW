using HarmonyLib;
using RimWorld;
using RVCRestructured.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Plants
{
    [HarmonyPatch(typeof(PlantUtility), "GrowthSeasonNow")]
    public static class CanGrowPrefix
    {
        public static bool Prefix(IntVec3 c, Map map, ref bool __result, bool forSowing)
        {
            Plant plant = (Plant)map.thingGrid.ThingAt(c, ThingCategory.Plant);
            if (plant == null)
            {
                return true;
            }
            __result = CanGrow(plant.def, c, map, forSowing);
            return false;
        }

        public static bool CanGrow(ThingDef plant, IntVec3 c, Map map, bool forSowing = false)
        {
            RVCPlantCompProperties props = (RVCPlantCompProperties)plant.comps.Find(x => x.GetType() == typeof(RVCPlantCompProperties));
            float temperature = c.GetTemperature(map);
            if (props != null)
            {
                return temperature > props.minPreferredTemp && temperature < props.maxPreferredTemp;
            }

            Room roomOrAdjacent = c.GetRoomOrAdjacent(map, RegionType.Set_All);
            if (roomOrAdjacent == null)
            {
                return false;
            }
            if (!roomOrAdjacent.UsesOutdoorTemperature)
            {
                return temperature > 0f && temperature < 58f;
            }
            return forSowing ? map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNowForSowing : map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow;
        }
    }
}
