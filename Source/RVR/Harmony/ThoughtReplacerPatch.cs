using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ThoughtReplacerPatch
    {
        public static void ReplacePatch(ref ThoughtDef def, MemoryThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.def is RaceDef rDef)
            {
                rDef.ThoughtReplacer.Replace(ref def);
            }
        }

        public static void ReplacePatchSIT(ref ThoughtDef def, SituationalThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.def is RaceDef rDef)
            {
                rDef.ThoughtReplacer.Replace(ref def);
            }
        }
    }
}
