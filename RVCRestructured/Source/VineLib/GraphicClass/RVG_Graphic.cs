using UnityEngine;

namespace RVCRestructured.Graphics;

public abstract class RVG_Graphic : Graphic
{
    public Color colorThree = Color.white;

    public new RVG_GraphicData? data;

    private RVG_Graphic? cachedShadowlessGraphic;

    public abstract RVG_Graphic GetColoredVersion(Color color, Color colorTwo, Color ColorThree);

    public override Graphic? GetShadowlessGraphic()
    {
        if (data == null || data.shadowData == null) return this;
        if (cachedShadowlessGraphic != null) return cachedShadowlessGraphic;

        RVG_GraphicData graphicData = new();
        graphicData.CopyFrom(data);
        graphicData.shadowData = null;

        cachedShadowlessGraphic = graphicData.Graphic;
        return cachedShadowlessGraphic;
    }

    public virtual void Init(RVG_GraphicRequest req)
    {

    }
}
