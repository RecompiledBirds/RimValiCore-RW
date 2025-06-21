namespace RVCRestructured.RVR.HarmonyPatches;

public static class ThoughtReplacerPatch
{
    private static readonly Dictionary<Pawn, ThoughtComp> cache = [];
    private static bool subscribedToCacheReset = false;
    private static ThoughtComp GetCached(Pawn pawn)
    {
        if (!subscribedToCacheReset)
        {
            WorldSpecificCacheSignaler.signalCachesReset += ResetCache;
            subscribedToCacheReset = true;
        }
        if(cache.TryGetValue(pawn, out ThoughtComp result))
        {
            return result;
        }
        return cache[pawn];
    }

    private static void ResetCache()
    {
        cache.Clear();
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
