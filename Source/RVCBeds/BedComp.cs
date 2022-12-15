using Verse;

namespace RimValiCore_RW.RVCBeds
{
    public class ResizedBedCompProperties : CompProperties
    {
        public ResizedBedCompProperties() => compClass = typeof(BedComp);
    }
    public class BedComp : ThingComp { }
}
