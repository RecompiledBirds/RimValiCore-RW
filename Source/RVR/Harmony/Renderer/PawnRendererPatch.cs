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
              //  Graphic_Multi gra = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(comp.GetTexPath(renderableDef), TriColorShader.tricolorshader, pos.size, set[1], set[2]);
                RVG_Graphic graphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(comp.GetTexPath(renderableDef), pos.size, set[0], set[1], set[2]);
                GenDraw.DrawMeshNowOrLater(graphic.MeshAt(rotation), rootLoc + pos.position.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat) * 114.59156f)), quat, graphic.MatAt(rotation), flags.FlagSet(PawnRenderFlags.DrawNow));
              //  GenDraw.DrawMeshNowOrLater(gra.MeshAt(rotation), rootLoc + pos.position.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat) * 114.59156f)), quat, gra.MatAt(rotation), flags.FlagSet(PawnRenderFlags.DrawNow));


            }
        }
    }
}
