using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics;

public class RVG_MaterialPool
{
    private static readonly Dictionary<RVG_MaterialRequest, Material> matDictionary = [];

    public static Material MatFrom(RVG_MaterialRequest req)
    {
        //Log.Message("thisine");
        if (!UnityData.IsInMainThread)
        {
            Log.Error("Tried to get a material from a different thread.");
            return null;
        }
        if (req.mainTex == null)
        {
            Log.Error("MatFrom with null sourceTex.");
            return RVGBaseContent.BadMat;
        }
        if (req.shader == null)
        {
            Log.Warning("Matfrom with null shader.");
            return RVGBaseContent.BadMat;
        }
        /*
			if (req.maskTex != null && !req.shader.SupportsMaskTex())
			{
				Log.Error("MaterialRequest has maskTex but shader does not support it. req=" + req.ToString(), false);
				req.maskTex = null;
			}
			*/
        // What's the reasoning behind this?
#pragma warning disable CS1717 // Assignment made to same variable
        req.color = req.color;
        req.colorTwo = req.colorTwo;
        req.colorThree = req.colorThree;
#pragma warning restore CS1717


        if (!matDictionary.TryGetValue(req, out Material material))
        {
            material = RVG_MaterialAllocator.Create(req.shader);
            material.name = req.shader.name + "_" + req.mainTex.name;
            material.mainTexture = req.mainTex;
            material.color = req.color;
            material.SetTexture(RVG_ShaderPropertyIDs.MaskTex, req.maskTex);
            material.SetColor(RVG_ShaderPropertyIDs.ColorTwo, req.colorTwo);
            material.SetColor(RVG_ShaderPropertyIDs.ColorThree, req.colorThree);
            // liQd Comment there it is
            material.SetTexture(ShaderPropertyIDs.MaskTex, req.maskTex);
            //Log.Message("thisthinghascolor3: " + req.colorThree);
            if (req.renderQueue != 0)
            {
                material.renderQueue = req.renderQueue;
            }

            if (!req.shaderParameters.NullOrEmpty())
            {
                int c = req.shaderParameters.Count;
                for (int i = 0; i < c; i++)
                {
                    RVCLog.Log(req.shaderParameters[i]);
                    req.shaderParameters[i].Apply(material);
                }
            }

            matDictionary.Add(req, material);
            if (req.shader == ShaderDatabase.CutoutPlant || req.shader == ShaderDatabase.TransparentPlant)
            {
                WindManager.Notify_PlantMaterialCreated(material);
            }
        }
        return material;
    }
}
