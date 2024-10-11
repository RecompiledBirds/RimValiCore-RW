using RimWorld;
using Verse;

namespace RVCRestructured;

public class ButcherThought
{
    public ThoughtDef butchered;
    public ThoughtDef knowButchered;

    public ThingDef race;
}

public class RVRButcherThoughtsComp : CompProperties
{
    public bool careAboutUndefinedRaces = true;
    public List<ButcherThought> butcherThoughts = [];

    public ThoughtDef GetThought(ThingDef race, bool isButcher)
    {
        foreach (ButcherThought thought in butcherThoughts)
        {
            if (thought.race != race)
                continue;

            return isButcher ? thought.butchered : thought.knowButchered;

        }

        if (!careAboutUndefinedRaces)
            return null;
        return null;
      //  return isButcher ? ThoughtDefOf.ButcheredHumanlikeCorpse : ThoughtDefOf.KnowButcheredHumanlikeCorpse;
    }
    public RVRButcherThoughtsComp() {
        compClass = typeof(RVRButcherComp);
    }
}

public class RVRButcherComp : ThingComp
{
    public RVRButcherThoughtsComp Props
    {
        get
        {
            return props as RVRButcherThoughtsComp;
        }
    }
}
