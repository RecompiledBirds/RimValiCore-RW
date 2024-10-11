using RimWorld;
using Verse;

namespace RVCRestructured;

public class FactionRelationDef : Def
{
    public FactionDef factionDef;

    public FactionDef otherFaction;

    public int opinion;
}
