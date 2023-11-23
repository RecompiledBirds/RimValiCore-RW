using RimWorld;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public class ReplaceThought
    {
        public ThoughtDef toReplace;
        public ThoughtDef replacer;
    }
    public class RVRThoughtComp : CompProperties
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

        public RVRThoughtComp()
        {
            compClass = typeof(ThoughtComp);
        }
    }

    public class ThoughtComp : ThingComp
    {
        public RVRThoughtComp Props
        {
            get
            {
                return props as RVRThoughtComp;
            }
        }
    }
}
