using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR
{
    public class RenderableDefNodeProperties : PawnRenderNodeProperties
    {
        public RenderableDef def;
    }

    public class RNodeWorker : PawnRenderNodeWorker
    {


        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            RenderableDefNode rNode = node as RenderableDefNode;
            pivot = Vector3.zero;
            return rNode.RProps.def.GetPos(parms.pawn.Rotation,node.tree,parms.pawn.InBed(),parms.Portrait).position;
        }
    }
    public class RenderableDefNode : PawnRenderNode
    {
        public RenderableDefNodeProperties RProps
        {
            get
            {
                return props as RenderableDefNodeProperties;
            }
        }
        public RenderableDefNode(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }
        public override GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            return HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn, 1, 1);
        }

      

        public override Graphic GraphicFor(Pawn pawn)
        {
            
            TriColorSet set = RProps.def.ColorSet(pawn);
           
            return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn), RProps.def.GetPos(pawn.Rotation).size, set[0], set[1], set[2], RProps.def.GetMaskPath(pawn));            
        }
    }
}
