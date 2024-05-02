using RVCRestructured.Shifter;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class BedPatch
    {
        public static void CanUseBed(ref bool __result, Pawn p, ThingDef bedDef)
        {
            if (!__result) return;
            __result = p.CanUse(bedDef);
        }
    }
}
