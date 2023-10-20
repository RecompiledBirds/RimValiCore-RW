using RimWorld;
using System.Security.Cryptography;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ThoughtReplacerPatch
    {
        public static void ReplacePatch(ref ThoughtDef def, MemoryThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            ThoughtComp comp = pawn.TryGetComp<ThoughtComp>();
            if (comp == null) return;
            comp.Props.Replace(ref def);
        }

        public static bool ReplacePatchCreateMemoryPrefix(Thought_Memory newThought, MemoryThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            ThoughtComp comp = pawn.TryGetComp<ThoughtComp>();
            if (comp == null) return true;
            comp.Props.Replace(ref newThought.def);
            newThought = ThoughtMaker.MakeThought(newThought.def, newThought.CurStageIndex);
            return true;
        }

        public static void ReplacePatchSIT(ref ThoughtDef def, SituationalThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            ThoughtComp comp = pawn.TryGetComp<ThoughtComp>();
            if (comp == null) return;
            comp.Props.Replace(ref def);
        }
    }
}
