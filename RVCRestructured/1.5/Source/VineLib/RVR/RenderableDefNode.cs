using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using UnityEngine;
using Verse;

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
        return rNode.RProps.def.GetPos(parms.pawn.Rotation, node.tree, parms.pawn.InBed(), parms.Portrait).position.y;
    }

    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        RenderableDefNode rNode = (RenderableDefNode)node;
        pivot = Vector3.zero;
        return rNode.RProps.def.GetPos(parms.pawn.Rotation,node.tree,parms.pawn.InBed(),parms.Portrait).position;
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
