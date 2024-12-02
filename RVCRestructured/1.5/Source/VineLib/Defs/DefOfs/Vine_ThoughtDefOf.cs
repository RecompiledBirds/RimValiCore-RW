namespace RVCRestructured.VineLib.Defs.DefOfs;

[DefOf]
public static class Vine_ThoughtDefOf
{
    static Vine_ThoughtDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(Vine_ThoughtDefOf));

    public static ThoughtDef Vine_ThoughtDidntCare = null!; 
    public static ThoughtDef AteHumanlikeMeatDirect = null!;
    public static ThoughtDef AteHumanlikeMeatAsIngredient = null!;
    public static ThoughtDef AteHumanlikeMeatAsIngredientCannibal = null!;
    public static ThoughtDef AteHumanlikeMeatDirectCannibal = null!;

    public static ThoughtDef ButcheredHumanlikeCorpse = null!;
    public static ThoughtDef KnowButcheredHumanlikeCorpse = null!;

    public static ThoughtDef KnowColonistOrganHarvested = null!;
    internal static ThoughtDef KnowGuestOrganHarvested = null!;
}

