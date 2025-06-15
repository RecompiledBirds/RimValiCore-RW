using UnityEngine;

namespace RVCRestructured.Graphics;

[StaticConstructorOnStartup]
public class RVG_GraphicDataBase
{
    private static readonly Dictionary<RVG_GraphicRequest, RVG_Graphic> graphics = [];

    private static T GetInner<T>(RVG_GraphicRequest req) where T : RVG_Graphic, new()
    {
        if (!graphics.TryGetValue(req, out RVG_Graphic graphic))
        {
            graphic = new T();
            graphic.Init(req);
            graphics.Add(req, graphic);
        }
        return (T)graphic;
    }

    public static RVG_Graphic Get<T>(string path, string? maskPath = null) where T : RVG_Graphic, new()
    {
        return GetInner<T>(new RVG_GraphicRequest(typeof(T), path, TriColorShader.tricolorshader, Vector2.one, Color.white, Color.white, Color.white, null, 0, null, maskPath));
    }

    public static RVG_Graphic Get<T>(string path, Vector2 drawSize, string? maskPath = null) where T : RVG_Graphic, new()
    {
        return GetInner<T>(new RVG_GraphicRequest(typeof(T), path, TriColorShader.tricolorshader, drawSize, Color.white, Color.white, Color.white, null, 0, null, maskPath));
    }

    public static RVG_Graphic Get<T>(string path, Vector2 drawSize, Color colorOne, Color colorTwo, Color colorThree, string? maskPath = null) where T : RVG_Graphic, new()
    {
        return GetInner<T>(new RVG_GraphicRequest(typeof(T), path, TriColorShader.tricolorshader, drawSize, colorOne, colorTwo, colorThree, null, 0, null, maskPath));
    }
}
