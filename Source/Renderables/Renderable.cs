using RimWorld;
using RVCRestructured.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured
{
    public enum BodyRegion
    {
        head,
        torso
    }
    public class Renderable : IRenderable
    {
        private string bodyPart;
        private BodyRegion region;
        private Graphic storedGraphic;
        private bool showsInBed;


        public Renderable(Graphic graphic, string bodyPart=null, bool showsInBed = true)
        {
            this.storedGraphic = graphic;
            this.bodyPart = bodyPart;
            this.showsInBed = showsInBed;

        }
       
        public bool CanDisplay(Pawn pawn, bool portrait = false)
        {
            IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();
            bool bodyIsHiding = bodyPart == null || bodyParts.Any(x => x.def.defName.ToLower() == bodyPart.ToLower() || x.Label.ToLower() == bodyPart.ToLower());
            return (portrait && !bodyIsHiding) || ((!pawn.InBed() || (pawn.CurrentBed().def.building.bed_showSleeperBody) || showsInBed) && bodyIsHiding);
        }

        public TriColorSet ColorSet(Pawn pawn)
        {
            return new TriColorSet(storedGraphic.Color, storedGraphic.colorTwo, storedGraphic.Color, false);
        }

        public string GetMaskPath(Pawn pawn)
        {
            return storedGraphic.maskPath;
        }

        public BodyPartGraphicPos GetPos(Rot4 rotation)
        {
            return new BodyPartGraphicPos()
            {
                position = storedGraphic.DrawOffset(rotation),
                size = storedGraphic.drawSize
            };
        }

        public string GetTexPath(Pawn pawn)
        {
            return storedGraphic.path;
        }

        public bool ShowsInBed()
        {
            return region != BodyRegion.torso;
        }
    }
}
