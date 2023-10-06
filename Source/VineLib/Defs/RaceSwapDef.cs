using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public class RaceSwapDef : Def
    {
        public List<ThingDef> targetRaces;
        public List<ThingDef> replacementRaces;
    }
}