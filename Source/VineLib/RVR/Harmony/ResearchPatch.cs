using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ResearchPatch
    {
        public static void ResearchPostfix(Pawn pawn, ref bool __result)
        {
            ResearchProjectDef def = Find.ResearchManager.currentProj;
            if (def == null)
                return;

            __result &= !RestrictionsChecker.IsRestricted(def) || (pawn.TryGetComp<RestrictionComp>()?.Props.restrictedResearchDefs.Contains(def) ?? false);
        }
    }
}
