using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class BodyTypeGenPatch
{
    public static bool Prefix(ref Pawn pawn)
    {
        RVRBodyGetter bodyGetter = pawn.GetComp<RVRBodyGetter>();
        if (bodyGetter != null)
        {
            bodyGetter.GenBody();
            return false;
        }
        return true;
    }
}
