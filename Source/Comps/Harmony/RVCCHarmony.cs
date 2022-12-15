using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Comps.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class RVCCHarmony
    {
        static RVCCHarmony()
        {
            Harmony harmony = new Harmony("RecompiledBirds.RVC.RVCC");
            RVCLog.Log("Starting RVCComps patches.");
            harmony.Patch(AccessTools.Method(typeof(Thing), "DeSpawn"),postfix:new HarmonyMethod(typeof(HealingDespawnPatch),nameof(HealingDespawnPatch.Postfix)));
            harmony.Patch(AccessTools.Method(typeof(Thing), "SpawnSetup"), postfix: new HarmonyMethod(typeof(HealingSpawnPatch), nameof(HealingDespawnPatch.Postfix)));

            RVCLog.Log("Finished RVCC patches.");
        }
    }
}
