using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class ApparelGenPatch
    {
        public static void ApparelGenPrefix(Pawn pawn)
        {
            Traverse apparelInfo = Traverse.Create(typeof(PawnApparelGenerator)).Field(name: "allApparelPairs");
            List<ThingStuffPair> thingStuffPairs = apparelInfo.GetValue<List<ThingStuffPair>>().Where(x => x.thing.ApparelAllowedForRace(pawn.def)).ToList();
            apparelInfo.SetValue(thingStuffPairs);
        }
    }
}
