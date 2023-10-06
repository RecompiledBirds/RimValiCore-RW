using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics
{
    public struct RVG_GraphicRequest : IEquatable<RVG_GraphicRequest>
    {
        public Type graphicClass;
        public string path;
        public Shader shader;
        public Vector2 drawSize;
        public Color color;
        public Color colorTwo;
        public Color colorThree;
        public RVG_GraphicData graphicData;
        public int renderQueue;
        public List<ShaderParameter> shaderParameters;
        public string maskPath;
        public RVG_GraphicRequest(
          Type graphicClass,
          string path,
          Shader shader,
          Vector2 drawSize,
          Color color,
          Color colorTwo,
          Color colorThree,
          RVG_GraphicData graphicData,
          int renderQueue,
          List<ShaderParameter> shaderParameters,
          string maskPath = null)
        {
            this.graphicClass = graphicClass;
            this.path = path;
            this.shader = shader;
            this.drawSize = drawSize;
            this.color = color;
            this.colorTwo = colorTwo;
            this.colorThree = colorThree;
            this.graphicData = graphicData;
            this.renderQueue = renderQueue;
            this.shaderParameters = shaderParameters.NullOrEmpty() ? null : shaderParameters;
            this.maskPath = maskPath != null ? maskPath : path;
        }

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
            return graphicClass == other.graphicClass && path == other.path && (shader == other.shader && drawSize == other.drawSize) && (color == other.color && colorTwo == other.colorTwo && (graphicData == other.graphicData && renderQueue == other.renderQueue)) && shaderParameters == other.shaderParameters;
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
}
