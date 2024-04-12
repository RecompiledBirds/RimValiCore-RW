using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR
{
    public class RenderableDefNodeProperties : PawnRenderNodeProperties
    {
        public RenderableDef def;
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
            return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(RProps.def.GetTexPath(pawn), RProps.def.GetTexPath(pawn)+"m");            
        }
    }
}
