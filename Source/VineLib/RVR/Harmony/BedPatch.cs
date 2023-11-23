using RVCRestructured.Shifter;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class BedPatch
    {
        public static void CanUseBed(ref bool __result, Pawn p, ThingDef bedDef)
        {
            RestrictionComp comp = p.TryGetComp<RestrictionComp>();
            RVRRestrictionComp restrictions = null;

            if (comp != null) restrictions = comp.Props;

            ShapeshifterComp shapeshifterComp = p.TryGetComp<ShapeshifterComp>();
            if (shapeshifterComp != null)
            {
                restrictions = shapeshifterComp.GetCompProperties<RVRRestrictionComp>();
            }
            __result &= !RestrictionsChecker.IsRestricted(bedDef) || (restrictions?.restrictedBeds.Contains(bedDef) ?? false);
        }
    }
}
