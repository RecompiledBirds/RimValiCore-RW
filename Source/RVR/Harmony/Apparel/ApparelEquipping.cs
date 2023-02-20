using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelEquipping
    {
        public static bool ApparelAllowedForRace(this ThingDef def, ThingDef race)
        {
            bool restricted = RestrictionsChecker.IsRestricted(def);
            if (!(race is RaceDef raceDef)) return !restricted;

            bool canOnlyWearApprovedApparel = !raceDef.RaceRestrictions.canUseAnyApparel;
            bool inAllowedDefs = raceDef.RaceRestrictions.allowedApparel.Contains(def) || raceDef.RaceRestrictions.restrictedApparel.Contains(def);

            return (restricted && inAllowedDefs) || (canOnlyWearApprovedApparel && inAllowedDefs) || !restricted && !canOnlyWearApprovedApparel;
        }

        public static void EquipPatch(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (!thing.def.IsApparel)
                return;
            bool allowed = thing.def.ApparelAllowedForRace(pawn.def);
            __result &= allowed;
            if (!allowed)
                cantReason = "CannotWearRVR".Translate(pawn.def.label.Named("RACE"));

        }
    }
}
