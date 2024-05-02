using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.RVR.HarmonyPatches;
using RVCRestructured.Source.VineLib.Restrictions;
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
            if (!(request.KindDef?.race is ThingDef raceDef)) return;
            RVRRestrictionComp comp = raceDef.GetCompProperties<RVRRestrictionComp>();

            bool restricted = __result.IsRestricted();
            if (comp is null)
            {
                if (restricted)
                {
                    if (!PawnGenerator.XenotypesAvailableFor(request.KindDef).Where(x => !x.Key.IsRestricted()).TryRandomElementByWeight(x => x.Value, out KeyValuePair<XenotypeDef, float> kvp)) return;
                    __result = kvp.Key;
                }
                return;
            }

            List<XenotypeDef> onlyAllowedXenoTypes = comp.restrictionsManager[RestrictionType.XenotypeDef].Where(info => info.IsRequired).Select(info => info.Def as XenotypeDef).ToList();
            if (onlyAllowedXenoTypes?.Count > 0)
            {
                __result = onlyAllowedXenoTypes.Contains(__result) ? __result : onlyAllowedXenoTypes.RandomElement();
                return;
            }

            if (!(comp.restrictionsManager[__result]?.CanUse ?? false) || restricted)
            {
                if (!PawnGenerator.XenotypesAvailableFor(request.KindDef).Where(x => (comp.restrictionsManager[x.Key]?.CanUse ?? false) || !x.Key.IsRestricted()).TryRandomElementByWeight(x => x.Value, out KeyValuePair<XenotypeDef, float> kvp)) return;
                __result = kvp.Key;
                return;
            }
        }
    }
}
