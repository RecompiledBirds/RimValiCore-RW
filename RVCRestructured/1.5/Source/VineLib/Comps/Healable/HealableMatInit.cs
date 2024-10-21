using Verse;

namespace RVCRestructured.Comps;

[StaticConstructorOnStartup]
public static class HealableMats
{
    private static readonly Dictionary<ThingDef, HealableMaterialCompProperties> mats = [];

    public static HealableMaterialCompProperties? HealableMat(Thing thing)
    {
        if (mats.ContainsKey(thing.def)) return mats[thing.def];
        if (thing.Stuff == null || !mats.ContainsKey(thing.Stuff)) return null;

        return mats[thing.Stuff];
    }

    static HealableMats()
    {
        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
        {
            if (def.GetCompProperties<HealableMaterialCompProperties>() == null) continue;

            mats.Add(def, def.GetCompProperties<HealableMaterialCompProperties>());
        }

        RVCLog.Log($"Initalized {mats.Count} healable materials");
    }
}
