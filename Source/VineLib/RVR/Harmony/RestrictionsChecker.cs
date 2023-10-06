using System.Collections.Generic;
using Verse;

namespace RVCRestructured.RVR
{
    public static class RestrictionsChecker
    {
        private static HashSet<Def> restrictedDefs = new HashSet<Def>();

        public static void AddRestriction(Def def)
        {
            if (restrictedDefs.Contains(def)) return;
            restrictedDefs.Add(def);
        }

        public static void AddRestrictions<T>(List<T> defs) where T : Def
        {
            foreach (Def def in defs)
            {
                AddRestriction(def);
            }
        }

        public static bool IsRestricted(Def def)
        {
            return restrictedDefs.Contains(def);
        }
    }
}
