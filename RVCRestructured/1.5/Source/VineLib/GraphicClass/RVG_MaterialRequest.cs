using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics;

public struct RVG_MaterialRequest : IEquatable<RVG_MaterialRequest>
{
    public Shader shader;
    public Texture2D mainTex;
    public Color color;
    public Color colorTwo;
    public Texture2D? maskTex;
    public int renderQueue;
    private List<ShaderParameter> shaderParameters = [];
    public Color colorThree;

    public string BaseTexPath
    {
        set => mainTex = ContentFinder<Texture2D>.Get(value);
    }

    public List<ShaderParameter> ShaderParameters 
    { 
        readonly get => shaderParameters; 
        set => shaderParameters = value ?? []; 
    }

    public RVG_MaterialRequest(Texture2D tex)
    {
        shader = ShaderDatabase.Cutout;
        mainTex = tex;
        color = Color.red;
        colorTwo = Color.green;
        colorThree = Color.blue;
        maskTex = null;
        renderQueue = 0;
        ShaderParameters = [];
    }

    public RVG_MaterialRequest(Texture2D tex, Shader shader)
    {
        this.shader = shader;
        mainTex = tex;
        color = Color.green;
        colorTwo = Color.blue;
        colorThree = Color.red;
        maskTex = null;
        renderQueue = 0;
        ShaderParameters = [];
    }

    public RVG_MaterialRequest(Texture2D tex, Shader shader, Color color)
    {
        this.shader = shader;
        mainTex = tex;
        this.color = color;
        colorTwo = Color.red;
        colorThree = Color.blue;
        maskTex = null;
        renderQueue = 0;
        ShaderParameters = [];
    }

    public override readonly int GetHashCode()
    {
        return Gen.HashCombine(Gen.HashCombineInt(Gen.HashCombine(Gen.HashCombine(Gen.HashCombineStruct(Gen.HashCombineStruct(Gen.HashCombine(0, shader), color), colorTwo), mainTex), maskTex), renderQueue), ShaderParameters);
    }

    public override readonly bool Equals(object obj)
    {
        return obj is RVG_MaterialRequest request && Equals(request);
    }

    public readonly bool Equals(RVG_MaterialRequest other)
    {
        return other.shader == shader && other.mainTex == mainTex && other.color == color && other.colorTwo == colorTwo && colorThree==other.colorThree && other.maskTex == maskTex && other.renderQueue == renderQueue && other.ShaderParameters == ShaderParameters;
    }

    public static bool operator ==(RVG_MaterialRequest lhs, RVG_MaterialRequest rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(RVG_MaterialRequest lhs, RVG_MaterialRequest rhs)
    {
        return !(lhs == rhs);
    }

    public override readonly string ToString()
    {
        return $"AvaliMaterialRequest({shader.name}, {mainTex.name}, {color}, {colorTwo}, {colorThree}, {maskTex}, {renderQueue})";
    }
}
