using System.IO;
using UnityEngine;
using Verse;
namespace RVCRestructured
{
    [StaticConstructorOnStartup]
    public static class TriColorShader
    {
        public static Shader tricolorshader;

        static TriColorShader()
        {
            //Get the path to the shader bundle
            string path = Path.Combine(RimValiCore.ModDir, "RimValiAssetBundles", "shader");
            //load bundle
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            AssetBundle assetBundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            //load shader
            tricolorshader = (Shader)assetBundle.LoadAllAssets()[0];
        }
    }
}
