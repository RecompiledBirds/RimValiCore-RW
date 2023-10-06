using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class BodyTypeGenPatch
    {
        public static void Posfix(ref Pawn pawn)
        {
            if(!(pawn.def is RaceDef raceDef))
            {

                if (pawn.def.GetType().Name != "ThingDef_AlienRace")
                {
                    IEnumerable<BodyTypeDef> defs = DefDatabase<BodyTypeDef>.AllDefs.Where(x => !RestrictionsChecker.IsRestricted(x));

                    if (pawn.story.bodyType != null&& defs.Contains(pawn.story.bodyType))
                        return;

                    pawn.story.bodyType = defs.RandomElement();
                }
                return;
            }

            raceDef.BodyTypeGetter.SetBodyType(ref pawn);
        }
    }
}
