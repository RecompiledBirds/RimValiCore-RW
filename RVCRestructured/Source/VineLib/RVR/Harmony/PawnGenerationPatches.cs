using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class PawnGenerationPatches
{

    internal static void GraphicsGenPostfix(ref Pawn __result) => __result.TryGetComp<RVRComp>()?.GenGraphics();
}
