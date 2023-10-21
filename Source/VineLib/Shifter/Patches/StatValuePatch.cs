using HarmonyLib;
using RimWorld;
using RVCRestructured.RVR.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Shifter.Patches
{
    public static class StatValuePatch
    {
        public static void Postfix(StatRequest req, StatDef ___stat, bool applyPostProcess, ref float __result)
        {
            if (!(req.Thing is Pawn pawn)) return;
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if(comp==null) return;
            __result += OffsetStat(comp, ___stat);
        }

        public static float OffsetStat(ShapeshifterComp comp, StatDef stat)
        {
            float result = 0;
            Pawn pawn = comp.parent as Pawn;
            if (comp.CurrentForm == pawn.def) return result;
            result -= pawn.def.statBases.GetStatOffsetFromList(stat);
            result += comp.CurrentForm.statBases.GetStatOffsetFromList(stat);
            return result;
        }

    }
}
