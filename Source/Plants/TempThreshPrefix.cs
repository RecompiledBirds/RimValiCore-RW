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
    public static class TempThreshPrefix
    {
        public static bool Prefix(Plant __instance, ref float __result)
        {
            RVCPlantComp plantComp = __instance.TryGetComp<RVCPlantComp>();
            if (plantComp != null)
            {
                __result = plantComp.Props.minPreferredTemp - 8;


                return false;
            }
            return true;
        }
    }
}
