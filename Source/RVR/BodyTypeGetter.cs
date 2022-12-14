using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.RVR.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Source.RVR
{
    public class BodyTypeGetter
    {
        private List<BodyTypeDef> cache = new List<BodyTypeDef>();
        private RaceDef parent;
        public BodyTypeGetter(RaceDef def)
        {
            parent = def;
        }

        public IEnumerable<BodyTypeDef> GetBodyTypes()
        {
            if (!cache.NullOrEmpty())
                return cache;
            
            bool isOnWhitelist = !parent.RaceRestrictions.allowedBodyTypes.NullOrEmpty();
            List<BodyTypeDef> defs = DefDatabase<BodyTypeDef>.AllDefsListForReading;

            foreach(BodyTypeDef def in defs)
            {
                bool restricted = RestrictionsChecker.IsRestricted(def);
                bool allowedByRestriction = parent.RaceRestrictions.restrictedBodyTypes.Contains(def);
                bool allowedByWhitelist = parent.RaceRestrictions.allowedBodyTypes.Contains(def);

                bool allowed= restricted ? allowedByRestriction || allowedByWhitelist : !isOnWhitelist || allowedByWhitelist;

                if(allowed)
                    cache.Add(def);
            }
            return cache;
        }


        public void SetBodyType(ref Pawn pawn)
        {
            pawn.story.bodyType = GetBodyTypes().RandomElement();
        }
    }
}
