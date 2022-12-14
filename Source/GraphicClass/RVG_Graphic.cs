using System;
using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics
{
    public class RVG_Graphic : Graphic
    {
        public Color colorThree = Color.white;

        public new RVG_GraphicData data;

        private RVG_Graphic cachedShadowlessGraphic;

        [Obsolete]
        public override Graphic GetCopy(Vector2 newDrawSize)
        {
            return RVG_GraphicDataBase.Get(GetType(), path, drawSize, color, colorTwo, colorThree);
        }

        public virtual RVG_Graphic GetColoredVersion(Color color, Color colorTwo, Color ColorThree)
        {
            return RVGBaseContent.BadGraphic;
        }

        public override Graphic GetShadowlessGraphic()
        {
            if (data == null || data.shadowData == null)
                return this;
            if (cachedShadowlessGraphic != null)
                return cachedShadowlessGraphic;
            RVG_GraphicData graphicData = new RVG_GraphicData();
            graphicData.CopyFrom(this.data);
            graphicData.shadowData = null;
            cachedShadowlessGraphic = graphicData.Graphic;
            return cachedShadowlessGraphic;
        }

        public virtual void Init(RVG_GraphicRequest req)
        {

        }
    }
}
