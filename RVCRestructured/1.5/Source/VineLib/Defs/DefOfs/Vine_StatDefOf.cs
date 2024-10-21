namespace RVCRestructured.VineLib.Defs.DefOfs;

[DefOf]
public static class Vine_StatDefOf
{
    static Vine_StatDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(Vine_StatDefOf));

    public static StatDef RVC_HealthOffset = null!;
}
