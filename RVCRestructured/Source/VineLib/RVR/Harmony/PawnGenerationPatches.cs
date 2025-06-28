using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class PawnGenerationPatches
{
    private static Dictionary<PawnKindDef, List<PawnKindDef>> swapOptionsFor = [];
    internal static void GraphicsGenPostfix(ref Pawn __result) => __result.TryGetComp<RVRComp>()?.GenGraphics();


    public static bool TryGetSwapOptionsFor(PawnKindDef def, FactionDef factionDef, out List<PawnKindDef>? options)
    {
        if(!swapOptionsFor.TryGetValue(def, out options))
        {
            foreach(PawnKindSwapDef swapDef in DefDatabase<PawnKindSwapDef>.AllDefsListForReading)
            {
                if (swapDef.forFactions.Contains(factionDef)) continue;
                foreach(PawnKindSwapOptions swapOptions in swapDef.swapOptions)
                {
                    if (swapOptions.pawnKindDef != def) continue;
                    options = [];
                    options.AddRange(swapOptions.swapWithOptions);
                    swapOptionsFor[def] = options;
                }
            }
        }
        return options != null;
    }
    public static void RequestChangePrefix(ref Pawn __result, PawnGenerationRequest request)
    {
        if (!request.KindDef.race.race.Humanlike) return;
        if (!TryGetSwapOptionsFor(request.KindDef,request.Faction.def, out List<PawnKindDef>? options)) return;
        if (options == null) return;
        request.KindDef = options.RandomElementWithFallback(request.KindDef);

    }
}
