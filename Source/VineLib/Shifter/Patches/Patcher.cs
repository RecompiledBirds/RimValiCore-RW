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
            harmony.Patch(AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValue)),prefix:new HarmonyMethod(typeof(StatValuePatch),nameof(StatValuePatch.PrefixGetStatValue)));
            harmony.Patch(AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValueForPawn)), prefix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.PrefixGetStatValueForPawn)));
            harmony.Patch(AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValueAbstract), parameters: new Type[]{ typeof(AbilityDef), typeof(StatDef) , typeof(Pawn) }), prefix: new HarmonyMethod(typeof(StatValuePatch), nameof(StatValuePatch.PrefixGetStatValueAbstract)));
            RVCLog.Log("Ran shifter patches.");

        }
    }
}
