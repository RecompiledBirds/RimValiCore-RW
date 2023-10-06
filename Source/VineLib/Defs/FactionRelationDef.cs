using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public class FactionRelationDef : Def
    {
        public FactionDef factionDef;

        public FactionDef otherFaction;

        public int opinion;
    }
}
