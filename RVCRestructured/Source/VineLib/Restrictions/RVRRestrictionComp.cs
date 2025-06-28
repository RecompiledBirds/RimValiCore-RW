using Verse;

namespace RVCRestructured;

public class RVRRestrictionComp : CompProperties
{
    public DefRestrictionManager restrictions = new();
    public DefRestrictionManager Restrictions => restrictions;

    public bool IsAlwaysAllowed(RestrictionType type) => restrictions.IsAlwaysAllowed(type);

    public bool IsAlwaysAllowed(Def def) => restrictions.IsAlwaysAllowed(def);

    [Unsaved]
    private bool resolved = false;

    public DefRestrictionInfo this[Def def] => restrictions[def];

    public HashSet<DefRestrictionInfo> this[RestrictionType type] => restrictions[type];

    public RVRRestrictionComp()
    {
        compClass=typeof(RestrictionComp);
    }

    public override void ResolveReferences(ThingDef parentDef)
    {
        if (resolved) return;
        resolved = true;

        restrictions.ResolveReferences(parentDef);
        base.ResolveReferences(parentDef);
    }
}

public class RestrictionComp : ThingComp
{
    public DefRestrictionInfo this[Def def] => Props[def];

    public HashSet<DefRestrictionInfo> this[RestrictionType type] => Props[type];

    public RVRRestrictionComp Props => (RVRRestrictionComp)props;
}
