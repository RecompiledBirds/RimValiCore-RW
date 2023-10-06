using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RVCRestructured.Comps
{
    using RimWorld;
    using RVCRestructured.Graphics;
    using System.Collections.Generic;
    using UnityEngine;
    using Verse;

    public class DrawCompProps : CompProperties
    {
        public string texPath;
        public Vector3 offset;

        public bool isAnimated = false;
        public int ticksBetweenTexture;
        public bool lockAtLastTex;
        public List<string> textures = new List<string>();

        public DrawCompProps()
        {
            compClass = typeof(DrawComp);
        }
    }

    public class DrawComp : ThingComp
    {
        public DrawCompProps Props => (DrawCompProps)props;
        public Graphic graphic;
        public int tex;
        public int tick;

        public override void CompTick()
        {
            if (Props.isAnimated)
            {
                tick++;

                if (tick == Props.ticksBetweenTexture)
                {
                    tick = 0;
                    tex = tex < Props.textures.Count - 1 ? tex++ : !Props.lockAtLastTex ? tex = 0 : tex = Props.textures.Count - 1;
                }
            }
            base.CompTick();
        }

        public override void PostDraw()
        {
            Draw();
        }

        private void Draw()
        {
            Vector3 offset = Props.offset;
            Vector3 pos = parent.DrawPos;
            pos.y += 1.5f + offset.y;
            pos.z += offset.z;
            pos.x += offset.x;

            if (Props.isAnimated)
            {
                graphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(Props.textures[tex], parent.Graphic.drawSize);
            }
            else
            {
                graphic = graphic ?? RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(Props.texPath, parent.Graphic.drawSize);
            }

            if (parent.TryGetComp<CompPowerTrader>() != null)
            {
                if (parent.TryGetComp<CompPowerTrader>().PowerOn && FlickUtility.WantsToBeOn(parent))
                {
                    graphic.Draw(pos, parent.Rotation, parent);
                }
            }
            else
            {
                graphic.Draw(pos, parent.Rotation, parent); 
            }
        }
    }
}
