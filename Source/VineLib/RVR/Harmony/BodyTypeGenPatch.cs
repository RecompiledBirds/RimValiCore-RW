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
            RVRBodyGetter bodyGetter = pawn.GetComp<RVRBodyGetter>();
            if(bodyGetter != null)
            {
                bodyGetter.GenBody();
                return;
            }

            IEnumerable<BodyTypeDef> defs = DefDatabase<BodyTypeDef>.AllDefs.Where(x => !RestrictionsChecker.IsRestricted(x));

            if (pawn.story.bodyType != null && defs.Contains(pawn.story.bodyType))
                return;

            pawn.story.bodyType = defs.RandomElement();
        }
    }
}
