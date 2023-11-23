using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class TraitPatch
    {
        public static bool TraitPrefix(Trait trait, TraitSet __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool restricted = RestrictionsChecker.IsRestricted(trait.def);

            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            if (comp!=null)
            {
                
                bool isAllowed =(comp.Props.allowedTraits.Contains(trait.def) || comp.Props.restrictedTraits.Contains(trait.def));
                return (!restricted || isAllowed);
            }

            return !restricted;
        }
    }
}