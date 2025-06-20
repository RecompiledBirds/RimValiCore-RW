using RimWorld;
using RVCRestructured.Shifter;
using Verse;
using Verse.AI;

namespace RVCRestructured.RVR;

public static class ConstructionPatch
{
    public static void Constructable(Thing t, Pawn p, ref bool __result)
    {
        if (!__result) return; //Skip code if this already can't be done

        GetLabelAndRestrictionsFor(p, out string label, out RVRRestrictionComp? restrictions);
        __result = (restrictions?[t.def].CanUse??false ) || !t.def.IsRestricted() || (restrictions?.IsAlwaysAllowed(t.def) ?? false);
        if (!__result) JobFailReason.Is(label + " " + "RVC_CannotBuild".Translate(label.Named("RACE")));
    }

    /// <summary>
    ///     Get's a <seealso cref="Pawn"/> <paramref name="pawn"/>s <seealso cref="RVRRestrictionComp"/> <paramref name="restrictions"/> and simultaniously
    ///     sets the <seealso cref="string"/> <paramref name="label"/> correctly depending on if the <paramref name="pawn"/> has a <seealso cref="ShapeshifterComp"/>
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="label"></param>
    /// <param name="restrictions"></param>
    private static void GetLabelAndRestrictionsFor(Pawn pawn, out string label, out RVRRestrictionComp? restrictions)
    {
        if (pawn.TryGetComp<ShapeshifterComp>() is ShapeshifterComp shapeshifterComp)
        {
            restrictions = shapeshifterComp.GetCompProperties<RVRRestrictionComp>();
            label = shapeshifterComp.Label();
        }
        else
        {
            restrictions = pawn.TryGetComp<RestrictionComp>()?.Props;
            label = pawn.def.label;
        }
    }
}
