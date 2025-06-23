using UnityEngine;

namespace RVCRestructured.RVR;

public class RNodeWorker_Head : RNodeWorker
{
    //public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    //{
    //    Vector3 offset = base.OffsetFor(node, parms, out pivot);
    //    if(parms.Portrait) return offset;
    //    if (parms.posture.InBed()&&!parms.pawn.CurrentBed().def.building.bed_showSleeperBody)
    //    {
    //        offset += ((RenderableDefNodeProperties_Head)node.Props).extraOffset;
    //    }

    //    return offset;
    //}
    //public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
    //{
    //    if (node is not RenderableDefNode rNode) return base.ScaleFor(node, parms);
    //    Pawn pawn = parms.pawn;
    //    if (ModsConfig.BiotechActive)
    //    {
    //        LifeStageDef lifeStageDef = pawn.ageTracker.CurLifeStage;
    //        if (lifeStageDef.bodySizeFactor is float scale && rNode.RProps.lifeStageOverrides.TryGetValue(lifeStageDef, out float extraScale))
    //        {
    //            return base.ScaleFor(node, parms) * scale * extraScale;
    //        }
    //    }
    //    return base.ScaleFor(node, parms);
    //}

}

public class RenderableDefNodeProperties_Head : RenderableDefNodeProperties
{
    public readonly Vector3 extraOffset;

    
}
