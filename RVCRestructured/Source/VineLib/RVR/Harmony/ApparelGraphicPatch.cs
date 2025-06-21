using RVCRestructured.Shifter;
using UnityEngine;

namespace RVCRestructured.RVR.Harmony;

public static class ApparelGraphicPatch
{
    public static void Postfix(ref Apparel apparel, ref BodyTypeDef bodyType, ref ApparelGraphicRecord rec)
    {
        if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
            return;
        BodyTypeDef typeDef = bodyType;
        Pawn pawn = apparel.Wearer;
        string path = "RVC/Empty";
        GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
        ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
        if (shapeshifterComp != null)
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
        else if ((comp == null || !comp.Props.useEmptyApparelIfNoDefault) && ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}_north", false))
        {
            path = $"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}_north";
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
