using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR
{
    [StaticConstructorOnStartup]
    public static class RaceLoaderOnStartup
    {
        static RaceLoaderOnStartup()
        {
            foreach(RaceDef race in DefDatabase<RaceDef>.AllDefs)
            {
                race.RaceRestrictions.OnLoad();
            }
        }
    }
}
