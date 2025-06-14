﻿using RimWorld.QuestGen;
using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using UnityEngine;

namespace RVCRestructured.RVR;

public class RenderableDefNodeProperties : PawnRenderNodeProperties
{
    [AllowNull] public readonly RenderableDef def;
}

public class RNodeWorker : PawnRenderNodeWorker
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        return ((RenderableDefNode)node).RProps.def.CanDisplay(parms.pawn, parms.Portrait);
    }

    public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    {
        Pawn pawn = parms.pawn;
        if (ModsConfig.BiotechActive && pawn.ageTracker.CurLifeStage.headSizeFactor is float num)
        {
            return base.OffsetFor(node, parms, out pivot) * num;
        }
        return base.OffsetFor(node, parms, out pivot);
    }

    
}
public class RenderableDefNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode(pawn, props, tree)
{
    
    public RenderableDefNodeProperties RProps => (RenderableDefNodeProperties)props;
    public override Graphic GraphicFor(Pawn pawn)
    {
        TriColorSet set = RProps.def.ColorSet(pawn);
        return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn),Vector2.one, set[0], set[1], set[2], RProps.def.GetMaskPath(pawn));
    }

    public override GraphicMeshSet MeshSetFor(Pawn pawn)
    {
        return base.MeshSetFor(pawn);
    }

}
