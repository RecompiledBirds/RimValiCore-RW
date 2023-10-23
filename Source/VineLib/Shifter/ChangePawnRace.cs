using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter
{
    public  static class PawnChanger
    {
        public static void ChangePawnRace(Pawn pawn, ThingDef toDef)
        {
            ThingDef oldDef = pawn.def;
            bool isHumanLike = pawn.def.race.Humanlike;
            pawn.pather.StopDead();
            
            pawn.jobs.StopAll();
            pawn.Strip();
            pawn.def = toDef;
            pawn.pather.StopDead();
           
            if (!isHumanLike)
            {
                /*
                pawn.abilities = new Pawn_AbilityTracker(pawn);
                pawn.skills = new Pawn_SkillTracker(pawn);
                pawn.story = new Pawn_StoryTracker(pawn);
                pawn.guest = new Pawn_GuestTracker(pawn);
                pawn.guilt = new Pawn_GuiltTracker(pawn);
                pawn.workSettings = new Pawn_WorkSettings(pawn);
                pawn.royalty = new Pawn_RoyaltyTracker(pawn);
                pawn.ideo = new Pawn_IdeoTracker(pawn);
                pawn.style = new Pawn_StyleTracker(pawn);
                pawn.styleObserver = new Pawn_StyleObserverTracker(pawn);
                pawn.surroundings = new Pawn_SurroundingsTracker(pawn);
                pawn.genes = new Pawn_GeneTracker(pawn);
                pawn.equipment = new Pawn_EquipmentTracker(pawn);
                pawn.apparel = new Pawn_ApparelTracker(pawn);
                //PawnComponentsUtility.CreateInitialComponents(pawn);
                pawn.story = new Pawn_StoryTracker(pawn);
                if (pawn.RaceProps.IsFlesh)
                {
                    pawn.relations = new Pawn_RelationsTracker(pawn);
                    pawn.psychicEntropy = new Pawn_PsychicEntropyTracker(pawn);
                }
                PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, false);
                */
                PawnComponentsUtility.CreateInitialComponents(pawn);
                pawn.kindDef = Utils.GetKindDef(pawn.def);
                pawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(pawn);
                pawn.story.bodyType=PawnGenerator.GetBodyTypeFor(pawn);
                pawn.story.TryGetRandomHeadFromSet(from x in DefDatabase<HeadTypeDef>.AllDefs where x.randomChosen select x);
                PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, oldDef.label, pawn.kindDef.defaultFactionType);
            }
            pawn.pather.StopDead();
            pawn.AllComps.Clear();
            pawn.InitializeComps();
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            if (comp != null)
            {
                comp.CleanAndGenGraphics();
                comp.InformGraphicsDirty();                
            }
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            pawn.pather.StopDead();
        }
    }
}
