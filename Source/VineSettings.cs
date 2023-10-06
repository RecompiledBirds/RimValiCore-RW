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
        public bool VGUIEnabled=false;

        public VineSettings() { }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref VGUIEnabled, nameof(VGUIEnabled));
            base.ExposeData();
        }


    }
}
