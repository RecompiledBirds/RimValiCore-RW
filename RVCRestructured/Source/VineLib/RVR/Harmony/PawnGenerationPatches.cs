using RimWorld;
using RVCRestructured.VineLib.Defs.DefOfs;
using System.Globalization;
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
                if (!result.Contains(option.kind) && (option.kind.RaceProps?.Humanlike ?? false))
                    result.Add(option.kind);
            }
        }
        cachedPawnKinds[faction] = result;
        return result;
    }
    private static int pawnsGenerated = 0;
    public static void RequestChangePrefix(ref PawnGenerationRequest request)
    {
        FactionDef? factionDef = request.Faction?.def ?? Faction.OfPlayerSilentFail?.def;
        if (ModsConfig.RoyaltyActive && factionDef == FactionDefOf.Empire) return;
        if (ModsConfig.AnomalyActive && factionDef == FactionDefOf.Entities) return;
        
        if (request.IsCreepJoiner) return;
        //check if caches need clearing
        if (VineSettings.flushGenerationCaches && pawnsGenerated++ == VineSettings.flushCachesAfterHowManyPawnsGenerated)
        {
            cachedPawnKinds = [];
            solvedBestPawnKinds = [];
        }

        if (!request.KindDef.race.race.Humanlike) return;


        //try get vine pawnkind swap defs
        if (!TryGetSwapOptionsFor(request.KindDef, factionDef, out List<SwapOption>? options) && !options.NullOrEmpty())
        {
            SwapOption option = options.RandomElement();
            request.KindDef = option.pawnKindDef;
            if (ModsConfig.BiotechActive && option.xenotypeDef != null) request.ForcedXenotype = option.xenotypeDef;
            return;
        }
        if (factionDef?.isPlayer ?? false) return;
        float ratio = VineSettings.overrideBlendDefaultRatio ? VineSettings.blendRatio : FactionData.defaultRatio;
        if (!VineSettings.factionBlender || !Rand.Chance(ratio)) return;
       
        if (
            //check faction has pawn group makers
            factionDef?.pawnGroupMakers != null
            // check faction is not vanilla
            && !factionDef.IsVanilla()
            // check pawnkind is vanila
            && request.KindDef.IsVanilla())
        {
            request.KindDef = FindBestReplacementCached(factionDef, request.KindDef);
            return;
        }

        if (factionDef == null && !request.KindDef.defName.Contains("refugee", StringComparison.OrdinalIgnoreCase)) return;
        int retries = 30;
        PawnKindDef? def = null;
        while (retries-- > 0)
        {
            factionDef = DefDatabase<FactionDef>.AllDefsListForReading.RandomElement();
            if (factionDef.IsVanilla()) continue;

            def = FindBestReplacementCached(factionDef, request.KindDef);
            if (def != request.KindDef) break;
        }
        if (def == null) return;
        request.KindDef = def;
        if (!ModsConfig.BiotechActive || request.ForcedXenotype != null || !def.race.HasComp<RestrictionComp>()) return;
        RVRRestrictionComp comp = def.race.GetCompProperties<RVRRestrictionComp>();
        IEnumerable<KeyValuePair<XenotypeDef, float>> xenoTypes = PawnGenerator.XenotypesAvailableFor(request.KindDef,factionDef,request.Faction);
        if (!xenoTypes.TryRandomElementByWeight(x => x.Value, out KeyValuePair<XenotypeDef, float> keyvp)) return;
        request.ForcedXenotype = keyvp.Key;

    }

}
