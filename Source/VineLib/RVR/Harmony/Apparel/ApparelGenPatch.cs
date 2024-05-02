using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelGenPatch
    {
        public static void CanUsePairPatch(ThingStuffPair pair, Pawn pawn, ref bool __result)
        {
            if (!__result) return;
            __result = pawn.CanUse(pair.thing);
        }
    }
}
