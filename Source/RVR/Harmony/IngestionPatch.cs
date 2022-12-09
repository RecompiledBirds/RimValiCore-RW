using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class IngestionPatch
    {
        public static void IngestionPostfix(Pawn ingester, Thing foodSource, ThingDef foodDef, ref List<FoodUtility.ThoughtFromIngesting> __result)
        {
            ThoughtDef findCannibal = ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal;
            ThoughtDef findNormal = ThoughtDefOf.AteHumanlikeMeatDirect;
            RaceDef raceDef = ingester.def as RaceDef;
            bool cannibal = ingester.story.traits.HasTrait(TraitDefOf.Cannibal);
            for (int i = 0; i < __result.Count; i++)
            {
                ThoughtDef t = __result[i].thought;

                //Raw
                if (t != findCannibal && t != findNormal)
                    continue;

                ThingDef source = foodDef.ingestible.sourceDef;
                if (source == null)
                    continue;



                //Cooked
                if (t != ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal && t != ThoughtDefOf.AteHumanlikeMeatAsIngredient)
                    continue;

                


            }
        }
    }
}
