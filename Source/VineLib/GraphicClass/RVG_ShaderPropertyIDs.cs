using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics
{
    [StaticConstructorOnStartup]
    public static class RVG_ShaderPropertyIDs
    {
        private static readonly string PlanetSunLightDirectionName = "_PlanetSunLightDirection";
        private static readonly string PlanetSunLightEnabledName = "_PlanetSunLightEnabled";
        private static readonly string PlanetRadiusName = "_PlanetRadius";
        private static readonly string MapSunLightDirectionName = "_CastVect";
        private static readonly string GlowRadiusName = "_GlowRadius";
        private static readonly string GameSecondsName = "_GameSeconds";
        private static readonly string ColorName = "_Color";
        private static readonly string ColorTwoName = "_ColorTwo";
        private static readonly string MaskTexName;
        private static readonly string SwayHeadName;
        private static readonly string ShockwaveSpanName;
        private static readonly string AgeSecsName;
        public static int PlanetSunLightDirection;
        public static int PlanetSunLightEnabled;
        public static int PlanetRadius;
        public static int MapSunLightDirection;
        public static int GlowRadius;
        public static int GameSeconds;
        public static int AgeSecs;
        public static int Color;
        public static int ColorTwo;
        public static int MaskTex;
        public static int SwayHead;
        public static int ShockwaveColor;
        public static int ShockwaveSpan;
        public static int WaterCastVectSun;
        public static int WaterCastVectMoon;
        public static int WaterOutputTex;
        public static int WaterOffsetTex;
        public static int ShadowCompositeTex;
        public static int FallIntensity;
        private static readonly string ColorThreeName = "_ColorThree";
        public static int ColorThree;

        static RVG_ShaderPropertyIDs()
        {
            MaskTexName = "_MaskTex";
            SwayHeadName = "_SwayHead";
            ShockwaveSpanName = "_ShockwaveSpan";
            AgeSecsName = "_AgeSecs";
            PlanetSunLightDirection = Shader.PropertyToID(PlanetSunLightDirectionName);
            PlanetSunLightEnabled = Shader.PropertyToID(PlanetSunLightEnabledName);
            PlanetRadius = Shader.PropertyToID(PlanetRadiusName);
            MapSunLightDirection = Shader.PropertyToID(MapSunLightDirectionName);
            GlowRadius = Shader.PropertyToID(GlowRadiusName);
            GameSeconds = Shader.PropertyToID(GameSecondsName);
            AgeSecs = Shader.PropertyToID(AgeSecsName);
            Color = Shader.PropertyToID(ColorName);
            ColorTwo = Shader.PropertyToID(ColorTwoName);
            ColorThree = Shader.PropertyToID(ColorThreeName);
            MaskTex = Shader.PropertyToID(MaskTexName);
            SwayHead = Shader.PropertyToID(SwayHeadName);
            ShockwaveColor = Shader.PropertyToID("_ShockwaveColor");
            ShockwaveSpan = Shader.PropertyToID(ShockwaveSpanName);
            WaterCastVectSun = Shader.PropertyToID("_WaterCastVectSun");
            WaterCastVectMoon = Shader.PropertyToID("_WaterCastVectMoon");
            WaterOutputTex = Shader.PropertyToID("_WaterOutputTex");
            WaterOffsetTex = Shader.PropertyToID("_WaterOffsetTex");
            ShadowCompositeTex = Shader.PropertyToID("_ShadowCompositeTex");
            FallIntensity = Shader.PropertyToID("_FallIntensity");
        }
    }
}
