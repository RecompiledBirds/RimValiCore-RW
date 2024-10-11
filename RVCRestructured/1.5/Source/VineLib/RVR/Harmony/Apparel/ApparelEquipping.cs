using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ApparelEquipping
{
    public static void EquipPatch(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
    {
        if (!__result) return;
        if (!thing.def.IsApparel) return;

        __result = pawn.CanUse(thing.def);
        if (!__result) cantReason = "RVC_CannotWear".Translate(pawn.def.label.Named("RACE"));
    }
}
