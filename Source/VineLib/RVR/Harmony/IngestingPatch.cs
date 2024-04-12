using RimWorld;
using RVCRestructured.Shifter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
/*
namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class IngestingPatch
    {
        private static bool AteDirectThought(ThoughtDef thought)
        {
            return thought == ThoughtDefOf.AteHumanlikeMeatDirect || thought == ThoughtDefOf.AteHumanlikeMeatDirectCannibal;
        }

        private static bool AteAsIngredientThought(ThoughtDef thought)
        {
            return thought == ThoughtDefOf.AteHumanlikeMeatAsIngredient || thought == ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal;
        }

        public static void IngestingPostfix(Pawn ingester, Thing foodSource, ThingDef foodDef, ref List<FoodUtility.ThoughtFromIngesting> __result)
        {
            
            if (!ingester.RaceProps.Humanlike)
                return;
            CannibalismComp comp = ingester.TryGetComp<CannibalismComp>();
            if (comp==null)
                return;
            RVRCannibalismComp thoughtGetter = comp.Props;
            ShapeshifterComp shapeshifterComp = ingester.TryGetComp<ShapeshifterComp>();
            if (shapeshifterComp != null)
            {
                thoughtGetter = shapeshifterComp.GetCompProperties<RVRCannibalismComp>();
            }
            if (thoughtGetter == null) return;
            List<FoodUtility.ThoughtFromIngesting> backupCopy = __result;
            List<FoodUtility.ThoughtFromIngesting> finalResult = new List<FoodUtility.ThoughtFromIngesting>();

            bool cannibal = ingester.ideo != null ? ingester.Ideo.GetPrecept(PreceptDefOf.Cannibalism_Preferred) != null || ingester.Ideo.GetPrecept(PreceptDefOf.Cannibalism_RequiredRavenous) != null || ingester.Ideo.GetPrecept(PreceptDefOf.Cannibalism_RequiredStrong) != null: false;
            try
            {
                for(int i =0; i < __result.Count; i++)
                {
                    ThoughtDef thought = __result[i].thought;
                    if(AteDirectThought(thought))
                        thoughtGetter.GetThoughtsForEatenRace(foodDef.ingestible?.sourceDef, cannibal);
                    if (AteAsIngredientThought(thought))
                    {
                        if (foodSource == null)
                            continue;

                        CompIngredients ingredients = foodSource.TryGetComp<CompIngredients>();

                        if (ingredients == null)
                            continue;

                        if (ingredients.ingredients == null)
                            continue;

                        ThingDef race = ingredients.ingredients.First(x => x.ingestible?.sourceDef?.race?.Humanlike ?? false).ingestible?.sourceDef;

                        if (race != null)
                            thought = thoughtGetter.GetThoughtsForEatenRace(race, cannibal, true);
                    }
                    finalResult.Add(new FoodUtility.ThoughtFromIngesting { fromPrecept = __result[i].fromPrecept,thought=thought });
                }
                __result= finalResult;
            }
            catch (Exception e)
            {
                RVCLog.Log($"Ingestible patch error:{e}", RVCLogType.Error);
                RVCLog.Log("Reverting ingestible thoughts to previous state.", RVCLogType.Warning);
                __result = backupCopy;
            }
                
            
        }
    }
}
*/