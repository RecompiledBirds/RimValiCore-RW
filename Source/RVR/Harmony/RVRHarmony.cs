using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    [StaticConstructorOnStartup]
    public static class RVRHarmony
    {
        static RVRHarmony()
        {
            RVCLog.Log("Staring RVR patches.");
            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("RecompiledBirds.RVC.RVR");
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(RotDrawMode), typeof(PawnRenderFlags) }),postfix:new HarmonyMethod(typeof(PawnRendererPatch),nameof(PawnRendererPatch.RenderingPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveAllGraphics"),postfix: new HarmonyMethod(typeof(ResolveGraphicsPostFix),nameof(ResolveGraphicsPostFix.ResolveGraphicsPatch)));
            harmony.Patch(AccessTools.Constructor(typeof(PawnTextureAtlas)),transpiler: new HarmonyMethod(typeof(RenderTextureTranspiler),nameof(RenderTextureTranspiler.Transpile)));
            RVCLog.Log("Patches completed.");
        }
    }
}
