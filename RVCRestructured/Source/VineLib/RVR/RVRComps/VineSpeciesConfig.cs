namespace RVCRestructured;

public class VineSpeciesConfig : CompProperties
{
    public bool linkHumanRecipes = true;
    public bool disableGeneFurRendering = false;
    public bool overrideNamerPatch = true;
    public VineSpeciesConfig() { this.compClass = typeof(VineSpeciesConfigComp); }

    public override void ResolveReferences(ThingDef parentDef)
    {
        ThingDef human = ThingDefOf.Human;
        bool containsHumanRecipes = human.recipes.All(parentDef.recipes.Contains);
        if (!linkHumanRecipes || containsHumanRecipes)
        {
            base.ResolveReferences(parentDef);
            return;
        }
        foreach (RecipeDef recipeDef in human.recipes)
        {
            recipeDef.recipeUsers?.Add(parentDef);
            parentDef.recipes.Add(recipeDef);
        }

        foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefs.Where(x => x.recipeUsers?.Contains(human) ?? false))
        {
            if(!recipe.recipeUsers.Contains(parentDef))
                recipe.recipeUsers.Add(parentDef);
        }
        base.ResolveReferences(parentDef);
    }
}

public class VineSpeciesConfigComp : ThingComp
{
    public VineSpeciesConfig Props => (VineSpeciesConfig)props;
}
