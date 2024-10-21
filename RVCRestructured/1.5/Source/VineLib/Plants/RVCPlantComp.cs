namespace RVCRestructured.Plants;

public class RVCPlantCompProperties : CompProperties
{
    private readonly float minPreferredTemp = 0;
    private readonly float maxPreferredTemp = 0;

    public float MinPreferredTemp => minPreferredTemp;
    public float MaxPreferredTemp => maxPreferredTemp;

    public RVCPlantCompProperties()
    {
        compClass = typeof(RVCPlantComp);
    }
}

public class RVCPlantComp : ThingComp
{
    public RVCPlantCompProperties Props => (RVCPlantCompProperties)props;
}
