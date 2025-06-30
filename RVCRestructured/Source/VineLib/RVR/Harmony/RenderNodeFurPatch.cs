namespace RVCRestructured.RVR.HarmonyPatches;
public static class RenderNodeFurPatch
{
    public static bool Prefix(Pawn pawn, ref Graphic? __result)
    {
        __result = GraphicDatabase.Get<Graphic_Multi>("RVC/Empty");
        return !(pawn.TryGetComp(out VineSpeciesConfigComp? comp) && (comp?.Props.disableGeneFurRendering ?? false));
    }
}
