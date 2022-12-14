using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class BedPatch
    {
        public static void CanUseBed(ref bool __result, Pawn p, ThingDef bedDef)
        {
            __result &= RestrictionsChecker.IsRestricted(bedDef) && ((p.def as RaceDef)?.RaceRestrictions.restrictedBeds.Contains(bedDef) ?? false);
        }
    }
}
