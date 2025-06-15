﻿
using RVCRestructured.VineLib.Defs.DefOfs;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ButcherPatch
    {
        public static void AddButcheredThought(Pawn butcher, Pawn dead, bool isButcher = true)
        {
            RVRButcherComp comp = butcher.TryGetComp<RVRButcherComp>();
            if (comp!=null)
            {
                butcher.needs.mood.thoughts.memories.TryGainMemory(comp.Props.GetThought(dead.def,isButcher), null);
                return;
            }
            if (isButcher)
            {
                butcher.needs.mood.thoughts.memories.TryGainMemory(Vine_ThoughtDefOf.ButcheredHumanlikeCorpse, null);
                return;
            }
            butcher.needs.mood.thoughts.memories.TryGainMemory(Vine_ThoughtDefOf.KnowButcheredHumanlikeCorpse, null);
        }

        public static bool ButcherPrefix(Pawn butcher, float efficiency, ref IEnumerable<Thing> __result, Corpse __instance)
        {
            TaleRecorder.RecordTale(TaleDefOf.ButcheredHumanlikeCorpse, [butcher]);
            Pawn deadPawn = __instance.InnerPawn;
            __result = deadPawn.ButcherProducts(butcher, efficiency);
            if (!deadPawn.RaceProps.Humanlike)
            {
                return false;
            }
            foreach (Pawn targetPawn in butcher.Map.mapPawns.SpawnedPawnsInFaction(butcher.Faction))
            {
                if (targetPawn != butcher)
                {
                    AddButcheredThought(butcher, deadPawn, false);
                    continue;
                }
                AddButcheredThought(butcher, deadPawn);
            }

            return false;
        }
    }
}

