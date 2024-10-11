using RVCRestructured.RVR;
using Verse;

namespace RVCRestructured;

public record DefRestrictionInfo
{
    public static DefRestrictionInfo Empty => new();

    public Def Def { get; }

    public Def User { get; private set; }

    public bool UserWhitelisted { get; private set; }

    public bool UserBlacklisted { get; private set; }

    public bool IsRequired { get; private set; }

    public bool CanUse => IsRequired || UserWhitelisted || (!Def.IsRestricted() && !UserBlacklisted);

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
        RestrictionsChecker.MarkRestricted(def);
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
