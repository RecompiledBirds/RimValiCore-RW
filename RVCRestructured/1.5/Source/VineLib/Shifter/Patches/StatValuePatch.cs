using RimWorld;
using Verse;

namespace RVCRestructured.Shifter;

public static class StatValuePatch
{
    public static void StatPostfix(StatRequest req, StatDef ___stat, bool applyPostProcess, ref float __result)
    {
        if (req.Thing is not Pawn pawn) return;
        ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
        if(comp==null) return;
        __result += comp.OffsetStat(___stat);
    }

   
}
