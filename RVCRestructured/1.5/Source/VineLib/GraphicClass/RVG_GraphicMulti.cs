using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics;

public class RVG_Graphic_Multi : RVG_Graphic
{
    private readonly Material[] mats = new Material[4];
    private bool westFlipped;
    private bool eastFlipped;
    private float drawRotatedExtraAngleOffset;

    public string GraphicPath => path;

    public override Material MatSingle => MatSouth;

    public override Material MatWest => mats[3];

    public override Material MatSouth => mats[2];

    public override Material MatEast => mats[1];

    public override Material MatNorth => mats[0];

    public override bool EastFlipped => eastFlipped;
    public override bool WestFlipped => westFlipped;

    public override RVG_Graphic GetColoredVersion(
     Color newColor,
     Color newColorTwo,
     Color newColorThree)
    {
        return RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(path, drawSize, newColor, newColorTwo, newColorThree);
    }
    public override bool ShouldDrawRotated
    {
        get
        {
            if (data != null && !data.drawRotated)
            {
                return false;
            }

            return MatEast == MatNorth || MatWest == MatNorth;
        }
    }

    public override float DrawRotatedExtraAngleOffset => drawRotatedExtraAngleOffset;

    public override void Init(RVG_GraphicRequest req)
    {
        if (req.maskPath == null)
            req.maskPath = req.path;

        data = req.graphicData;
        path = req.path;
        color = req.color;
        colorTwo = req.colorTwo;
        colorThree = req.colorThree;
        drawSize = req.drawSize;
        Texture2D[] texture2DArray1 = new Texture2D[mats.Length];
        texture2DArray1[0] = ContentFinder<Texture2D>.Get(req.path + "_north", false);
        texture2DArray1[1] = ContentFinder<Texture2D>.Get(req.path + "_east", false);
        texture2DArray1[2] = ContentFinder<Texture2D>.Get(req.path + "_south", false);
        texture2DArray1[3] = ContentFinder<Texture2D>.Get(req.path + "_west", false);
        if (texture2DArray1[0] == null)
        {
            if (texture2DArray1[2] != null)
            {
                texture2DArray1[0] = texture2DArray1[2];
                drawRotatedExtraAngleOffset = 180f;
            }
            else if (texture2DArray1[1] != null)
            {
                texture2DArray1[0] = texture2DArray1[1];
                drawRotatedExtraAngleOffset = -90f;
            }
            else if (texture2DArray1[3] != null)
            {
                texture2DArray1[0] = texture2DArray1[3];
                drawRotatedExtraAngleOffset = 90f;
            }
            else
            {
                texture2DArray1[0] = ContentFinder<Texture2D>.Get(req.path, false);
            }
        }
        if (texture2DArray1[0] == null)
        {
            Log.Error("Failed to find any textures at " + req.path + " while constructing " + this.ToStringSafe());
        }
        else
        {
            if (texture2DArray1[2] == null)
            {
                texture2DArray1[2] = texture2DArray1[0];
            }

            if (texture2DArray1[1] == null)
            {
                if (texture2DArray1[3] != null)
                {
                    texture2DArray1[1] = texture2DArray1[3];
                    eastFlipped = DataAllowsFlip;
                }
                else
                {
                    texture2DArray1[1] = texture2DArray1[0];
                }
            }
            if (texture2DArray1[3] == null)
            {
                if (texture2DArray1[1] != null)
                {
                    texture2DArray1[3] = texture2DArray1[1];
                    westFlipped = DataAllowsFlip;
                }
                else
                {
                    texture2DArray1[3] = texture2DArray1[0];
                }
            }
            Texture2D[] texture2DArray2 = new Texture2D[mats.Length];
            //if (req.shader.SupportsMaskTex())
            if (req.shader == TriColorShader.tricolorshader)
            {
                //Log.Message("Generating MaskTex");
                texture2DArray2[0] = ContentFinder<Texture2D>.Get(req.maskPath + "_northm", false);
                texture2DArray2[1] = ContentFinder<Texture2D>.Get(req.maskPath + "_eastm", false);
                texture2DArray2[2] = ContentFinder<Texture2D>.Get(req.maskPath + "_southm", false);
                texture2DArray2[3] = ContentFinder<Texture2D>.Get(req.maskPath + "_westm", false);
                if (texture2DArray2[0] == null)
                {
                    if (texture2DArray2[2] != null)
                    {
                        texture2DArray2[0] = texture2DArray2[2];
                    }
                    else if (texture2DArray2[1] != null)
                    {
                        texture2DArray2[0] = texture2DArray2[1];
                    }
                    else if (texture2DArray2[3] != null)
                    {
                        texture2DArray2[0] = texture2DArray2[3];
                    }
                }
                if (texture2DArray2[2] == null)
                {
                    texture2DArray2[2] = texture2DArray2[0];
                }

                if (texture2DArray2[1] == null)
                {
                    texture2DArray2[1] = !(texture2DArray2[3] != null) ? texture2DArray2[0] : texture2DArray2[3];
                }

                if (texture2DArray2[3] == null)
                {
                    texture2DArray2[3] = !(texture2DArray2[1] != null) ? texture2DArray2[0] : texture2DArray2[1];
                }
            }
            for (int index = 0; index < mats.Length; ++index)
            {
                //this.mats[index] = MaterialPool.MatFrom(new MaterialRequest()
                mats[index] = RVG_MaterialPool.MatFrom(new RVG_MaterialRequest()
                {
                    mainTex = texture2DArray1[index],
                    shader = req.shader,
                    color = color,
                    colorTwo = colorTwo,
                    colorThree = colorThree,
                    maskTex = texture2DArray2[index],
                    shaderParameters = req.shaderParameters
                });
            };
        }
    }
}
