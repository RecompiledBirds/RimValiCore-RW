using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using UnityEngine;

namespace RVCRestructured.RVR;

public class RenderableDefNodeProperties : PawnRenderNodeProperties
{
    public readonly RenderableDef def = null!;
}

public class RNodeWorker : PawnRenderNodeWorker
{
    public override float LayerFor(PawnRenderNode node, PawnDrawParms parms)
    {
        RenderableDefNode rNode = (RenderableDefNode)node;
        return rNode.RProps.def.GetPos(parms.pawn.Rotation, parms.pawn.InBed(), parms.Portrait).position.y;
    }

    public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
    {
        RenderableDefNode rNode = (RenderableDefNode)node;
        Vector2 size = rNode.RProps.def.GetPos(parms.pawn.Rotation, parms.pawn.InBed(), parms.Portrait).size;
        return new Vector3 (size.x,1,size.y);
    }

    //protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
    //{
    //    if (!((RenderableDefNode)node).RProps.def.CanDisplay(parms.pawn, parms.Portrait)) return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("Empty");
    //    return base.GetGraphic(node, parms);
    //}

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        return ((RenderableDefNode)node).RProps.def.CanDisplay(parms.pawn, parms.Portrait);
    }
    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        RenderableDefNode rNode = (RenderableDefNode)node;
        Vector3 pos = rNode.RProps.def.GetPos(parms.pawn.Rotation, parms.pawn.InBed(), parms.Portrait).position;
        Vector3 newPos = new Vector3(pos.x,0,pos.z);
        Quaternion quat = base.RotationFor(node, parms);
        pivot = newPos.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 114.59156f); 
        return pos;
    }

    
}
public class RenderableDefNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode(pawn, props, tree)
{
    public RenderableDefNodeProperties RProps => (RenderableDefNodeProperties)props;

    public override GraphicMeshSet MeshSetFor(Pawn pawn) => HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn, 1, 1);

    public override Graphic GraphicFor(Pawn pawn)
    {
        TriColorSet set = RProps.def.ColorSet(pawn);

        //TODO: Check if this works?
        return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn), RProps.def.GetPos(pawn.Rotation).size, set[0], set[1], set[2], RProps.def.GetMaskPath(pawn));
    }
}
