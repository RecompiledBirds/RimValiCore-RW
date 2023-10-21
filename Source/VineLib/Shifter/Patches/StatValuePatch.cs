using HarmonyLib;
using RimWorld;
using RVCRestructured.RVR.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Shifter
{
    public static class StatValuePatch
    {
        public static void StatPostfix(StatRequest req, StatDef ___stat, bool applyPostProcess, ref float __result)
        {
            if (!(req.Thing is Pawn pawn)) return;
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if(comp==null) return;
            __result += comp.OffsetStat(___stat);
        }

        public static IEnumerable<StatDrawEntry> RacePostfix(IEnumerable<StatDrawEntry> __result, ThingDef parentDef, StatRequest req, RaceProperties __instance)
        {
            Pawn pawn = req.Pawn ?? (req.Thing as Pawn);
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if (comp == null)
            {
                foreach(StatDrawEntry entry in __result)
                {
                    yield return entry;
                }
                yield break;
               
            }
            if (comp.IsParentDef())
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }

                yield break;

            }
            foreach (StatDrawEntry entry in __result)
            {
                if (entry.LabelCap == "Race".Translate().CapitalizeFirst())
                {
                    yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "Race".Translate(), comp.label().CapitalizeFirst(), comp.CurrentForm.description, 2100, null, null, false);
                    continue;
                }
                yield return entry;
            
            }
            yield break;
        }

        public static IEnumerable<StatDrawEntry> PawnPostfix(IEnumerable<StatDrawEntry> __result, Pawn __instance)
        {
            ShapeshifterComp comp = __instance.TryGetComp<ShapeshifterComp>();
            if (comp == null)
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }
                yield break;

            }
            if (comp.IsParentDef())
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }

                yield break;

            }
            foreach (StatDrawEntry entry in __result)
            {
                if (entry.LabelCap == "Race".Translate().CapitalizeFirst())
                {
                    string reportText = __instance.genes.UniqueXenotype ? "UniqueXenotypeDesc".Translate().ToString() : comp.CurrentForm.description;
                    yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "Race".Translate(), comp.label().CapitalizeFirst() + " (" + __instance.genes.XenotypeLabel + ")", reportText, 2100, null, __instance.genes.UniqueXenotype ? null : Gen.YieldSingle(new Dialog_InfoCard.Hyperlink(__instance.genes.Xenotype, -1)), false);
                    
                    continue;
                }
                yield return entry;

            }
            yield break;
        }

        public static IEnumerable<StatDrawEntry> SourcePostFix(IEnumerable<StatDrawEntry> __result, StatRequest req, Def __instance)
        {
            Pawn pawn = req.Pawn ?? (req.Thing as Pawn);
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if (comp == null)
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }
                yield break;

            }
            if (comp.IsParentDef())
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }
                yield break;

            }
            foreach (StatDrawEntry entry in __result)
            {
                if (entry.LabelCap == "Stat_Source_Label".Translate().CapitalizeFirst())
                {
                    TaggedString t = comp.ContentPack.IsOfficialMod ? "Stat_Source_OfficialExpansionReport".Translate() : "Stat_Source_ModReport".Translate();
                    yield return new StatDrawEntry(StatCategoryDefOf.Source, "Stat_Source_Label".Translate(), comp.ContentPack.Name, t + ": " + comp.ContentPack.Name, 90000, null, null, false);
                    continue;
                }
                yield return entry;

            }
            yield break;
        }

        public static IEnumerable<StatDrawEntry> DescPostFix(IEnumerable<StatDrawEntry> __result, StatRequest req, Def __instance)
        {
            Pawn pawn = req.Pawn ?? (req.Thing as Pawn);
            ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
            if (comp == null)
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }
                yield break;

            }
            if (comp.IsParentDef())
            {
                foreach (StatDrawEntry entry in __result)
                {
                    yield return entry;
                }
                yield break;

            }
            foreach (StatDrawEntry entry in __result)
            {
                if (entry.LabelCap == "Stat_Source_Label".Translate().CapitalizeFirst())
                {
                    TaggedString t = comp.ContentPack.IsOfficialMod ? "Stat_Source_OfficialExpansionReport".Translate() : "Stat_Source_ModReport".Translate();
                    yield return new StatDrawEntry(StatCategoryDefOf.Source, "Stat_Source_Label".Translate(), comp.ContentPack.Name, t + ": " + comp.ContentPack.Name, 90000, null, null, false);
                    continue;
                }
                yield return entry;

            }
            yield break;
        }
    }
}
