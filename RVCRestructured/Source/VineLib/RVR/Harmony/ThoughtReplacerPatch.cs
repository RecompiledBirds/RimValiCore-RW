using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ThoughtReplacerPatch
{
    private static readonly Dictionary<Pawn, ThoughtComp> cache = [];

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
        ThoughtComp? comp = GetCached(pawn);
        if (comp == null) return; 
        comp.Props.Replace(ref thought);
    }

    public static void ReplacePatch(ref ThoughtDef def, MemoryThoughtHandler __instance)
    {
        Pawn pawn = __instance.pawn;
        ReplaceThought(ref def, pawn); 
    }

    public static void ReplacePatchCreateMemoryPrefix(Thought_Memory newThought, MemoryThoughtHandler __instance)
    {
        Pawn pawn = __instance.pawn;
        ThoughtDef def = newThought.def;
        
        ReplaceThought(ref def, pawn);
        newThought.def = def;
    }

    public static void ReplacePatchSIT(ref ThoughtDef def, SituationalThoughtHandler __instance)
    {
        Pawn pawn = __instance.pawn;
        ReplaceThought(ref def, pawn);
    }
}
