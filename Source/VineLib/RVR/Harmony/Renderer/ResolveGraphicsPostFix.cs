using RimWorld;
using RVCRestructured.Graphics;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ResolveGraphicsPostFix
    {
        public static void ResolveGraphicsPatch(PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            GraphicsComp graphicsComp = pawn.TryGetComp<GraphicsComp>();
            if (graphicsComp == null)
                return;

            RVRComp comp = pawn.TryGetComp<RVRComp>();
            comp.GenGraphics();
            Color skinOne = pawn.story.SkinColor;
            Color skinTwo = pawn.story.SkinColor;
            Color skinThree = pawn.story.SkinColor;
            if (graphicsComp.Props.skinColorSet != null)
            {
                string set = graphicsComp.Props.skinColorSet;
                skinOne = comp[set][0];
                skinTwo = comp[set][1];
                skinThree = comp[set][2];
            }
            __instance.geneGraphics = new System.Collections.Generic.List<GeneGraphicRecord>();
            __instance.bodyTattooGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.faceTattooGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.furCoveredGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");

            __instance.desiccatedHeadGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.dessicatedGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.desiccatedHeadStumpGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");

            __instance.headStumpGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");

            __instance.nakedGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(graphicsComp.Props.bodyTex, graphicsComp.Props.bodySize, skinOne, skinTwo, skinThree);
            if (!graphicsComp.Props.hasHair)
            {
                __instance.beardGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
                __instance.hairGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            }
            __instance.headGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(graphicsComp.Props.headTex, graphicsComp.Props.headSize, skinOne, skinTwo, skinThree);
            __instance.SetApparelGraphicsDirty();
            PortraitsCache.SetDirty(pawn);
        }
    }
}
