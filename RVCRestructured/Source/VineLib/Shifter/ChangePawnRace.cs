
using RVCRestructured.RVR.HarmonyPatches;

namespace RVCRestructured.Shifter
{
    public  static class PawnChanger
    {

        public static void ChangePawnRace(Pawn pawn, ThingDef toDef, bool humanlike)
        {
            if (humanlike)
            {
                ChangePawnRace(pawn,toDef);
                return;
            }
            ChangePawnRaceNonHumanlike(pawn,toDef);
        }

        public static void ChangePawnRace(Pawn pawn, ThingDef toDef)
        {
            RegionListersUpdater.DeregisterInRegions(pawn, pawn.Map);
            bool isHumanLike = pawn.def.race.Humanlike;
            pawn.Strip();
            pawn.def = toDef;
            if (!isHumanLike)
            {
                PawnComponentsUtility.CreateInitialComponents(pawn);
                pawn.kindDef = PawnKindDefOf.WildMan;
                pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
                pawn.story.bodyType = PawnGenerator.GetBodyTypeFor(pawn);
                pawn.story.TryGetRandomHeadFromSet(from x in DefDatabase<HeadTypeDef>.AllDefs where x.randomChosen select x);
                pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn);
                NamePatch.GenName(out NameTriple name, pawn);
                PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn,Faction.OfPlayer.def,new PawnGenerationRequest() { Faction=Faction.OfPlayer});
                pawn.Name = name;
            }
            RegionListersUpdater.RegisterInRegions(pawn, pawn.Map);
            RebuildComps(pawn);
            StopPathingAndRender(pawn);
        }

        public static void ChangePawnRaceUnspawned(Pawn pawn, ThingDef toDef)
        {
            bool isHumanLike = pawn.def.race.Humanlike;
            pawn.def = toDef;
            if (!isHumanLike)
            {
                PawnComponentsUtility.CreateInitialComponents(pawn);
                pawn.kindDef = PawnKindDefOf.WildMan;
                pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
                pawn.story.bodyType = PawnGenerator.GetBodyTypeFor(pawn);
                pawn.story.TryGetRandomHeadFromSet(from x in DefDatabase<HeadTypeDef>.AllDefs where x.randomChosen select x);
                pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn);
                NamePatch.GenName(out NameTriple name, pawn);
                PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn,pawn.Faction.def,new PawnGenerationRequest() { KindDef=PawnKindDefOf.WildMan, AllowDead=false, AllowDowned=false, Faction=pawn.Faction});
                pawn.Name = name;
            }
            RebuildComps(pawn);
        }

        private static void StopPathingAndRender(Pawn pawn)
        {
            pawn.jobs.StopAll();
            pawn.pather.StopDead();
            pawn.Drawer.renderer.SetAllGraphicsDirty();
            pawn.jobs.EndCurrentJob(Verse.AI.JobCondition.InterruptForced);
        }

        public static void ChangePawnRaceNonHumanlike(Pawn pawn, ThingDef toDef)
        {
            RegionListersUpdater.DeregisterInRegions(pawn, pawn.Map);
            bool isHumanLike = pawn.def.race.Humanlike;
            pawn.Strip();
            pawn.def = toDef;
            if (isHumanLike)
            {
                pawn.skills = null;
                pawn.guest = null;
                pawn.guilt = null;
                pawn.workSettings = null;
                pawn.royalty = null;
                pawn.ideo = null;
                pawn.genes = null;
                pawn.surroundings = null;
            }
            RegionListersUpdater.RegisterInRegions(pawn, pawn.Map);
            RebuildComps(pawn);
            StopPathingAndRender(pawn);
        }

        private static void RebuildComps(Pawn pawn)
        {
            pawn.AllComps.Clear();
            pawn.InitializeComps();
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            if (comp != null)
            {
                comp.CleanAndGenGraphics();
                comp.InformGraphicsDirty();
            }
        }
    }
}
