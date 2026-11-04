using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.Shifter;
using System.Runtime.CompilerServices;
using System.Xml;

namespace RVCRestructured;

public static class Utils
{
    private static readonly Dictionary<RaceProperties, ThingDef> thingDefCache = [];
    private static readonly Dictionary<ThingDef, PawnKindDef> pawnKindCache = [];

    private static bool subscribedToCacheReset = false;
    private static Dictionary<Pawn, DefRestrictionManager?> pawnRestrictionManager = [];

    public static ThingDef GetDef(RaceProperties race)
    {
        if (!thingDefCache.ContainsKey(race))
            thingDefCache.Add(race, DefDatabase<ThingDef>.AllDefs.First(x => x.race == race));
        return thingDefCache[race];

    }
    public static PawnKindDef GetKindDef(ThingDef race)
    {
        if (!pawnKindCache.ContainsKey(race))
            pawnKindCache.Add(race, DefDatabase<PawnKindDef>.AllDefs.First(x => x.race == race));
        return pawnKindCache[race];
    }

    public static float GetStatOffsetOrBaseFromList(this List<StatModifier> mods, StatDef stat)
    {
        return mods.GetStatValueFromList(stat, stat.defaultBaseValue);
    }

    public static bool TryGetMayRequireAttributeValues(this XmlNode node, [NotNullWhen(true)] out string? mayRequireMod, [NotNullWhen(true)] out string? mayRequireAnyMod)
    {
        mayRequireMod = null;
        mayRequireAnyMod = null;

        if (node.Attributes is not XmlAttributeCollection attributes) return false;
        mayRequireMod = attributes["MayRequire"]?.Value.ToLower() ?? string.Empty;
        mayRequireAnyMod = attributes["MayRequireAnyOf"]?.Value.ToLower() ?? string.Empty;

        return true;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVanilla(this Def def) => def.modContentPack.IsCoreMod || def.modContentPack.IsOfficialMod;
    // This cuts off 10ms
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DefRestrictionManager? GetRelevantRestrictionComp(this Pawn pawn)
    {
        // First time call, sets up the cache reset callback
        if (!subscribedToCacheReset)
        {
            WorldSpecificCacheSignaler.signalCachesReset += ResetCache;
            subscribedToCacheReset = true;
        }
        // attempt to pull a pawn's def retriction manager from cache - if it is not cached, find and add it.
        if (!pawnRestrictionManager.TryGetValue(pawn, out DefRestrictionManager? restrictionManager))
        {
            ShapeshifterComp? shifterComp = pawn.TryGetComp<ShapeshifterComp>();
            RestrictionComp? restrictionComp = pawn.TryGetComp<RestrictionComp>();
            restrictionManager = shifterComp?.GetCompProperties<RVRRestrictionComp>().Restrictions ?? restrictionComp?.Props.Restrictions;
            pawnRestrictionManager[pawn] = restrictionManager;
        }
      
        if (!ModsConfig.BiotechActive || pawn.genes==null || pawn.genes.GenesListForReading==null) return restrictionManager;

        foreach (Gene gene in pawn.genes.GenesListForReading)
        {
            if (gene.def.HasModExtension<RestrictionModExtension>())
            {
                if (restrictionManager != null)
                    restrictionManager = restrictionManager.MergeManagers(restrictionManager, gene.def.GetModExtension<RestrictionModExtension>().Restrictions);
                else
                    restrictionManager = gene.def.GetModExtension<RestrictionModExtension>().Restrictions;
            }
        }
        return restrictionManager;
    }
    /// <summary>
    /// Clean the pawn comp cache every time we change worlds
    /// </summary>
    private static void ResetCache()
    {
        pawnRestrictionManager = [];
    }

    /// <summary>
    ///     Tests a pawns shapeshifter or RestrictionComp props if a pawn can use a def
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    public static bool CanUse(this Pawn pawn, Def def)
    {
        DefRestrictionManager? restrictionManager = pawn.GetRelevantRestrictionComp();
        if (restrictionManager == null) return !RestrictionsChecker.IsRestricted(def);
        
        return restrictionManager.IsAlwaysAllowed(def) || restrictionManager[def].CanUse;
    }

    
}
