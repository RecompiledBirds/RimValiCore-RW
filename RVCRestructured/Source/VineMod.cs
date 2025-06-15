using UnityEngine;
using Verse;

namespace RVCRestructured;

public class VineMod : Mod
{
    public static VineSettings VineSettings => settings ?? throw new NullReferenceException();
    private readonly string dir;

    public string ModDir => dir;
    private static VineSettings? settings;
    private readonly ModContentPack modContentPack;

    public VineMod(ModContentPack content) : base(content)
    {
        modContentPack= content;
        settings = GetSettings<VineSettings>();
        dir = modContentPack.RootDir;
    }

    //Todo: Translation Strings
    public override void DoSettingsWindowContents(Rect inRect)
    {
        if (settings == null) return;

        Listing_Standard listing_Standard = new();
        listing_Standard.Begin(inRect);
        listing_Standard.CheckboxLabeled("Enable VGUI editor: ", ref settings.VGUIEnabled);
        listing_Standard.CheckboxLabeled("Race blending enabled: ", ref settings.RaceBlender);
        listing_Standard.CheckboxLabeled("Debug mode: ", ref VineSettings.debugMode);
        if (VineSettings.debugMode)
        {
            listing_Standard.CheckboxLabeled("Harmony debuggers: ", ref VineSettings.harmonyDebuggers);
        }
        listing_Standard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Vine Settings";
    }
}
