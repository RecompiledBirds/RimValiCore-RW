using RimWorld;
using RVCRestructured.Graphics;
using RVCRestructured.Shifter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured
{ 
    /*
    public static class ShapeShifterGraphicsResolver
    {
        public static void ResolveNonHuman(ShapeshifterComp shapeshifterComp,Pawn pawn, PawnRenderTree __instance, RVG_Graphic empty)
        {
            RaceProperties props = shapeshifterComp.CurrentForm.race;
            __instance.bodyTattooGraphic = empty;
            __instance.faceTattooGraphic = empty;
            __instance.furCoveredGraphic = empty;

            __instance.desiccatedHeadGraphic = empty;
            __instance.dessicatedGraphic = empty;
            __instance.desiccatedHeadStumpGraphic = empty;

            __instance.headStumpGraphic = empty;
            __instance.headGraphic =  empty  ;
            __instance.beardGraphic = empty;
            __instance.hairGraphic = empty;
            __instance.nakedGraphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;

            if (props.packAnimal)
            {
                __instance.packGraphic = GraphicDatabase.Get<Graphic_Multi>(__instance.nakedGraphic.path + "Pack", ShaderDatabase.Cutout, __instance.nakedGraphic.drawSize, Color.white);
            }
        }
        public static void ResolveHumanLike(ShapeshifterComp shapeshifterComp, RVRComp comp, PawnRenderTree __instance, Pawn pawn, Color skinTwo, Color skinThree, RVG_Graphic empty)
        {
            __instance.bodyTattooGraphic = empty;
            __instance.faceTattooGraphic = empty;
            __instance.furCoveredGraphic = empty;

            __instance.desiccatedHeadGraphic = HeadGraphicShifted(shapeshifterComp, skinTwo, skinThree, true);
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
            __instance.nakedGraphic = BodyGraphicShifted(shapeshifterComp, skinTwo, skinThree);

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
        }
        public static Graphic HeadGraphicShifted(ShapeshifterComp shapeshifterComp, Color skinTwo, Color skinThree, bool desiccated = false)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            Pawn pawn = shapeshifterComp.parent as Pawn;
            if (comp == null)
            {
                return shapeshifterComp.MimickedHead.GetGraphic(pawn,skinTwo);
            }
            return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(HeadTexShifted(shapeshifterComp), Vector2.one, pawn.story.SkinColor, skinTwo, skinThree);
        }
        public static Graphic BodyGraphicShifted(ShapeshifterComp shapeshifterComp, Color skinTwo, Color skinThree)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            Pawn pawn = shapeshifterComp.parent as Pawn;
            if (comp == null)
            {
                return GraphicDatabase.Get<Graphic_Multi>(shapeshifterComp.MimickedBodyType.bodyNakedGraphicPath, ShaderUtility.GetSkinShader(pawn), Vector2.one, pawn.story.SkinColor);
            }
            return comp.bodyTex != null ? RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(comp.bodyTex, Vector2.one, pawn.story.SkinColor, skinTwo, skinThree) : GraphicDatabase.Get<Graphic_Multi>(shapeshifterComp.MimickedBodyType.bodyNakedGraphicPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
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

        private static bool ShiftedHasHair(ShapeshifterComp shapeshifterComp)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            if (comp == null)
            {
                return true;
            }
            return comp.hasHair;
        }

        private static bool UsesCustomHead(ShapeshifterComp shapeshifterComp)
        {
            RVRGraphicsComp comp = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            return comp != null;
        }
    }
    */
}
