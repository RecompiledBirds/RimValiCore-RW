using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter
{
    [StaticConstructorOnStartup]
    public static class Patcher
    {
        static Patcher()
        {
            Harmony harmony = new Harmony("Vine.Shifter");
            harmony.Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized)),postfix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.StatPostfix)));
            harmony.Patch(AccessTools.Method(typeof(RaceProperties), nameof(RaceProperties.SpecialDisplayStats)), postfix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.RacePostfix)));
            harmony.Patch(AccessTools.Method(typeof(Def), nameof(Def.SpecialDisplayStats)), postfix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.SourcePostFix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Def.SpecialDisplayStats)), postfix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.PawnPostfix)));
            RVCLog.Log("Ran shifter patches.");

        }
    }
}
