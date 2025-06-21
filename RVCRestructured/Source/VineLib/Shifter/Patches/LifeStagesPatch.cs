using System.Reflection;
using Verse;

namespace RVCRestructured.Shifter;

public static class LifeStagesPatch
{
    private static bool didCECheck=false;
    private static bool isCELoaded=false;
    public static void PostfixLifeStageIndex(ref int __result, Pawn_AgeTracker __instance)
    {
        if (!didCECheck)
        {
            VineLog.Log("Doing character editor check!");
            didCECheck = true;
            isCELoaded = ModLister.HasActiveModWithName("Character Editor");
            VineLog.Log("Found character editor was loaded. Using simple logic.", RVCLogType.Message, isCELoaded);
        }

        Pawn pawn = (Pawn)typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
        
        if (pawn?.TryGetComp<ShapeshifterComp>() is null) return;

        if (isCELoaded)
        {
            __result = pawn.RaceProps.lifeStageAges.Count - 1;
            return;
        }
        
        typeof(Pawn_AgeTracker).GetMethod("RecalculateLifeStageIndex", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(__instance, []);
    }

    public static bool PrefixCurKindLifeStage(Pawn_AgeTracker __instance, ref PawnKindLifeStage __result)
    {
        Pawn pawn = (Pawn)typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
        ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
        if (comp == null) return true;
        PawnKindDef def = Utils.GetKindDef(comp.CurrentForm);
        if (def == null) return true;
        __result = def.lifeStages.Last();
        return false;
        
    }
}
