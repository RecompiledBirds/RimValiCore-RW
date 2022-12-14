using HarmonyLib;
using RimWorld;
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
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(RotDrawMode), typeof(PawnRenderFlags) }), postfix: new HarmonyMethod(typeof(PawnRendererPatch), nameof(PawnRendererPatch.RenderingPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "ResolveAllGraphics"), postfix: new HarmonyMethod(typeof(ResolveGraphicsPostFix), nameof(ResolveGraphicsPostFix.ResolveGraphicsPatch)));
            harmony.Patch(AccessTools.Constructor(typeof(PawnTextureAtlas)), transpiler: new HarmonyMethod(typeof(RenderTextureTranspiler), nameof(RenderTextureTranspiler.Transpile)));
            harmony.Patch(AccessTools.Method(typeof(RaceProperties), "CanEverEat", new[] { typeof(ThingDef) }), postfix: new HarmonyMethod(typeof(EatingPatch), nameof(EatingPatch.EdiblePatch)));
            harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateStartingApparelFor"), prefix: new HarmonyMethod(typeof(ApparelGenPatch), nameof(ApparelGenPatch.ApparelGenPrefix)));
            harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool) }), postfix: new HarmonyMethod(typeof(ApparelEquipping), nameof(ApparelEquipping.EquipPatch)));
            harmony.Patch(AccessTools.Method(typeof(RestUtility), "CanUseBedEver"), postfix: new HarmonyMethod(typeof(BedPatch), nameof(BedPatch.CanUseBed)));
            harmony.Patch(AccessTools.Method(typeof(TraitSet), "GainTrait"), prefix: new HarmonyMethod(typeof(TraitPatch), nameof(TraitPatch.TraitPrefix)));
            harmony.Patch(AccessTools.Method(typeof(WorkGiver_Researcher), "ShouldSkip"), postfix: new HarmonyMethod(typeof(ResearchPatch), nameof(ResearchPatch.ResearchPostfix)));
            harmony.Patch(AccessTools.Method(typeof(GenConstruct), "CanConstruct", new[] { typeof(Thing), typeof(Pawn), typeof(WorkTypeDef), typeof(bool) }), postfix: new HarmonyMethod(typeof(ConstructionPatch), nameof(ConstructionPatch.Constructable)));
            harmony.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), "GetFirstMemoryOfDef"), prefix: new HarmonyMethod(typeof(ThoughtReplacerPatch), nameof(ThoughtReplacerPatch.ReplacePatch)));
            harmony.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), "NumMemoriesOfDef"), prefix: new HarmonyMethod(typeof(ThoughtReplacerPatch), nameof(ThoughtReplacerPatch.ReplacePatch)));
            harmony.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), "OldestMemoryOfDef"), prefix: new HarmonyMethod(typeof(ThoughtReplacerPatch), nameof(ThoughtReplacerPatch.ReplacePatch)));
            harmony.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), "RemoveMemoriesOfDef"), prefix: new HarmonyMethod(typeof(ThoughtReplacerPatch), nameof(ThoughtReplacerPatch.ReplacePatch)));
            harmony.Patch(AccessTools.Method(typeof(MemoryThoughtHandler), "RemoveMemoriesOfDefIf"), prefix: new HarmonyMethod(typeof(ThoughtReplacerPatch), nameof(ThoughtReplacerPatch.ReplacePatch)));
            harmony.Patch(AccessTools.Method(typeof(SituationalThoughtHandler), "TryCreateThought"), prefix: new HarmonyMethod(typeof(ThoughtReplacerPatch), nameof(ThoughtReplacerPatch.ReplacePatchSIT)));
            harmony.Patch(AccessTools.Method(typeof(ThoughtUtility), "CanGetThought"), postfix: new HarmonyMethod(typeof(CanGetThoughtPatch), nameof(CanGetThoughtPatch.CanGetPatch)));
            harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool) }), postfix: new HarmonyMethod(typeof(EquipingPatch), "EquipingPostfix"));
            harmony.Patch(AccessTools.Method(typeof(FoodUtility), "ThoughtsFromIngesting"), postfix: new HarmonyMethod(typeof(IngestingPatch),nameof(IngestingPatch.IngestingPostfix)));
            harmony.Patch(AccessTools.Method(typeof(BodyPartDef), "GetMaxHealth"),postfix: new HarmonyMethod(typeof(BodyPartHealthPatch),nameof(BodyPartHealthPatch.HealthPostfix)));
            harmony.Patch(AccessTools.Method(typeof(ThoughtUtility), "GiveThoughtsForPawnOrganHarvested"), prefix: new HarmonyMethod(typeof(OrganPatch),nameof(OrganPatch.OrganHarvestPrefix)));
            harmony.Patch(AccessTools.Method(typeof(Corpse), "ButcherProducts"),prefix:new HarmonyMethod(typeof(ButcherPatch),nameof(ButcherPatch.ButcherPrefix)));
            RVCLog.Log("RVR Patches completed.");
        }
    }
}
