using Verse;

namespace RVCRestructured.Shifter;

/// <summary>
/// Changes some of the display UI elements to better fit shapeshifters.
/// </summary>
public static class StatDrawEntryPatches
{
    /// <summary>
    /// Displays the race as the shifted race
    /// </summary>
    /// <param name="__result"></param>
    /// <param name="parentDef"></param>
    /// <param name="req"></param>
    /// <param name="__instance"></param>
    /// <returns></returns>
    public static IEnumerable<StatDrawEntry> RacePostfix(IEnumerable<StatDrawEntry> __result, ThingDef parentDef, StatRequest req, RaceProperties __instance)
    {
        if (req.Thing is not Pawn pawn) yield break;
        if (!pawn.TryGetComp(out ShapeshifterComp comp))
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
        bool healthUnstable = comp.FormUnstable();
        foreach (StatDrawEntry entry in __result)
        {
            if (entry.LabelCap == "Race".Translate().CapitalizeFirst())
            {
                yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "Race".Translate(), $"{comp.Label().CapitalizeFirst()}{(healthUnstable ? "?" : "")}", comp.CurrentForm.description, 2100, null, null, false);
                continue;
            }
            yield return entry;

        }
        yield break;
    }
    /// <summary>
    /// Displays the race as the shifted race
    /// </summary>
    /// <param name="__result"></param>
    /// <param name="parentDef"></param>
    /// <param name="req"></param>
    /// <param name="__instance"></param>
    /// <returns></returns>
    public static IEnumerable<StatDrawEntry> PawnPostfix(IEnumerable<StatDrawEntry> __result, Pawn __instance)
    {
        if (!__instance.TryGetComp(out ShapeshifterComp comp))
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
        bool healthUnstable = comp.FormUnstable();
        foreach (StatDrawEntry entry in __result)
        {
            if (entry.LabelCap == "Race".Translate().CapitalizeFirst())
            {
                string reportText = __instance.genes.UniqueXenotype ? "UniqueXenotypeDesc".Translate().ToString() : comp.CurrentForm.description;
                yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "Race".Translate(), $"{comp.Label().CapitalizeFirst()}" + " (" + __instance.genes.XenotypeLabel + ")" + $"{(healthUnstable ? "?" : "")}", reportText, 2100, null, __instance.genes.UniqueXenotype ? null : Gen.YieldSingle(new Dialog_InfoCard.Hyperlink(__instance.genes.Xenotype, -1)), false);

                continue;
            }
            yield return entry;

        }
        yield break;
    }
    /// <summary>
    /// Updates the source display.
    /// </summary>
    /// <param name="__result"></param>
    /// <param name="req"></param>
    /// <param name="__instance"></param>
    /// <returns></returns>
    public static IEnumerable<StatDrawEntry> SourcePostFix(IEnumerable<StatDrawEntry> __result, StatRequest req, Def __instance)
    {
        if (req.Pawn==null) return __result;
        Pawn pawn = req.Pawn;
        if (!pawn.TryGetComp(out ShapeshifterComp comp)) return __result;

        if (comp.IsParentDef()) return __result;

        return ModifyResult(__result, comp);
    }

    private static IEnumerable<StatDrawEntry> ModifyResult(IEnumerable<StatDrawEntry> __result, ShapeshifterComp comp)
    {
        foreach (StatDrawEntry entry in __result)
        {
            if (entry.LabelCap == "Stat_Source_Label".Translate().CapitalizeFirst())
            {
                TaggedString t = comp.ContentPack.IsOfficialMod ? "Stat_Source_OfficialExpansionReport".Translate() : "Stat_Source_ModReport".Translate();
                yield return new StatDrawEntry(StatCategoryDefOf.Source, "Stat_Source_Label".Translate(), comp.ContentPack.Name, t + ": " + comp.ContentPack.Name, 90000);
                continue;
            }

            yield return entry;
        }
    }

    /// <summary>
    /// Updates the description to match the shifted race.
    /// </summary>
    /// <param name="__result"></param>
    /// <param name="req"></param>
    /// <param name="__instance"></param>
    /// <returns></returns>
    public static IEnumerable<StatDrawEntry> DescPostFix(IEnumerable<StatDrawEntry> __result, StatRequest req, Def __instance)
    {
        Pawn pawn = req.Pawn ?? ((Pawn)req.Thing);
        if (!pawn.TryGetComp(out ShapeshifterComp comp))
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
