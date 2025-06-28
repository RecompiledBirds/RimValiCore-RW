using RimWorld;
using Verse;

namespace RVCRestructured.Shifter;

public static class StatValuePatch
{
    public static void StatPostfix(StatRequest req, StatDef ___stat, bool applyPostProcess, ref float __result)
    {
        if (req.Thing is not Pawn pawn) return;
        if (!pawn.TryGetComp(out ShapeshifterComp comp)) return;
        __result += comp.OffsetStat(___stat);
    }

   
}
