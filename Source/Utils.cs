using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public static class Utils
    {
        private static Dictionary<RaceProperties, ThingDef> thingDefCache = new Dictionary<RaceProperties, ThingDef>();
        private static Dictionary<ThingDef, PawnKindDef> pawnKindCache = new Dictionary<ThingDef, PawnKindDef>();

        public static ThingDef GetDef(RaceProperties race)
        {
            if (!thingDefCache.ContainsKey(race))
                thingDefCache.Add(race, DefDatabase<ThingDef>.AllDefs.First(x => x.race == race));
            return thingDefCache[race];

        }
        public static PawnKindDef GetKindDef(ThingDef race)
        {
            if (!pawnKindCache.ContainsKey(race))
                pawnKindCache.Add(race, DefDatabase<PawnKindDef>.AllDefs.First(x => x.race == race));
            return pawnKindCache[race];

        }
    }
}
