using UnityEngine;
using Verse;

namespace RVCRestructured;

public class VineSettings : ModSettings
{
    public bool VGUIEnabled = false;
    public bool RaceBlender = true;

    public static List<Color> savedColors = [Color.black];
    public static bool debugMode = false;
    public static bool harmonyDebuggers = false;

    public static bool DebugHarmony => debugMode && harmonyDebuggers;
    public VineSettings() { }
    public override void ExposeData()
    {
        Scribe_Collections.Look(ref savedColors, "savedColors");

        Scribe_Values.Look(ref VGUIEnabled, nameof(VGUIEnabled));
        Scribe_Values.Look(ref RaceBlender, nameof(RaceBlender));
        Scribe_Values.Look(ref debugMode, nameof(debugMode));
        Scribe_Values.Look(ref harmonyDebuggers, nameof(harmonyDebuggers));
        base.ExposeData();
    }


}
