using RimWorld;
using Verse;

namespace RVCRestructured.RVR;

public static class FactionStartRelations
{
    public static void Postfix(Faction __instance, Faction other)
    {
        if (__instance?.def == null ||other?.def == null) return;
        foreach (FactionRelationDef factionRelationDef in DefDatabase<FactionRelationDef>.AllDefs)
        {
            if (__instance.def != factionRelationDef.FactionDef)
                continue;
            if (other.def != factionRelationDef.OtherFaction)
                continue;

            FactionRelation rel = __instance.RelationWith(other);
            if (rel == null) return;
            rel.baseGoodwill = factionRelationDef.Opinion;
        }
    }
}
