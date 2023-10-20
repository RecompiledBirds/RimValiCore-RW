using Verse;
using Verse.AI;

namespace RVCRestructured.RVR
{
    public static class ConstructionPatch
    {
        public static void Constructable(Thing t, Pawn pawn, WorkTypeDef workType, bool forced, ref bool __result)
        {
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            bool restricted = RestrictionsChecker.IsRestricted(t.def);
            bool allowedToUse = comp?.Props.allowedBuildings.Contains(t.def) ?? false;
            bool final = !restricted || allowedToUse;
            if (!final)
                JobFailReason.Is(pawn.def.label + " " + "CannotBuildRVR".Translate(pawn.def.label.Named("RACE")));
            __result &= final;
        }
    }
}
