using Verse;

namespace RVCRestructured.Comps;

public class HealableMaterialCompProperties : CompProperties
{
    public int amountHealed = 1;

    public int ticksBetweenHeal = 120;

    public HealableMaterialCompProperties()
    {
        this.compClass = typeof(HealableMaterialCompProperties);
    }
}


public class HealableMaterialComp : ThingComp
{
    public HealableMaterialCompProperties Props => this.props as HealableMaterialCompProperties;
}
