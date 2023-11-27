using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public class RVRRecipeGetterComp : CompProperties
    {
        private bool resolved = false;
        
        public override void ResolveReferences(ThingDef parentDef)
        {
            if(resolved) return;
            resolved = true;
            foreach (RecipeDef def in ThingDefOf.Human.recipes)
            {
                if (!parentDef.recipes.Contains(def))
                {
                    parentDef.recipes.Add(def);
                }
            }
            parentDef.ResolveReferences();
        }
        
        public RVRRecipeGetterComp()
        {
            this.compClass=typeof(RVRRecipeSetterComp);
        }
    }

    public class RVRRecipeSetterComp : ThingComp{
        
    }
}
