using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RVCRestructured.RVR
{
    public class ReplaceThought
    {
        public ThoughtDef toReplace;
        public ThoughtDef replacer;
    }
    public class ThoughtReplacer
    {
        public List<ReplaceThought> thoughtReplacers = new List<ReplaceThought>();
        private Dictionary<ThoughtDef, ThoughtDef> cachedReplacer = new Dictionary<ThoughtDef, ThoughtDef>();
        public bool Replace(ref ThoughtDef def)
        {
            if (cachedReplacer.ContainsKey(def))
            {
                def = cachedReplacer[def];
                return true;
            }

            foreach (ReplaceThought replaceThought in thoughtReplacers)
            {
                if (replaceThought.toReplace.defName == def.defName)
                {
                    cachedReplacer[def] = replaceThought.replacer;
                    def = cachedReplacer[def];
                    return true;
                }
            }
            return false;
        }
    }
}
