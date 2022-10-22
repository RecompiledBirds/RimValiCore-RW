using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Defs
{
    /// <summary>
    /// Used to render the graphics of a pawn into the world.
    /// </summary>
    public class RenderableDef
    {
        public List<BaseTex> textures = new List<BaseTex>();

        public BodyPartGraphicPos east;
        public BodyPartGraphicPos west = null;
        public BodyPartGraphicPos south;
        public BodyPartGraphicPos north;

        public BodyPartGraphicPos GetPos(Pawn pawn)
        {
            if (west == null)
            {
                west = new BodyPartGraphicPos()
                {
                    position = -east.position,
                    size = east.size
                };
                west.position.z = east.position.z;
            }
            
            switch (pawn.Rotation.AsInt)
            {
                case 0:
                    return north;
                case 2:
                    return south;
                case 1:
                    return east;
                case 3:
                    return west;
                default:
                    return null;
            }
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
            return pawn.gender == Gender.Female ? alternateFemaleMaskPaths.Count > 0 : alternateMaskPaths.Count > 0;
        }

        /// <summary>
        /// Gets all possible masks for a pawn.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public List<string> GetMasks(Pawn pawn)
        {
            return HasAlternateMasks(pawn) ? pawn.gender == Gender.Male ? alternateMaskPaths : alternateFemaleMaskPaths : new List<string>();
        }
    }
}
