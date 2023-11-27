using RVCRestructured.Shifter;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelEquipping
    {
        public static bool ApparelAllowedForRace(this ThingDef def, Pawn pawn)
        {
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            RVRRestrictionComp restrictions = null;

            if (comp != null) restrictions = comp.Props;

            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            if (shapeshifterComp != null)
            {
                if (!shapeshifterComp.CurrentForm.race.Humanlike)
                {
                    return false;
                }
                restrictions = shapeshifterComp.GetCompProperties<RVRRestrictionComp>();
            }
            bool restricted = RestrictionsChecker.IsRestricted(def);
            bool noComp = restrictions == null;
            bool canOnlyWearApprovedApparel =!(noComp || restrictions.canUseAnyApparel);
            bool inAllowedDefs = (noComp || restrictions.allowedApparel.Contains(def)) ||(noComp ||restrictions.restrictedApparel.Contains(def));
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
