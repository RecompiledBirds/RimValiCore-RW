
using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured
{
    public class OrganThought
    {
        public ThingDef race;
        public ThoughtDef guest;
        public ThoughtDef colonist;
    }
    public class RVROrganHarvestComp : CompProperties
    {
        public RVROrganHarvestComp() { 
            compClass=typeof(OrganComp);
        }

        public List<OrganThought> harvestThoughts = new List<OrganThought>();
        public bool careAboutUndefinedRaces = true;

        public ThoughtDef myOrganHarvested = null;
        public bool caresAboutHarvestsFromSelf = true;

        public ThoughtDef? GetHarvestedSelfThought()
        {
            if (!caresAboutHarvestsFromSelf)
                return null;
            return myOrganHarvested ?? ThoughtDefOf.MyOrganHarvested;
        }

        public ThoughtDef? GetHarvestedThought(ThingDef race, bool colonist)
        {
            bool guest = !colonist;
            for (int i = 0; i < harvestThoughts.Count; i++)
            {
                OrganThought thought = harvestThoughts[i];
                if (thought.race != race)
                    continue;

                if (colonist)
                    return thought.colonist;

                if (guest)
                {
                    if (thought.guest != null)
                        return thought.guest;
                    return thought.colonist;
                }
            }

            if (!careAboutUndefinedRaces)
                return null;
            if (colonist)
                return Vine_ThoughtDefOf.KnowColonistOrganHarvested;

            if (guest)
            {
                return Vine_ThoughtDefOf.KnowGuestOrganHarvested;
            }

            return null;

        }
    }

    public class OrganComp : ThingComp
    {
        public RVROrganHarvestComp Props
        {
            get
            {
                return props as RVROrganHarvestComp;
            }
        }
    }
}

