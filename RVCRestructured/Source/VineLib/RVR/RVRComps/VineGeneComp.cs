using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RVCRestructured.RVR.Harmony; 
public class TriColorGeneOptions
{
    public List<VineColorGeneDef> genes= [];
}
public class VineGeneCompProperties : CompProperties
{
    public List<TriColorGeneOptions> geneOptions = [];
    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        if (!ModsConfig.BiotechActive)
        {
            VineLog.Warn($"{parentDef} is using GeneCompProperties, but biotech is not active.");
        }
        return base.ConfigErrors(parentDef);
    }
    public VineGeneCompProperties()
    {
        this.compClass = typeof(VineGeneComp);
    }
}
public class VineGeneComp : ThingComp
{
    public VineGeneCompProperties Props => (VineGeneCompProperties)this.props;
}
