using RimWorld;
using RVCRestructured.Source.VineLib.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Source.VineLib.RVR.Harmony
{
    public static class FactionStartRelations
    {
        public static void Postfix(Faction __instance, Faction other)
        {
            foreach (FactionRelationDef factionRelationDef in DefDatabase<FactionRelationDef>.AllDefs)
            {
                if (__instance.def != factionRelationDef.factionDef)
                    continue;
                if (other.def != factionRelationDef.otherFaction)
                    continue;

                FactionRelation rel = __instance.RelationWith(other);
                rel.baseGoodwill = factionRelationDef.opinion;
            }
        }
    }
}
