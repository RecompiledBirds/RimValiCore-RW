using RimWorld;
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
            ThingDef oldDef = pawn.def;
            bool isHumanLike = pawn.def.race.Humanlike;
            pawn.Strip();
            pawn.def = toDef;
            if (!isHumanLike)
            {
                PawnComponentsUtility.CreateInitialComponents(pawn);
                pawn.kindDef = PawnKindDefOf.WildMan;
                pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
                pawn.story.bodyType=PawnGenerator.GetBodyTypeFor(pawn);
                pawn.story.TryGetRandomHeadFromSet(from x in DefDatabase<HeadTypeDef>.AllDefs where x.randomChosen select x);
                PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, oldDef.label, pawn.kindDef.defaultFactionType);
            }
            pawn.AllComps.Clear();
            pawn.InitializeComps();
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            if (comp != null)
            {
                comp.CleanAndGenGraphics();
                comp.InformGraphicsDirty();                
            }
            pawn.pather.StopDead();
            typeof(Pawn_PathFollower).GetMethod("PatherFailed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(pawn.pather, new object[] { });
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            pawn.pather = new Pawn_PathFollower(pawn);
            pawn.jobs=new Pawn_JobTracker(pawn);
            Job job = JobMaker.MakeJob(JobDefOf.Wait_Wander, pawn.Position);
            job.expiryInterval = 10;
            pawn.jobs.TryTakeOrderedJob(job);
        }
    }
}
