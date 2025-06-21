using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.Shifter;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.Harmony;

public static class HeadOffsetPatch
{
    public static void Postfix(ref Vector3 __result, Rot4 rotation, PawnRenderer __instance)
    {
        Pawn pawn = __instance.renderTree.pawn;

        ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
        if (shapeshifterComp == null) { return; }

        __result = ShiftedHeadOffset(pawn,shapeshifterComp, rotation);
        return;



    }

    public static Vector3 ShiftedHeadOffset(Pawn pawn,ShapeshifterComp shapeshifterComp,Rot4 rotation)
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
                RVCLog.Error("Shifted-BaseHeadOffsetAt error in " + pawn);
                return Vector3.zero;
        }

    }
}
