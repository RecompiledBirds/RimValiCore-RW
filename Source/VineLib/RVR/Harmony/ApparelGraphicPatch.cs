using RimWorld;
using RVCRestructured.Shifter;
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
            if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
                return;
            BodyTypeDef typeDef = bodyType;
            Pawn pawn = apparel.Wearer;
            string path;
            GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            if(shapeshifterComp != null)
            {
                typeDef = shapeshifterComp.MimickedBodyType;
            }

            Graphic graphic;
            string altPath = $"{apparel.WornGraphicPath}_{typeDef.defName}";
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
                
                if(comp==null || comp.Props.useEmptyApparelIfNoDefault) 
                    path = $"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}_north";
                else
                    path = "RVC/Empty";
            }


            //empty texture, avoids errors..
            else
            {
                if (comp!=null && comp.Props.throwApparelError)
                    RVCLog.Log($"Could not find texture for {apparel.def} using bodytype {typeDef.defName}, no bodytype, or thin bodytype. Returning an empty texture...");

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
