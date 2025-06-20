using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVCRestructured;

public class FactionRestrictionsDef : Def
{
    public List<ResearchProjectDef> disAllowedResearch = [];

    public override void PostLoad()
    {
        base.PostLoad();
    }
}
