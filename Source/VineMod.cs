using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured
{
    public class VineMod : Mod
    {
        public static VineSettings VineSettings
        {
            get
            {
                return settings;
            }
        }
        private static VineSettings settings;
        private ModContentPack modContentPack;
        public VineMod(ModContentPack content) : base(content)
        {
            modContentPack= content;
            settings = GetSettings<VineSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("Enable VGUI editor: ",ref settings.VGUIEnabled);
            listing_Standard.CheckboxLabeled("Race blending enabled: ", ref settings.RaceBlender);
            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Vine Settings";
        }
    }
}
