using RimWorld;
using RVCRestructured.RVR;
using RVCRestructured.Shifter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace RVCRestructured
{
    public static class Utils
    {
        private static Dictionary<RaceProperties, ThingDef> thingDefCache = new Dictionary<RaceProperties, ThingDef>();
        private static Dictionary<ThingDef, PawnKindDef> pawnKindCache = new Dictionary<ThingDef, PawnKindDef>();

        public static ThingDef GetDef(RaceProperties race)
        {
            if (!thingDefCache.ContainsKey(race))
                thingDefCache.Add(race, DefDatabase<ThingDef>.AllDefs.First(x => x.race == race));
            return thingDefCache[race];

        }
        public static PawnKindDef GetKindDef(ThingDef race)
        {
            if (!pawnKindCache.ContainsKey(race))
                pawnKindCache.Add(race, DefDatabase<PawnKindDef>.AllDefs.First(x => x.race == race));
            return pawnKindCache[race];
        }

        public static float GetStatOffsetOrBaseFromList(this List<StatModifier> mods, StatDef stat)
        {
            return mods.GetStatValueFromList(stat, stat.defaultBaseValue);
        }

        public static bool TryGetMayRequireAttributeValues(this XmlNode node, out string mayRequireMod, out string mayRequireAnyMod)
        {
            mayRequireMod = null;
            mayRequireAnyMod = null;

            if (!(node.Attributes is XmlAttributeCollection attributes)) return false;
            mayRequireMod = attributes["MayRequire"]?.Value.ToLower();
            mayRequireAnyMod = attributes["MayRequireAnyOf"]?.Value.ToLower();

            return true;
        }

        public static RVRRestrictionComp GetRelevantRestrictionComp(this Pawn pawn)
        {
            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            return shapeshifterComp?.GetCompProperties<RVRRestrictionComp>() ?? comp?.Props;
        }

        /// <summary>
        ///     Tests a pawns shapeshifter or RestrictionComp props if a pawn can use a def
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static bool CanUse(this Pawn pawn, Def def)
        {
            RVRRestrictionComp props = pawn.GetRelevantRestrictionComp();
            if (props is null)
            {
                return !RestrictionsChecker.IsRestricted(def);
            }

            return (props[def]?.CanUse ?? false) || !def.IsRestricted() || props.IsAlwaysAllowed(def);
        }
    }
}
