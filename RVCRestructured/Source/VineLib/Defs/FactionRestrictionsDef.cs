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
    public List<ResearchProjectDef> unlocksResearch = [];
    public override void ResolveReferences()
    {
        foreach (ResearchProjectDef def in disallowedResearch)
        {
            RVR.HarmonyPatches.ResearchPatch.DisallowResearch(factionDef, def);
        }
        foreach (ResearchProjectDef def in unlocksResearch)
        {
            RVR.HarmonyPatches.ResearchPatch.GrantResearch(factionDef, def);
        }
    }
}
