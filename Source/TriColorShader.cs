using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;
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
            FileStream stream= new FileStream(path, FileMode.Open, FileAccess.Read);
            AssetBundle assetBundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            //load shader
            tricolorshader = (Shader)assetBundle.LoadAllAssets()[0];
        }
    }
}
