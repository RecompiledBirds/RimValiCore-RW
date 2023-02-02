using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class ApparelGraphicPatch
    {
        public static void Postfix(ref Apparel apparel, ref BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
        {
            Pawn pawn = apparel.Wearer;
            string path = $"{apparel.def.apparel.wornGraphicPath}";
            string bodyTypePath = $"{apparel.def.apparel.wornGraphicPath}_{bodyType.defName}";

            if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
                return;

            Graphic graphic;
            if (!apparel.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead))
            {
                if (ContentFinder<Texture2D>.Get($"{bodyTypePath}_south"))
                {
                    graphic = GraphicDatabase.Get<Graphic_Multi>(bodyTypePath, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
                    rec = new ApparelGraphicRecord(graphic, apparel);
                    return;
                }

                if (!ContentFinder<Texture2D>.Get($"{path}_south"))
                    return;

                graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
                rec = new ApparelGraphicRecord(graphic, apparel);
                return;
            }


            if (!(pawn.def is RaceDef))
            {
                graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
                rec = new ApparelGraphicRecord(graphic, apparel);
                return;
            }

            RaceDef rDef = pawn.def as RaceDef;

            if (!rDef.hasUniqueHeadApparel)
            {
                graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
                rec = new ApparelGraphicRecord(graphic, apparel);
                return;
            }

            if (!apparel.def.apparel.layers.Contains(ApparelLayerDefOf.Overhead))
            {
                graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
                rec = new ApparelGraphicRecord(graphic, apparel);
                return;
            }
                        
            
            if (ContentFinder<Texture2D>.Get($"{bodyTypePath}_south"))
            {
                graphic = GraphicDatabase.Get<Graphic_Multi>(bodyTypePath, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
                rec = new ApparelGraphicRecord(graphic, apparel);
                return;
            }

            if (!ContentFinder<Texture2D>.Get($"{path}_south"))
                return;

            graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.DrawSize, apparel.DrawColor);
            rec = new ApparelGraphicRecord(graphic, apparel);
        }
    }
}
