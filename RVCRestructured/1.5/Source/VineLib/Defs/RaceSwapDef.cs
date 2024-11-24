namespace RVCRestructured;

public class RaceSwapDef : Def
{
    private readonly List<ThingDef> targetRaces = [];
    private readonly List<ThingDef> replacementRaces = [];

    public List<ThingDef> TargetRaces => targetRaces;
    public List<ThingDef> ReplacementRaces => replacementRaces;
}
