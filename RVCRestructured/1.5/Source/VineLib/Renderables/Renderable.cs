using RimWorld;
using RVCRestructured.Defs;
using UnityEngine;
using Verse;

namespace RVCRestructured;

public enum BodyRegion
{
    head,
    torso
}
public class Renderable : IRenderable
{
    private readonly string bodyPart;
    private readonly BodyRegion region;
    private readonly Graphic storedGraphic;
    private readonly bool showsInBed;
    private readonly string colorSet;

    public Renderable(Graphic graphic,string colorSet=null, string bodyPart=null, bool showsInBed = true)
    {
        this.storedGraphic = graphic;
        this.bodyPart = bodyPart;
        this.showsInBed = showsInBed;
        this.colorSet= colorSet;

    }
   
    public bool CanDisplay(Pawn pawn, bool portrait = false)
    {
        IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();
        bool bodyIsHiding = bodyPart == null || bodyParts.Any(x => x.def.defName.ToLower() == bodyPart.ToLower() || x.Label.ToLower() == bodyPart.ToLower());
        return (portrait && !bodyIsHiding) || ((!pawn.InBed() || (pawn.CurrentBed().def.building.bed_showSleeperBody) || showsInBed) && bodyIsHiding);
    }

    public TriColorSet ColorSet(RVRComp comp)
    {
        TriColorSet set = null;
        if (colorSet != null)
            set = comp[colorSet];
        if (set == null)
        {
            set = new TriColorSet(Color.red, Color.green, Color.blue, true);
        }
        return set;
    }

    public TriColorSet ColorSet(Pawn pawn)
    {
        
        RVRComp comp = pawn.TryGetComp<RVRComp>();
        if (comp == null)
        {
            return new TriColorSet(pawn.DrawColor, pawn.DrawColorTwo, pawn.DrawColorTwo, false);
        }
        return ColorSet(comp);
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

    public BodyPartGraphicPos GetPos(Rot4 rotation, PawnRenderTree set, bool inBed, bool portrait = false)
    {
        throw new NotImplementedException();
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
