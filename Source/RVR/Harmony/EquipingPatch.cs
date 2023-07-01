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
        public static void EquipingPostfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (thing.def.IsApparel) return;

            bool restricted = RestrictionsChecker.IsRestricted(thing.def);

            bool allowed = !restricted;
            if (pawn.def is RaceDef rDef)
            {
                allowed = allowed? rDef.RaceRestrictions.allowedEquipment.Contains(thing.def) || rDef.restrictions.restrictedEquipment.Contains(thing.def) : rDef.restrictions.restrictedEquipment.Contains(thing.def);
            }

            __result &= allowed;

            if (!allowed)
            {
                cantReason = "RVC_CannotUse".Translate(pawn.def.label.Named("RACE"));
            }
        }
    }
}
