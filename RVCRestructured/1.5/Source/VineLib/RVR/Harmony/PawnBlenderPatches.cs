using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class PawnBlenderPatches
{
    private static List<ExcludedRaceFactionShuffleDef> ExcludedFactions = DefDatabase<ExcludedRaceFactionShuffleDef>.AllDefsListForReading;
    private static List<ExcludedRaceShuffleDef> ExcludedDefs => DefDatabase<ExcludedRaceShuffleDef>.AllDefsListForReading;
    private static List<RaceSwapDef> ShuffleDefs => DefDatabase<RaceSwapDef>.AllDefsListForReading;

    private static ThingDef[] HumanLikeRaces { get; } = DefDatabase<ThingDef>.AllDefsListForReading.Where(IsValidRaceDef).ToArray();

    private static bool skipOnce = false;
    private static ThingDef? modifyThingMakerDef;

    public static void SkipReplacementGeneratorOnce() => skipOnce = true;

    internal static void GraphicsGenPostfix(ref Pawn __result) => __result.TryGetComp<RVRComp>()?.GenGraphics();

    public static void ModifyThingMaker(ref ThingDef def)
    {
        if (!VineMod.VineSettings.RaceBlender) return;
        if (modifyThingMakerDef == null) return;
        def = modifyThingMakerDef;
        modifyThingMakerDef = null;
    }

    internal static void ModifyPawnGenerationRequest(ref PawnGenerationRequest request)
    {
        if (!VineMod.VineSettings.RaceBlender) return;
        if (request.KindDef.race == null) return;
        ThingDef def = request.KindDef.race;
        FactionDef factionDef = request.Faction.def;
        if (ExcludedFactions.All(x => x.excludedFactions.Contains(factionDef)))
        {
            return;
        }
        //safety check for scenario pawns
        if (!CanModify(in request)) return;
        if (!CanSwapRace(def)) return;

        if (skipOnce)
        {
            skipOnce = false;
            return;
        }

        if (ShouldSwitchPawnkindBased(in request))
        {
            RaceSwapDef randomSwapDef = ShuffleDefs.Where(x => x.TargetRaces.Contains(def)).RandomElement();
            modifyThingMakerDef = randomSwapDef.ReplacementRaces.RandomElement();
            return;
        }

        if (Rand.Chance(0.3f) && CanSwapPawnkind(request.KindDef))
        {
            modifyThingMakerDef = HumanLikeRaces.AsSpan().RandomElement();
        }
    }

    private static bool IsValidRaceDef(ThingDef def)
    {
        return (def.race?.Humanlike ?? false) && !def.IsCorpse;
    }

    private static bool CanModify(ref readonly PawnGenerationRequest request)
    {
        return !request.Context.HasFlag(PawnGenerationContext.PlayerStarter) && request.KindDef.RaceProps.Humanlike;
    }

    private static bool ShouldSwitchPawnkindBased(ref readonly PawnGenerationRequest request)
    {
        ThingDef race = request.KindDef.race;
        return ShuffleDefs.Count > 0 && ShuffleDefs.Any(x => x.TargetRaces.Contains(race));
    }

    private static bool CanSwapRace(ThingDef def) =>
        !ExcludedDefs.Any(x => x.ExcludedRaces.Contains(def));

    private static bool CanSwapPawnkind(PawnKindDef def) =>
        ExcludedDefs.NullOrEmpty() || !ExcludedDefs.Any(x => !x.ExcludedPawnKinds.NullOrEmpty() && x.ExcludedPawnKinds.Contains(def));
}
