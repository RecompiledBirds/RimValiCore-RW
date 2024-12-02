namespace RVCRestructured.VineLib.Defs.DefOfs;

[DefOf]
public static class Vine_ThoughtDefOf
{
    static Vine_ThoughtDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(Vine_ThoughtDefOf));
    [AllowNull]
    public static ThoughtDef Vine_ThoughtDidntCare;
    [AllowNull]
    public static ThoughtDef AteHumanlikeMeatDirect;
    [AllowNull]
    public static ThoughtDef AteHumanlikeMeatAsIngredient;
    [AllowNull]
    public static ThoughtDef AteHumanlikeMeatAsIngredientCannibal;
    [AllowNull]
    public static ThoughtDef AteHumanlikeMeatDirectCannibal;
    [AllowNull]
    public static ThoughtDef ButcheredHumanlikeCorpse;
    [AllowNull]
    public static ThoughtDef KnowButcheredHumanlikeCorpse;
    [AllowNull]
    public static ThoughtDef KnowColonistOrganHarvested;
    [AllowNull]
    public static ThoughtDef KnowGuestOrganHarvested;
}

