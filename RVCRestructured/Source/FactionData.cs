using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVCRestructured;
[StaticConstructorOnStartup]
public static class FactionData
{
    public static float defaultRatio;
    static FactionData()
    {
        float count = DefDatabase<FactionDef>.AllDefsListForReading.Where(faction => !(faction.IsVanilla()
                                                                        || faction.pawnGroupMakers.NullOrEmpty())
                                                                        && !faction.isPlayer 
                                                                        && faction.pawnGroupMakers.Any(x => x.options?.Any(x => x.kind?.RaceProps?.Humanlike??false)??false)).Count();
        defaultRatio = 3f / (count + 3f);
    }
}
