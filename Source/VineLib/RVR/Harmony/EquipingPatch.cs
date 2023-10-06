using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class EquipingPatch
    {
        public static bool EquipmentAllowedForRace(this ThingDef def, ThingDef race)
        {
            bool restricted = RestrictionsChecker.IsRestricted(def);
            if (!(race is RaceDef raceDef)) return !restricted;

            bool inAllowedDefs = raceDef.RaceRestrictions.allowedEquipment.Contains(def) || raceDef.RaceRestrictions.restrictedEquipment.Contains(def);

            return (restricted && inAllowedDefs) || !restricted;
        }

        public static void EquipingPostfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (thing.def.IsApparel) return;

            bool allowed = EquipmentAllowedForRace(thing.def,pawn.def);

            __result &= allowed;

            if (!allowed)
            {
                cantReason = "RVC_CannotUse".Translate(pawn.def.label.Named("RACE"));
            }
        }
    }
}
