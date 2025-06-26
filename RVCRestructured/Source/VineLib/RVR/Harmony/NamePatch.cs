namespace RVCRestructured.RVR.HarmonyPatches;

public static class NamePatch
{
    public static Name GenName(out NameTriple name, Pawn pawn)
    {
        string nameString = NameGenerator.GenerateName(pawn.def.race.GetNameGenerator(pawn.gender));
        name = NameTriple.FromString(nameString);
        return name;
    }

    public static bool Prefix(ref Name __result, Pawn pawn, NameStyle style = NameStyle.Full, string? forcedLastName = null)
    {
        if (pawn.TryGetComp<RVRComp>() == null) return true;
        __result = GenName(out NameTriple name, pawn);


        if (Rand.Chance(0.2f) && SteamUtility.SteamPersonaName != "???")
        {
            __result = new NameTriple(UnityEngine.Random.Range(1, 100) < 5 ? name.First : SteamUtility.SteamPersonaName, name.Nick ?? name.First, name.Last);
        }
        return false;
    }
}
