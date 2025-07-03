using RimWorld;
using RVCRestructured.RVR;
using System.Security.Cryptography;
using Verse;

namespace RVCRestructured.RVR.Harmony;

public static class XenoTypeGenPatch
{
    public static void Postfix(PawnGenerationRequest request, ref XenotypeDef __result)
    {
        if (request.KindDef?.race is not ThingDef raceDef) return;
        RVRRestrictionComp comp = raceDef.GetCompProperties<RVRRestrictionComp>();

        bool restricted = __result.IsRestricted();
        if (comp == null)
        {
            if (restricted)
            {
                if (!PawnGenerator.XenotypesAvailableFor(request.KindDef).Where(x => !x.Key.IsRestricted()).TryRandomElementByWeight(x => x.Value, out KeyValuePair<XenotypeDef, float> keyvp)) return;
                __result = keyvp.Key;
            }
            return;
        }

        XenotypeDef[] onlyAllowedXenoTypes = comp.restrictions[RestrictionType.XenotypeDef].Where(info => info.IsRequired).Select(info => (XenotypeDef)info.Def).ToArray();
        if (onlyAllowedXenoTypes?.Length > 0)
        {
            __result = onlyAllowedXenoTypes.Contains(__result) ? __result : onlyAllowedXenoTypes.RandomElement();
            return;
        }
        if (comp.restrictions[__result].CanUse)
        {
            return;
        }

        if (!PawnGenerator.XenotypesAvailableFor(request.KindDef).Where(x => (comp.restrictions[x.Key].CanUse) || !x.Key.IsRestricted()).TryRandomElementByWeight(x => x.Value, out KeyValuePair<XenotypeDef, float> kvp))
            __result = kvp.Key;
    }

    public static void XenotypesAvailableForPostfix(PawnKindDef kind, FactionDef factionDef, Faction faction, ref Dictionary<XenotypeDef, float> __result)
    {
        List<XenotypeDef> keysCopied = [.. __result.Keys];
        if (!kind.race.HasComp<RestrictionComp>()) return;
        RVRRestrictionComp comp = kind.race.GetCompProperties<RVRRestrictionComp>();
        foreach (XenotypeDef xenotypeDef in keysCopied)
        {
            if (!xenotypeDef.IsRestricted() || (comp.IsAlwaysAllowed(xenotypeDef) || comp[xenotypeDef].CanUse)) continue;
            __result.Remove(xenotypeDef);
        }
    }
}
