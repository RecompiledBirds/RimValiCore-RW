using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class NamePatch
    {
        //gonna clean this up
        //it fixes 
        public static bool Patch(ref Name __result, Pawn pawn, NameStyle style = NameStyle.Full, string forcedLastName = null)
        {
            if (!(pawn.def is RaceDef rimValiRaceDef))
            {
                return true;
            }

            string nameString = NameGenerator.GenerateName(rimValiRaceDef.race.GetNameGenerator(pawn.gender));
            NameTriple name = NameTriple.FromString(nameString);
            if (pawn.def.defName == "RimVali" && UnityEngine.Random.Range(1, 100) == 30)
            {

                __result = new NameTriple(UnityEngine.Random.Range(1, 100) != 30 ? name.First : SteamUtility.SteamPersonaName, name.Nick ?? name.First, name.Last);
                return true;
            }
            __result = new NameTriple(name.First, name.Nick ?? name.First, name.Last);

            return false;
        }
    }
}
