using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.RVR.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Source.RVR.Harmony
{
    public static class XenoTypeGenPatch
    {
        public static void Postfix(PawnGenerationRequest request,ref XenotypeDef __result)
        {
            bool restricted = RestrictionsChecker.IsRestricted(__result);
            ThingDef thing =EatingPatch.GetDef(request.KindDef.RaceProps);
            bool isRace = thing is RaceDef ;
            if(!isRace)
            {
                if(restricted)
                {
                    //try again;
                    __result=PawnGenerator.GetXenotypeForGeneratedPawn(request);
                    return;
                }
            }
            RaceDef race  = thing as RaceDef ;
            if((restricted&&!(race.RaceRestrictions.restrictedXenoTypes.Contains(__result) )||race.RaceRestrictions.xenoTypeWhitelist.Contains(__result)))
            {
                //try again
                __result = PawnGenerator.GetXenotypeForGeneratedPawn(request);
                return;
            }

            if (!race.RaceRestrictions.xenoTypeWhitelist.NullOrEmpty()&&!race.RaceRestrictions.xenoTypeWhitelist.Contains(__result))
            {
                __result = race.RaceRestrictions.xenoTypeWhitelist.RandomElement();
            }
        }
    }
}
