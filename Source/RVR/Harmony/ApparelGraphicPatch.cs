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
            string path = "";

            if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
                return;

            Graphic graphic;
            string altPath = $"{apparel.WornGraphicPath}_{bodyType.defName}";
            if (ContentFinder<Texture2D>.Get($"{altPath}_north", false))
            {
                path = altPath;
            }
            else if (ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_north", false))
            {
                path = apparel.WornGraphicPath;
            }
            else if (ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}_north", false))
            {
                if(!(pawn.def is RaceDef race) || !race.useEmptyApparelIfNoDefault) 
                    path = $"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}_north";
                else
                    path = "RVC/Empty";
            }


            //empty texture, avoids errors..
            else
            {
                if (pawn.def is RaceDef race && race.throwApparelError)
                    RVCLog.Log($"Could not find texture for {apparel.def} using bodytype {bodyType.defName}, no bodytype, or thin bodytype. Returning an empty texture...");

                path = "RVC/Empty";
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
