using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR
{
    public class ButcherThoughtGetter
    {
        public bool careAboutUndefinedRaces = true;
        public List<ButcherThought> butcherThoughts = new List<ButcherThought>();

        public ThoughtDef GetThought(ThingDef race, bool isButcher)
        {
            foreach(ButcherThought thought in butcherThoughts)
            {
                if (thought.race != race)
                    continue;

                return isButcher ? thought.butchered : thought.knowButchered;

            }

            if (!careAboutUndefinedRaces)
                return null;

            return isButcher ? ThoughtDefOf.ButcheredHumanlikeCorpse : ThoughtDefOf.KnowButcheredHumanlikeCorpse;
        }
    }

    public class ButcherThought
    {
        public ThoughtDef butchered;
        public ThoughtDef knowButchered;

        public ThingDef race;
    }
}
