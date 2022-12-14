using HarmonyLib;
using RimWorld;
using RVCRestructured.Plants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Plants
{
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
                __result = Mathf.InverseLerp(plantComp.Props.minPreferredTemp, plantComp.Props.maxPreferredTemp + 6f, temperature);
            }
            if (temperature > 42f)
            {
                __result = Mathf.InverseLerp(plantComp.Props.maxPreferredTemp, plantComp.Props.maxPreferredTemp + 42f, temperature);
            }
            return false;
        }
    }
}
