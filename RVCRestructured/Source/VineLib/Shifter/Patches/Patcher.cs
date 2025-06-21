using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.Shifter;

[StaticConstructorOnStartup]
public static class Patcher
{
    static Patcher()
    {
        ShifterPatches();

    }

    private static void ShifterPatches()
    {
        Harmony harmony = new("Vine.Shifter");
        
        harmony.Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized)), postfix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.StatPostfix)));
        harmony.Patch(AccessTools.Method(typeof(RaceProperties), nameof(RaceProperties.SpecialDisplayStats)), postfix: new HarmonyMethod(typeof(StatDrawEntryPatches), nameof(StatDrawEntryPatches.RacePostfix)));
        harmony.Patch(AccessTools.Method(typeof(Def), nameof(Def.SpecialDisplayStats)), postfix: new HarmonyMethod(typeof(StatDrawEntryPatches), nameof(StatDrawEntryPatches.SourcePostFix)));
        harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Def.SpecialDisplayStats)), postfix: new HarmonyMethod(typeof(StatDrawEntryPatches), nameof(StatDrawEntryPatches.PawnPostfix)));
        harmony.Patch(AccessTools.Method(typeof(Pawn), "get_RaceProps"), prefix: new HarmonyMethod(typeof(RacePropsPatch), nameof(RacePropsPatch.RacePropsPrefix)));

        harmony.Patch(AccessTools.Method(typeof(Pawn_AgeTracker), "get_CurKindLifeStage"), prefix: new HarmonyMethod(typeof(LifeStagesPatch), nameof(LifeStagesPatch.PrefixCurKindLifeStage)));
        //INCOMPATIBLE WITH CHARACTER EDITOR.
        harmony.Patch(AccessTools.Method(typeof(Pawn_AgeTracker), "get_CurLifeStageIndex"), postfix: new HarmonyMethod(typeof(LifeStagesPatch), nameof(LifeStagesPatch.PostfixLifeStageIndex)));
        VineLog.Log("Ran shifter patches.");
    }

}
