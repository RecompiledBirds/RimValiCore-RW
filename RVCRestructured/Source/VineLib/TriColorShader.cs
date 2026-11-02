using UnityEngine;
using Verse;
namespace RVCRestructured;


[StaticConstructorOnStartup]
public static class TriColorShader
{
    public static Shader tricolorshader;

    static TriColorShader()
    {
        tricolorshader = ShaderDatabase.LoadShader("TriColorShader");

        Log.Message(tricolorshader == null);
    }
}
