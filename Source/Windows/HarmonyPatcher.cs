using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Windows
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            Harmony harmony = new Harmony("RecompiledBirds.RVC.Windows");
           // harmony.Patch(AccessTools.Method(typeof(Page_ConfigureStartingPawns), "DoWindowContents"), postfix: new HarmonyMethod(typeof(ConfigurePatch), nameof(ConfigurePatch.Patch)));
        }
    }
}
