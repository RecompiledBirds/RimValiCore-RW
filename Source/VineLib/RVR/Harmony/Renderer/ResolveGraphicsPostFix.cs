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
                if(empty == null)
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
            if(!__instance.pawn.RaceProps.Humanlike)return true;
            Pawn pawn = __instance.pawn;
            GraphicsComp graphicsComp = pawn.TryGetComp<GraphicsComp>();
            if (graphicsComp == null) return true;
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            comp.GenGraphics();
            __instance.ClearCache();
            if (comp.ShouldResetGraphics)
            {
                __instance.SetAllGraphicsDirty();;
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
            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            if (shapeshifterComp != null && !shapeshifterComp.IsParentDef())
            {
                __instance.bodyTattooGraphic = empty;
                __instance.faceTattooGraphic = empty;
                __instance.furCoveredGraphic = empty;

                __instance.desiccatedHeadGraphic = HeadGraphicShifted(shapeshifterComp, skinTwo, skinThree,true);
                __instance.dessicatedGraphic = empty;
                

                __instance.headGraphic = HeadGraphicShifted(shapeshifterComp, skinTwo, skinThree);
                if (UsesCustomHead(shapeshifterComp))
                {
                    Color color = pawn.story.SkinColorOverriden ? (PawnGraphicSet.RottingColorDefault * pawn.story.SkinColor) : PawnGraphicSet.RottingColorDefault;
                    __instance.desiccatedHeadStumpGraphic = HeadTypeDefOf.Stump.GetGraphic(color, false, pawn.story.SkinColorOverriden);
                    __instance.headStumpGraphic = HeadTypeDefOf.Stump.GetGraphic(pawn.story.SkinColor, false, pawn.story.SkinColorOverriden);
                }
                else
                {
                    __instance.desiccatedHeadStumpGraphic = empty;
                    __instance.headStumpGraphic = empty;
                }
                __instance.nakedGraphic = BodyGraphicShifted(shapeshifterComp,skinTwo,skinThree);

                if (!ShiftedHasHair(shapeshifterComp))
                {
                    __instance.beardGraphic = empty;
                    __instance.hairGraphic = empty;
                }
                else
                {
                    Color hairColor = pawn.story.HairColor;
                    if (pawn.story.hairDef != null)
                    {
                        __instance.hairGraphic = pawn.story.hairDef.noGraphic ? null : GraphicDatabase.Get<Graphic_Multi>(pawn.story.hairDef.texPath, ShaderDatabase.Transparent, Vector2.one, hairColor);
                    }
                    Pawn_StyleTracker style = pawn.style; ;
                    if (style != null && style.beardDef != null && !style.beardDef.noGraphic)
                    {
                        __instance.beardGraphic = GraphicDatabase.Get<Graphic_Multi>(style.beardDef.texPath, ShaderDatabase.Transparent, Vector2.one, hairColor);
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
            }
            #endregion
            #region unshifted

            __instance.bodyTattooGraphic = empty;
            __instance.faceTattooGraphic = empty;
            __instance.furCoveredGraphic = empty;

            __instance.desiccatedHeadGraphic = empty;
            __instance.dessicatedGraphic = empty;
            __instance.desiccatedHeadStumpGraphic = empty;

            __instance.headStumpGraphic = empty;

            __instance.nakedGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(graphicsComp.Props.bodyTex, graphicsComp.Props.bodySize, skinOne, skinTwo, skinThree);
            if (!graphicsComp.Props.hasHair)
            {
                __instance.beardGraphic = empty;
                __instance.hairGraphic = empty;
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

        private static string HeadTexShifted(ShapeshifterComp shapeshifterComp)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            if (comp == null)
            {
               return shapeshifterComp.MimickedHead.graphicPath;
            }
            return comp.headTex ?? shapeshifterComp.MimickedHead.graphicPath;

        }


        public static Graphic HeadGraphicShifted(ShapeshifterComp shapeshifterComp, Color skinTwo, Color skinThree,bool desiccated=false)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            Pawn pawn = shapeshifterComp.parent as Pawn;
            if (comp == null)
            {
                return shapeshifterComp.MimickedHead.GetGraphic(pawn.story.SkinColor,desiccated, pawn.story.SkinColorOverriden);
            }
            return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(HeadTexShifted(shapeshifterComp), Vector2.one, pawn.story.SkinColor, skinTwo, skinThree);
        }
        public static Graphic BodyGraphicShifted(ShapeshifterComp shapeshifterComp, Color skinTwo, Color skinThree)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            Pawn pawn = shapeshifterComp.parent as Pawn;
            if (comp == null)
            {
                return GraphicDatabase.Get<Graphic_Multi>(shapeshifterComp.MimickedBodyType.bodyNakedGraphicPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
            }
            return comp.bodyTex!=null? RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(comp.bodyTex, Vector2.one, pawn.story.SkinColor, skinTwo, skinThree) : GraphicDatabase.Get<Graphic_Multi>(shapeshifterComp.MimickedBodyType.bodyNakedGraphicPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
        }
        private static bool UsesCustomHead(ShapeshifterComp shapeshifterComp)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            return comp != null;
        }

        private static bool ShiftedHasHair(ShapeshifterComp shapeshifterComp)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            if (comp == null)
            {
                return true;
            }
            return comp.hasHair;
        }
    }
}
