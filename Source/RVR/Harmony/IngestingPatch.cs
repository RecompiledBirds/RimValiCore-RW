using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class IngestingPatch
    {
        public static void IngestingPostfix(Pawn ingester, Thing foodSource, ThingDef foodDef, ref List<FoodUtility.ThoughtFromIngesting> __result)
        {
            bool cannibal = ingester.story.traits.HasTrait(TraitDefOf.Cannibal);

            if (!(ingester.def is RaceDef rDef))
                return;
            ThingDef r;
            List<FoodUtility.ThoughtFromIngesting> backupCopy = __result;
            try
            {
                for (int i = 0; i < __result.Count; i++)
                {
                    ThoughtDef t = __result[i].thought;

                    if (t == ThoughtDefOf.AteHumanlikeMeatDirect || t == ThoughtDefOf.AteHumanlikeMeatDirectCannibal)
                    {
                        r = foodDef.ingestible.sourceDef;

                        if (r == null)
                            continue;

                        //TODO: get thoughts for eaten races
                        __result[i] = new FoodUtility.ThoughtFromIngesting()
                        {
                            thought = rDef.CannibalismThoughtsGetter.GetThoughtsForEatenRace(r, cannibal, false)
                        };

                    }

                    if (t == ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal || t == ThoughtDefOf.AteHumanlikeMeatAsIngredient)
                    {
                        r = foodSource.TryGetComp<CompIngredients>().ingredients.Find(x => x.ingestible != null && true);


                        if (r == null)
                            continue;

                        //TODO: get thoughts for eaten races
                        __result[i] = new FoodUtility.ThoughtFromIngesting()
                        {
                            thought = rDef.CannibalismThoughtsGetter.GetThoughtsForEatenRace(r, cannibal, true)
                        };

                    }
                }
            }catch(Exception e)
            {
                RVCLog.Log($"Ingestible patch error:{e}", RVCLogType.Error);
                RVCLog.Log("Reverting ingestible thoughts to previous state.", RVCLogType.Warning);
                __result = backupCopy;
            }
        }
    }
}
