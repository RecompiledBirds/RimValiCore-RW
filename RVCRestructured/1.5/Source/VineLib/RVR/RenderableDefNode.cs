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
    public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
    {
        return base.RotationFor(node, parms);
    }

    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        return ((RenderableDefNode)node).RProps.def.CanDisplay(parms.pawn, parms.Portrait);
    }
    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        Vector3 pos = ((RenderableDefNode)node).RProps.def.GetPos(parms.pawn.Rotation, parms.pawn.InBed(), parms.Portrait).position;
        pivot = PivotFor(node, parms);
        return pos+base.OffsetFor(node,parms,out pivot);
    }

     
    
    
    protected override Vector3 PivotFor(PawnRenderNode node, PawnDrawParms parms)
    {
        
        Vector3 pos = ((RenderableDefNode)node).RProps.def.GetPos(parms.pawn.Rotation, parms.pawn.InBed(), parms.Portrait).position;
        Quaternion quat = RotationFor(node, parms);
        Vector3 pivot = pos.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 114.60f);
        return pivot;
    }

}
public class RenderableDefNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode(pawn, props, tree)
{
    public RenderableDefNodeProperties RProps => (RenderableDefNodeProperties)props;

    public override GraphicMeshSet MeshSetFor(Pawn pawn) => MeshPool.GetMeshSetForSize(RProps.def.GetPos(pawn.Rotation).size);

    //is this section of code actually helping? no idea!
    public override Mesh GetMesh(PawnDrawParms parms)
    {
        //if we dont do this little check here
        //it crashes your some really important drivers!
        //or at least it does on my computer
        if(rvrGraphic==null)
            return base.GetMesh(parms);
        return rvrGraphic.MeshAt(parms.facing);
    }

    protected override bool FlipGraphic
    {
        get
        {
            return !ContentFinder<Texture2D>.Get($"{RProps.def.GetTexPath(pawn)}_west");
        }
    }
    //end section of code that might or might not be helping

    RVG_Graphic? rvrGraphic = null;
    public override Graphic GraphicFor(Pawn pawn)
    {
        TriColorSet set = RProps.def.ColorSet(pawn);
        rvrGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn), RProps.def.GetPos(pawn.Rotation).size, set[0], set[1], set[2], RProps.def.GetMaskPath(pawn)); ;
        return rvrGraphic;
    }
}
