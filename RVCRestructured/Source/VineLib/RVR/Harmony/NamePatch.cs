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
        if (!(pawn.TryGetComp(out VineSpeciesConfigComp comp)&&comp.Props.overrideNamerPatch)) return true;
        __result = GenName(out NameTriple name, pawn);


        if (Rand.Chance(0.1f) && SteamUtility.SteamPersonaName != "???")
        {
            __result = new NameTriple(Rand.Chance(0.05f)? SteamUtility.SteamPersonaName : name.First, name.Nick ?? name.First, name.Last);
        }
        return false;
    }
}
