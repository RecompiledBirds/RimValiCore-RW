using RVCRestructured.Shifter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public static class RacePropsPatch
    {
        public static bool RacePropsPrefix(ref RaceProperties __result, Pawn __instance)
        {
            ShapeshifterComp comp = __instance.TryGetComp<ShapeshifterComp>();
            if (comp == null) return true;
            __result = comp.CurrentForm.race;
            return false;

        }
    }
}
