namespace RVCRestructured.RVR.HarmonyPatches;

public static class NameTriplePatch
{
    public static bool Patch(ref Name __result, Pawn pawn, NameStyle style = NameStyle.Full, string? forcedLastName = null)
    {
        if (pawn.TryGetComp<RVRComp>()==null)
        {
            return true;
        }

        string nameString = NameGenerator.GenerateName(pawn.def.race.GetNameGenerator(pawn.gender));
        NameTriple name = NameTriple.FromString(nameString);
        string nick = (SteamUtility.SteamPersonaName != "???" ? SteamUtility.SteamPersonaName : name.Nick ?? name.First);
        bool trySteamName = pawn.def.defName == "RimVali" && UnityEngine.Random.Range(1, 100) == 30;
        __result = trySteamName ? new NameTriple(name.First, nick, name.Last) : new NameTriple(name.First, name.Nick ?? name.First, name.Last);
        return false;
    }
}
