using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.RVR;
using UnityEngine;
using Verse;

namespace RVCRestructured.Source.RVR.Harmony
{
    public static class HeadOffsetPatch
    {
        public static void Postfix(ref Vector3 __result, PawnRenderer __instance)
        {
            Pawn pawn = __instance.graphics.pawn;
            GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
            if (comp==null)
                return;

            RenderableDef renderableDef = comp.Props.renderableDefs.Find(x => x.bodyPart == BodyPartDefOf.Head.defName);

            if (renderableDef == null)
                return;

            __result = new Vector3(renderableDef.GetPos(pawn).position.x,__result.y,renderableDef.GetPos(pawn).position.z);
        }
    }
}
