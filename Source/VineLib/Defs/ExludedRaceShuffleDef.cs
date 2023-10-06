using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Source.VineLib.Defs
{
    public class ExcludedRaceShuffleDef : Def
    {
        public List<PawnKindDef> excludedPawnKinds;
        public List<ThingDef> excludedRaces;
    }
}