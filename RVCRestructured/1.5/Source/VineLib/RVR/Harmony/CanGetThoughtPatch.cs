using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class CanGetThoughtPatch
{
    public static void CanGetPatch(Pawn pawn, ThoughtDef def, bool checkIfNullified, ref bool __result)
    {
        if (!__result) return; //skip false results

        __result = pawn.CanUse(def);
    }
}
