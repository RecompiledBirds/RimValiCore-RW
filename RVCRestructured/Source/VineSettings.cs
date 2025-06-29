using UnityEngine;
using Verse;

namespace RVCRestructured;

public class VineSettings : ModSettings
{
    public bool VGUIEnabled = false;

    public static List<Color> savedColors = [Color.black];
    public static bool debugMode = false;
    public static bool harmonyDebuggers = false;
    public static bool factionBlender = true;
    public static bool overrideBlendDefaultRatio;
    public static float blendRatio = 0.3f;
    public static bool flushGenerationCaches = false;
    public static int flushCachesAfterHowManyPawnsGenerated = 10;
    public static bool DebugHarmony => debugMode && harmonyDebuggers;
    public VineSettings() { }
    public override void ExposeData()
    {
        Scribe_Collections.Look(ref savedColors, "savedColors");

        Scribe_Values.Look(ref VGUIEnabled, nameof(VGUIEnabled));
        Scribe_Values.Look(ref debugMode, nameof(debugMode));
        Scribe_Values.Look(ref harmonyDebuggers, nameof(harmonyDebuggers));
        Scribe_Values.Look(ref factionBlender, nameof(factionBlender));
        Scribe_Values.Look(ref overrideBlendDefaultRatio, nameof(overrideBlendDefaultRatio));
        Scribe_Values.Look(ref blendRatio, nameof(blendRatio));
        Scribe_Values.Look(ref flushGenerationCaches, nameof(flushGenerationCaches));
        Scribe_Values.Look(ref flushCachesAfterHowManyPawnsGenerated, nameof(flushCachesAfterHowManyPawnsGenerated));
        base.ExposeData();
    }


}
