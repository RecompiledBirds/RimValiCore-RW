using System.Collections.Generic;
using Verse;

namespace RVCRestructured.RVR
{



    public class RaceDef : ThingDef
    {
        public RaceGraphics raceGraphics = new RaceGraphics();
        public RaceRestrictions restrictions = new RaceRestrictions();
        public ThoughtReplacer thoughtReplacer = new ThoughtReplacer();

        public ThoughtReplacer ThoughtReplacer
        {
            get
            {
                return thoughtReplacer;
            }
        }
        public RaceRestrictions RaceRestrictions
        {
            get
            {
                return restrictions;
            }
        }

        public RaceGraphics RaceGraphics
        {
            get
            {
                return raceGraphics;
            }
        }

        public override IEnumerable<string> ConfigErrors()
        {
            RVCLog.Log($"Restrictions for {defName} are null.", RVCLogType.Error, restrictions == null);
            RVCLog.Log($"RaceGraphics for {defName} are null.", RVCLogType.Error, raceGraphics == null);
            return base.ConfigErrors();
        }

        public override void PostLoad()
        {
            this.comps.Add(new RVRCP());
            base.PostLoad();
        }


    }
}