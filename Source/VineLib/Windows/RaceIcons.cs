using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Source.VineLib.Windows
{
    public static class RaceIcons
    {
        private static readonly Dictionary<(ThingDef thingDef, XenotypeDef xenoDef, Vector2 size), Texture> textures = new Dictionary<(ThingDef thingDef, XenotypeDef xenoDef, Vector2 size), Texture>();

        public static Dictionary<ThingDef, PawnKindDef> pawnKindDefCache = new Dictionary<ThingDef, PawnKindDef>();

        private static readonly FactionDef factionDef;

        private static readonly Faction faction;

        static RaceIcons()
        {
            factionDef = GenFactionDef();
            faction = new Faction() { def = factionDef};
            faction.TryMakeInitialRelationsWith(Faction.OfPlayer);
        }

        public static Texture GetThingDefTexture((ThingDef thingDef, XenotypeDef xenoDef, Vector2 size) key)
        {
            if (textures.ContainsKey(key)) return textures[key];

            PawnGenerationRequest request = new PawnGenerationRequest(GeneratePawnKindDef(key.thingDef))
            {
                ForceGenerateNewPawn = true,
                AllowDead = false,
                AllowAddictions = false,
                AllowDowned = false,
                AllowFood = false,
                AllowGay = false,
                AllowPregnant = false,
                AllowedDevelopmentalStages = DevelopmentalStage.Adult,
                BiocodeApparelChance = 0,
                FixedBiologicalAge = 50,
                ForbidAnyTitle = true,
                FixedChronologicalAge = 50,
                BiocodeWeaponChance = 0,
                ForceAddFreeWarmLayerIfNeeded = false,
                CertainlyBeenInCryptosleep = false,
                CanGeneratePawnRelations = false,
                ColonistRelationChanceFactor = 0,
                Context = PawnGenerationContext.NonPlayer,
                Faction = faction,
                ForceBaselinerChance = 0,
                ForceDead = false,
                ForcedXenotype = key.xenoDef,
                ForceNoIdeo = true,
                ForceRecruitable = false,
                ForceRedressWorldPawnIfFormerColonist = false,
                Inhabitant = false,
                MinChanceToRedressWorldPawn = 0,
                MustBeCapableOfViolence = false,
                WorldPawnFactionDoesntMatter = true
            };

            Current.ProgramState = ProgramState.Entry;
            Pawn pawn;

            try
            {
                pawn = PawnGenerator.GeneratePawn(request);
            }
            catch (Exception ex)
            {
                RVCLog.Error($"Failed to generate preview pawn with key: ThingDef: {key.thingDef.defName}, XenoDef {key.xenoDef?.defName}, ex: {ex.Message}");
                return null;
            }

            Current.ProgramState = ProgramState.Playing;

            PortraitsCache.SetDirty(pawn);
            PortraitsCache.PortraitsCacheUpdate();
            RenderTexture image = PortraitsCache.Get(pawn, key.size, Rot4.South, new Vector3(0f, 0f, 0.25f), stylingStation: true, cameraZoom: 2.5f, supersample: false);

            textures.Add(key, image);

            return textures[key];
        }

        public static PawnKindDef GeneratePawnKindDef(ThingDef thingDef)
        {
            if (pawnKindDefCache.ContainsKey(thingDef)) return pawnKindDefCache[thingDef];

            PawnKindDef pawnKindDef = new PawnKindDef()
            {
                race = thingDef,
                useFactionXenotypes = false,
                defaultFactionType = factionDef,
                canStrip = true,
                defName = "VineInternal",
                apparelMoney = new FloatRange(0f, 0f),
                chronologicalAgeRange = new FloatRange(0f, 0f),
                combatPower = 0f,
                allowInMechClusters = false,
                allowOldAgeInjuries = false,
                allowRoyalApparelRequirements = false,
                allowRoyalRoomRequirements = false,
                apparelAllowHeadgearChance = 0f,
                backstoryFilters = new List<BackstoryCategoryFilter>() {  },
                forcedAddictions = { },
                forcedHair = HairDefOf.Bald,
                chemicalAddictionChance = 0f,
                lifeStages = { }
            };

            pawnKindDefCache.Add(thingDef, pawnKindDef);

            return pawnKindDefCache[thingDef];
        }

        public static FactionDef GenFactionDef()
        {
            FactionDef def = new FactionDef()
            {
                allowedArrivalTemperatureRange = new FloatRange(float.MinValue, float.MaxValue),
                apparelStuffFilter = new ThingFilter(),
                attackersDownPercentageRangeForAutoFlee = new FloatRange(),
                autoFlee = true,
                canStageAttacks = false,
                canUseAvoidGrid = false,
                baseTraderKinds = { },
                canMakeRandomly = false,
                canSiege = false,
                basicMemberKind = null,
                caravanTraderKinds = { },
                allowedCultures = { },
                allowedMemes = { },
                maxConfigurableAtWorldCreation = 0,
                maxCountAtGameStart = 0,
                requiredCountAtGameStart = 0,
                defName = "VineInternal",
                generated = false,
                hidden = true,
                rescueesCanJoin = false,
                earliestRaidDays = float.MaxValue,
                backstoryFilters = Faction.OfPlayer.def.backstoryFilters
            };

            return def;
        }
    }
}
