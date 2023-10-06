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

            if(pawn.def is RaceDef raceDef)
            {
                bool isAllowed = raceDef.RaceRestrictions.allowedTraits.Contains(trait.def) || raceDef.RaceRestrictions.restrictedTraits.Contains(trait.def);
                return (!restricted || isAllowed);
            }

            return !restricted;
        }
    }
}