using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class CanGetThoughtPatch
    {
        public static void CanGetPatch(Pawn pawn, ThoughtDef def, bool checkIfNullified, ref bool __result)
        {
            bool restricted = RestrictionsChecker.IsRestricted(def);
            bool allowed = !restricted;
            bool disabled = restricted; 
            if(pawn.def is RaceDef rDef)
            {
                allowed |= rDef.RaceRestrictions.allowThoughtDefs.Contains(def);
                disabled &= !rDef.RaceRestrictions.disabledThoughts.Contains(def);
            }

            __result &= allowed && !disabled;
        }
    }
}
