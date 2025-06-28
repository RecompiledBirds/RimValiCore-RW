using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RVCRestructured;

public class PawnKindSwapDef :Def
{
    [AllowNull]
    public List<FactionDef> forFactions;
    public List<PawnKindSwapOptions> swapOptions = [];

}

public class PawnKindSwapOptions
{
    [AllowNull]
    public PawnKindDef pawnKindDef;
    public List<PawnKindDef> swapWithOptions = [];
}
