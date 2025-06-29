using UnityEngine;

namespace RVCRestructured.RVR;

public class RNodeWorker_Head : RNodeWorker
{
    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        Vector3 offset = base.OffsetFor(node, parms, out pivot);

        RenderableDefNodeProperties_Head props = (RenderableDefNodeProperties_Head)node.Props;

        if (parms.pawn.DevelopmentalStage is <= DevelopmentalStage.Child)
        {
            offset *= props.childOffsetScale;
        }

        if (parms.pawn.DevelopmentalStage is <= DevelopmentalStage.Baby)
        {
            offset *= .8f;
        }

        if (parms.Portrait) return offset;
        if (parms.posture.InBed() && !parms.pawn.CurrentBed().def.building.bed_showSleeperBody)
        {
            offset += props.extraOffset;
        }

        return offset;
    }
}

public class RenderableDefNodeProperties_Head : RenderableDefNodeProperties
{
    public readonly Vector3 extraOffset;
    public readonly float childOffsetScale;

    public RenderableDefNodeProperties_Head()
    {
        this.workerClass = typeof(RNodeWorker_Head);
    }
}
