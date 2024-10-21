using RimWorld;
using RVCRestructured.RVR;
using Verse;

namespace RVCRestructured.Source.RVR.Harmony;

public static class XenoTypeGenPatch
{
    public static void Postfix(PawnGenerationRequest request, ref XenotypeDef __result)
    {
        if (request.KindDef?.race is not ThingDef raceDef) return;
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

        XenotypeDef[] onlyAllowedXenoTypes = comp.restrictions[RestrictionType.XenotypeDef].Where(info => info.IsRequired).Select(info => (XenotypeDef)info.Def).ToArray();
        if (onlyAllowedXenoTypes?.Length > 0)
        {
            __result = onlyAllowedXenoTypes.Contains(__result) ? __result : onlyAllowedXenoTypes.RandomElement();
            return;
        }

        if (!(comp.restrictions[__result]?.CanUse ?? false) || restricted)
        {
            if (!PawnGenerator.XenotypesAvailableFor(request.KindDef).Where(x => (comp.restrictions[x.Key]?.CanUse ?? false) || !x.Key.IsRestricted()).TryRandomElementByWeight(x => x.Value, out KeyValuePair<XenotypeDef, float> kvp)) return;
            __result = kvp.Key;
            return;
        }
    }
}
