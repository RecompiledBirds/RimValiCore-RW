using RVCRestructured.VineLib.Defs.DefOfs;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class OrganPatch
{
    public static bool OrganHarvestPrefix(Pawn victim)
    {
        if (!victim.RaceProps.Humanlike)
            return true;
        
        OrganComp comp = victim.TryGetComp<OrganComp>();
        
        if (comp==null)
            victim.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.MyOrganHarvested, null);
        else
        {
            ThoughtDef? thought = comp.Props.GetHarvestedSelfThought();
            if(thought != null)
                victim.needs.mood.thoughts.memories.TryGainMemory(thought, null);

        }
        foreach (Pawn pawn in victim.Map.mapPawns.AllPawnsSpawned)
        {
            if (!pawn.IsColonist) continue;
            if (pawn.needs.mood == null)
                continue;

            if (pawn == victim)
                continue;
            
            OrganComp secondComp=pawn.TryGetComp<OrganComp>();
            if (secondComp == null)
            {
                victim.needs.mood.thoughts.memories.TryGainMemory(Vine_ThoughtDefOf.KnowColonistOrganHarvested, null);
                continue;
            }

            ThoughtDef? thought = secondComp.Props.GetHarvestedThought(victim.def, victim.IsColonist);
            if (thought != null)
                pawn.needs.mood.thoughts.memories.TryGainMemory(thought, null);
            

        }

        return true;
    }
}
