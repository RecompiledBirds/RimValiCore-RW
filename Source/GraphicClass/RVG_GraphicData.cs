using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics
{
    public class RVG_GraphicData : GraphicData
    {
        public Color colorThree = Color.white;

        [Unsaved(false)]
        private RVG_Graphic cachedGraphic;



        public void Init()
        {
            if (graphicClass == null)
            {
                cachedGraphic = null;
                return;
            }
        }

        public void CopyFrom(RVG_GraphicData other)
        {
            texPath = other.texPath;
            graphicClass = other.graphicClass;
            shaderType = other.shaderType;
            color = other.color;
            colorTwo = other.colorTwo;
            colorThree = other.colorThree;
            drawSize = other.drawSize;
            drawOffset = other.drawOffset;
            drawOffsetNorth = other.drawOffsetNorth;
            drawOffsetEast = other.drawOffsetEast;
            drawOffsetSouth = other.drawOffsetSouth;
            drawOffsetWest = other.drawOffsetSouth;
            onGroundRandomRotateAngle = other.onGroundRandomRotateAngle;
            drawRotated = other.drawRotated;
            allowFlip = other.allowFlip;
            flipExtraRotation = other.flipExtraRotation;
            shadowData = other.shadowData;
            damageData = other.damageData;
            linkType = other.linkType;
            linkFlags = other.linkFlags;
            cachedGraphic = null;
        }


        public new RVG_Graphic Graphic
        {
            get
            {
                if (cachedGraphic == null)
                    Init();

                return cachedGraphic;
            }
        }
    }
}
