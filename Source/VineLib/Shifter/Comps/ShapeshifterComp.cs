using RVCRestructured.Defs;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter
{
    public class ShapeshifterComp : ThingComp
    {
        private ThingDef currentForm;
        public ThingDef CurrentForm
        {
            get
            {
                return currentForm;
            }
        }
        

        public void SetForm(ThingDef def)
        {
            currentForm = def;
            Pawn pawn = this.parent as Pawn;
            GraphicsComp graphicsComp = pawn.TryGetComp<GraphicsComp>();
            if (graphicsComp == null)
                return;

            RVRComp data = pawn.TryGetComp<RVRComp>();
            if (graphicsComp!=null)
            {
                if (graphicsComp.Props.renderableDefs.Count > 0)
                {
                    data.RenderableDefs.Clear();
                    foreach (RenderableDef renderableDef in graphicsComp.Props.renderableDefs)
                    {
                        data.RenderableDefs.Add(renderableDef);
                    }
                }
            }
            else
            {
                data.RenderableDefs.Clear();
                if (def.race.Humanlike)
                {
                    Pawn thingRace = ThingMaker.MakeThing(def) as Pawn;
                    PawnGraphicSet set = thingRace.Drawer.renderer.graphics;
                    Renderable renderableHead = new Renderable(set.headGraphic);
                    Renderable renderableTorso = new Renderable(set.nakedGraphic, showsInBed: false);
                    Renderable renderableHair = new Renderable(set.hairGraphic, showsInBed: false);

                    data.RenderableDefs.Add(renderableHair);
                    data.RenderableDefs.Add(renderableTorso);
                    data.RenderableDefs.Add(renderableHead);
                }
                else
                {
                    data.RenderableDefs.Add(new Renderable(def.graphic));
                }
            }
        }

        public void RevertForm()
        {
            SetForm(parent.def);
        }
    }
}
