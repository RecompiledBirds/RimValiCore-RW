using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelEquipping
    {
        public static bool ApparelAllowedForRace(this ThingDef def, Pawn pawn)
        {
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            
            bool restricted = RestrictionsChecker.IsRestricted(def);
            bool canOnlyWearApprovedApparel =!( comp == null || comp.Props.canUseAnyApparel);
            bool inAllowedDefs = (comp==null || comp.Props.allowedApparel.Contains(def)) ||(comp==null||comp.Props.restrictedApparel.Contains(def));
            return (restricted && inAllowedDefs) || (canOnlyWearApprovedApparel && inAllowedDefs) || !restricted && !canOnlyWearApprovedApparel;
        }

        public static void EquipPatch(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (!thing.def.IsApparel)
                return;
            bool allowed = thing.def.ApparelAllowedForRace(pawn);
            __result &= allowed;
            if (!allowed)
                cantReason = "RVC_CannotWear".Translate(pawn.def.label.Named("RACE"));

        }
    }
}
