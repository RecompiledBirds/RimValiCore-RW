using Verse;

namespace RVCRestructured.Shifter;

public static class RacePropsPatch
{
    public static bool RacePropsPrefix(ref RaceProperties __result, Pawn __instance)
    {
        if (!__instance.TryGetComp<ShapeshifterComp>(out ShapeshifterComp comp)) return true;
        __result = comp.RaceProperties;
        return false;

    }
}
