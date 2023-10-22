using RVCRestructured.Shifter;
using Verse;
using Verse.AI;

namespace RVCRestructured.RVR
{
    public static class ConstructionPatch
    {
        public static void Constructable(Thing t, Pawn pawn, WorkTypeDef workType, bool forced, ref bool __result)
        {
            string label = pawn.def.label;
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            RVRRestrictionComp restrictions = comp?.Props;
            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            if(shapeshifterComp!= null)
            {
                restrictions=shapeshifterComp.GetCompProperties<RVRRestrictionComp>();
                label = shapeshifterComp.Label();
            }
            bool restricted = RestrictionsChecker.IsRestricted(t.def);
            bool allowedToUse =restrictions?.allowedBuildings.Contains(t.def) ?? false;
            bool final = !restricted || allowedToUse;
            if (!final)
                JobFailReason.Is(label + " " + "CannotBuildRVR".Translate(label.Named("RACE")));
            __result &= final;
        }
    }
}
