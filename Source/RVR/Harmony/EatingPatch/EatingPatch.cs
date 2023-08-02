using RVCRestructured.RVR;
using RVCRestructured.RVR.HarmonyPatches;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RVCRestructured
{
    public static class EatingPatch
    {
        private static Dictionary<RaceProperties,ThingDef> cache = new Dictionary<RaceProperties, ThingDef> ();

        private static ThingDef GetDef(RaceProperties race)
        {
            if (!cache.ContainsKey(race))
               cache.Add(race,DefDatabase<ThingDef>.AllDefs.First(x => x.race == race));
            return cache[race];

        }

        public static void EdiblePatch(ref bool __result, RaceProperties __instance, ThingDef t)
        {
            bool restricted = RestrictionsChecker.IsRestricted(t);
            ThingDef def = GetDef(__instance);
            if (!(def is RaceDef raceDef))
            {
                __result &= !restricted;
                if (restricted)
                {
                    JobFailReason.Is($"{def.label} {"CannotEatRVR".Translate(def.label.Named("RACE"))}");
                }
                return;
            }

            bool isInAllowedlists = raceDef.RaceRestrictions.allowedFoodDefs.Contains(t) || raceDef.RaceRestrictions.restrictedFoodDefs.Contains(t);

            bool canEatAnyFood = raceDef.RaceRestrictions.canEatAnyFood;

            bool allowed = (restricted && isInAllowedlists) || (canEatAnyFood || isInAllowedlists);

            __result &= allowed;

            if(!allowed) JobFailReason.Is($"{def.label} {"CannotEatRVR".Translate(def.label.Named("RACE"))}");
        }
    }
}
