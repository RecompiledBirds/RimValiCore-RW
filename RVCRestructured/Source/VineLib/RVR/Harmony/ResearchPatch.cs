using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ResearchPatch
{
    public static void ResearchPostfix(Pawn pawn, ref bool __result)
    {
        if (__result) return; //Should skip was true
        if (Find.ResearchManager.GetProject() is not ResearchProjectDef def) return;

        //The original function asks if researching should be skipped, so we need to return true only if researching this def is disallowed
        __result = !pawn.CanUse(def);
    }
}
