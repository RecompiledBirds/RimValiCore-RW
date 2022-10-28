using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class ApparelEquipping
    {
        public static bool ApparelAllowedForRace(this ThingDef def, ThingDef race)
        {
            RaceDef raceDef = race as RaceDef;
            bool canOnlyWearApprovedApparel = raceDef?.RaceRestrictions.canUseAnyApparel ?? true;
            bool inAllowedDefs = raceDef?.RaceRestrictions.allowedApparel.Contains(def)??false;
            bool restricted = RestrictionsChecker.IsRestricted(def);

            return (restricted && inAllowedDefs) || canOnlyWearApprovedApparel?inAllowedDefs:restricted;
        }

        public static void EquipPatch(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (!thing.def.IsApparel)
                return;
            bool allowed = thing.def.ApparelAllowedForRace(pawn.def);
            __result &= allowed;
            if(!allowed)
                cantReason = "CannotWearRVR".Translate(pawn.def.label.Named("RACE"));

        }
    }
}
