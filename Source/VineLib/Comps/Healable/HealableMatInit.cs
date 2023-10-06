using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RVCRestructured.Comps
{
    [StaticConstructorOnStartup]
    public static class HealableMats
    {
        private static readonly Dictionary<ThingDef, HealableMaterialCompProperties> mats = new Dictionary<ThingDef, HealableMaterialCompProperties>();

        public static HealableMaterialCompProperties HealableMat(Thing thing)
        {
            if (mats.ContainsKey(thing.def))
                return mats[thing.def];

            if (thing.Stuff == null || !mats.ContainsKey(thing.Stuff))
                return null;
            return mats[thing.Stuff];
        }

        static HealableMats()
        {
            int amount = 0;
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.GetCompProperties<HealableMaterialCompProperties>() == null)
                    continue;

                mats.Add(def, def.GetCompProperties<HealableMaterialCompProperties>());
                amount++;
            }
            RVCLog.Log($"Initalized {amount} healable materials");
        }
    }
}
