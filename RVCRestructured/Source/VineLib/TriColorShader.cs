using UnityEngine;
using Verse;
namespace RVCRestructured;

[StaticConstructorOnStartup]
public static class TriColorShader
{
    public static Shader tricolorshader;

    static TriColorShader()
    {
        if (tricolorshader != null)
        {
            VineLog.Warn("Tricolor shader already loaded, aborting.. (Why are you loading this twice?)");
            return;
        }
        //Get the path to the shader bundle
        string path = Path.Combine(RimValiCore.ModDir, "RimValiAssetBundles", "shader");
        if (path == null)
        {
            VineLog.Error("Failure to resolve path for tricolor shader asset bundle! Defaulting it to CutoutComplex.");
            tricolorshader = ShaderDatabase.CutoutComplex;
            return;
        }
        //load bundle
        AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
        if (assetBundle == null)
        {
            VineLog.Error("Failure to load assetbundle for tricolor shader! Defaulting it to CutoutComplex.");
            tricolorshader = ShaderDatabase.CutoutComplex;
            return;
        }
        //load shader
        tricolorshader = (Shader)assetBundle.LoadAllAssets()[0];
        if (tricolorshader == null)
        {
            VineLog.Error("Failure to load tricolor shader! Defaulting it to CutoutComplex.");
            tricolorshader = ShaderDatabase.CutoutComplex;
            return;
        }
    }
}
