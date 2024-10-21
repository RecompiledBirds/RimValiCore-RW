using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class PawnBlenderPatches
{
    private static List<ExcludedRaceShuffleDef> ExcludedDefs => DefDatabase<ExcludedRaceShuffleDef>.AllDefsListForReading;
    private static List<RaceSwapDef> ShuffleDefs => DefDatabase<RaceSwapDef>.AllDefsListForReading;

    private static ThingDef[] HumanLikeRaces { get; } = DefDatabase<ThingDef>.AllDefsListForReading.Where(IsValidRaceDef).ToArray();

    private static bool skipOnce = false;
    private static ThingDef? modifyThingMakerDef;

    public static void SkipReplacementGeneratorOnce() => skipOnce = true;

    internal static void GraphicsGenPostfix(ref Pawn __result) => __result.TryGetComp<RVRComp>()?.GenGraphics();

    public static void ModifyThingMaker(ref ThingDef def)
    {
        if (modifyThingMakerDef == null) return;
        def = modifyThingMakerDef;
        modifyThingMakerDef = null;
    }

    internal static void ModifyPawnGenerationRequest(ref PawnGenerationRequest request)
    {
        ThingDef def = request.KindDef.race;

        if (!VineMod.VineSettings.RaceBlender) return;

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
            RaceSwapDef randomSwapDef = ShuffleDefs.Where(x => x.targetRaces.Contains(def)).RandomElement();
            modifyThingMakerDef = randomSwapDef.replacementRaces.RandomElement();
            return;
        }

        if (Rand.Chance(0.3f) && CanSwapPawnkind(request.KindDef))
        {
            modifyThingMakerDef = HumanLikeRaces.AsSpan().RandomElement();
        }
    }

    private static bool IsValidRaceDef(ThingDef def)
    {
        if (!(def.race?.Humanlike ?? false)) return false;
        return !def.IsCorpse;
    }

    private static bool CanModify(ref readonly PawnGenerationRequest request)
    {
        if (request.Context.HasFlag(PawnGenerationContext.PlayerStarter)) return false;
        if (!request.KindDef.RaceProps.Humanlike) return false;

        return true;
    }

    private static bool ShouldSwitchPawnkindBased(ref readonly PawnGenerationRequest request)
    {
        ThingDef race = request.KindDef.race;
        return ShuffleDefs.Count > 0 && ShuffleDefs.Any(x => x.targetRaces.Contains(race));
    }

    private static bool CanSwapRace(ThingDef def) =>
        !ExcludedDefs.Any(x => x.excludedRaces.Contains(def));

    private static bool CanSwapPawnkind(PawnKindDef def) =>
        ExcludedDefs.NullOrEmpty() || !ExcludedDefs.Any(x => !x.excludedPawnKinds.NullOrEmpty() && x.excludedPawnKinds.Contains(def));
}
