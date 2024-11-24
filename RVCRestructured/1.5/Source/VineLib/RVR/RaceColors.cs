namespace RVCRestructured;

public class RaceColors
{
    public string name = string.Empty;
    public TriColorGenerator colorGenerator = null!;
    public TriColorGenerator colorGeneratorFemale = null!;

    //If the female generator is not null, we know it is meant to have gendered colors.
    public bool IsGendered => colorGeneratorFemale != null;

    //Find the generator for a pawn.
    public TriColorGenerator GeneratorToUse(Pawn pawn)
    {
        if (IsGendered && pawn.gender == Gender.Female)
        {
            return colorGeneratorFemale;
        }
        return colorGenerator;
    }
}
