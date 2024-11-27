using RimWorld;
using RVCRestructured.Shifter;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RVCRestructured.Defs;

/// <summary>
/// Used to render the graphics of a pawn into the world.
/// </summary>
public class RenderableDef : Def, IRenderable
{
    private readonly List<BaseTex> textures = [];

    private readonly BodyPartGraphicPos? east = null;
    private readonly BodyPartGraphicPos? south = null;
    private readonly BodyPartGraphicPos? north = null;
    private BodyPartGraphicPos? west = null;

    private readonly RenderableDef? linkTexWith = null;
    private readonly RenderableDef? linkPosWith = null;

    private readonly string? colorSet = null;
    private readonly string? bodyPart = null;

    private readonly bool flipLayerEastWest = true;
    private readonly bool showsInBed = true;
    private readonly bool flipYPos = false;


    public BodyPartGraphicPos East => east ?? throw new NullReferenceException();
    public BodyPartGraphicPos West => west ??= GenerateWest();
    public BodyPartGraphicPos South => south ?? throw new NullReferenceException();
    public BodyPartGraphicPos North => north ?? throw new NullReferenceException();

    public List<BaseTex> Textures => textures;
    public RenderableDef? LinkTexWith => linkTexWith;
    public RenderableDef? LinkPosWith => linkPosWith;

    public string? BodyPart => bodyPart;
    public bool FlipLayerEastWest => flipLayerEastWest;
    private bool FlipYPos => flipYPos;

    public BodyPartGraphicPos this[int i] => GetBodyPartGraphicPosFromIntRot(i);

    public override IEnumerable<string> ConfigErrors()
    {
        RVCLog.Log($"{defName} has a null east GraphicPos.", RVCLogType.Error, East == null);
        RVCLog.Log($"{defName} has a null south GraphicPos.", RVCLogType.Error, South == null);
        RVCLog.Log($"{defName} has a null north GraphicPos.", RVCLogType.Error, North == null);
        RVCLog.Log($"{defName} has no textures.", RVCLogType.Error, Textures.EnumerableNullOrEmpty());
        return base.ConfigErrors();
    }


    public bool CanDisplay(Pawn pawn, bool portrait = false)
    {
        IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();
        bool shownByBody = BodyPart == null || bodyParts.Any(x => x.def.defName.ToLower() == BodyPart.ToLower() || x.Label.ToLower() == BodyPart.ToLower());
        return (portrait || NotInBedOrShouldShowBody(pawn)) && shownByBody;
    }
    public bool NotInBedOrShouldShowBody(Pawn pawn) => ((!pawn.InBed() || showsInBed) || pawn.CurrentBed().def.building.bed_showSleeperBody);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BodyPartGraphicPos GetPos(Rot4 rot) => GetBodyPartGraphicPosFromIntRot(rot.AsInt);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Vector3 GetPosRef(int rot)
    {
        switch (rot)
        {
            case 0:
                return ref North.position;

            case 1:
                return ref East.position;

            case 2:
                return ref South.position;

            case 3:
                return ref West.position;
        }

        throw new ArgumentOutOfRangeException(nameof(rot), "Parameter has to be either 0, 1, 2, 3");
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref Vector2 GetSizeRef(int rot)
    {
        switch (rot)
        {
            case 0:
                return ref North.size;

            case 1:
                return ref East.size;

            case 2:
                return ref South.size;

            case 3:
                return ref West.size;
        }

        throw new ArgumentOutOfRangeException(nameof(rot), "Parameter has to be either 0, 1, 2, 3");
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BodyPartGraphicPos GetPos(Rot4 rot, bool inBed = false, bool portrait = false) 
        => GetBodyPartGraphicPosFromIntRot(rot.AsInt, inBed, portrait);

    private BodyPartGraphicPos GenerateWest()
    {
        BodyPartGraphicPos graphicPos = new()
        {
            position = -East.position,
            size = East.size,
            offsetInBed = East.offsetInBed
        };

        if (!FlipLayerEastWest) graphicPos.position.y = East.position.y;
        if (!FlipYPos) graphicPos.position.z = East.position.z;

        return graphicPos;
    }

    readonly bool useScalingForPos = true;
    private readonly Dictionary<(bool inBed, int rot), Vector3> posCache = [];

    /// <summary>
    /// Travels along the parents of the renderabledef until it reaches the root.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPosRecursively(int rot, bool inBed, (bool inBed, int rot) pair, bool portrait = false)
    {
        if (posCache.TryGetValue(pair, out Vector3 position)) return position;

        Vector3 recursizePos = (LinkPosWith != null ? LinkPosWith.GetPosRecursively(rot, inBed, pair, portrait) : Vector3.zero);



        BodyPartGraphicPos? graphicPos = null;
        switch (rot)
        {

            case 1:
                graphicPos = East;
                break;
            case 2:
                graphicPos = South;
                break;
            case 3:
                graphicPos = West;
                break;
            default:
                graphicPos = North;
                break;
        }
        float scalar = useScalingForPos ? graphicPos.size.x : 1;
        position = graphicPos.position.MultipliedBy(new(scalar, 1f, scalar)) + recursizePos;

        if (inBed)
        {
            position.z -= graphicPos.offsetInBed.y;
            position.x -= graphicPos.offsetInBed.x;
        }

        return posCache[pair] = position;
    }
    private readonly Dictionary<(bool inBed, int rot), BodyPartGraphicPos> partCache = [];

    private BodyPartGraphicPos GetBodyPartGraphicPosFromIntRot(int rot, bool inBed=false, bool portrait = false)
    {
        
        (bool inBed, int rot) key;
        if (portrait)
        {
            rot = 2;
            inBed = false;
        }

        key = (inBed, rot);

        if (partCache.TryGetValue(key, out BodyPartGraphicPos graphicPos)) return graphicPos;
        Vector3 pos = GetPosRecursively(rot, inBed, key, portrait);


        BodyPartGraphicPos? newPos = null;
        switch (rot)
        {
            case 0:
                newPos = new BodyPartGraphicPos
                {
                    position = pos,
                    size = North.size,
                    offsetInBed = North.offsetInBed
                };
                break;
            case 2:
                newPos = new BodyPartGraphicPos
                {
                    position = pos,
                    size = South.size,
                    offsetInBed = South.offsetInBed
                };
                break;
            case 1:
                newPos = new BodyPartGraphicPos
                {
                    position = pos,
                    size = East.size,
                    offsetInBed = East.offsetInBed
                };
                break;
            case 3:
                newPos = new BodyPartGraphicPos
                {
                    position = pos,
                    size = West.size,
                    offsetInBed = West.offsetInBed
                };
                break;

            default:
                //Default returns south
                newPos = new BodyPartGraphicPos
                {
                    position = pos,
                    size = South.size,
                    offsetInBed = South.offsetInBed
                };
                RVCLog.Log("Rotation parameter has to be either 0, 1, 2, 3!",type:RVCLogType.Error);
                break;
        }

        return partCache[key] = newPos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BodyPartGraphicPos GetPos(Pawn pawn)
    {
        return GetPos(pawn.Rotation);
    }


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
