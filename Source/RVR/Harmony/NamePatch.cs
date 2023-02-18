using HarmonyLib;
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
        public static bool Prefix(ref Name __result, Pawn pawn, NameStyle style = NameStyle.Full, string forcedLastName = null)
        {
            if (!(pawn.def is RaceDef raceDef)) return true;


            string nameString = NameGenerator.GenerateName(raceDef.race.GetNameGenerator(pawn.gender));
            NameTriple name = NameTriple.FromString(nameString);

            if (Rand.Chance(0.01f))
            {

                __result = new NameTriple(UnityEngine.Random.Range(1, 100) != 30 ? name.First : SteamUtility.SteamPersonaName, name.Nick ?? name.First, name.Last);
                return true;
            }
            __result = new NameTriple(name.First, name.Nick ?? name.First, name.Last);

            return false;
        }
    }
}
