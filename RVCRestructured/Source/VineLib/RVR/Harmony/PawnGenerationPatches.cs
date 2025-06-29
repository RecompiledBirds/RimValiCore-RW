using RimWorld;
using RVCRestructured.VineLib.Defs.DefOfs;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class PawnGenerationPatches
{
    private static Dictionary<PawnKindDef, List<SwapOption>> swapOptionsFor = [];
    internal static void GraphicsGenPostfix(ref Pawn __result) => __result.TryGetComp<RVRComp>()?.GenGraphics();


    public static bool TryGetSwapOptionsFor(PawnKindDef def, FactionDef? factionDef, out List<SwapOption>? options)
    {
        if (factionDef == null)
        {
            options = null;
            return false;
        }
        if (!swapOptionsFor.TryGetValue(def, out options))
        {
            foreach (PawnKindSwapDef swapDef in DefDatabase<PawnKindSwapDef>.AllDefsListForReading)
            {
                if (swapDef.forFactions.Contains(factionDef) || (swapDef.forAllFactionsExcept.Count > 0 && !swapDef.forAllFactionsExcept.Contains(factionDef))) continue;
                foreach (PawnKindSwapOptions swapOptions in swapDef.swapOptions)
                {
                    if (swapOptions.pawnKindDef != def || !Rand.Chance(swapOptions.chanceOfSwap)) continue;
                    options = [];
                    options.AddRange(swapOptions.swapWithOptions);
                    swapOptionsFor[def] = options;
                }
            }
        }
        return options != null;
    }
    private static Dictionary<FactionDef, HashSet<PawnKindDef>> cachedPawnKinds = [];
    private static Dictionary<FactionDef, Dictionary<PawnKindDef, PawnKindDef>> solvedBestPawnKinds = [];

    public static PawnKindDef TrySolveForMostSimilarPawnKind(HashSet<PawnKindDef> givenDefs, PawnKindDef toReplace)
    {
        float power = toReplace.combatPower;
        PawnKindDef? result = null;
        if (givenDefs.NullOrEmpty())
        {
            return toReplace;
        }
        foreach (PawnKindDef pawnKind in givenDefs)
        {
            if (result == null)
            {
                result = pawnKind;
                continue;
            }
            if (MathF.Abs(result.combatPower - power) > MathF.Abs(pawnKind.combatPower - power))
            {
                result = pawnKind;
            }
        }
        result ??= toReplace;
        return result;
    }
    public static PawnKindDef FindBestReplacementCached(FactionDef forFaction, PawnKindDef toReplace)
    {
        HashSet<PawnKindDef> kinds = GetCachedPawnKinds(forFaction);
        if(!solvedBestPawnKinds.TryGetValue(forFaction, out Dictionary<PawnKindDef, PawnKindDef>? replacers))
        {
            replacers = [];
            solvedBestPawnKinds[forFaction] = replacers;
        }
        if(replacers.TryGetValue(toReplace, out PawnKindDef? result))
        {
            return result;
        }
        replacers[toReplace] = TrySolveForMostSimilarPawnKind(kinds, toReplace);
        return replacers[toReplace];
    }
    public static HashSet<PawnKindDef> GetCachedPawnKinds(FactionDef faction)
    {
        if (cachedPawnKinds.TryGetValue(faction, out HashSet<PawnKindDef> result)) return result;
        result = [];
        if (faction.pawnGroupMakers == null)
        {
            cachedPawnKinds[faction] = result;
            return result;
        }
        foreach (PawnGroupMaker maker in faction.pawnGroupMakers)
        {
            //avoid accidentally overwriting the orignal generation list.
            List<PawnGenOption> optionsCopySafely = [];
            maker.options.CopyToList(optionsCopySafely);
            optionsCopySafely.AddRange(maker.traders);
            optionsCopySafely.AddRange(maker.carriers);
            optionsCopySafely.AddRange(maker.guards);
            foreach (PawnGenOption option in optionsCopySafely)
            {
                if (!result.Contains(option.kind)&&option.kind.RaceProps.Humanlike)
                    result.Add(option.kind);
            }
        }
        cachedPawnKinds[faction] = result;
        return result;
    }
    public static void RequestChangePrefix(ref Pawn __result, ref PawnGenerationRequest request)
    {
        if (!request.KindDef.race.race.Humanlike) return;
        FactionDef? factionDef = Faction.OfPlayerSilentFail?.def;
        if (request.Faction != null) factionDef = request.Faction.def;
        if (!TryGetSwapOptionsFor(request.KindDef, factionDef, out List<SwapOption>? options) && !options.NullOrEmpty())
        {
            SwapOption option = options.RandomElement();
            request.KindDef = option.pawnKindDef;
            if (ModsConfig.BiotechActive && option.xenotypeDef != null) request.ForcedXenotype = option.xenotypeDef;
            return;
        }
        float ratio = VineSettings.overrideBlendDefaultRatio ? VineSettings.blendRatio : FactionData.defaultRatio;
        if (!VineSettings.factionBlender && !Rand.Chance(ratio)) return;
        if (request.IsCreepJoiner||(factionDef?.isPlayer ?? false)) return;
        if(factionDef!=null&& factionDef.pawnGroupMakers != null && !(request.Faction?.def.modContentPack.IsOfficialMod??true
            || (request.Faction?.def.modContentPack.IsCoreMod ?? true))&&(request.KindDef.modContentPack.IsOfficialMod||request.KindDef.modContentPack.IsCoreMod))
        {
            request.KindDef=FindBestReplacementCached(factionDef, request.KindDef);
            return;
        }
        if (factionDef==null && !request.KindDef.defName.ToLower().Contains("refugee")) return;
        int retries = 30;
        PawnKindDef? def = null;
        HashSet<PawnKindDef>? pawnKinds = null;
        while (retries>0&&!pawnKinds.NullOrEmpty())
        {
            retries--;
            factionDef = DefDatabase<FactionDef>.AllDefsListForReading.RandomElement();
            if (factionDef.modContentPack.IsCoreMod || factionDef.modContentPack.IsOfficialMod) continue;
            pawnKinds = GetCachedPawnKinds(factionDef);
            if (pawnKinds.NullOrEmpty()) continue;

            def = FindBestReplacementCached(factionDef, request.KindDef);
            if (def != null) break;
        }
        if (def == null) return;
        request.KindDef = def;
    }

}
