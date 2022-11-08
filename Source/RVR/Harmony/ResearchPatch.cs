using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class ResearchPatch
    {
        public static void ResearchPostfix(Pawn pawn, ref bool __result)
        {
            ResearchProjectDef def = Find.ResearchManager.currentProj;
            if (def == null)
                return;

            __result &= !RestrictionsChecker.IsRestricted(def) || ((pawn.def as RaceDef)?.RaceRestrictions.restrictedResearchDefs.Contains(def) ?? false);
        }
    }
}
