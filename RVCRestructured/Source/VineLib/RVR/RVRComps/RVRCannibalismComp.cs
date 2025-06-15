using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured;

public class RVRCannibalismComp : CompProperties
{
    public bool caresAboutUndefinedRaces = true;
    public List<CannibalismThoughts> thoughts = [];

    public ThoughtDef? GetThoughtsForEatenRace(ThingDef race, bool cannibal, bool cooked = false)
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
       
        
         if (cannibal)
         {
             if (cooked)
                 return Vine_ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal;
             return Vine_ThoughtDefOf.AteHumanlikeMeatDirectCannibal;
         }
         if (cooked)
             return Vine_ThoughtDefOf.AteHumanlikeMeatAsIngredient;
         return Vine_ThoughtDefOf.AteHumanlikeMeatDirect;
    }

    public RVRCannibalismComp()
    {
        compClass = typeof(CannibalismComp);
    }
}

public class CannibalismThoughts
{
    public readonly ThoughtDef rawEatenThought = null!;
    public readonly ThoughtDef rawCannibalThought = null!;

    public readonly ThoughtDef cookedEatenThought = null!;
    public readonly ThoughtDef cookedCannibalThought = null!;

    public readonly ThingDef race = null!;
}

public class CannibalismComp : ThingComp
{
    public RVRCannibalismComp Props => (RVRCannibalismComp)props;
}
