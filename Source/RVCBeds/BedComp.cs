using Verse;

namespace RVCRestructured.RVCBeds
{
    public class ResizedBedCompProperties : CompProperties
    {
        public ResizedBedCompProperties() => compClass = typeof(BedComp);
    }
    public class BedComp : ThingComp { }
}
