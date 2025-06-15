using Verse;

namespace RVCRestructured.Shifter;

public static class RacePropsPatch
{
    public static bool RacePropsPrefix(ref RaceProperties __result, Pawn __instance)
    {
        ShapeshifterComp comp = __instance.TryGetComp<ShapeshifterComp>();
        if (comp == null) return true;
        __result = comp.RaceProperties;
        return false;

    }
}
