using RimWorld.QuestGen;
using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR;

public class RenderableDefNodeProperties : PawnRenderNodeProperties
{
    [AllowNull] public readonly RenderableDef def;
    public List<LifeStageRenderableDefOverride> lifeStageScaleOverrides = [];
    public Dictionary<LifeStageDef, float> lifeStageOverrides = [];
    public override void ResolveReferences()
    {
        foreach(LifeStageRenderableDefOverride lifeStageRenderableDefOverride in lifeStageScaleOverrides)
        {
            lifeStageOverrides[lifeStageRenderableDefOverride.lifeStage] = lifeStageRenderableDefOverride.scale;
        }
        base.ResolveReferences();
       
    }
    public RenderableDefNodeProperties()
    {
        this.nodeClass = typeof(RenderableDefNode);
        this.workerClass = typeof(RNodeWorker);
    }
    
}
public class LifeStageRenderableDefOverride
{
    [AllowNull]
    public LifeStageDef lifeStage;
    public float scale;
}
public class RNodeWorker : PawnRenderNodeWorker
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        return ((RenderableDefNode)node).RProps.def.CanDisplay(parms.pawn, parms.Portrait);
    }
    //public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
    //{
    //    if (node is not RenderableDefNode rNode) return base.ScaleFor(node, parms);
    //    Pawn pawn = parms.pawn;
    //    if (ModsConfig.BiotechActive)
    //    {
    //        LifeStageDef lifeStageDef = pawn.ageTracker.CurLifeStage;
    //        if (lifeStageDef.bodySizeFactor is float scale &&rNode.RProps.lifeStageOverrides.TryGetValue(lifeStageDef, out float extraScale))
    //        {
    //            return base.ScaleFor(node, parms) * scale * extraScale;
    //        }
    //    }
    //    return base.ScaleFor(node, parms);
    //}

    //public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
    //{
    //    Vector3 baseVector = base.OffsetFor(node, parms, out pivot);

    //    if (node is not RenderableDefNode rNode)
    //    {
    //        return baseVector;
    //    }

    //    Pawn pawn = parms.pawn;
    //    if (!pawn.DevelopmentalStage.Child())
    //    {
    //        return baseVector;
    //    }

    //    rNode.RProps.

    //    return baseVector;
    //}
}
public class RenderableDefNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : PawnRenderNode(pawn, props, tree)
{

    public RenderableDefNodeProperties RProps => (RenderableDefNodeProperties)props;
    public override Graphic GraphicFor(Pawn pawn)
    {
        Vector2 scale = Vector2.one;
     
        TriColorSet set = RProps.def.ColorSet(pawn);
        return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn),scale, set[0], set[1], set[2], RProps.def.GetMaskPath(pawn));
    }

}
