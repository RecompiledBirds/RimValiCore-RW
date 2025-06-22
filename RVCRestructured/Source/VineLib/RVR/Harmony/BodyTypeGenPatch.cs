using RimWorld;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches;

public static class BodyTypeGenPatch
{
    public static void Postfix(ref Pawn pawn)
    {
        RVRBodyGetter bodyGetter = pawn.GetComp<RVRBodyGetter>();
        if (bodyGetter != null)
        {
            bodyGetter.GenBody();
            return;
        }
        if (!RestrictionsChecker.IsRestricted(pawn.story.bodyType)) return;

        if (ModsConfig.BiotechActive)
        {
            if (pawn.DevelopmentalStage.Juvenile())
            {
                pawn.story.bodyType = pawn.DevelopmentalStage == DevelopmentalStage.Baby ? BodyTypeDefOf.Baby : BodyTypeDefOf.Child;
                return;
            }
            if (pawn.genes != null)
            {
                List<BodyTypeDef> bodies = [];
                foreach(Gene gene in pawn.genes.GenesListForReading)
                {
                    if (gene.def.bodyType == null) continue;
                    bodies.Add(gene.def.bodyType.Value.ToBodyType(pawn));
                }
                if (bodies.TryRandomElement(out BodyTypeDef bodyTypeDef))
                {
                    pawn.story.bodyType = bodyTypeDef;
                    return;
                }
            }
            
        }
        List<BodyTypeDef> defs = [.. DefDatabase<BodyTypeDef>.AllDefs.Where(x => !RestrictionsChecker.IsRestricted(x))];
        defs.Remove(BodyTypeDefOf.Child);
        defs.Remove(BodyTypeDefOf.Baby);
        if (pawn.gender == Gender.Male) defs.Remove(BodyTypeDefOf.Female);
        if (pawn.gender == Gender.Female) defs.Remove(BodyTypeDefOf.Male);
        if (pawn.story.bodyType != null && defs.Contains(pawn.story.bodyType))
            return;

        pawn.story.bodyType = defs.RandomElement();
    }
}
