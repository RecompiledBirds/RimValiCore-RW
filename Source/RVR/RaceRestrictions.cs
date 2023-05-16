using RimWorld;
using RVCRestructured.RVR.HarmonyPatches;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RVCRestructured.RVR
{
    public class RaceRestrictions
    {
        //Defs restricted to this race
        public List<ResearchProjectDef> restrictedResearchDefs = new List<ResearchProjectDef>();

        public List<ThingDef> restrictedFoodDefs = new List<ThingDef>();

        public List<ThingDef> restrictedBuildings = new List<ThingDef>();

        public List<ThoughtDef> restrictedThoughtDefs = new List<ThoughtDef>();

        public List<BodyTypeDef> restrictedBodyTypes = new List<BodyTypeDef>();

        public List<ThingDef> restrictedBeds = new List<ThingDef>();

        public List<ThingDef> restrictedEquipment = new List<ThingDef>();

        public List<ThingDef> restrictedApparel = new List<ThingDef>();

        public List<TraitDef> restrictedTraits = new List<TraitDef>();


        //Disabled items
        public List<ThoughtDef> disabledThoughts = new List<ThoughtDef>();
        public List<TraitDef> disabledTraits = new List<TraitDef>();

        //Allow equipment that would otherwise be disabled
        public List<ThingDef> allowedEquipment = new List<ThingDef>();

        public List<ThingDef> allowedFoodDefs = new List<ThingDef>();

        public List<ThingDef> allowedBuildings = new List<ThingDef>();

        public List<ThoughtDef> allowThoughtDefs = new List<ThoughtDef>();

        public List<ThingDef> allowedApparel = new List<ThingDef>();

        public List<TraitDef> allowedTraits = new List<TraitDef>();

        public List<BodyTypeDef> allowedBodyTypes = new List<BodyTypeDef>();   


        //Mod items
        //Restricted items
        public List<string> modRestrictedFoodDefs = new List<string>();

        public List<string> modRestrictedEquipment = new List<string>();

        public List<string> modRestrictedThoughts = new List<string>();

        public List<string> modRestrictedBodyTypes = new List<string>();

        public List<string> modRestrictedBuildings = new List<string>();

        public List<string> modRestrictedResearch = new List<string>();

        public List<string> modRestrictedApparel = new List<string>();


        //Enabled items
        public List<string> modAllowedFoodDefs = new List<string>();

        public List<string> modAllowedEquipment = new List<string>();

        public List<string> modAllowedThoughts = new List<string>();

        public List<string> modAllowedBodyTypes = new List<string>();

        public List<string> modAllowedBuildings = new List<string>();

        public List<string> modAllowedResearch = new List<string>();

        public List<string> modAllowedApparel = new List<string>();


        public bool useHumanRecipes = true;

        public bool canEatAnyFood = true;

        public bool canUseAnyApparel = true;

        /// <summary>
        /// Helper function that sets a modList's data based on what it can find from that mod
        /// </summary>
        /// <param name="modNames"></param>
        /// <param name="modList"></param>
        /// <param name="extraParams"></param>
        /// <typeparam name="T"></typeparam>
        private void AddContentsToList<T>(List<string> modNames, ref List<T> modList, System.Func<T, bool> extraParams) where T : Def
        {
            foreach (string mod in modNames)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                if (pack == null) continue;
                //Extra params
                modList.AddRange(DefDatabase<T>.AllDefsListForReading.Where(x => x.modContentPack == pack && extraParams(x)));
            }
        }

        /// <summary>
        /// Helper function that does what <see cref="AddContentsToList"></see> does, and adds it to the restriction checker.
        /// </summary>
        /// <param name="modNames"></param>
        /// <param name="modList"></param>
        /// <param name="extraParams"></param>
        /// <typeparam name="T"></typeparam>
        private void AddAndRestrict<T>(List<string> modNames, ref List<T> modList, System.Func<T, bool> extraParams) where T : Def
        {
            AddContentsToList(modNames, ref modList, extraParams);
            RestrictionsChecker.AddRestrictions(modList);
        }

        /// <summary>
        /// Do some tasks on load, such as getting the modContent lists
        /// </summary>
        public void OnLoad()
        {
            #region ---Restricting Content---

            AddAndRestrict(modRestrictedFoodDefs, ref restrictedFoodDefs, x => x.IsNutritionGivingIngestible);
            AddAndRestrict(modRestrictedEquipment, ref restrictedEquipment, x => x.IsWeapon);
            AddAndRestrict(modRestrictedThoughts, ref restrictedThoughtDefs, x => true);
            AddAndRestrict(modRestrictedBodyTypes, ref restrictedBodyTypes, x => true);
            AddAndRestrict(modRestrictedBuildings, ref restrictedBuildings, x => x.building != null && x.BuildableByPlayer && x.blueprintDef != null);
            AddAndRestrict(modRestrictedResearch, ref restrictedResearchDefs, x => true);
            AddAndRestrict(modRestrictedApparel, ref restrictedApparel, x => x.IsApparel);

            #endregion
            #region ---Allowing Content---

            AddContentsToList(modAllowedFoodDefs, ref allowedFoodDefs, x => x.IsNutritionGivingIngestible);
            AddContentsToList(modAllowedEquipment, ref allowedEquipment, x => x.IsWeapon);
            //AddContentsToList(modAllowedThoughts, ref allowedThoughtDefs, x => true);
            AddContentsToList(modAllowedBodyTypes, ref allowedBodyTypes, x => true);
            AddContentsToList(modAllowedBuildings, ref allowedBuildings, x => true);
            //AddContentsToList(modAllowedResearch, ref allowedResearch, x => true);
            AddContentsToList(modAllowedApparel, ref allowedApparel, x => true);

            #endregion



        }
    }
}
