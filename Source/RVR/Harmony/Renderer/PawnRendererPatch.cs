using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class PawnRendererPatch
    {
        public static void RenderingPostfix(Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags, PawnRenderer __instance)
        {
            Pawn pawn = __instance.graphics.pawn;
            if(!(pawn.def is RaceDef rDef))
                return;
            //Portrait checks
            bool portrait = flags.HasFlag(PawnRenderFlags.Portrait);
            Rot4 rotation = portrait ? Rot4.South : bodyFacing;

            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);

            RVRComp comp = pawn.TryGetComp<RVRComp>();

            foreach(RenderableDef renderableDef in rDef.RaceGraphics.renderableDefs)
            {
                //Render the def
                BodyPartGraphicPos pos = renderableDef.GetPos(rotation);

                TriColorSet set = null;
                if(renderableDef.colorSet != null)
                    set = comp[renderableDef.colorSet];
                if (set == null)
                {
                    set = new TriColorSet(Color.red, Color.green, Color.blue, true);
                }

                RVG_Graphic graphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(comp.GetTexPath(renderableDef), pos.size, set[0], set[1], set[2]);
                GenDraw.DrawMeshNowOrLater(graphic.MeshAt(rotation), rootLoc + pos.position.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 114.60f), Quaternion.AngleAxis(angle, Vector3.up) * quat, graphic.MatAt(rotation), portrait||flags.FlagSet(PawnRenderFlags.DrawNow));    


            }
        }
    }
}
