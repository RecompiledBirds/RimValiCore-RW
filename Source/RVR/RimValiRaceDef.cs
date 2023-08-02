using RimWorld;
using RVCRestructured.Source.RVR;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RVCRestructured.RVR
{



    public class RaceDef : ThingDef
    {
        public RaceGraphics raceGraphics = new RaceGraphics();
        public RaceRestrictions restrictions = new RaceRestrictions();
        public ThoughtReplacer thoughtReplacer = new ThoughtReplacer();
        public CannibalismThoughtsGetter cannibalismThoughts = new CannibalismThoughtsGetter();
        public OrganHarvestThoughtGetter organHarvestThoughts = new OrganHarvestThoughtGetter();
        public ButcherThoughtGetter butcherThoughtGetter = new ButcherThoughtGetter();
        public BodyTypeGetter bodyTypeGetter = null;
        public bool useHumanRecipes = true;
        public bool hasUniqueHeadApparel = false;
        public bool throwApparelError = false;
        public bool useEmptyApparelIfNoDefault = true;

        public BodyTypeGetter BodyTypeGetter
        {
            get
            {
                return bodyTypeGetter;
            }
        }
        public ButcherThoughtGetter ButcherThoughtGetter
        {
            get
            {
                return butcherThoughtGetter;
            }
        }
        public OrganHarvestThoughtGetter OrganHarvestThoughtGetter
        {
            get
            {
                return organHarvestThoughts;
            }
        }

        public CannibalismThoughtsGetter CannibalismThoughtsGetter
        {
            get
            {
                return cannibalismThoughts;
            }
        }

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
            
            bodyTypeGetter = new BodyTypeGetter(this);
            
            base.PostLoad();
        }
        public override void ResolveReferences()
        {
            if (useHumanRecipes)
            {
                foreach (RecipeDef def in ThingDefOf.Human.recipes)
                {
                    if (!this.recipes.Contains(def))
                    {
                        this.recipes.Add(def);
                    }
                }

                IEnumerable<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefs.Where(x => x.recipeUsers.Contains(ThingDefOf.Human));
                foreach (RecipeDef def in recipes)
                {
                    def.recipeUsers.Add(this);
                    def.ResolveReferences();
                }
            }
            base.ResolveReferences();
        }


    }
}