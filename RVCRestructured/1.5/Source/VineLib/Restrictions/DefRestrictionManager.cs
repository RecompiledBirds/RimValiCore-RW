using RimWorld;
using RVCRestructured.RVR;
using System.Xml;
using Verse;

namespace RVCRestructured;

public class DefRestrictionManager
{
    private readonly Dictionary<Def, DefRestrictionInfo> restrictionInfos = [];

    private readonly List<ModRestrictInstructions> modInstructions = [];
    private readonly List<DefRestrictInstructions> defInstructions = [];

    private readonly Dictionary<RestrictionType, Func<Def, bool>> restrictionTypeDefValidators = new()
    {
        { RestrictionType.ResearchDef, def => def is ResearchProjectDef },
        { RestrictionType.BodyTypeDef, def => def is BodyTypeDef },
        { RestrictionType.XenotypeDef, def => def is XenotypeDef },
        { RestrictionType.ThoughtDef, def => def is ThoughtDef },
        { RestrictionType.Equipment, def => def is ThingDef thingDef && thingDef.IsWeapon },
        { RestrictionType.TraitDef, def => def is TraitDef },
        { RestrictionType.Building, def => def is ThingDef thingDef && thingDef.building != null && thingDef.BuildableByPlayer && thingDef.blueprintDef != null && !thingDef.IsBed },
        { RestrictionType.FoodDef, def => def is ThingDef thingDef && thingDef.IsNutritionGivingIngestible },
        { RestrictionType.Apparel, def => def is ThingDef thingDef && thingDef.IsApparel && !thingDef.IsWeapon },
        { RestrictionType.Beds, def => def is ThingDef thingDef && thingDef.IsBed }
    };

    private readonly Dictionary<RestrictionType, Type> typeToDefType = new()
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

    private readonly Dictionary<RestrictionType, HashSet<DefRestrictionInfo>> typeToDefSet = new()
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

    private readonly HashSet<RestrictionType> alwaysAllowedTypes = [];

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
        LoadAllwaysAllowedTypes(root.SelectSingleNode("alwaysAllowed"));
    }

    private void LoadAllwaysAllowedTypes(XmlNode root)
    {
        if (root?.ChildNodes is not XmlNodeList allowedTypes) return;

        int allowedTypeCount = allowedTypes.Count;
        for (int i = 0; i < allowedTypeCount; i++)
        {
            XmlNode type = allowedTypes[i];

            try
            {
                alwaysAllowedTypes.Add(ParseHelper.FromString<RestrictionType>(type.Name));
            }
            catch (Exception ex)
            {
                throw new XmlException($"Type node is: {type.InnerXml}", ex);
            }
        }
    }

    private void LoadDefRestrictions(XmlNode root)
    {
        if (root?.ChildNodes is not XmlNodeList defRestrictions) return;

        int typeCount = defRestrictions.Count;
        for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
        {
            XmlNode restriction = defRestrictions[typeIndex];

            try
            {
                if (restriction.ChildNodes is not XmlNodeList singleDefNodes) continue;

                int childCount = singleDefNodes.Count;
                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    XmlNode defNode = singleDefNodes[childIndex];

                    RestrictionType restrictionType = ParseHelper.FromString<RestrictionType>(restriction.Name);
                    DefRestrictInstructions instructions = new()
                    {
                        type = restrictionType,
                        how = ParseHelper.FromString<RestrictionHow>(defNode.InnerText)
                    };

                    defNode.TryGetMayRequireAttributeValues(out string? mayRequireMod, out string? mayRequireAnyMod);
                    DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(instructions, nameof(instructions.def), defNode.Name, mayRequireMod, mayRequireAnyMod, typeToDefType[restrictionType]);

                    defInstructions.Add(instructions);
                }
            }
            catch (Exception ex)
            {
                throw new XmlException($"Restriction node is: {restriction.InnerXml}", ex);
            }
        }
    }

    public void LoadModRestrictions(XmlNode root)
    {
        if (root?.ChildNodes is not XmlNodeList modRestrictions) return;

        int count = modRestrictions.Count;
        for (int i = 0; i < count; i++)
        {
            XmlNode mod = modRestrictions[i];

            try
            {
                if (LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod.Name || x.PackageId.ToLower() == mod.Name.ToLower()) is not ModContentPack pack) continue;

                XmlNodeList modRestrictionTypes = mod.ChildNodes;
                int restrictionCount = modRestrictionTypes.Count;

                Dictionary<RestrictionType, RestrictionHow> restrictions = new(restrictionCount);
                for (int j = 0; j < restrictionCount; j++)
                {
                    XmlNode restriction = modRestrictionTypes[j];

                    try
                    {
                        RestrictionType restrictionType = ParseHelper.FromString<RestrictionType>(restriction.Name);
                        RestrictionHow restrictionHow = ParseHelper.FromString<RestrictionHow>(restriction.InnerText);

                        restrictions[restrictionType] = restrictionHow;
                    }
                    catch (Exception ex)
                    {
                        throw new XmlException($"Mod restriction node is: {restriction.OuterXml}", ex);
                    }
                }

                modInstructions.Add(new ModRestrictInstructions { restrictions = restrictions, pack = pack });
            }
            catch (Exception ex)
            {
                throw new XmlException($"Mod node is: {mod.OuterXml}", ex);
            }
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
            AddRestrictionInfo(parentDef, def, how, out DefRestrictionInfo? info);
            if (info == null) continue;

            typeToDefSet[type].Add(info);
        }

        defInstructions.Clear();
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
                    AddRestrictionInfo(parentDef, def, restrictions[type], out DefRestrictionInfo? info);
                    if (info == null) continue;

                    typeToDefSet[type].Add(info);
                }
            }
        }

        modInstructions.Clear();
    }

    private void AddRestrictionInfo(ThingDef parentDef, Def? def, RestrictionHow how, out DefRestrictionInfo? info)
    {
        info = null;
        if (def == null) return;

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

        if (restrictionInfos.TryGetValue(def, out DefRestrictionInfo oldInfo))
        {
            if (info.User != oldInfo.User) throw new InvalidOperationException("Illegal ParentDef");

            if (info != oldInfo) Log.Warning($"Tried to add restriction with existing key:\n{info}");

            info = oldInfo;
            return;
        }

        restrictionInfos.Add(def, info);
    }

    private class DefRestrictInstructions
    {
        internal RestrictionType type;
        internal RestrictionHow how;
        internal Def def = null!;

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

        internal readonly void Deconstruct(out Dictionary<RestrictionType, RestrictionHow> restrictions, out ModContentPack pack)
        {
            restrictions = this.restrictions;
            pack = this.pack;
        }
    }
}

public enum RestrictionType
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
public enum RestrictionHow
{
    BlackListSelf = 0,
    WhiteListSelf = 1,
    ForbidEveryone = 2,
    Required = 4,
    RequiredAndWhitelist = Required | WhiteListSelf,
}
