using RimWorld;
using RVCRestructured.VineLib.Defs.DefOfs;
using Verse;

namespace RVCRestructured;

public class RVRCannibalismComp : CompProperties
{
    public bool caresAboutUndefinedRaces = true;
    public List<CannibalismThoughts> thoughts = [];
    public ThoughtDef GetThoughtsForEatenRace(ThingDef race, bool cannibal, bool cooked = false)
    {
        for (int i = 0; i < thoughts.Count; i++)
        {
            CannibalismThoughts thought = thoughts[i];

            if (thought.race == race)
            {
                if (cannibal)
                {
                    if (cooked)
                        return thought.cookedCannibalThought;
                    return thought.rawCannibalThought;
                }
                if (cooked)
                    return thought.cookedEatenThought;
                return thought.rawEatenThought;
            }
        }

        if (!caresAboutUndefinedRaces)
            return Vine_ThoughtDefOf.Vine_ThoughtDidntCare;
        /*
         if (cannibal)
         {
             if (cooked)
                 return ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal;
             return ThoughtDefOf.AteHumanlikeMeatDirectCannibal;
         }
         if (cooked)
             return ThoughtDefOf.AteHumanlikeMeatAsIngredient;
         return ThoughtDefOf.AteHumanlikeMeatDirect;*/
        return null;
    }

    public RVRCannibalismComp()
    {
        compClass=typeof(CannibalismComp);
    }
}

public class CannibalismThoughts
{
    public ThoughtDef rawEatenThought;
    public ThoughtDef rawCannibalThought;

    public ThoughtDef cookedEatenThought;
    public ThoughtDef cookedCannibalThought;

    public ThingDef race;
}

public class CannibalismComp : ThingComp
{
    public RVRCannibalismComp Props => (RVRCannibalismComp)props;
}
