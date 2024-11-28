using RVCRestructured.RVR;
namespace RVCRestructured;

public record DefRestrictionInfo
{
    public static DefRestrictionInfo Empty => new();

    public Def Def { get; } = null!;

    public Def User { get; private set; } = null!;

    public bool UserWhitelisted { get; private set; }

    public bool UserBlacklisted { get; private set; }

    public bool IsRequired { get; private set; }

    public bool CanUse => this != Empty && IsRequired || UserWhitelisted || (!Def.IsRestricted() && !UserBlacklisted);

    private DefRestrictionInfo() 
    {
        UserWhitelisted = false;
        UserBlacklisted = false;
        IsRequired = false;
    }

    public DefRestrictionInfo(Def def) : this()
    {
        Def = def;
    }

    public void WhiteListUser(Def def)
    {
        User = def;
        UserWhitelisted = true;
        RestrictionsChecker.MarkRestricted(Def);
    }

    public void BlackListUser(Def def)
    {
        User = def;
        UserBlacklisted = true;
    }

    public void SetRequired(Def def)
    {
        User = def;
        IsRequired = true;
    }
}
