using Verse;

namespace RVCRestructured.Comps;

public class HealableMaterialCompProperties : CompProperties
{
    private readonly int amountHealed = 1;
    private readonly int ticksBetweenHeal = 120;

    public int AmountHealed => amountHealed;
    public int TicksBetweenHeal => ticksBetweenHeal;

    public HealableMaterialCompProperties()
    {
        compClass = typeof(HealableMaterialCompProperties);
    }
}

public class HealableMaterialComp : ThingComp
{
    public HealableMaterialCompProperties Props => (HealableMaterialCompProperties)props;
}
