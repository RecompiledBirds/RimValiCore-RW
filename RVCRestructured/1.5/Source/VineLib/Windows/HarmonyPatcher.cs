using HarmonyLib;
using RimWorld;
using Verse;

namespace RVCRestructured.Windows;

[StaticConstructorOnStartup]
public static class HarmonyPatcher
{
    static HarmonyPatcher()
    {
        Harmony harmony = new("RecompiledBirds.RVC.Windows");
        harmony.Patch(AccessTools.Method(typeof(Page_ConfigureStartingPawns), "DoWindowContents"), postfix: new HarmonyMethod(typeof(ConfigurePatch), nameof(ConfigurePatch.Patch)));
    }
}
