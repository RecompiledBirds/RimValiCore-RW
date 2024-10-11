using Verse;

namespace RVCRestructured.Plants;

public class RVCPlantCompProperties : CompProperties
{
    public float minPreferredTemp;
    public float maxPreferredTemp;
    public RVCPlantCompProperties()
    {
        compClass = typeof(RVCPlantComp);
    }
}

public class RVCPlantComp : ThingComp
{
    public RVCPlantCompProperties Props
    {
        get
        {
            return props as RVCPlantCompProperties;
        }
    }
}
