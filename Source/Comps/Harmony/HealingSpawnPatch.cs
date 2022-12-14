using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Comps.HarmonyPatches
{
    public static class HealingSpawnPatch
    {
        public static void Postfix(Thing __instance)
        {
            HealableGameComp healableGameComp = Find.World.GetComponent<HealableGameComp>();
            HealableMaterialCompProperties mat = HealableMats.HealableMat(__instance);
            if (mat == null)
                return;
            healableGameComp.RegisterThing(__instance, mat);
        }
    }
}
