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
        public static bool EquipmentAllowedForRace(this ThingDef def, Pawn pawn)
        {
            bool restricted = RestrictionsChecker.IsRestricted(def);
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            if (comp==null) return !restricted;

            bool inAllowedDefs = comp.Props.allowedEquipment.Contains(def) || comp.Props.restrictedEquipment.Contains(def);

            return (restricted && inAllowedDefs) || !restricted;
        }

        public static void EquipingPostfix(ref bool __result, Thing thing, Pawn pawn, ref string cantReason)
        {
            if (thing.def.IsApparel) return;

            bool allowed = EquipmentAllowedForRace(thing.def,pawn);

            __result &= allowed;

            if (!allowed)
            {
                cantReason = "RVC_CannotUse".Translate(pawn.def.label.Named("RACE"));
            }
        }
    }
}
