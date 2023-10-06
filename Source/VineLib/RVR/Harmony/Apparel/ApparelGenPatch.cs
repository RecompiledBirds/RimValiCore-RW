using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelGenPatch
    {
        public static void CanUsePairPatch(ThingStuffPair pair, Pawn pawn, ref bool __result)
        {
            __result &= pair.thing.ApparelAllowedForRace(pawn.def);
        }
    }
}
