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
        listing_Standard.CheckboxLabeled("Vine_EnableVGUI".Translate(), ref settings.VGUIEnabled);
        listing_Standard.CheckboxLabeled("Vine_EnableFactionBlending".Translate(), ref VineSettings.factionBlender);
        if (VineSettings.factionBlender)
        {
            listing_Standard.CheckboxLabeled($"Vine_OverrideDefaultBlendingRatio".Translate(FactionData.defaultRatio.ToString().Named("DEFAULT")), ref VineSettings.overrideBlendDefaultRatio);
            if (VineSettings.overrideBlendDefaultRatio)
            {
                VineSettings.blendRatio = Mathf.Round(listing_Standard.SliderLabeled("Vine_BlendingUserSetRatio".Translate(VineSettings.blendRatio.ToString().Named("RATIO")),VineSettings.blendRatio*100, 1, 100))/100;
            }
            listing_Standard.CheckboxLabeled("Vine_FlushCachesAfterXPawnsGenerated".Translate(VineSettings.flushCachesAfterHowManyPawnsGenerated.Named("X")), ref VineSettings.flushGenerationCaches, "Vine_FlushCachesToolTip".Translate());
            if (VineSettings.flushGenerationCaches)
            {
                VineSettings.flushCachesAfterHowManyPawnsGenerated = (int)listing_Standard.Slider(VineSettings.flushCachesAfterHowManyPawnsGenerated, 1, 100);
            }
        }
        listing_Standard.CheckboxLabeled("Vine_DebugMode".Translate(), ref VineSettings.debugMode);
        if (VineSettings.debugMode)
        {
            listing_Standard.CheckboxLabeled("Vine_HarmonyDebugger".Translate(), ref VineSettings.harmonyDebuggers);
        }
        listing_Standard.End();
        base.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Vine Settings";
    }
}
