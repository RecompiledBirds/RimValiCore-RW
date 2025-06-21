using System.Runtime.CompilerServices;
using Verse;

namespace RVCRestructured.RVR;

public static class RestrictionsChecker
{
    private static readonly HashSet<Def> restrictedDefs = [];

    
    public static void MarkRestricted(Def def)
    {
        VineLog.Log($"Restricted {def}.",debugOnly:true);
        restrictedDefs.Add(def);
    }

    public static void MarkRestricted<T>(IEnumerable<T> defs) where T : Def
    {
        foreach (Def def in defs)
        {
            MarkRestricted(def);
        }
    }

    public static bool IsRestricted(this Def def)
    {
        return restrictedDefs.Contains(def);
    }
}
