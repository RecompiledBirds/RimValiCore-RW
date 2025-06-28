using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVCRestructured;

public class RestrictionModExtension : DefModExtension
{
    public DefRestrictionManager restrictions = new();
    public DefRestrictionManager Restrictions => restrictions;

    public override void ResolveReferences(Def parentDef)
    {
        restrictions.ResolveReferences(parentDef);
        base.ResolveReferences(parentDef);
    }
}
