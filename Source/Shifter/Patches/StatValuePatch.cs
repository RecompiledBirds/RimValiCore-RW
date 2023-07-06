using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter.Patches
{
    public static class StatValuePatch
    {
        public static bool PrefixGetStatValue(this Thing thing, StatDef stat, bool applyPostProcess, int cacheStaleAfterTicks,ref float __result)
        {
            ShapeshifterComp shapeshifter = thing.TryGetComp<ShapeshifterComp>();
            if (shapeshifter!=null && shapeshifter.CurrentForm != thing.def)
            {
                __result = stat.Worker.GetValue(ThingMaker.MakeThing(shapeshifter.CurrentForm,thing.Stuff), applyPostProcess, cacheStaleAfterTicks);
                return false;
            }
            return true;
        }

        public static bool PrefixGetStatValueForPawn(this Thing thing, StatDef stat, Pawn pawn, bool applyPostProcess,ref  float __result)
        {
            ShapeshifterComp shapeshifter = pawn.TryGetComp<ShapeshifterComp>();
            if (shapeshifter != null && shapeshifter.CurrentForm != pawn.def)
            {
                __result = stat.Worker.GetValue(thing, ThingMaker.MakeThing(shapeshifter.CurrentForm, thing.Stuff) as Pawn, applyPostProcess);
                return false;
            }
            return true;
        }

        public static bool PrefixGetStatValueAbstract(this AbilityDef def, StatDef stat, Pawn forPawn,ref float __result)
        {
            if (forPawn == null)
                return true;
            ShapeshifterComp shapeshifter = forPawn.TryGetComp<ShapeshifterComp>();
            if (shapeshifter != null && shapeshifter.CurrentForm != forPawn.def)
            {
                __result = stat.Worker.GetValueAbstract(def, ThingMaker.MakeThing(shapeshifter.CurrentForm) as Pawn);
                return false;
            }
            return true;
        }
    }
}
