using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ResearchPatch
{
    public static void ResearchPostfix(Pawn pawn, ref bool __result)
    {
        if (__result) return;
        if (Find.ResearchManager.GetProject() is not ResearchProjectDef def) return;

        __result = pawn.CanUse(def);
    }
}
