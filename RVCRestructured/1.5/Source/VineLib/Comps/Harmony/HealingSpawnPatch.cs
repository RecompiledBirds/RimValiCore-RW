using Verse;

namespace RVCRestructured.Comps;

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
