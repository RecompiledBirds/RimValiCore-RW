using RimWorld;
using RimWorld.BaseGen;
using RVCRestructured.Graphics;
using RVCRestructured.Shifter;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ResolveGraphicsPostFix
    {
        private static RVG_Graphic empty;

        public static RVG_Graphic GetEmpty
        {
            get
            {
                if (empty == null)
                {
                    empty = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
                }
                return empty;
            }
        }
        private static Color SwaddleColor(PawnGraphicSet __instance)
        {
            Rand.PushState(__instance.pawn.thingIDNumber);
            float num = Rand.Range(0.6f, 0.89f);
            float num2 = Rand.Range(-0.1f, 0.1f);
            float num3 = Rand.Range(-0.1f, 0.1f);
            float num4 = Rand.Range(-0.1f, 0.1f);
            Rand.PopState();
            return new Color(num + num2, num + num3, num + num4);
        }
        public static bool ResolveGraphicsPatch(PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            if (!pawn.RaceProps.Humanlike && shapeshifterComp == null) return true;
            
            GraphicsComp graphicsComp = pawn.TryGetComp<GraphicsComp>();
            if (graphicsComp == null) return true;
            __instance.swaddledBabyGraphic = GetEmpty;
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            comp.GenGraphics();
            __instance.ClearCache();
            if (comp.ShouldResetGraphics)
            {
                __instance.SetAllGraphicsDirty(); ;
                PortraitsCache.SetDirty(pawn);
            }
            __instance.CalculateHairMats();
            __instance.ResolveGeneGraphics();

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
            #region shapeshifter

            if (shapeshifterComp!=null && !shapeshifterComp.CurrentForm.race.Humanlike)
            {
                ShapeShifterGraphicsResolver.ResolveNonHuman(shapeshifterComp, pawn, __instance, GetEmpty);
                return false;
            }

            if (shapeshifterComp != null && !shapeshifterComp.IsParentDef())
            {
                ShapeShifterGraphicsResolver.ResolveHumanLike(shapeshifterComp, comp, __instance, pawn, skinTwo, skinThree,GetEmpty);
                return false;
            }
            #endregion
            #region unshifted

            __instance.bodyTattooGraphic = GetEmpty;
            __instance.faceTattooGraphic = GetEmpty;
            __instance.furCoveredGraphic = GetEmpty;

            __instance.desiccatedHeadGraphic = GetEmpty;
            __instance.dessicatedGraphic = GetEmpty;
            __instance.desiccatedHeadStumpGraphic = GetEmpty;

            __instance.headStumpGraphic = GetEmpty;

            __instance.nakedGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(graphicsComp.Props.bodyTex, graphicsComp.Props.bodySize, skinOne, skinTwo, skinThree);
            if (!graphicsComp.Props.hasHair)
            {
                __instance.beardGraphic = GetEmpty;
                __instance.hairGraphic = GetEmpty;
            }
            __instance.headGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(graphicsComp.Props.headTex, graphicsComp.Props.headSize, skinOne, skinTwo, skinThree);

            if (ModsConfig.BiotechActive)
            {
                __instance.geneGraphics = new System.Collections.Generic.List<GeneGraphicRecord>();
                __instance.swaddledBabyGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Apparel/SwaddledBaby/Swaddled_Child", ShaderDatabase.Cutout, Vector2.one, SwaddleColor(__instance));
            }
            if (__instance.pawn.style != null && ModsConfig.IdeologyActive && ModLister.BiotechInstalled && __instance.pawn.genes != null && !__instance.pawn.genes.GenesListForReading.Any((Gene x) => x.def.graphicData != null && !x.def.graphicData.tattoosVisible && x.Active))
            {

                Color skinColor = __instance.pawn.story.SkinColor;
                skinColor.a *= 0.8f;
                if (__instance.pawn.style.FaceTattoo != null && __instance.pawn.style.FaceTattoo != TattooDefOf.NoTattoo_Face)
                {
                    __instance.faceTattooGraphic = GraphicDatabase.Get<Graphic_Multi>(__instance.pawn.style.FaceTattoo.texPath, ShaderDatabase.CutoutSkinOverlay, Vector2.one, skinColor, Color.white, null, __instance.pawn.story.headType.graphicPath);
                }
                else
                {
                    __instance.faceTattooGraphic = null;
                }
                if (__instance.pawn.style.BodyTattoo != null && __instance.pawn.style.BodyTattoo != TattooDefOf.NoTattoo_Body)
                {
                    __instance.bodyTattooGraphic = GraphicDatabase.Get<Graphic_Multi>(__instance.pawn.style.BodyTattoo.texPath, ShaderDatabase.CutoutSkinOverlay, Vector2.one, skinColor, Color.white, null, __instance.pawn.story.bodyType.bodyNakedGraphicPath);
                }
                else
                {
                    __instance.bodyTattooGraphic = null;
                }

            }
            __instance.ResolveApparelGraphics();
            if (comp.ShouldResetGraphics)
            {
                __instance.SetAllGraphicsDirty();
                __instance.ClearCache();
                PortraitsCache.SetDirty(pawn);
            }
            return false;
            #endregion
        }





    }
}
