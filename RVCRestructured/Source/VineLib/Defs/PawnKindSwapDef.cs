using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RVCRestructured;

public class PawnKindSwapDef :Def
{
    public List<FactionDef> forFactions = [];
    public List<FactionDef> forAllFactionsExcept = [];
    public List<PawnKindSwapOptions> swapOptions = [];

}

public class PawnKindSwapOptions
{
    [AllowNull]
    public PawnKindDef pawnKindDef;
    public float chanceOfSwap = 0.5f;
    public List<SwapOption> swapWithOptions = [];
}

public class SwapOption
{
    [AllowNull]
    public PawnKindDef? pawnKindDef;
    public XenotypeDef? xenotypeDef;
}
