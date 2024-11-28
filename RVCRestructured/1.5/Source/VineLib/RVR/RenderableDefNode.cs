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
        return rNode.RProps.def.GetPos(parms.facing, PawnLyingDown(parms.pawn), parms.Portrait).position.y+base.LayerFor(node,parms);
    }

    public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
    {
        RenderableDefNode rNode = (RenderableDefNode)node;
        Vector2 size = rNode.RProps.def.GetPos(parms.facing, PawnLyingDown(parms.pawn), parms.Portrait).size;
        return new Vector3 (size.x,1,size.y).MultipliedBy(base.ScaleFor(node,parms));
    }

    

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        return ((RenderableDefNode)node).RProps.def.CanDisplay(parms.pawn, parms.Portrait)&&base.CanDrawNow(node,parms);
    }
    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        BodyPartGraphicPos pos = ((RenderableDefNode)node).RProps.def.GetPos(parms.facing, PawnLyingDown(parms.pawn), parms.Portrait);
        RVCLog.Log($"Offset for {((RenderableDefNode)node).RProps.def} : { parms.pawn.Name.ToStringShort} with following parms {parms.pawn.Rotation},{parms.pawn.InBed()}, {parms.Portrait} is: {pos.position}",debugOnly:true);
        return pos.position+base.OffsetFor(node,parms,out pivot);
    }

    //protected override Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
    //{
    //    RenderableDefNodeProperties RProps = ((RenderableDefNode)node).RProps;
    //    RenderableDef def = RProps.def;
    //    Pawn pawn = parms.pawn;
    //    TriColorSet set = RProps.def.ColorSet(pawn);
    //    return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(def.GetTexPath(pawn), def.GetPos(parms.facing).size, set[0], set[1], set[2], def.GetMaskPath(pawn));
    //}


    protected override Vector3 PivotFor(PawnRenderNode node, PawnDrawParms parms)
    {
        
        Vector3 pos = ((RenderableDefNode)node).RProps.def.GetPos(parms.facing,PawnLyingDown(parms.pawn) , parms.Portrait).position;
        Quaternion quat = RotationFor(node, parms);
        Vector3 pivot = pos.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 114.60f);
        return pivot;
    }

    

    private bool PawnLyingDown(Pawn pawn)
    {
        return pawn.InBed() || pawn.DeadOrDowned;
    }

}
public class RenderableDefNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode(pawn, props, tree)
{
    public RenderableDefNodeProperties RProps => (RenderableDefNodeProperties)props;

    public override GraphicMeshSet MeshSetFor(Pawn pawn) => MeshPool.GetMeshSetForSize(RProps.def.GetPos(pawn.Rotation).size);

    public override Mesh GetMesh(PawnDrawParms parms)
    {
        return rvrGraphic?.MeshAt(parms.pawn.Rotation)??base.GetMesh(parms);
    }
    RVG_Graphic? rvrGraphic = null;
    public override Graphic GraphicFor(Pawn pawn)
    {
        TriColorSet set = RProps.def.ColorSet(pawn);
        rvrGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn), RProps.def.GetPos(pawn.Rotation).size, set[0], set[1], set[2], RProps.def.GetMaskPath(pawn));
        return rvrGraphic;
    }
}
