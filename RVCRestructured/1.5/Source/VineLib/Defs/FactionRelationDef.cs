namespace RVCRestructured;

public class FactionRelationDef : Def
{
    private readonly FactionDef? factionDef = null;
    private readonly FactionDef? otherFaction = null;
    private readonly int opinion = 0;

    public override IEnumerable<string> ConfigErrors()
    {
        if (factionDef == null) yield return "factionDef may not be empty";
        if (otherFaction == null) yield return "otherFaction may not be empty";

        foreach (string item in base.ConfigErrors())
        {
            yield return item;   
        }
    }

    public FactionDef FactionDef => factionDef ?? throw new NullReferenceException();
    public FactionDef OtherFaction => otherFaction ?? throw new NullReferenceException();
    public int Opinion => opinion;
}
