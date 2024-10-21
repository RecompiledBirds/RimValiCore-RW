using RimWorld;
using RVCRestructured.Shifter;
using UnityEngine;
using Verse;

namespace RVCRestructured.Defs;

/// <summary>
/// Used to render the graphics of a pawn into the world.
/// </summary>
public class RenderableDef : Def, IRenderable
{
    public List<BaseTex> textures = [];
    public BodyPartGraphicPos east;
    public BodyPartGraphicPos west = null;
    public BodyPartGraphicPos south;
    public BodyPartGraphicPos north;

    public RenderableDef linkTexWith;
    public RenderableDef linkPosWith;

    public bool linkWithHeadPos = false;

    public string colorSet;

    public bool showsInBed = true;

    public string bodyPart;

    readonly bool flipLayerEastWest = true;
    readonly bool flipYPos = false;
    public BodyPartGraphicPos this[int i] => GetBodyPartGraphicPosFromIntRot(i);

    public override IEnumerable<string> ConfigErrors()
    {
        RVCLog.Log($"{defName} has a null east GraphicPos.", RVCLogType.Error, east == null);
        RVCLog.Log($"{defName} has a null south GraphicPos.", RVCLogType.Error, south == null);
        RVCLog.Log($"{defName} has a null north GraphicPos.", RVCLogType.Error, north == null);
        RVCLog.Log($"{defName} has no textures.", RVCLogType.Error, textures.EnumerableNullOrEmpty());
        return base.ConfigErrors();
    }


    public bool CanDisplay(Pawn pawn, bool portrait = false)
    {
        IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();
        ShapeshifterComp comp = pawn.TryGetComp<ShapeshifterComp>();
        bool bodyIsHiding =(( bodyPart == null || pawn.TryGetComp<ShapeshifterComp>() == null) || bodyParts.Any(x => x.def.defName.ToLower() == bodyPart.ToLower() || x.Label.ToLower() == bodyPart.ToLower()));
        return (portrait && !bodyIsHiding) || ((!pawn.InBed() || (pawn.CurrentBed().def.building.bed_showSleeperBody) || showsInBed) && !bodyIsHiding);
    }
    public BodyPartGraphicPos GetPos(Rot4 rot)
    {
        GenerateWestIfNeeded();

        return GetBodyPartGraphicPosFromIntRot(rot.AsInt);
    }

    public ref Vector3 GetPosRef(int rot)
    {
        switch (rot)
        {
            case 0:
                return ref north.position;

            case 1:
                return ref east.position;

            case 2:
                return ref south.position;

            case 3:
                return ref west.position;
        }

        throw new ArgumentOutOfRangeException(nameof(rot), "Parameter has to be either 0, 1, 2, 3");
    }

    public ref Vector2 GetSizeRef(int rot)
    {
        switch (rot)
        {
            case 0:
                return ref north.size;

            case 1:
                return ref east.size;

            case 2:
                return ref south.size;

            case 3:
                return ref west.size;
        }

        throw new ArgumentOutOfRangeException(nameof(rot), "Parameter has to be either 0, 1, 2, 3");
    }

    public BodyPartGraphicPos GetPos(Rot4 rot,PawnRenderTree tree, bool inBed= false,bool portrait=false)
    {
        GenerateWestIfNeeded();

        return GetBodyPartGraphicPosFromIntRot(rot.AsInt, inBed, portrait);
    }

    private void GenerateWestIfNeeded()
    {
        if (west == null)
        {
            west = new BodyPartGraphicPos()
            {
                position = -east.position,
                size = east.size,
                offsetInBed = east.offsetInBed
            };

            if (!flipLayerEastWest) west.position.y = east.position.y;
            if (!flipYPos) west.position.z = east.position.z;
        }
    }
    readonly bool useScalingForPos = true;
    private readonly Dictionary<(bool inBed, int rot), Vector3> posCache = [];

    /// <summary>
    /// Travels along the parents of the renderabledef until it reaches the root.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPosRecursively(int rot, bool inBed, (bool inBed, int rot) pair, bool portrait = false)
    {
        if (posCache.TryGetValue(pair, out Vector3 pos)) return pos;

        Vector3 position;
        Vector3 recursizePos = (linkPosWith != null ? linkPosWith.GetPosRecursively(rot, inBed, pair, portrait) : Vector3.zero);
        BodyPartGraphicPos graphicPos = north;

        switch (rot)
        {
            case 1:
                graphicPos = east;
                break;
            case 2:
                graphicPos = south;
                break;
            case 3:
                GenerateWestIfNeeded();
                graphicPos = west;
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
        BodyPartGraphicPos newPos = rot switch
        {
            0 => new BodyPartGraphicPos()
            {
                position = pos,
                size = north.size,
                offsetInBed = north.offsetInBed
            },
            2 => new BodyPartGraphicPos()
            {
                position = pos,
                size = south.size,
                offsetInBed = south.offsetInBed
            },
            1 => new BodyPartGraphicPos()
            {
                position = pos,
                size = east.size,
                offsetInBed = east.offsetInBed
            },
            3 => new BodyPartGraphicPos()
            {
                position = pos,
                size = west.size,
                offsetInBed = west.offsetInBed
            },
            _ => throw new ArgumentOutOfRangeException(nameof(rot), "Parameter has to be either 0, 1, 2, 3")
        };

        return partCache[key] = newPos;
    }

    private BodyPartGraphicPos GetBodyPartGraphicPosFromIntRot(int rot, PawnRenderTree set)
    {
        BodyPartGraphicPos pos = GetBodyPartGraphicPosFromIntRot(rot,set.pawn.InBed());

        if (!linkWithHeadPos) return pos;
        Vector3 offset = set.HeadGraphic.DrawOffset(new Rot4(rot));
        pos.position.x +=offset.x;
        pos.position.y += offset.y;
        pos.position.z += offset.z;
        return pos;
    }


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

    public bool ShowsInBed()
    {
        return showsInBed;
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
            return new TriColorSet(pawn.DrawColor,pawn.DrawColorTwo,pawn.DrawColorTwo,false);
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
    public HediffDef hediffDef;

    public override bool CanApply(Pawn pawn)
    {
        return pawn.health.hediffSet.HasHediff(hediffDef);
    }
}

public class BackstoryTex : BaseTex
{
    public string backstoryTitle;

    public override bool CanApply(Pawn p)
    {
        return p.story.Adulthood.identifier == backstoryTitle || p.story.Childhood.identifier == backstoryTitle;
    }
}
public class HediffStoryTex : BaseTex
{
    public string backstoryTitle;
    public HediffDef hediffDef;
    public override bool CanApply(Pawn p)
    {
        return (p.story.Adulthood.identifier == backstoryTitle || p.story.Childhood.identifier == backstoryTitle) && p.health.hediffSet.HasHediff(hediffDef);
    }
}

public class BaseTex
{
    public string texPath;
    public string femaleTexPath;

    public List<string> alternateMaskPaths = [];
    public List<string> alternateFemaleMaskPaths = [];
    public List<string> alternateMaleMaskPaths = [];

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
