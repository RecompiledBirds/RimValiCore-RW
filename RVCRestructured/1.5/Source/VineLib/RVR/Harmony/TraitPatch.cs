using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class TraitPatch
{
    public static bool TraitPrefix(Trait trait, TraitSet __instance)
    {
        Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        return pawn.CanUse(trait.def);
    }
}
