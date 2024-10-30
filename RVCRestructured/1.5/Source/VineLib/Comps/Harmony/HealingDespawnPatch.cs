using Verse;

namespace RVCRestructured.Comps.HarmonyPatches;

public static class HealingDespawnPatch
{
    public static void Postfix(Thing __instance)
    {
        HealableGameComp healableGameComp = Find.World.GetComponent<HealableGameComp>();
        healableGameComp.UnregisterThing(__instance);
    }
}
