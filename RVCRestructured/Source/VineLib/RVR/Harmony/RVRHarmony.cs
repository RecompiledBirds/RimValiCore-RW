using HarmonyLib;
using RVCRestructured.RVR.Harmony;
using Verse.AI;

namespace RVCRestructured.RVR.HarmonyPatches;

[StaticConstructorOnStartup]
public static class RVRHarmony
{
    static RVRHarmony()
    {
        RVRPatcher();
    }

    private static void RVRPatcher()
    {
        VineLog.Log("Staring RVR patches.");
        HarmonyLib.Harmony.DEBUG = VineSettings.DebugHarmony;
        HarmonyLib.Harmony harmony = new("RecompiledBirds.RVC.RVR");
        try
        {
            //Rendering patches
           // harmony.Patch(AccessTools.Constructor(typeof(PawnTextureAtlas)), transpiler: new HarmonyMethod(typeof(RenderTextureTranspiler), nameof(RenderTextureTranspiler.Transpile)));


            //Restriction patches
            harmony.Patch(AccessTools.Method(typeof(RaceProperties), "CanEverEat", [typeof(ThingDef)]), postfix: new HarmonyMethod(typeof(EatingPatch), nameof(EatingPatch.CanEverEatPostFix)));
            harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "CanUsePair"), postfix: new HarmonyMethod(typeof(ApparelGenPatch), nameof(ApparelGenPatch.CanUsePairPatch)));
            harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", [typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool)]), postfix: new HarmonyMethod(typeof(ApparelEquipping), nameof(ApparelEquipping.EquipPatch)));
            harmony.Patch(AccessTools.Method(typeof(RestUtility), "CanUseBedEver"), postfix: new HarmonyMethod(typeof(BedPatch), nameof(BedPatch.CanUseBed)));
            harmony.Patch(AccessTools.Method(typeof(WorkGiver_Researcher), "ShouldSkip"), postfix: new HarmonyMethod(typeof(ResearchPatch), nameof(ResearchPatch.ShouldSkipPostFix)));
            harmony.Patch(AccessTools.Method(typeof(MainTabWindow_Research), "get_VisibleResearchProjects"), postfix: new HarmonyMethod(typeof(ResearchPatch), nameof(ResearchPatch.ResearchVisiblePostFix)));
            harmony.Patch(AccessTools.Method(typeof(ResearchProjectDef), "get_AnalyzedThingsCompleted"), prefix: new HarmonyMethod(typeof(ResearchPatch), nameof(ResearchPatch.AnalyzedThingsCompletedPrefix)));
            harmony.Patch(AccessTools.Method(typeof(JobDriver_Equip), nameof(JobDriver_Equip.TryMakePreToilReservations)), prefix: new HarmonyMethod(typeof(EquipingPatch), nameof(EquipingPatch.JobDriver_EquipPrefix)));
            harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", [typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool)]), postfix: new HarmonyMethod(typeof(EquipingPatch), nameof(EquipingPatch.EquipingPostfix)));
            harmony.Patch(AccessTools.Method(typeof(GenConstruct), "CanConstruct", [typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool), typeof(JobDef)]), postfix: new HarmonyMethod(typeof(ConstructionPatch), nameof(ConstructionPatch.Constructable)));


            //Thoughts
            Type thoughtReplacer = typeof(ThoughtReplacerPatch);
            string replacerPatchName = nameof(ThoughtReplacerPatch.ReplacePatch);
            Type memoryHandler = typeof(MemoryThoughtHandler);
            HarmonyMethod thoughtPatchMethod = new(thoughtReplacer, replacerPatchName);
            harmony.Patch(AccessTools.Method(memoryHandler, "GetFirstMemoryOfDef"), prefix: thoughtPatchMethod);
            harmony.Patch(AccessTools.Method(memoryHandler, "NumMemoriesOfDef"), prefix: thoughtPatchMethod);
            harmony.Patch(AccessTools.Method(memoryHandler, "OldestMemoryOfDef"), prefix: thoughtPatchMethod);
            harmony.Patch(AccessTools.Method(memoryHandler, "RemoveMemoriesOfDef"), prefix: thoughtPatchMethod);
            harmony.Patch(AccessTools.Method(memoryHandler, "RemoveMemoriesOfDefIf"), prefix: thoughtPatchMethod);
            harmony.Patch(AccessTools.Method(memoryHandler, "TryGainMemory", [typeof(Thought_Memory), typeof(Pawn)]), prefix: new HarmonyMethod(thoughtReplacer, nameof(ThoughtReplacerPatch.ReplacePatchCreateMemoryPrefix)));
            harmony.Patch(AccessTools.Method(typeof(SituationalThoughtHandler), "TryCreateThought"), prefix: new HarmonyMethod(thoughtReplacer, nameof(ThoughtReplacerPatch.ReplacePatchSIT)));
            harmony.Patch(AccessTools.Method(typeof(ThoughtUtility), "CanGetThought"), postfix: new HarmonyMethod(typeof(CanGetThoughtPatch), nameof(CanGetThoughtPatch.CanGetPatch)));
            harmony.Patch(AccessTools.Method(typeof(ThoughtUtility), "GiveThoughtsForPawnOrganHarvested"), prefix: new HarmonyMethod(typeof(OrganPatch), nameof(OrganPatch.OrganHarvestPrefix)));


            //Max health
            harmony.Patch(AccessTools.Method(typeof(BodyPartDef), "GetMaxHealth"), postfix: new HarmonyMethod(typeof(BodyPartHealthPatch), nameof(BodyPartHealthPatch.HealthPostfix)));


            //Factions
            harmony.Patch(AccessTools.Method(typeof(Faction), "TryMakeInitialRelationsWith"), postfix: new HarmonyMethod(typeof(FactionStartRelations), nameof(FactionStartRelations.Postfix)));


            //Body Type Patches
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateBodyType"), postfix: new HarmonyMethod(typeof(BodyTypeGenPatch), nameof(BodyTypeGenPatch.Postfix)));
            harmony.Patch(AccessTools.Method(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel)), prefix: new HarmonyMethod(typeof(ApparelGraphicPatch), nameof(ApparelGraphicPatch.Prefix)));
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "BaseHeadOffsetAt"), postfix: new HarmonyMethod(typeof(HeadOffsetPatch), nameof(HeadOffsetPatch.Postfix)));


            //Pawn Generation Patches
            harmony.Patch(AccessTools.Method(typeof(TraitSet), "GainTrait"), prefix: new HarmonyMethod(typeof(TraitPatch), nameof(TraitPatch.TraitPrefix)));
            harmony.Patch(AccessTools.Method(typeof(PawnBioAndNameGenerator), "GeneratePawnName"), prefix: new HarmonyMethod(typeof(NamePatch), nameof(NamePatch.Prefix)));

            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "TryGenerateNewPawnInternal"), postfix: new HarmonyMethod(typeof(PawnGenerationPatches), nameof(PawnGenerationPatches.GraphicsGenPostfix)));
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), [typeof(PawnGenerationRequest)]), prefix: new HarmonyMethod(typeof(PawnGenerationPatches), nameof(PawnGenerationPatches.RequestChangePrefix)));
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GetXenotypeForGeneratedPawn)), postfix: new HarmonyMethod(typeof(XenoTypeGenPatch), nameof(XenoTypeGenPatch.Postfix)));
            VineLog.Log("Completed all RVR patches with no issues!");
        }
        catch (Exception e)
        {
            VineLog.Error(e.ToString());
        }

        VineLog.Log($"{harmony.GetPatchedMethods().Count()} RVR Patches completed.");
    }
}
