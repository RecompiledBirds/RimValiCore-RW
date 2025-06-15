using RimWorld;
using RVCRestructured.Shifter;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RVCRestructured.Defs;

/// <summary>
/// Used to render the graphics of a pawn into the world.
/// </summary>
public class RenderableDef : Def
{
    private readonly List<BaseTex> textures = [];



    private readonly RenderableDef? linkTexWith = null;

    private readonly string? colorSet = null;
    private readonly string? bodyPart = null;

    private readonly bool showsInBed = true;



    public List<BaseTex> Textures => textures;
    public RenderableDef? LinkTexWith => linkTexWith;


    public string? BodyPart => bodyPart;


    

    public override IEnumerable<string> ConfigErrors()
    {

        RVCLog.Log($"{defName} has no textures.", RVCLogType.Error, Textures.EnumerableNullOrEmpty());
        return base.ConfigErrors();
    }


    public bool CanDisplay(Pawn pawn, bool portrait = false)
    {
        IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();
        bool shownByBody = BodyPart == null || bodyParts.Any(x => x.def.defName.ToLower() == BodyPart.ToLower() || x.Label.ToLower() == BodyPart.ToLower());
        return (portrait || NotInBedOrShouldShowBody(pawn)) && shownByBody;
    }

    public bool NotInBedOrShouldShowBody(Pawn pawn) => !pawn.InBed() || pawn.CurrentBed().def.building.bed_showSleeperBody || showsInBed;




    public string GetTexPath(Pawn pawn)
    {
        RVRComp comp = pawn.TryGetComp<RVRComp>();
        return comp.GetTexPath(this);
    }

    public string GetMaskPath(Pawn pawn)
    {
        RVRComp comp = pawn.TryGetComp<RVRComp>();
        return comp.GetMaskPath(this, pawn);
    }

    public bool ShowsInBed() => showsInBed;
    
    public TriColorSet ColorSet(RVRComp comp)
    {
        TriColorSet? set = null;

        if (colorSet != null) set = comp[colorSet];
        set ??= new TriColorSet(Color.red, Color.green, Color.blue, true);
        return set;
    }

    public TriColorSet ColorSet(Pawn pawn)
    {
        RVRComp comp = pawn.TryGetComp<RVRComp>();
        if (comp == null)
        {
            return new TriColorSet(pawn.DrawColor, pawn.DrawColorTwo, pawn.DrawColorTwo,false);
        }
        return ColorSet(comp);
    }
}


public class BodyPartGraphicPos
{
    public Vector3 position;
    public Vector2 size;
    public Vector2 offsetInBed;
}

public class HediffTex : BaseTex
{
    private readonly HediffDef? hediffDef = null;

    public override bool CanApply(Pawn pawn)
    {
        return pawn.health.hediffSet.HasHediff(hediffDef);
    }
}

public class BackstoryTex : BaseTex
{
    private readonly string? backstoryTitle = null;

    public override bool CanApply(Pawn p)
    {
        return p.story.Adulthood.identifier == backstoryTitle || p.story.Childhood.identifier == backstoryTitle;
    }
}

public class HediffStoryTex : BaseTex
{
    private readonly string? backstoryTitle = null;
    private readonly HediffDef? hediffDef = null;

    public override bool CanApply(Pawn p)
    {
        return (p.story.Adulthood.identifier == backstoryTitle || p.story.Childhood.identifier == backstoryTitle) && p.health.hediffSet.HasHediff(hediffDef);
    }
}

public class BaseTex
{
    private readonly string? texPath = null;
    private readonly string? femaleTexPath = null;

    public List<string> alternateMaskPaths = [];
    public List<string> alternateFemaleMaskPaths = [];
    public List<string> alternateMaleMaskPaths = [];

    public string TexPath => texPath ?? throw new NullReferenceException();
    public string FemaleTexPath => femaleTexPath ?? throw new NullReferenceException();

    public List<string> MaskPaths(Pawn pawn)
    {
        //TODO: Make sure that this never returns an empty list
        if(alternateFemaleMaskPaths.Count>0&&pawn.gender==Gender.Female)return alternateFemaleMaskPaths;
        if (alternateMaleMaskPaths.Count > 0 && pawn.gender == Gender.Male) return alternateMaleMaskPaths;
        return alternateMaskPaths;
    }

    /// <summary>
    /// Can the texture be applied to a pawn?
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public virtual bool CanApply(Pawn pawn)
    {
        return true;
    }

    /// <summary>
    /// Checks if the pawn can have alternative masks.
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public bool HasAlternateMasks(Pawn pawn)
    {
        return MaskPaths(pawn).Count>0;
    }

    /// <summary>
    /// Gets all possible masks for a pawn.
    /// </summary>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public List<string> GetMasks(Pawn pawn)
    {
        return MaskPaths(pawn);
    }
}
