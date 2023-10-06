using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR
{
    public class OrganHarvestThoughtGetter
    {
        public List<OrganThought> harvestThoughts = new List<OrganThought>();
        public bool careAboutUndefinedRaces = true;

        public ThoughtDef myOrganHarvested = null;
        public bool caresAboutHarvestsFromSelf = true;

        public ThoughtDef GetHarvestedSelfThought()
        {
            if (!caresAboutHarvestsFromSelf)
                return null;
            return myOrganHarvested != null ? myOrganHarvested : ThoughtDefOf.MyOrganHarvested;
        }

        public ThoughtDef GetHarvestedThought(ThingDef race, bool colonist)
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
                return ThoughtDefOf.KnowColonistOrganHarvested;

            if (guest)
            {
                return ThoughtDefOf.KnowGuestOrganHarvested;
            }

            return null;

        }
    }

    public class OrganThought
    {
        public ThingDef race;
        public ThoughtDef guest;
        public ThoughtDef colonist;
    }
}
