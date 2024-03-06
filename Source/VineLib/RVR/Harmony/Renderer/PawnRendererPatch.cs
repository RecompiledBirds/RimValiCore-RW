using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.Graphics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR
{
    public static class PawnRendererPatch
    {
        public static void RenderingPostfix(Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags, PawnRenderer __instance)
        {
            Pawn pawn = __instance.graphics.pawn;
            //Portrait checks
            bool portrait = flags.HasFlag(PawnRenderFlags.Portrait);
            Rot4 rotation = portrait ? Rot4.South : bodyFacing;
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);

            bool inBed = pawn.InBed();
            bool bedHidesBody = inBed && !pawn.CurrentBed().def.building.bed_showSleeperBody;
            //genetic drawing

            if (pawn.genes!=null && !pawn.genes.GenesListForReading.NullOrEmpty())
            {
                IEnumerable<Gene> defs = pawn.genes.GenesListForReading.Where(x => x.def is GeneRenderableDef);

                foreach (Gene gene in defs)
                {
                    GeneRenderableDef def = gene.def as GeneRenderableDef;
                    RenderableDef renderableDef = def.renderableDef;
                    BodyPartGraphicPos pos = renderableDef.GetPos(rotation, __instance.graphics,inBed,portrait);
                    Vector3 position = pos.position;
                    
                    if (bedHidesBody && !renderableDef.ShowsInBed())
                        continue;
                    TriColorSet set = renderableDef.ColorSet(pawn);

                    RVG_Graphic graphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(renderableDef.GetTexPath(pawn), pos.size, set[0], set[1], set[2], renderableDef.GetMaskPath(pawn));
                    GenDraw.DrawMeshNowOrLater(graphic.MeshAt(rotation), rootLoc + position.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 114.60f),
                       quat, graphic.MatAt(rotation), flags.FlagSet(PawnRenderFlags.DrawNow));

                }
            }
            
            
            
            
           
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            if (comp == null) return;

            foreach (IRenderable renderableDef in comp.RenderableDefs)
            {
                
                //Render the def
                BodyPartGraphicPos pos = renderableDef.GetPos(rotation,__instance.graphics,inBed,portrait);
                Vector3 position = pos.position;

                if (bedHidesBody && !renderableDef.ShowsInBed())
                    continue;
                TriColorSet set = renderableDef.ColorSet(comp);

                RVG_Graphic graphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(renderableDef.GetTexPath(pawn), pos.size, set[0], set[1], set[2], renderableDef.GetMaskPath(pawn));
                GenDraw.DrawMeshNowOrLater(graphic.MeshAt(rotation), rootLoc + position.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 114.60f),
                   quat, graphic.MatAt(rotation), flags.FlagSet(PawnRenderFlags.DrawNow));


            }
        }
    }
}
