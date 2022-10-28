using HarmonyLib;
using RimWorld;
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
            harmony.Patch(AccessTools.Method(typeof(RaceProperties), "CanEverEat", new[] { typeof(ThingDef) }),postfix: new HarmonyMethod(typeof(EatingPatch),nameof(EatingPatch.EdiblePatch)));
            harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateStartingApparelFor"), prefix: new HarmonyMethod(typeof(ApparelGenPatch), nameof(ApparelGenPatch.ApparelGenPrefix)));
            harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool) }), postfix: new HarmonyMethod(typeof(ApparelEquipping), nameof(ApparelEquipping.EquipPatch)));
            harmony.Patch(AccessTools.Method(typeof(RestUtility), "CanUseBedEver"), postfix: new HarmonyMethod(typeof(BedPatch), nameof(BedPatch.CanUseBed)));
            harmony.Patch(AccessTools.Method(typeof(TraitSet), "GainTrait"), prefix: new HarmonyMethod(typeof(TraitPatch), nameof(TraitPatch.TraitPrefix)));
            harmony.Patch(AccessTools.Method(typeof(WorkGiver_Researcher), "ShouldSkip"),postfix: new HarmonyMethod(typeof(ResearchPatch),nameof(ResearchPatch.ResearchPostfix)));
            RVCLog.Log("Patches completed.");
        }
    }
}
