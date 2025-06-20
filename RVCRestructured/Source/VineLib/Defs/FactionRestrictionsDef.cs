using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVCRestructured;

public class FactionRestrictionsDef : Def
{
    [AllowNull]
    public FactionDef factionDef;
    public List<ResearchProjectDef> disallowedResearch = [];

    public override void PostLoad()
    {
        foreach(ResearchProjectDef def in disallowedResearch)
        {
            RVR.HarmonyPatches.ResearchPatch.DisallowResearch(factionDef, def);
        }
        base.PostLoad();
    }
}
