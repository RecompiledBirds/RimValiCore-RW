using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class BodyTypeGenPatch
{
    public static void Posfix(ref Pawn pawn)
    {
        RVRBodyGetter bodyGetter = pawn.GetComp<RVRBodyGetter>();
        if(bodyGetter != null)
        {
            bodyGetter.GenBody();
            return;
        }
        if (!RestrictionsChecker.IsRestricted(pawn.story.bodyType))return;
        IEnumerable<BodyTypeDef> defs = DefDatabase<BodyTypeDef>.AllDefs.Where(x => !RestrictionsChecker.IsRestricted(x));

        if (pawn.story.bodyType != null && defs.Contains(pawn.story.bodyType))
            return;

        pawn.story.bodyType = defs.RandomElement();
    }
}
