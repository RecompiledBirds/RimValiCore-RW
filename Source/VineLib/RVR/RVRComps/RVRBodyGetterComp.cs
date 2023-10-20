using Mono.Unix.Native;
using RimWorld;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public class RVRBodyGetterComp : CompProperties
    {
        public RVRBodyGetterComp() { 
            this.compClass = typeof(RVRBodyGetter);
        }
        public List<BodyTypeDef> allowedBodyTypes = new List<BodyTypeDef>();


    }

    public class RVRBodyGetter : ThingComp
    {
        public RVRBodyGetterComp Props
        {
            get
            {
                return props as RVRBodyGetterComp;
            }
        }

        public void GenBody()
        {
            if (!(parent is Pawn pawn)) return;
            if (Props.allowedBodyTypes.Contains(pawn.story.bodyType)) return;
            pawn.story.bodyType = Props.allowedBodyTypes.RandomElement();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            GenBody();
        }
    }
}
