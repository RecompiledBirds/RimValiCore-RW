using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ApparelScoreGainPatch
{
    public static void ApparelScoreGain_NewTmp(Pawn pawn, Apparel ap, List<float> wornScoresCache, ref float __result)
    {
        if (!pawn.CanUse(ap.def))
        {
            __result = int.MinValue;
            return;
        }
    }
}
