using RimWorld;
using RVCRestructured.Shifter;
using UnityEngine;

namespace RVCRestructured.RVR.Harmony;

public static class ApparelGraphicPatch
{
    public static string FindPath(string path, string defaultPath, Apparel apparel, GraphicsComp comp)
    {
        if (ContentFinder<Texture2D>.Get($"{defaultPath}_north", false))
        {
            return $"{defaultPath}";
        }
        if (ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_north", false))
        {
            return apparel.WornGraphicPath;
        }
        if ((comp == null || !comp.Props.useEmptyApparelIfNoDefault) && ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}_north", false))
        {
            return $"{apparel.WornGraphicPath}_{BodyTypeDefOf.Thin}";
        }

        string newPath = apparel.def.apparel.wornGraphicPath;
        if (ContentFinder<Texture2D>.Get($"{newPath}_north", false))
        {
            return $"{newPath}";
        }
        else if ((comp == null || !comp.Props.useEmptyApparelIfNoDefault) && ContentFinder<Texture2D>.Get($"{newPath}_{BodyTypeDefOf.Thin}_north", false))
        {
            return $"{newPath}_{BodyTypeDefOf.Thin}";
        }

        return path;
    }

    public static bool Prefix(ref Apparel apparel, ref BodyTypeDef bodyType, ref ApparelGraphicRecord rec, ref bool __result)
    {
        if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
            return true;
        if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || apparel.RenderAsPack() || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath) return true;
        if (bodyType == null) return true;
        Graphic resultGraphic;
        Pawn pawn = apparel.Wearer;
        BodyTypeDef copyBodyType = bodyType;
        string path = "RVC/Empty";
        GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
        ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
        if (comp == null)
        {
            return true;
        }
        if (shapeshifterComp != null)
        {
            copyBodyType = shapeshifterComp.MimickedBodyType;
        }
        string defaultPath = $"{apparel.WornGraphicPath}_{copyBodyType.defName}";
        path = FindPath(path, defaultPath, apparel, comp);
        Shader shader = ShaderDatabase.CutoutComplex;
        resultGraphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, apparel.def.graphicData.drawSize, apparel.DrawColor);
        rec = new ApparelGraphicRecord(resultGraphic, apparel);
        __result = true;
        return false;
    }
}
