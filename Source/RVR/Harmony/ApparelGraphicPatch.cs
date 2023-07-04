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

            if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
                return;

            Graphic graphic;
            if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)
            {
                string altPath = $"{apparel.WornGraphicPath}_{bodyType.defName}";
                if (ContentFinder<Texture2D>.Get($"{altPath}_north", false))
                {
                    path = altPath;
                }
                else
                {
                    path = apparel.WornGraphicPath;
                }
            }
            else
            {
                path = apparel.WornGraphicPath + "_" + bodyType.defName;
            }

            Shader shader = ShaderDatabase.Cutout;
            if (apparel.def.apparel.useWornGraphicMask)
            {
                shader = ShaderDatabase.CutoutComplex;
            }

            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, apparel.DrawSize, apparel.DrawColor);

            rec = new ApparelGraphicRecord(graphic, apparel);
            
        }
    }
}
