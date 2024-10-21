using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ApparelGenPatch
{
    public static void CanUsePairPatch(ThingStuffPair pair, Pawn pawn, ref bool __result)
    {
        if (!__result) return;
        __result = pawn.CanUse(pair.thing);
    }
}
