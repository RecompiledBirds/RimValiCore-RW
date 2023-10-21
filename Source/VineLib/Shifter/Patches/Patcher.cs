using HarmonyLib;
using RimWorld;
using RVCRestructured.Shifter.Patches;
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
            harmony.Patch(AccessTools.Method(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized)),postfix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.Postfix)));
            RVCLog.Log("Ran shifter patches.");

        }
    }
}
