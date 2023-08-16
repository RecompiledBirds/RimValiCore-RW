using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.RVR.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
            bool restricted = __result != null && RestrictionsChecker.IsRestricted(__result);
            ThingDef thing = request.KindDef!=null? request.KindDef.race ?? (request.KindDef.RaceProps != null ? EatingPatch.GetDef(request.KindDef.RaceProps) : null) : null;
            if (thing == null) return;
           
            bool isRace = thing is RaceDef ;
            if(!isRace)
            {
                if(restricted)
                {
                    while(__result==null|| RestrictionsChecker.IsRestricted(__result))
                        __result = PawnGenerator.XenotypesAvailableFor(request.KindDef).RandomElementByWeight(x=>x.Value).Key;
                    
                }
                return;
            }
            RaceDef race  = thing as RaceDef ;

            if (restricted && !(!race.RaceRestrictions.restrictedXenoTypes.NullOrEmpty() && __result != null && race.RaceRestrictions.restrictedXenoTypes.Contains(__result)) || (!race.RaceRestrictions.xenoTypeWhitelist.NullOrEmpty() && __result != null && race.RaceRestrictions.xenoTypeWhitelist.Contains(__result)))
            {

                //try again
                while (__result == null || RestrictionsChecker.IsRestricted(__result) && !race.RaceRestrictions.restrictedXenoTypes.Contains(__result))
                    __result = PawnGenerator.XenotypesAvailableFor(request.KindDef).RandomElementByWeight(x => x.Value).Key;
                return;
            }
            if (!race.RaceRestrictions.xenoTypeWhitelist.NullOrEmpty()&&!race.RaceRestrictions.xenoTypeWhitelist.Contains(__result))
            {
                __result = race.RaceRestrictions.xenoTypeWhitelist.RandomElement();
            }
        }
    }
}
