using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RVCRestructured.Defs;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class GenerationPatches
    {
        public static void GraphicsGenPostfix(ref Pawn __result)
        {
            RVRComp comp = __result.TryGetComp<RVRComp>();
            if (comp != null)
            {
                comp.GenGraphics();
            }
        }
        public static IEnumerable<CodeInstruction> BlendTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            List<CodeInstruction> codes = instructions.ToList();
            for (int a = 0; a < codes.Count; a++)
            {
                //Look for where the pawn is created.
                CodeInstruction code = codes[a];
                CodeInstruction next = a<codes.Count-1? codes[a + 1]:null;
                if (code.opcode == OpCodes.Ldarg_0 && !found)
                {
                    if(next!=null && next.opcode==OpCodes.Call && next.Calls(typeof(PawnGenerationRequest).GetMethod("get_KindDef"))){
                        yield return codes[a];
                        yield return new CodeInstruction(OpCodes.Ldobj, typeof(PawnGenerationRequest));
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GenerationPatches), nameof(GetHumanoidRace), new Type[] { typeof(PawnGenerationRequest) }));
                        a += 4;
                        found = true;

                    }
                    else
                    {
                        yield return codes[a];
                    }
                }
                else
                {
                    yield return codes[a];
                }
            }
        }
        private static void test(PawnGenerationRequest request)
        {
            RVCLog.MSG(request.KindDef.race);
        }

        private static bool SkipOnce;
        private static List<ExcludedRaceShuffleDef> excludedDefs = null;

        private static List<ExcludedRaceShuffleDef> ExcludedDefs
        {
            get
            {
                if (excludedDefs != null)
                    return excludedDefs;
                excludedDefs = DefDatabase<ExcludedRaceShuffleDef>.AllDefsListForReading;
                return excludedDefs;
            }
        }

        private static List<RaceSwapDef> shuffle = null;

        private static List<RaceSwapDef> ShuffleDefs
        {
            get
            {
                if (shuffle != null)
                    return shuffle;
                shuffle = DefDatabase<RaceSwapDef>.AllDefsListForReading;
                return shuffle;
            }
        }

        public static void SkipReplacementGeneratorOnce()
        {
            SkipOnce = true;
        }

        public static bool ShouldSwitchPawnkindBased(PawnGenerationRequest request)
        {
            return ShuffleDefs.Count != 0 && ShuffleDefs.Any(x => x.targetRaces.Contains(request.KindDef.race)) && request.KindDef.RaceProps.Humanlike;
        }
        public static bool ShouldSwitch(PawnGenerationRequest request)
        {
            return request.KindDef.RaceProps.Humanlike && Rand.Chance(0.3f);
        }

        public static bool CanSwapRace(ThingDef def)
        {
            return ExcludedDefs.NullOrEmpty() || !ExcludedDefs.Any(x => x.excludedRaces.Count > 0 && x.excludedRaces.Contains(def));
        }

        public static bool CanSwapPawnkind(PawnKindDef def)
        {
            return ExcludedDefs.NullOrEmpty() || !ExcludedDefs.Any(x => !x.excludedPawnKinds.NullOrEmpty() && x.excludedPawnKinds.Contains(def));
        }

        public static Thing GetHumanoidRace(PawnGenerationRequest request)
        {
            ThingDef def = request.KindDef.race;
            //saftey check for scenario pawns
            if (request.Context.HasFlag(PawnGenerationContext.PlayerStarter) || !VineMod.VineSettings.RaceBlender)
                return CreateThing(def);
            if (!CanSwapRace(def))
            {
                return CreateThing(def);
            }
            if (SkipOnce)
            {
                SkipOnce = false;
                return CreateThing(def);
            }
            if (ShouldSwitch(request) && CanSwapPawnkind(request.KindDef))
            {
                List<ThingDef> defs = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.race != null && x.race.Humanlike).ToList();
                def = defs.RandomElement();
                
            }
            if (ShouldSwitchPawnkindBased(request))
            {
                RaceSwapDef randomSwapDef = ShuffleDefs.Where(x => x.targetRaces.Contains(def)).RandomElement();
                def = randomSwapDef.replacementRaces.RandomElement();
            }
          return CreateThing(def);
        }

        private static Thing CreateThing(ThingDef def)
        {
            Thing thing = ThingMaker.MakeThing(def);
            return thing;
        }

    }
}
