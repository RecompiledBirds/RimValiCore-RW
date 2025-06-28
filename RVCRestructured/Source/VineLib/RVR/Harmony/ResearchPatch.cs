using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ResearchPatch
{
    private static Dictionary<FactionDef, List<ResearchProjectDef>> disallowedResearch = [];
    private static Dictionary<FactionDef, List<ResearchProjectDef>> getsResearch = [];
    private static List<ResearchProjectDef> restricted = [];
    public static void DisallowResearch(FactionDef faction, ResearchProjectDef researchProject)
    {
        if (!disallowedResearch.ContainsKey(faction))
        {
            disallowedResearch[faction] = [];
        }
        disallowedResearch[faction].Add(researchProject);
    }
    public static void GrantResearch(FactionDef faction, ResearchProjectDef researchProject)
    {
        if (!getsResearch.ContainsKey(faction))
        {
            getsResearch[faction] = [];
        }
        getsResearch[faction].Add(researchProject);
        if (!restricted.Contains(researchProject)) restricted.Add(researchProject);
    }
    public static void ShouldSkipPostFix(Pawn pawn, ref bool __result)
    {
        if (__result) return; //Should skip was true
        if (Find.ResearchManager.GetProject() is not ResearchProjectDef def) return;
        FactionDef factionDef = Faction.OfPlayer.def;
        //The original function asks if researching should be skipped, so we need to return true only if researching this def is disallowed
        __result = !pawn.CanUse(def) && (!disallowedResearch.ContainsKey(factionDef) || !disallowedResearch[factionDef].Contains(def));
    }

    /// <summary>
    /// Hides research if the player cannot currently access it due to restrictions.
    /// </summary>
    /// <param name="__result"></param>
    public static void ResearchVisiblePostFix(ref List<ResearchProjectDef> __result)
    {
        Faction faction = Find.FactionManager.OfPlayer;
        List<Pawn> pawns = [];
        Find.Maps.ForEach(x => pawns.AddRange(PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive.Where(x => x.Faction == faction)));
        __result.RemoveAll(def => !pawns.Any(pawn => pawn.CanUse(def)) || FactionCantUseResearch(def, faction));
    }
    public static bool AnalyzedThingsCompletedPrefix(ref int __result, ResearchProjectDef __instance)
    {
        Faction faction = Find.FactionManager.OfPlayer;
        if (FactionUnlocksResearch(__instance, faction))
        {
            __result = __instance.requiredAnalyzed?.Count()??0;
            return false;
        }
        return true;
    }
    private static bool FactionUnlocksResearch(ResearchProjectDef def, Faction faction)
    {
        return getsResearch.TryGetValue(faction.def, out List<ResearchProjectDef> defs) && defs.Contains(def);
    }
    private static bool FactionCantUseResearch(ResearchProjectDef def, Faction faction)
    {
        return disallowedResearch.ContainsKey(faction.def) && disallowedResearch[faction.def].Contains(def);
    }
}
