using RimWorld;
using RVCRestructured.Shifter;
using UnityEngine;

namespace RVCRestructured.RVR.Harmony;

public static class ApparelGraphicPatch
{
    public static string FindPath(string emptyPath, string defaultPath,BodyTypeDef bodyTypeDef, Apparel apparel, GraphicsComp comp)
    {
       
        if (ContentFinder<Texture2D>.Get($"{defaultPath}_north", false))
        {
            return $"{defaultPath}";
        }
        if (ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_north", false))
        {
            return apparel.WornGraphicPath;
        }
        bool defaultUseApparelIfNoTexture = (comp?.Props.useEmptyApparelIfNoDefault ?? false);
        if (ContentFinder<Texture2D>.Get($"{apparel.WornGraphicPath}_{bodyTypeDef}_north", false))
        {
            return $"{apparel.WornGraphicPath}_{bodyTypeDef}";
        }

        string newPath = apparel.def.apparel.wornGraphicPath;
        if (ContentFinder<Texture2D>.Get($"{newPath}_north", false))
        {
            return $"{newPath}";
        }
        else if (ContentFinder<Texture2D>.Get($"{newPath}_{bodyTypeDef}_north", false))
        {
            return $"{newPath}_{bodyTypeDef}";
        }
        if (!defaultUseApparelIfNoTexture)
        {
            return $"{apparel.WornGraphicPath}_{bodyTypeDef}";
        }
        return emptyPath;
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
        bool shouldTryToFindNewPath = true;
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
            shouldTryToFindNewPath = false;
        }
        if (shapeshifterComp != null)
        {
            copyBodyType = shapeshifterComp.MimickedBodyType;
        }
        if (shouldTryToFindNewPath)
        {
            string defaultPath = $"{apparel.WornGraphicPath}_{copyBodyType.defName}";
            finalPath = FindPath(finalPath, defaultPath, copyBodyType, apparel, comp);
        }
        Shader shader = ShaderDatabase.CutoutComplex;
        resultGraphic = GraphicDatabase.Get<Graphic_Multi>(finalPath, shader, apparel.def.graphicData.drawSize, apparel.DrawColor);
        rec = new ApparelGraphicRecord(resultGraphic, apparel);
        __result = true;
        return false;
    }
}
