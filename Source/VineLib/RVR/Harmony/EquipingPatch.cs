using RVCRestructured.Shifter;
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
            if (!__result) return;
            if (thing.def.IsApparel) return;

            __result = pawn.CanUse(thing.def);

            if (!__result)
            {
                cantReason = "RVC_CannotUse".Translate(pawn.def.label.Named("RACE"));
            }
        }
    }
}
