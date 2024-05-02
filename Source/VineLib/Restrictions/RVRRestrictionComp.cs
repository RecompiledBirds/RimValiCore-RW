using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.Source.VineLib.Restrictions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RVCRestructured
{
    public class RVRRestrictionComp : CompProperties
    {
        public DefRestrictionManager restrictionsManager;

        public bool IsAlwaysAllowed(RestrictionType type) => restrictionsManager.IsAlwaysAllowed(type);

        public bool IsAlwaysAllowed(Def def) => restrictionsManager.IsAlwaysAllowed(def);

        [Unsaved]
        private bool resolved = false;

        public DefRestrictionInfo this[Def def] => restrictionsManager[def];

        public HashSet<DefRestrictionInfo> this[RestrictionType type] => restrictionsManager[type];

        public RVRRestrictionComp()
        {
            this.compClass=typeof(RestrictionComp);
        }

        public override void ResolveReferences(ThingDef parentDef)
        {
            if (resolved) return;
            resolved = true;

            restrictionsManager.ResolveReferences(parentDef);
            base.ResolveReferences(parentDef);
        }
    }

    public class RestrictionComp : ThingComp
    {
        public DefRestrictionInfo this[Def def] => Props[def];

        public HashSet<DefRestrictionInfo> this[RestrictionType type] => Props[type];

        public RVRRestrictionComp Props
        {
            get
            {
                return props as RVRRestrictionComp;
            }
        }
    }
}
