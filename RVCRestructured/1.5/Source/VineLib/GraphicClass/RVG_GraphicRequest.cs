using UnityEngine;

namespace RVCRestructured.Graphics;

public struct RVG_GraphicRequest(
  Type graphicClass,
  string path,
  Shader shader,
  Vector2 drawSize,
  Color color,
  Color colorTwo,
  Color colorThree,
  RVG_GraphicData? graphicData,
  int renderQueue,
  List<ShaderParameter>? shaderParameters,
  string? maskPath = null) : IEquatable<RVG_GraphicRequest>
{
    public Type graphicClass = graphicClass;
    public string path = path;
    public Shader shader = shader;
    public Vector2 drawSize = drawSize;
    public Color color = color;
    public Color colorTwo = colorTwo;
    public Color colorThree = colorThree;
    public RVG_GraphicData? graphicData = graphicData;
    public int renderQueue = renderQueue;
    public List<ShaderParameter>? shaderParameters = shaderParameters;
    public string maskPath = maskPath ?? path;

    public override int GetHashCode()
    {
        if (path == null)
        {
            path = BaseContent.BadTexPath;
        }

        return Gen.HashCombine(Gen.HashCombine(Gen.HashCombine(Gen.HashCombineStruct(Gen.HashCombineStruct(Gen.HashCombineStruct(Gen.HashCombine(Gen.HashCombine(Gen.HashCombine(0, graphicClass), path), shader), drawSize), color), colorTwo), graphicData), renderQueue), shaderParameters);
    }

    public override bool Equals(object obj)
    {
        return obj is RVG_GraphicRequest other && Equals(other);
    }

    public bool Equals(RVG_GraphicRequest other)
    {
        return graphicClass == other.graphicClass && path == other.path && (shader == other.shader && drawSize == other.drawSize) && (colorThree == other.colorThree && color == other.color && colorTwo == other.colorTwo && (graphicData == other.graphicData && renderQueue == other.renderQueue)) && shaderParameters == other.shaderParameters;
    }

    public static bool operator ==(RVG_GraphicRequest lhs, RVG_GraphicRequest rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(RVG_GraphicRequest lhs, RVG_GraphicRequest rhs)
    {
        return !(lhs == rhs);
    }
}
