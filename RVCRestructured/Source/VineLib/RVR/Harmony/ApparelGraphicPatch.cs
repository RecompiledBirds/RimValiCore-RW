using RimWorld;
using RVCRestructured.Shifter;
using UnityEngine;

namespace RVCRestructured.RVR.Harmony;

public static class ApparelGraphicPatch
{
    public static string FindPath(string defaultPath,BodyTypeDef bodyTypeDef, Apparel apparel, GraphicsComp comp)
    {
        string path ="RVC/Empty";
        if (ContentFinder<Texture2D>.Get($"{defaultPath}_north", false))
        {
            return $"{defaultPath}";
        }
        if (ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_north", false))
        {
            return apparel.WornGraphicPath;
        }
        bool dontUseEmpty=comp == null || !comp.Props.useEmptyApparelIfNoDefault;
        if (dontUseEmpty && ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_{bodyTypeDef}_north", false))
        {
            return $"{apparel.WornGraphicPath}_{bodyTypeDef}";
        }

        string newPath = apparel.def.apparel.wornGraphicPath;
        if (ContentFinder<Texture2D>.Get($"{newPath}_north", false))
        {
            return $"{newPath}";
        }
        else if (dontUseEmpty && ContentFinder<Texture2D>.Get($"{newPath}_{bodyTypeDef}_north", false))
        {
            return $"{newPath}_{bodyTypeDef}";
        }
        return path;
    }

    public static bool Prefix(ref Apparel apparel, ref BodyTypeDef bodyType, ref ApparelGraphicRecord rec, ref bool __result)
    {
       
        Graphic resultGraphic;
        string finalPath = "RVC/Empty";
        if (bodyType == null)
        {
            VineLog.Warn($"Got null bodytype for a pawn {apparel.Wearer.Name} while generating grpahics");
            resultGraphic = GraphicDatabase.Get<Graphic_Multi>(finalPath, ShaderDatabase.CutoutComplex, apparel.def.graphicData.drawSize, apparel.DrawColor);
            rec = new ApparelGraphicRecord(resultGraphic, apparel);
            __result = true;
            return false;
        }
        if (apparel.def.apparel.wornGraphicPath.NullOrEmpty())
            return true;
        Pawn pawn = apparel.Wearer;
        BodyTypeDef copyBodyType = bodyType;
        GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
        ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
        if (comp == null)
        {
            return true;
        }
        if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || apparel.RenderAsPack() || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)
        {
            if (!comp.Props.useBodyTypedHeadApparel)
            {
                return true;
            }
        }
        if (shapeshifterComp != null)
        {
            copyBodyType = shapeshifterComp.MimickedBodyType;
        }
        string defaultPath = $"{apparel.WornGraphicPath}_{copyBodyType.defName}";
        Shader shader = ShaderDatabase.CutoutComplex;
        resultGraphic = GraphicDatabase.Get<Graphic_Multi>(FindPath(defaultPath, copyBodyType, apparel, comp), shader, apparel.def.graphicData.drawSize, apparel.DrawColor);
        rec = new ApparelGraphicRecord(resultGraphic, apparel);
        __result = true;
        return false;
    }
}
