using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class CanGetThoughtPatch
    {
        public static void CanGetPatch(Pawn pawn, ThoughtDef def, bool checkIfNullified, ref bool __result)
        {
            bool restricted = RestrictionsChecker.IsRestricted(def);
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            if(comp==null)
            {
                __result = __result && !restricted;
                return;
            }
            bool inAllowedDefs = (comp.Props.restrictedThoughtDefs.Contains(def) || comp.Props.allowThoughtDefs.Contains(def));
            bool allowed = (!restricted || inAllowedDefs) && !comp.Props.disabledThoughts.Contains(def);
            __result = __result && allowed;
        }
    }
}
