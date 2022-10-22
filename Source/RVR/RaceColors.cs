using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR
{
    public class RaceColors
    {
        public string name;
        public TriColorGenerator colorGenerator;
        public TriColorGenerator colorGeneratorFemale;


        //If the female generator is not null, we know it is meant to have gendered colors.
        public bool IsGendered
        {
            get
            {
                return colorGeneratorFemale != null;
            }
        }

        //Find the generator for a pawn.
        public TriColorGenerator GeneratorToUse(Pawn pawn)
        {
            if (IsGendered && pawn.gender == Gender.Female)
            {
                return colorGeneratorFemale;
            }
            return colorGenerator;
        }
    }
}
