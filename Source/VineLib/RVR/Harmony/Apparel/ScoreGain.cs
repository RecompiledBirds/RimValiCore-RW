using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelScoreGainPatch
    {
        public static void ApparelScoreGain_NewTmp(Pawn pawn, Apparel ap, List<float> wornScoresCache, ref float __result)
        {
            ThingDef def = ap.def;
            if (!ApparelEquipping.ApparelAllowedForRace(def, pawn))
            {
                __result = -100;
                return;
            }
        }
    }
}
