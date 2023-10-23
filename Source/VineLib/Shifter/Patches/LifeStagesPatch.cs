using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter
{
    public static class LifeStagesPatch
    {
       public static void PostfixLifeStageIndex(ref int __result, Pawn_AgeTracker __instance)
        {
            Pawn pawn = (Pawn)typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if (comp == null) return;
            typeof(Pawn_AgeTracker).GetMethod("RecalculateLifeStageIndex", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(__instance,new object[] {});
        }

        public static bool PrefixCurKindLifeStage(Pawn_AgeTracker __instance, ref PawnKindLifeStage __result)
        {
            Pawn pawn = (Pawn)typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if (comp == null) return true;
            PawnKindDef def = Utils.GetKindDef(comp.CurrentForm);
            if (def == null) return true;
            __result = def.lifeStages.Last();
            return false;
            
        }
    }
}
