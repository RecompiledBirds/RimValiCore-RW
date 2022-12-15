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
            RaceDef raceDef = pawn.def as RaceDef;
            bool restricted = RestrictionsChecker.IsRestricted(raceDef);

            return (restricted && (raceDef?.RaceRestrictions.restrictedTraits.Contains(trait.def) ?? false)) || ((!(raceDef?.RaceRestrictions.disabledTraits.Contains(trait.def) ?? true)) && !restricted);
        }
    }
}