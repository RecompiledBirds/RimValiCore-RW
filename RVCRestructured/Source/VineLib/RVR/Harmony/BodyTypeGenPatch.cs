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

        IEnumerable<BodyTypeDef> defs = DefDatabase<BodyTypeDef>.AllDefs.Where(x => !RestrictionsChecker.IsRestricted(x));

        if (pawn.story.bodyType != null && defs.Contains(pawn.story.bodyType))
            return;

        pawn.story.bodyType = defs.RandomElement();
    }
}
