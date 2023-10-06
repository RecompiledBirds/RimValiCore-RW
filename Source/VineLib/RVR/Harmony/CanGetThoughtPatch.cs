using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class CanGetThoughtPatch
    {
        public static void CanGetPatch(Pawn pawn, ThoughtDef def, bool checkIfNullified, ref bool __result)
        {
            bool restricted = RestrictionsChecker.IsRestricted(def);
            if(!(pawn.def is RaceDef rDef))
            {
                __result = __result && !restricted;
                return;
            }
            bool inAllowedDefs = (rDef.RaceRestrictions.restrictedThoughtDefs.Contains(def) || rDef.RaceRestrictions.allowThoughtDefs.Contains(def));
            bool allowed = (!restricted || inAllowedDefs) && !rDef.RaceRestrictions.disabledThoughts.Contains(def);
            __result = __result && allowed;
        }
    }
}
