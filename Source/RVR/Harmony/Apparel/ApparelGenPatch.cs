using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ApparelGenPatch
    {
        public static void ApparelGenPrefix(Pawn pawn)
        {
            Traverse apparelInfo = Traverse.Create(typeof(PawnApparelGenerator)).Field(name: "allApparelPairs");
            List<ThingStuffPair> thingStuffPairs = apparelInfo.GetValue<List<ThingStuffPair>>();
            foreach (ThingStuffPair thingStuffPair in thingStuffPairs)
            {
                if (!thingStuffPair.thing.ApparelAllowedForRace(pawn.def))
                {
                    Log.ErrorOnce(thingStuffPair.thing.defName+", "+pawn.def.defName, (pawn.def.defName + thingStuffPair.thing.defName).GetHashCode());
                }
            }
            thingStuffPairs.RemoveAll(x => !x.thing.ApparelAllowedForRace(pawn.def));
            apparelInfo.SetValue(thingStuffPairs);
        }
    }
}
