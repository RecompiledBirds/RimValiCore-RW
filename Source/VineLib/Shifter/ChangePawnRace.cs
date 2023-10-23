using RimWorld;
using RVCRestructured.RVR.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RVCRestructured.Shifter
{
    public  static class PawnChanger
    {
        public static void ChangePawnRace(Pawn pawn, ThingDef toDef)
        {
            RegionListersUpdater.DeregisterInRegions(pawn, pawn.Map);
            ThingDef oldDef = pawn.def;
            bool isHumanLike = pawn.def.race.Humanlike;
            pawn.Strip();
            pawn.def = toDef;
            RegionListersUpdater.RegisterInRegions(pawn, pawn.Map);
            if (!isHumanLike)
            {
                PawnComponentsUtility.CreateInitialComponents(pawn);
                pawn.kindDef = PawnKindDefOf.WildMan;
                pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
                pawn.story.bodyType=PawnGenerator.GetBodyTypeFor(pawn);
                pawn.story.TryGetRandomHeadFromSet(from x in DefDatabase<HeadTypeDef>.AllDefs where x.randomChosen select x);
                pawn.Name=PawnBioAndNameGenerator.GeneratePawnName(pawn);
                NameTriple name=null;
                NamePatch.GenName(ref name, pawn);
                PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, oldDef.label, pawn.kindDef.defaultFactionType);
                pawn.Name=name;
            }
            pawn.AllComps.Clear();
            pawn.InitializeComps();
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            if (comp != null)
            {
                comp.CleanAndGenGraphics();
                comp.InformGraphicsDirty();                
            }
            pawn.jobs.StopAll();
            pawn.pather.StopDead();
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
           // pawn.pather = new Pawn_PathFollower(pawn);
          //  pawn.jobs=new Pawn_JobTracker(pawn);
        }
    }
}
