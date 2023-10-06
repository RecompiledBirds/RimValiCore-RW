using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Plants
{
    public class RVCPlantCompProperties : CompProperties
    {
        public float minPreferredTemp;
        public float maxPreferredTemp;
        public RVCPlantCompProperties()
        {
            this.compClass = typeof(RVCPlantComp);
        }
    }

    public class RVCPlantComp : ThingComp
    {
        public RVCPlantCompProperties Props
        {
            get
            {
                return this.props as RVCPlantCompProperties;
            }
        }
    }
}
