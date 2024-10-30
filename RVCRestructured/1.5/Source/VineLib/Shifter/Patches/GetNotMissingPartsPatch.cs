using Verse;

namespace RVCRestructured.Shifter;

public static class GetNotMissingPartsPatch
{
    public static IEnumerable<BodyPartRecord> PawnPostfix(IEnumerable<BodyPartRecord> __result, HediffSet __instance, BodyPartHeight height, BodyPartDepth depth, BodyPartTagDef tag, BodyPartRecord partParent)
    {
        ShapeshifterComp comp = __instance.pawn.TryGetComp<ShapeshifterComp>();
        if (comp == null)
        {
            foreach (BodyPartRecord entry in __result)
            {
                yield return entry;
            }
            yield break;

        }
        bool healthUnstable = comp.FormUnstable();
        if(healthUnstable)
        {
            foreach (BodyPartRecord entry in __result)
            {
                yield return entry;
            }
            yield break;
        }

        IEnumerable<BodyPartRecord> body= comp.GetBodyPartRecords(__instance,height,depth,tag,partParent);
        foreach (BodyPartRecord entry in body)
        {
            if (__instance.PartIsMissing(entry)) continue;
            if((height == BodyPartHeight.Undefined || entry.height == height) && (depth == BodyPartDepth.Undefined || entry.depth == depth) && (tag == null || entry.def.tags.Contains(tag)) && (partParent == null || entry.parent == partParent))
            {
                yield return entry;
            }
        }

        yield break;
    }
}
