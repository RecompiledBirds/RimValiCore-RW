using UnityEngine;

namespace RVCRestructured.RVR;

public class RNodeWorker_Head : RNodeWorker
{
    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        Vector3 offset = base.OffsetFor(node, parms, out pivot);

        if (parms.posture.InBed())
        {
            offset += ((RenderableDefNodeProperties_Head)node.Props).extraOffset;
        }

        return offset;
    }
}

public class RenderableDefNodeProperties_Head : RenderableDefNodeProperties
{
    public readonly Vector3 extraOffset;
}
