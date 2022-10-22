using RimWorld;
using RVCRestructured.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR
{



    public class RaceDef : ThingDef
    {
        public RaceGraphics raceGraphics;
        public RaceRestrictions restrictions;

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

        public override void PostLoad()
        {
            //If useHumanRecipes, then pull all recipes from humans
            if (restrictions.useHumanRecipes)
            {
                foreach (RecipeDef recipeDef in Enumerable.Where(DefDatabase<RecipeDef>.AllDefsListForReading, (RecipeDef x) => x.recipeUsers != null && x.recipeUsers.Contains(ThingDefOf.Human)))
                {
                    if(!recipeDef.recipeUsers.Contains(this))
                        recipeDef.recipeUsers.Add(this);
                }
                if (recipes == null)
                {
                    recipes = new List<RecipeDef>();
                }
                List<BodyPartDef> list = new List<BodyPartDef>();
                foreach (BodyPartRecord bodyPartRecord in race.body.AllParts)
                {
                    list.Add(bodyPartRecord.def);
                }
                foreach (RecipeDef recipeDef2 in Enumerable.Where(ThingDefOf.Human.recipes, (RecipeDef recipe) => recipe.targetsBodyPart || !recipe.appliedOnFixedBodyParts.NullOrEmpty()))
                {
                    foreach (BodyPartDef bodyPartDef in Enumerable.Intersect(recipeDef2.appliedOnFixedBodyParts, list))
                    {
                        if(!recipes.Contains(recipeDef2))
                            recipes.Add(recipeDef2);
                    }
                }
            }
            this.comps.Add(new RVRCP());
            base.PostLoad();
        }

        
    }
}