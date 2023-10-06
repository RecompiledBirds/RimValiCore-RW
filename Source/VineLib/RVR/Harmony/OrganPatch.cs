using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches {
    public static class OrganPatch
    {
        public static bool OrganHarvestPrefix(Pawn victim)
        {
            if (!victim.RaceProps.Humanlike)
                return true;

            if (!(victim.def is RaceDef rDef))
                victim.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.MyOrganHarvested, null);
            else
            {
                ThoughtDef thought = rDef.OrganHarvestThoughtGetter.GetHarvestedSelfThought();
                if(thought != null)
                    victim.needs.mood.thoughts.memories.TryGainMemory(thought, null);

            }
            foreach (Pawn pawn in victim.Map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.needs.mood == null)
                    continue;

                if (pawn == victim)
                    continue;

                if (!(pawn.def is RaceDef raceDef))
                    continue;

                ThoughtDef thought = raceDef.OrganHarvestThoughtGetter.GetHarvestedThought(victim.def, victim.IsColonist);
                if (thought != null)
                    pawn.needs.mood.thoughts.memories.TryGainMemory(thought, null);

            }

            return true;
        }
    }
}
