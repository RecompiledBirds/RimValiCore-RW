using RimWorld;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class CanGetThoughtPatch
    {
        public static void CanGetPatch(Pawn pawn, ThoughtDef def, bool checkIfNullified, ref bool __result)
        {

            RaceDef raceDef = pawn.def as RaceDef;
            bool restricted = RestrictionsChecker.IsRestricted(def);
            bool disabled = !raceDef.RaceRestrictions.disabledThoughts.NullOrEmpty() && raceDef.RaceRestrictions.disabledThoughts.Contains(def);
            bool allowed = raceDef.RaceRestrictions.allowThoughtDefs.Contains(def);

            __result &= (restricted && allowed) && !disabled;

        }
    }
}
