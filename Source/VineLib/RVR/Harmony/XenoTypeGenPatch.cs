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
        public static void Postfix(PawnGenerationRequest request, ref XenotypeDef __result)
        {
            bool restricted = __result != null && RestrictionsChecker.IsRestricted(__result);
            ThingDef thing = request.KindDef != null ? request.KindDef.race ?? (request.KindDef.RaceProps != null ? Utils.GetDef(request.KindDef.RaceProps) : null) : null;
            if (thing == null) return;
            RVRRestrictionComp comp = thing.GetCompProperties<RVRRestrictionComp>();

            if (comp==null)
            {
                if (restricted)
                {
                    while (__result == null || RestrictionsChecker.IsRestricted(__result))
                        __result = PawnGenerator.XenotypesAvailableFor(request.KindDef).RandomElementByWeight(x => x.Value).Key;

                }
                return;
            }
            if ((restricted && !(!comp.restrictedXenoTypes.NullOrEmpty() && __result != null &&comp.restrictedXenoTypes.Contains(__result))) && comp.xenoTypeWhitelist.NullOrEmpty())
            {

                //try again
                while (__result == null || RestrictionsChecker.IsRestricted(__result) && !comp.restrictedXenoTypes.Contains(__result))
                    __result = PawnGenerator.XenotypesAvailableFor(request.KindDef).RandomElementByWeight(x => x.Value).Key;
                return;
            }
            if (!comp.xenoTypeWhitelist.NullOrEmpty() && !comp.xenoTypeWhitelist.Contains(__result))
            {
                __result =comp.xenoTypeWhitelist.RandomElement();
            }
        }
    }
}
