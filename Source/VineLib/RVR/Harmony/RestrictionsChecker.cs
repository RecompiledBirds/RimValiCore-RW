using System.Collections.Generic;
using Verse;

namespace RVCRestructured.RVR
{
    public static class RestrictionsChecker
    {
        private static readonly HashSet<Def> restrictedDefs = new HashSet<Def>();

        public static void MarkRestricted(Def def)
        {
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
}
