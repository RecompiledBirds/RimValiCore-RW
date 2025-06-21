using RimWorld;
using RVCRestructured;
using System.Reflection;
using UnityEngine;
using Verse;
using Color = UnityEngine.Color;

namespace RimValiCore_RW.Source;

[StaticConstructorOnStartup]
public static class FloorConstructor
{

    private static readonly List<DesignationCategoryDef> toUpdateDesignationCatDefs = [];
    private static readonly List<DesignatorDropdownGroupDef> toUpdateDropdownDesDefs = [];
    private static readonly List<string> materials = [];
    private static readonly HashSet<TerrainDef> floorsMade = [];
    private static bool canGenerate = true;
    private static int renderPres = 180;
    static FloorConstructor()
    {
        GenFloors();
    }
    public static void GenFloors()
    {
        List<TerrainDef> workOn = [.. DefDatabase<TerrainDef>.AllDefs];
        //Tells us to clone a terrain
        foreach (TerrainDef def in workOn)
        {
            bool hasDoneTask = false;
            if (def.tags.NullOrEmpty())
            {
                continue;
            }
            if (def.tags.Any(str => str.Contains("cloneMaterial")))
            {
                List<string> tags = def.tags.Where(x => x.Contains("cloneMaterial") && !x.NullOrEmpty()).ToList();
                foreach (string s in tags)
                {
                    //Gets the category name between cloneMaterial_ and [ENDCATNAME]
                    string cS = string.Copy(s);
                    int startIndex = cS.IndexOf("cloneMaterial_") + "cloneMaterial_".Length;
                    int endIndex = cS.IndexOf("[ENDCATNAME]");
                    int length = endIndex - startIndex;
                    string res = cS.Substring(startIndex, length);
                    GenAllVars(def, res);
                }
            }

            if (def.tags.Any(str => str.Contains("removeFromResearch")))
            {
                List<string> tags = def.tags.Where(x => x.Contains("removeFromResearch_") && !x.NullOrEmpty()).ToList();
                for (int a = 0; a < tags.Count; a++)
                {
                    string s = tags[a];
                    hasDoneTask = true;
                    //Gets the category name between cloneMaterial_ and [ENDCATNAME]
                    string cS = string.Copy(s);
                    int startIndex = cS.IndexOf("removeFromResearch_") + "removeFromResearch_".Length;
                    int endIndex = cS.IndexOf("[ENDRESNAME]");
                    int length = endIndex - startIndex;
                    string res = cS.Substring(startIndex, length);
                    ResearchProjectDef proj = def.researchPrerequisites.Find(x => x.defName == res);
                    def.researchPrerequisites.Remove(proj);
                    proj.PostLoad();
                    proj.ResolveReferences();
                }
            }
            if (hasDoneTask)
            {
                def.PostLoad();
                def.ResolveReferences();
            }
        }
        //Ensures we are adding to the DefDatabase. Just a saftey check.
        foreach (TerrainDef def in floorsMade)
        {
            if (DefDatabase<TerrainDef>.AllDefs.Select(terrainDef => terrainDef.defName).Contains(def.defName))
            {
                continue;
            }
            def.PostLoad();
            DefDatabase<TerrainDef>.Add(def);
        }

        

        //Updates/refreshes menus
        foreach (DesignationCategoryDef def in toUpdateDesignationCatDefs)
        {
            def.PostLoad();
            def.ResolveReferences();
        }

        foreach (DesignatorDropdownGroupDef def in toUpdateDropdownDesDefs)
        {
            def.PostLoad();
            def.ResolveReferences();
        }
        //   Log.Message(builder.ToString());
        VineLog.Log($"Updated {toUpdateDesignationCatDefs.Count} designation categories & {toUpdateDropdownDesDefs.Count} dropdown designations.");
        VineLog.Log($"Built  {floorsMade.Count} floors from {materials.Count} materials.");
        //We need to do this or RW has a fit
        WealthWatcher.ResetStaticData();
        
    }

    public static void GenAllVars(TerrainDef def, string name)
    {
        IEnumerable<ThingDef> floorMats = DefDatabase<ThingDef>.AllDefs.Where(x => x.stuffProps?.categories?.Any(cat => cat.defName == name) ?? false);

        foreach(ThingDef thingDef in floorMats)
        {
            if (!materials.Contains(thingDef.defName))
                materials.Add(thingDef.defName);
            ushort id = (ushort)$"{def.defName}_{thingDef.defName}".GetHashCode();
            if (!DefDatabase<TerrainDef>.AllDefs.Any(t => t.defName == $"{t.defName}_{t.defName}"))
            {
                id = GetId(def, thingDef);
            }

            TerrainDef output = GetOutputTerrain(def, thingDef, id);

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            //This copies some of the varibles from the floor we are duplicating over
            //We don't want it to touch the fields we've already set, so I keep a list here to help.

            List<string> avoidFields = ["color", "defname", "label", "debugrandomid", "index", "shorthash", "costlist", "uiiconcolor", "designatordropdown"];
            foreach (FieldInfo field in def.GetType().GetFields(bindingFlags).Where(f => !avoidFields.Contains(f.Name.ToLower())))
            {
                output.GetType().GetField(field.Name,bindingFlags).SetValue(output, field.GetValue(def));
            }

            List<string> toRemove = [];
            foreach (string str in output.tags)
            {
                //This looks for a DesignationCategoryDef with a defname that matches the string between AddDesCat_ and [ENDDESNAME]
                AddDesCat(output, str);
                //This looks for a DesignationCategoryDef with a defname that matches the string between AddDesDropDown_ and [ENDDNAME]
                AddDesDropdown(output, str);
                //This removes the tag from clones.
                if (str.EndsWith("RemoveFromClones") || str.EndsWith("_RFC"))
                {
                    toRemove.Add(str);
                }
            }
            foreach (string str in toRemove)
            {
                output.tags.Remove(str);
            }
            //How vanilla RW sets up some stuff

            //Blueprint
            ThingDef blueprintDef = CreateBluePrint(output);
            

            //Framedef
            ThingDef frameDef = GenFrameDef(output);
            
            //This makes sure everything is setup how it should be
            output.PostLoad();
            output.ResolveReferences();

            floorsMade.Add(output);
        }





    }

    private static void AddDesDropdown(TerrainDef output, string str)
    {
        if (str.Contains("AddDesDropDown_"))
        {
            string cS = string.Copy(str);
            string res = cS.Substring(cS.IndexOf("AddDesDropDown_") + "AddDesDropDown_".Length, (cS.IndexOf("[ENDDNAME]") - ("[ENDDNAME]".Length + 5)) - cS.IndexOf("AddDesDropDown_"));
            if (DefDatabase<DesignatorDropdownGroupDef>.AllDefs.Any(cat => cat.defName == res))
            {
                DesignatorDropdownGroupDef designator = DefDatabase<DesignatorDropdownGroupDef>.AllDefs.Where(cat => cat.defName == res).First();
                if (designator!=null)
                {
                    toUpdateDropdownDesDefs.Add(designator);
                }
                output.designatorDropdown = designator;
            }
        }
    }

    private static void AddDesCat(TerrainDef output, string str)
    {
        if (str.Contains("AddDesCat_"))
        {
            string cS = string.Copy(str);
            string res = cS.Substring(cS.IndexOf("AddDesCat_") + "AddDesCat_".Length, (cS.IndexOf("[ENDDESNAME]") - ("[ENDDESNAME]".Length - 2)) - cS.IndexOf("AddDesCat_"));
            if (DefDatabase<DesignationCategoryDef>.AllDefs.Any(cat => cat.defName == res))
            {
                DesignationCategoryDef designation = DefDatabase<DesignationCategoryDef>.AllDefs.Where(cat => cat.defName == res).First();
                if (designation!=null)
                {
                    toUpdateDesignationCatDefs.Add(designation);
                }
                output.designationCategory = designation;
            }
        }
    }

    private static ThingDef GenFrameDef(TerrainDef output)
    {
        ThingDef frameDef = new()
        {
            isFrameInt = true,
            thingClass = typeof(Frame),
            altitudeLayer = AltitudeLayer.Building,
            selectable = true,
            building = new BuildingProperties()
            {
                artificialForMeditationPurposes = false,
                isEdifice=false
            },
            comps =
                     {
                         new CompProperties_Forbiddable()
                     },
            scatterableOnMapGen = false,
            leaveResourcesWhenKilled = true,


            defName = ThingDefGenerator_Buildings.BuildingFrameDefNamePrefix + output.defName,
            label = output.label + "FrameLabelExtra".Translate(),
            useHitPoints = false,
            fillPercent = 0f,
            description = "Terrain building in progress.",
            passability = Traversability.Standable,
            constructEffect = output.constructEffect,
            constructionSkillPrerequisite = output.constructionSkillPrerequisite,
            artisticSkillPrerequisite = output.artisticSkillPrerequisite,
            clearBuildingArea = false,
            modContentPack = output.modContentPack,
            category = ThingCategory.Ethereal,
            entityDefToBuild = output,
            ignoreIllegalLabelCharacterConfigError = true,
        };
        output.frameDef = frameDef;
        return frameDef;
    }

    private static ThingDef CreateBluePrint(TerrainDef output)
    {
        ThingDef blueprintDef = new()
        {
            category = ThingCategory.Ethereal,
            altitudeLayer = AltitudeLayer.Blueprint,
            useHitPoints = false,
            selectable = true,
            seeThroughFog = true,
            comps =
                    {
                        new CompProperties_Forbiddable()
                     },
            drawerType = DrawerType.MapMeshAndRealTime,
            ignoreIllegalLabelCharacterConfigError = true,
            thingClass = typeof(Blueprint_Build),
            defName = ThingDefGenerator_Buildings.BlueprintDefNamePrefix + output.defName,


            label = output.label + "BlueprintLabelExtra".Translate(),
            entityDefToBuild = output,
            graphicData = new GraphicData
            {
                shaderType = ShaderTypeDefOf.MetaOverlay,
                texPath = "Things/Special/TerrainBlueprint",
                graphicClass = typeof(Graphic_Single)
            },
            constructionSkillPrerequisite = output.constructionSkillPrerequisite,
            artisticSkillPrerequisite = output.artisticSkillPrerequisite,
            clearBuildingArea = false,
            modContentPack = output.modContentPack
        };
        output.blueprintDef = blueprintDef;
        
        return blueprintDef;
    }

    private static TerrainDef GetOutputTerrain(TerrainDef def, ThingDef thingDef, ushort id)
    {
        TerrainDef output = new()
        {
            color = def.GetColorForStuff(thingDef),
            defName = $"{def.defName}_{thingDef.defName}",
            label = string.Format(def.label, thingDef.label),
            debugRandomId = id,
            index = id,
            shortHash = id,
            costList = ((Func<List<ThingDefCountClass>>)delegate
                {
                    List<ThingDefCountClass> costList = [];
                    int amount = 0;
                    foreach (ThingDefCountClass thingDefCountClass in def.costList)
                    {
                        amount += thingDefCountClass.count;
                    }
                    costList.Add(new ThingDefCountClass()
                    {
                        thingDef = thingDef,
                        count = amount
                    });
                    return costList;
                })(),
            renderPrecedence = renderPres++,
            designationCategory = def.designationCategory,
            designatorDropdown = def.designatorDropdown,
            ignoreIllegalLabelCharacterConfigError = def.ignoreIllegalLabelCharacterConfigError,
            pollutionColor = new Color(1f, 1f, 1f, 0.8f),
            pollutionOverlayScale = new Vector2(0.75f, 0.75f),
            pollutionOverlayTexturePath = "Terrain/Surfaces/PollutionFloorSmooth"
        };
        return output;
    }

    private static ushort GetId(TerrainDef def, ThingDef tDef)
    {
        bool hasmaxedout = false;
        bool hasminedout = false;
        ushort uS = (ushort)$"{def.defName}_{tDef.defName}".GetHashCode();
        while (DefDatabase<TerrainDef>.AllDefs.Any(terrain => terrain.shortHash == uS) || floorsMade.Any(t => t.shortHash == uS) && canGenerate)
        {
            if (uS < ushort.MaxValue && !hasmaxedout)
            {
                uS += 1;
            }
            else if (uS == ushort.MaxValue)
            {
                hasmaxedout = true;
            }
            if (uS > ushort.MinValue && hasmaxedout && !hasminedout)
            {
                uS -= 1;
            }
            else if (uS == ushort.MinValue && hasmaxedout)
            {
                hasminedout = true;
            }
            if (hasminedout && hasmaxedout)
            {
                //If you ever see this i'll be impressed
                Log.Warning($"[RimVali Core/FloorConstructor] Could not generate tile {string.Format(def.label, tDef.label)}'s unique short hash, aborting..");
                canGenerate = false;
                return ushort.MinValue;
            }
        }
        return uS;
    }
}
