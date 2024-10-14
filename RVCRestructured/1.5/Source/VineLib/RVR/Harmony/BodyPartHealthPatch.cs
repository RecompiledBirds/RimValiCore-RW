using RimWorld;
using RVCRestructured.VineLib.Defs.DefOfs;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR;

public static class BodyPartHealthPatch
{
    public static void HealthPostfix(ref float __result, Pawn pawn, BodyPartDef __instance)
    {
        float num = 0f;
        float multNum = 0f;

        foreach(Hediff hediff in pawn.health.hediffSet.hediffs)
        {
            if (hediff.Part == null)
                return;
            if (hediff.Part.def != __instance)
                return;
            StatModifier modifier = hediff.CurStage?.statOffsets?.Find(x => x.stat == Vine_StatDefOf.RVC_HealthOffset)??null;
            if (modifier != null)
                num += modifier.value;
            modifier = hediff.CurStage?.statFactors?.Find(x => x.stat == Vine_StatDefOf.RVC_HealthOffset)??null;
            if (modifier != null)
            {
                multNum += modifier.value;
            }
        }

        if (multNum > 0f)
        {
            __result = Mathf.CeilToInt(multNum * __instance.hitPoints * pawn.HealthScale) + num;
            return;
        }
        __result = Mathf.CeilToInt(__instance.hitPoints * pawn.HealthScale) + num;
    }
}
