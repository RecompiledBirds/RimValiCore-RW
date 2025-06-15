namespace RVCRestructured;

public class ExcludedRaceShuffleDef : Def
{
    private readonly List<PawnKindDef> excludedPawnKinds = [];
    private readonly List<ThingDef> excludedRaces = [];

    public List<PawnKindDef> ExcludedPawnKinds => excludedPawnKinds;
    public List<ThingDef> ExcludedRaces => excludedRaces;
}
public class ExcludedRaceFactionShuffleDef : Def
{
    public List<FactionDef> excludedFactions = [];
}
