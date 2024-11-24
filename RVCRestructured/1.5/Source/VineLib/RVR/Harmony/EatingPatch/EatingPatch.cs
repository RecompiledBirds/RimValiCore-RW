using RVCRestructured.RVR;
using Verse;
using Verse.AI;

namespace RVCRestructured;

public static class EatingPatch
{
    
    public static void CanEverEatPostFix(ref bool __result, RaceProperties __instance, ThingDef t)
    {
        if (!__result) return;

        bool restricted = t.IsRestricted();
        if (!restricted) return;

        ThingDef def = Utils.GetDef(__instance);
        RVRRestrictionComp comp = def.GetCompProperties<RVRRestrictionComp>();
        if (comp == null)
        {
            if (restricted)
            {
                JobFailReason.Is($"{def.label} {"CannotEatRVR".Translate(def.label.Named("RACE"))}");
            }
            return;
        }

        __result = comp.IsAlwaysAllowed(RestrictionType.FoodDef) || (comp[def]?.CanUse ?? false) || !def.IsRestricted();

        if(!__result) JobFailReason.Is($"{def.label} {"CannotEatRVR".Translate(def.label.Named("RACE"))}");
    }
}
