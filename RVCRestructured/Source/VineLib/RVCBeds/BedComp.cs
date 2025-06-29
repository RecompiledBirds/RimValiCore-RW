using Verse;

namespace RVCRestructured.RVCBeds;

public class ResizedBedCompProperties : CompProperties
{
    public ResizedBedCompProperties() => compClass = typeof(BedComp);
    public bool isPile = false;
    public FloatRange rotationRange = new(min: -180, max: 180);
}
public class BedComp : ThingComp { }
