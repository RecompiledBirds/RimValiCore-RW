using RVCRestructured.RVR;
using RVCRestructured.RVR.Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RVCRestructured
{
    public static class EatingPatch
    {
        private static readonly Dictionary<RaceProperties, bool> cachedDefs = new Dictionary<RaceProperties, bool>();
        public static void EdiblePatch(ref bool __result, RaceProperties __instance, ThingDef t)
        {
            ThingDef pawn = DefDatabase<ThingDef>.AllDefs.First(x => x.race == __instance);
            if (cachedDefs.NullOrEmpty() || !cachedDefs.ContainsKey(__instance))
            {
                bool restricted = RestrictionsChecker.IsRestricted(t);
                RaceDef raceDef = pawn as RaceDef;
                // Food is restricted
                // Pawn is RaceDef
                // And the race can eat it
                bool allowedByRestrictor = restricted && pawn is RaceDef && raceDef.RaceRestrictions.allowedFoodDefs.Contains(t);
                //
                // Pawn is not RaceDef = true
                // or
                // Pawn can eat any food and the food is not restricted= true
                // or
                // It is an allowed fooditem
                
                bool allowedByWhitelist = !(pawn is RaceDef) ||(!restricted&& (raceDef?.RaceRestrictions.canEatAnyFood??true)) || (raceDef?.RaceRestrictions.allowedFoodDefs.Contains(t)??true);

                bool cacheData = (allowedByRestrictor || allowedByWhitelist);

                cachedDefs[__instance] = cacheData;
            }

            if (!cachedDefs[__instance])
            {
                JobFailReason.Is($"{pawn.label} {"CannotEatRVR".Translate(pawn.label.Named("RACE"))}");
            }
            __result &= cachedDefs[__instance];
         
        }
    }
}
