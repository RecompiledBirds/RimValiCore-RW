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
    public class RVRRestrictionComp : CompProperties
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

        public List<XenotypeDef> restrictedXenoTypes = new List<XenotypeDef>();

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

        public List<XenotypeDef> xenoTypeWhitelist = new List<XenotypeDef>();

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


        public bool canEatAnyFood = true;

        public bool canUseAnyApparel = true;

        /// <summary>
        /// Do some tasks on load, such as getting the modContent lists
        /// </summary>
        public void OnLoad()
        {
            #region ---Restricting Content---

            //Do food restrictions
            foreach (string mod in modRestrictedFoodDefs)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be food
                restrictedFoodDefs.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.IsNutritionGivingIngestible));

            }

            //Do equipment restrictions
            foreach (string mod in modRestrictedEquipment)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be a weapon
                restrictedEquipment.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.IsWeapon));
            }

            //Do thought restrictions
            foreach (string mod in modRestrictedThoughts)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Get all thoughts from the mod
                restrictedThoughtDefs.AddRange(DefDatabase<ThoughtDef>.AllDefsListForReading.Where(x => x.modContentPack == pack));
            }

            //Do BodyType restrictions
            foreach (string mod in modRestrictedBodyTypes)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Get all bodytypes from the mod
                restrictedBodyTypes.AddRange(DefDatabase<BodyTypeDef>.AllDefsListForReading.Where(x => x.modContentPack == pack));
            }

            //Do building restrictions
            foreach (string mod in modRestrictedBuildings)
            {

                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be a building
                restrictedBuildings.AddRange(DefDatabase<ThingDef>.AllDefs.Where(x => x.modContentPack == pack && x.building != null && x.BuildableByPlayer && x.blueprintDef != null));
            }

            //Do research restrictions
            foreach (string mod in modRestrictedResearch)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Get all researchprojectdefs from the mod
                restrictedResearchDefs.AddRange(DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where(x => x.modContentPack == pack));
            }

            //Do apparel restrictions
            foreach (string mod in modRestrictedApparel)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be apparel
                restrictedApparel.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.IsApparel));
            }
            RestrictionsChecker.AddRestrictions(restrictedXenoTypes);
            RestrictionsChecker.AddRestrictions(restrictedBuildings);
            RestrictionsChecker.AddRestrictions(restrictedResearchDefs);
            RestrictionsChecker.AddRestrictions(restrictedApparel);
            RestrictionsChecker.AddRestrictions(restrictedBodyTypes);
            RestrictionsChecker.AddRestrictions(restrictedThoughtDefs);
            RestrictionsChecker.AddRestrictions(restrictedEquipment);
            RestrictionsChecker.AddRestrictions(restrictedFoodDefs);
            RestrictionsChecker.AddRestrictions(restrictedTraits);
            #endregion
            #region ---Allowing Content---

            foreach (string mod in modAllowedBodyTypes)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be food
                allowedBodyTypes.AddRange(DefDatabase<BodyTypeDef>.AllDefsListForReading.Where(x => x.modContentPack == pack));
            }


            //Do food allowances
            foreach (string mod in modAllowedFoodDefs)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be food
                allowedFoodDefs.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.IsNutritionGivingIngestible));
            }

            //Do equipment allowances
            foreach (string mod in modAllowedEquipment)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be a weapon
                allowedEquipment.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.IsWeapon));
            }

            //Do thought allowances
            foreach (string mod in modAllowedThoughts)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Get all thoughts from the mod
                allowThoughtDefs.AddRange(DefDatabase<ThoughtDef>.AllDefsListForReading.Where(x => x.modContentPack == pack));
            }


            //Do building allowances
            foreach (string mod in modAllowedBuildings)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be a building
                allowedBuildings.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.building != null));
            }



            //Do apparel allowances
            foreach (string mod in modAllowedApparel)
            {
                //Try to find the mod.
                ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
                //If we can't find it, skip
                if (pack == null) continue;
                //Add everything considered to be apparel
                allowedApparel.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.modContentPack == pack && x.IsApparel));
            }
            #endregion



        }

        public RVRRestrictionComp()
        {
            this.compClass=typeof(RestrictionComp);
        }
    }

    public class RestrictionComp : ThingComp
    {
        public RVRRestrictionComp Props
        {
            get
            {
                return props as RVRRestrictionComp;
            }
        }
    }
}
