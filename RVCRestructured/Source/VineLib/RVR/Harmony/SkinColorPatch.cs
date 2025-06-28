using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVCRestructured.RVR.Harmony;
public static class SkinColorPatch
{
    public static bool Prefix(Pawn pawn, ref GeneDef? __result)
    {
        if (!ModsConfig.BiotechActive) return false;
        if (!pawn.TryGetComp(out VineGeneComp comp) || comp.Props.geneOptions.Count==0) return false;

        __result = null;
        
        return true;
    }
}
