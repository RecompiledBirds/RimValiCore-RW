using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public class VineSettings : ModSettings
    {
        public bool VGUIEnabled = false;
        public bool RaceBlender = true;

        public static List<Color> savedColors = new List<Color>() { Color.black };
        public static bool debugMode = false;


        public VineSettings() { }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref VGUIEnabled, nameof(VGUIEnabled));
            Scribe_Values.Look(ref RaceBlender, nameof(RaceBlender));
            Scribe_Values.Look(ref debugMode, nameof(debugMode));
            base.ExposeData();
        }


    }
}
