using RimWorld;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RVCRestructured.Source.VineLib.Restrictions
{
    public class DefRestrictionManager
    {
        private readonly Dictionary<Def, DefRestrictionInfo> restrictionInfos = new Dictionary<Def, DefRestrictionInfo>();

        private List<ModRestrictInstructions> modInstructions = new List<ModRestrictInstructions>();
        private List<DefRestrictInstructions> defInstructions = new List<DefRestrictInstructions>();

        private readonly Dictionary<RestrictionType, Func<Def, bool>> restrictionTypeDefValidators = new Dictionary<RestrictionType, Func<Def, bool>>
        {
            { RestrictionType.ResearchDef, def => true },
            { RestrictionType.BodyTypeDef, def => true },
            { RestrictionType.XenotypeDef, def => true },
            { RestrictionType.ThoughtDef, def => true },
            { RestrictionType.Equipment, def => def is ThingDef thingDef && thingDef.IsWeapon },
            { RestrictionType.TraitDef, def => true },
            { RestrictionType.Building, def => def is ThingDef thingDef && thingDef.building != null && thingDef.BuildableByPlayer && thingDef.blueprintDef != null },
            { RestrictionType.FoodDef, def => def is ThingDef thingDef && thingDef.IsNutritionGivingIngestible },
            { RestrictionType.Apparel, def => def is ThingDef thingDef && thingDef.IsApparel },
            { RestrictionType.Beds, def => true }
        };

        private readonly Dictionary<RestrictionType, Type> typeToDefType = new Dictionary<RestrictionType, Type>
        {
            { RestrictionType.ResearchDef, typeof(ResearchProjectDef) },
            { RestrictionType.BodyTypeDef, typeof(BodyTypeDef) },
            { RestrictionType.XenotypeDef, typeof(XenotypeDef) },
            { RestrictionType.ThoughtDef, typeof(ThoughtDef) },
            { RestrictionType.Equipment, typeof(ThingDef) },
            { RestrictionType.TraitDef, typeof(TraitDef) },
            { RestrictionType.Building, typeof(ThingDef) },
            { RestrictionType.FoodDef, typeof(ThingDef) },
            { RestrictionType.Apparel, typeof(ThingDef) },
            { RestrictionType.Beds, typeof(ThingDef) }
        };

        private readonly Dictionary<RestrictionType, HashSet<DefRestrictionInfo>> typeToDefSet = new Dictionary<RestrictionType, HashSet<DefRestrictionInfo>>
        {
            { RestrictionType.ResearchDef, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.BodyTypeDef, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.XenotypeDef, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.ThoughtDef, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.Equipment, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.TraitDef, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.Building, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.FoodDef, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.Apparel, new HashSet<DefRestrictionInfo>() },
            { RestrictionType.Beds, new HashSet<DefRestrictionInfo>() }
        };

        private readonly HashSet<RestrictionType> alwaysAllowedTypes = new HashSet<RestrictionType>();

        public Dictionary<RestrictionType, Type> TypeToDefType => typeToDefType;

        public Dictionary<RestrictionType, HashSet<DefRestrictionInfo>> TypeToDefSet => typeToDefSet;

        public DefRestrictionInfo this[Def def] => restrictionInfos.TryGetValue(def, DefRestrictionInfo.Empty);

        public HashSet<DefRestrictionInfo> this[RestrictionType type] => typeToDefSet[type];

        public bool IsAlwaysAllowed(RestrictionType type) => alwaysAllowedTypes.Contains(type);

        public bool IsAlwaysAllowed(Def def)
        {
            foreach (RestrictionType type in alwaysAllowedTypes)
            {
                if (restrictionTypeDefValidators[type](def)) return true;
            }

            return false;
        }

        public void AddOrOverwriteRestriction(DefRestrictionInfo info) => restrictionInfos[info.Def] = info;

        public bool IsUserWhitelisted(Def def) => restrictionInfos.ContainsKey(def) && restrictionInfos[def].UserWhitelisted; 

        public bool IsUserRestricted(Def def) => restrictionInfos.ContainsKey(def) && restrictionInfos[def].UserBlacklisted;

        public void LoadDataFromXmlCustom(XmlNode root)
        {
            LoadModRestrictions(root.SelectSingleNode("modRestrictions"));
            LoadDefRestrictions(root.SelectSingleNode("defRestrictions"));
            LoadAllwaysAllowedTypes(root.SelectSingleNode("allwaysAllowed"));
        }

        private void LoadAllwaysAllowedTypes(XmlNode root)
        {
            if (!(root?.ChildNodes is XmlNodeList allowedTypes)) return;

            int allowedTypeCount = allowedTypes.Count;
            for (int i = 0; i < allowedTypeCount; i++)
            {
                XmlNode type = allowedTypes[i];
                alwaysAllowedTypes.Add(ParseHelper.FromString<RestrictionType>(type.Name));
            }
        }

        private void LoadDefRestrictions(XmlNode root)
        {
            if (!(root?.ChildNodes is XmlNodeList defRestrictions)) return;

            int modCount = defRestrictions.Count;
            for (int i = 0; i < modCount; i++)
            {
                XmlNode restriction = defRestrictions[i];

                if (restriction.ChildNodes is XmlNodeList singleDefNodes)
                {
                    XmlNode defNode = singleDefNodes[i];

                    RestrictionType restrictionType = ParseHelper.FromString<RestrictionType>(restriction.Name);
                    DefRestrictInstructions instructions = new DefRestrictInstructions
                    {
                        type = restrictionType,
                        how = ParseHelper.FromString<RestrictionHow>(defNode.Value)
                    };

                    DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(instructions, nameof(instructions.def), defNode.Name, overrideFieldType: typeToDefType[restrictionType]);
                    
                    defInstructions.Add(instructions);
                }
            }
        }

        public void LoadModRestrictions(XmlNode root)
        {
            if (!(root?.ChildNodes is XmlNodeList modRestrictions)) return;

            int modCount = modRestrictions.Count;
            for (int i = 0; i < modCount; i++)
            {
                XmlNode mod = modRestrictions[i];
                if (!(LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod.Name || x.PackageId.ToLower() == mod.Name.ToLower()) is ModContentPack pack)) continue;

                XmlNodeList modRestrictionTypes = mod.ChildNodes;
                int restrictionCount = modRestrictionTypes.Count;

                Dictionary<RestrictionType, RestrictionHow> restrictions = new Dictionary<RestrictionType, RestrictionHow>(restrictionCount);
                for (int j = 0; j < restrictionCount; j++)
                {
                    XmlNode restriction = modRestrictionTypes[j];

                    RestrictionType restrictionType = ParseHelper.FromString<RestrictionType>(restriction.Name);
                    RestrictionHow restrictionHow = ParseHelper.FromString<RestrictionHow>(restriction.Value);

                    restrictions[restrictionType] = restrictionHow;
                }

                modInstructions.Add(new ModRestrictInstructions { restrictions = restrictions, pack = pack });
            }
        }

        public void ResolveReferences(ThingDef parentDef)
        {
            ResolveModReferences(parentDef);
            ResolveDefReferences(parentDef);
        }

        private void ResolveDefReferences(ThingDef parentDef)
        {
            int count = defInstructions.Count;
            for (int i = 0; i < count; i++)
            {
                (RestrictionType type, RestrictionHow how, Def def) = defInstructions[i];
                AddRestrictionInfo(parentDef, def, how, out DefRestrictionInfo info);
                typeToDefSet[type].Add(info);
            }

            defInstructions = null; //Empty so that GC can pick it up
        }

        public void ResolveModReferences(ThingDef parentDef)
        {
            int count = modInstructions.Count;
            for (int i = 0; i < count; i++)
            {
                (Dictionary<RestrictionType, RestrictionHow> restrictions, ModContentPack pack) = modInstructions[i];

                int restrictionCount = restrictions.Count;
                foreach (Def def in pack.AllDefs)
                {
                    foreach (RestrictionType type in restrictions.Keys.Where(key => restrictionTypeDefValidators[key](def)))
                    {
                        AddRestrictionInfo(parentDef, def, restrictions[type], out DefRestrictionInfo info);
                        typeToDefSet[type].Add(info);
                    }
                }
            }

            modInstructions = null; //Empty so that GC can pick it up
        }

        private void AddRestrictionInfo(ThingDef parentDef, Def def, RestrictionHow how, out DefRestrictionInfo info)
        {
            info = new DefRestrictionInfo(def);
            switch (how)
            {
                case RestrictionHow.BlackListSelf:
                    info.BlackListUser(parentDef);
                    break;

                case RestrictionHow.WhiteListSelf:
                    info.WhiteListUser(parentDef);
                    break;

                case RestrictionHow.ForbidEveryone:
                    RestrictionsChecker.MarkRestricted(def);
                    break;

                case RestrictionHow.Required:
                    info.SetRequired(parentDef);
                    break;

                case RestrictionHow.RequiredAndWhitelist:
                    info.SetRequired(parentDef);
                    info.WhiteListUser(parentDef);
                    break;
            }

            restrictionInfos.Add(def, info);
        }

        private class DefRestrictInstructions
        {
            internal RestrictionType type;
            internal RestrictionHow how;
            internal Def def;

            internal void Deconstruct(out RestrictionType type, out RestrictionHow how, out Def def)
            {
                type = this.type;
                how = this.how; 
                def = this.def;
            }
        }

        private struct ModRestrictInstructions
        {
            internal Dictionary<RestrictionType, RestrictionHow> restrictions;
            internal ModContentPack pack;

            internal void Deconstruct(out Dictionary<RestrictionType, RestrictionHow> restrictions, out ModContentPack pack)
            {
                restrictions = this.restrictions;
                pack = this.pack;
            }
        }
    }

    public enum RestrictionType : byte
    {
        ResearchDef = 0,
        BodyTypeDef = 1,
        XenotypeDef = 2,
        ThoughtDef = 3,
        Equipment = 4,
        TraitDef = 5,
        Building = 6,
        FoodDef = 7,
        Apparel = 8,
        Beds = 9
    }

    [Flags]
    public enum RestrictionHow : byte
    {
        BlackListSelf = 0,
        WhiteListSelf = 1,
        ForbidEveryone = 2,
        Required = 4,
        RequiredAndWhitelist = Required | WhiteListSelf,
    }
}
