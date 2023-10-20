using RimWorld;
using RVCRestructured.RVR;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RVCRestructured.Defs
{
    /// <summary>
    /// Used to render the graphics of a pawn into the world.
    /// </summary>
    public class RenderableDef : Def, IRenderable
    {
        public List<BaseTex> textures = new List<BaseTex>();
        public bool hiddenInBed = false;
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

        bool flipLayerEastWest = true;
        bool flipYPos = false;
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
            bool bodyIsHiding = bodyPart == null || bodyParts.Any(x => x.def.defName.ToLower() == bodyPart.ToLower() || x.Label.ToLower() == bodyPart.ToLower());
            return (portrait && !bodyIsHiding) || ((!pawn.InBed() || (pawn.CurrentBed().def.building.bed_showSleeperBody) || showsInBed) && bodyIsHiding);
        }
        public BodyPartGraphicPos GetPos(Rot4 rot)
        {
            if (west == null)
            {
                west = new BodyPartGraphicPos()
                {
                    position = -east.position,
                    size = east.size
                };
                if (!flipLayerEastWest)
                    west.position.y = east.position.y;
                if (!flipYPos)
                    west.position.z = east.position.z;
            }

            return GetBodyPartGraphicPosFromIntRot(rot.AsInt);
        }


        public BodyPartGraphicPos GetPos(Rot4 rot,PawnGraphicSet set)
        {
            if (west == null)
            {
                west = new BodyPartGraphicPos()
                {
                    position = -east.position,
                    size = east.size
                };
                if (!flipLayerEastWest)
                    west.position.y = east.position.y;
                if (!flipYPos)
                    west.position.z = east.position.z;
            }

            return GetBodyPartGraphicPosFromIntRot(rot.AsInt,set);
        }
        private Dictionary<int, Vector3> posCache = new Dictionary<int, Vector3>();
        private Vector3 GetPosRecursively(int rot)
        {
            if (!posCache.ContainsKey(rot))
            {
                Vector3 position;
                Vector3 recursizePos = (linkPosWith != null ? linkPosWith.GetPosRecursively(rot) : Vector3.zero);
                switch (rot)
                {
                    case 0:
                        position= north.position + recursizePos;
                        break;
                    case 2:
                        position= south.position + recursizePos;
                        break;
                    case 1:
                        position= east.position + recursizePos;
                        break;
                    case 3:
                        if (west == null)
                        {
                            west = new BodyPartGraphicPos()
                            {
                                position = -east.position,
                                size = east.size
                            };
                            if (!flipLayerEastWest)
                                west.position.y = east.position.y;
                            if (!flipYPos)
                                west.position.z = east.position.z;
                        }
                        position = west.position + recursizePos;
                        break;
                    default:
                        position= Vector3.zero;
                        break;
                }
                posCache[rot] = position;
            }
            return posCache[rot];
        }
        private Dictionary<int, BodyPartGraphicPos> partCache = new Dictionary<int, BodyPartGraphicPos>();
        private BodyPartGraphicPos GetBodyPartGraphicPosFromIntRot(int rot)
        {
            if (!partCache.ContainsKey(rot))
            {
                Vector3 pos = GetPosRecursively(rot);
                BodyPartGraphicPos newPos;
                switch (rot)
                {
                    case 0:

                        newPos = new BodyPartGraphicPos()
                        {
                            position = pos,
                            size = north.size
                        };
                        break;

                    case 2:
                        newPos = new BodyPartGraphicPos()
                        {
                            position = pos,
                            size = south.size
                        };
                        break;

                    case 1:

                        newPos = new BodyPartGraphicPos()
                        {
                            position = pos,
                            size = east.size
                        };
                        break;
                    case 3:
                        newPos = new BodyPartGraphicPos()
                        {
                            position = pos,
                            size = west.size
                        };
                        break;

                    default:
                        newPos = null;
                        break;
                }
                partCache[rot] = newPos;
            }
            return partCache[rot];
        }

        private BodyPartGraphicPos GetBodyPartGraphicPosFromIntRot(int rot,PawnGraphicSet set)
        {
            BodyPartGraphicPos pos = GetBodyPartGraphicPosFromIntRot(rot);
            if (!linkWithHeadPos) return pos;
            Vector3 offset = set.headGraphic.DrawOffset(new Rot4(rot));
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

        public List<string> alternateMaskPaths = new List<string>();
        public List<string> alternateFemaleMaskPaths = new List<string>();
        public List<string> alternateMaleMaskPaths = new List<string>();

        public List<string> MaskPaths(Pawn pawn)
        {
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
}
