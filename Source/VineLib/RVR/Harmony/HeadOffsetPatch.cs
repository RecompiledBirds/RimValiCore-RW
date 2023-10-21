using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.RVR;
using RVCRestructured.Shifter;
using UnityEngine;
using Verse;

namespace RVCRestructured.Source.RVR.Harmony
{
    public static class HeadOffsetPatch
    {
        public static void Postfix(ref Vector3 __result, Rot4 rotation, PawnRenderer __instance)
        {
            Pawn pawn = __instance.graphics.pawn;
            GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
            ShapeshifterComp shapeshifterComp;
            if (comp == null)
            {
                shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
                if (shapeshifterComp == null) return;
                __result = ShiftedHeadOffset(shapeshifterComp, __result,rotation);
            }

            RenderableDef renderableDef = comp.Props.renderableDefs.Find(x => x.bodyPart == BodyPartDefOf.Head.defName);

            if (renderableDef != null)
                __result = new Vector3(renderableDef.GetPos(pawn).position.x,__result.y,renderableDef.GetPos(pawn).position.z);
            
            shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            if (shapeshifterComp == null) return;
            __result= ShiftedHeadOffset(shapeshifterComp,__result,rotation);

        }

        public static Vector3 ShiftedHeadOffset(ShapeshifterComp shapeshifterComp, Vector3 __result,Rot4 rotation)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            Pawn pawn = (Pawn)shapeshifterComp.parent;
            if (comp == null)
            {
                Vector2 vector = shapeshifterComp.MimickedBodyType.headOffset * Mathf.Sqrt(pawn.ageTracker.CurLifeStage.bodySizeFactor);
                switch (rotation.AsInt)
                {
                    case 0:
                        return new Vector3(0f, 0f, vector.y);
                    case 1:
                        return new Vector3(vector.x, 0f, vector.y);
                    case 2:
                        return new Vector3(0f, 0f, vector.y);
                    case 3:
                        return new Vector3(-vector.x, 0f, vector.y);
                    default:
                        Log.Error("BaseHeadOffsetAt error in " + pawn);
                        return Vector3.zero;
                }
            }
            return __result;

        }
    }
}
