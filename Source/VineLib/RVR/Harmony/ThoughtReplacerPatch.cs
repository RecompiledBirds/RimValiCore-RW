using RimWorld;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ThoughtReplacerPatch
    {
        private static Dictionary<Pawn, ThoughtComp> cache = new Dictionary<Pawn, ThoughtComp>();

        private static ThoughtComp GetCached(Pawn pawn)
        {
            if (!cache.ContainsKey(pawn))
            {
                cache[pawn] = pawn.TryGetComp<ThoughtComp>();
            }
            return cache[pawn];
        }


        private static void ReplaceThought(ref ThoughtDef thought, Pawn pawn)
        {
            ThoughtComp comp = GetCached(pawn);
            if (comp == null) return;
            comp.Props.Replace(ref thought);
        }

        public static void ReplacePatch(ref ThoughtDef def, MemoryThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            ReplaceThought(ref def, pawn); 
        }

        public static bool ReplacePatchCreateMemoryPrefix(Thought_Memory newThought, MemoryThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            ReplaceThought(ref newThought.def, pawn); 
            newThought = ThoughtMaker.MakeThought(newThought.def, newThought.CurStageIndex);
            return true;
        }

        public static void ReplacePatchSIT(ref ThoughtDef def, SituationalThoughtHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            ReplaceThought(ref def, pawn);
        }
    }
}
