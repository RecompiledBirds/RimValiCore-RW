using UnityEngine;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class ApparelScalingPatch
{
    public static void Postfix(PawnRenderNode node, PawnDrawParms parms, ref Vector3 __result)
    {
        if (node is PawnRenderNode_Apparel pawnRenderNode_Apparel)
        {
            Apparel apparel = pawnRenderNode_Apparel.apparel;
            Pawn pawn = parms.pawn;
            Vector2 scale = Vector2.one;
            GraphicsComp rVRGraphicsComp = pawn.TryGetComp<GraphicsComp>();
            if (rVRGraphicsComp != null)
            {
                if (apparel.def.apparel.parentTagDef == PawnRenderNodeTagDefOf.ApparelHead)
                {
                    scale = rVRGraphicsComp.Props.apparelScaleHead;
                }
                else
                    scale = rVRGraphicsComp.Props.apparelScaleBody;
            }
            Vector3 s = new Vector3(scale.x, 1, scale.y);
            __result = __result.MultipliedBy(s);
            return;
        }
    }
}
