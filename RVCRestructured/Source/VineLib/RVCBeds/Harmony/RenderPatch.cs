using HarmonyLib;
using RimWorld;
using RVCRestructured.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVCBeds;
    public class RenderData
    {
        public float angle = 0f;
        public Vector3 pos = Vector3.zero;
        public Rot4? rot = null;
    }
class RenderPatch
{
    private static Dictionary<Pawn, RenderData> renderDatas = new Dictionary<Pawn, RenderData>();
    private static bool skipPatch = false;

    internal static void Patch(Harmony harmony)
    {
        // Patch render stuff for position and rotation
        Type patchType = typeof(RenderPatch);

        // Prefixes
        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.RenderPawnAt)),
            prefix: new HarmonyMethod(patchType, nameof(PawnRenderer_RenderPawnAt_Prefix)));


        //harmony.Patch(
        //    AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.RenderCache)),
        //    prefix: new HarmonyMethod(patchType, nameof(PawnRenderer_RenderCache_Prefix)));

        // Postfixes
        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.RenderPawnAt)),
            postfix: new HarmonyMethod(patchType, nameof(PawnRenderer_GetBodyPos_Postfix)));

        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.BodyAngle)),
            postfix: new HarmonyMethod(patchType, nameof(PawnRenderer_BodyAngle_Postfix)));

        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing)),
            postfix: new HarmonyMethod(patchType, nameof(PawnRenderer_LayingFacing_Postfix)));
    }

    // PREFIX
    public static void PawnRenderer_RenderPawnAt_Prefix(PawnRenderer __instance, Pawn ___pawn, ref Vector3 drawLoc, Rot4? rotOverride = null, bool neverAimWeapon = false)
    {
        try
        {
            if (___pawn.Dead ||
                ___pawn.GetPosture() == PawnPosture.Standing ||
                ___pawn.CurJob == null ||
                (___pawn.CurJob.def.defName != "LayDown" &&
                ___pawn.CurJob.def.defName != "Lovin"))
            {
                renderDatas.Remove(___pawn);
                return;
            }
            Building_Bed bed = ___pawn.CurrentBed();
            if (bed is null || !Patcher.Is2DBed(bed.def))
            {
                renderDatas.Remove(___pawn);
                return;
            }

            if (!renderDatas.TryGetValue(___pawn, out RenderData renderData))
            {
                renderDatas[___pawn] = new RenderData();
                renderData = renderDatas[___pawn];
            }

            int seed = (Find.TickManager.TicksGame + ___pawn.thingIDNumber * 600) / 20000 + ___pawn.thingIDNumber * 600;

            // Facing
            switch (Rand.RangeSeeded(0, 4, seed))
            {
                case 0:
                    renderData.rot = Rot4.East;
                    break;
                case 1:
                    renderData.rot = Rot4.West;
                    break;
                case 2:
                    renderData.rot = Rot4.South;
                    break;
                case 3:
                    renderData.rot = Rot4.North;
                    break;
            }

            // Angle
            renderData.angle = Rand.RangeSeeded(-180f, 180f, seed + 200);

            // Position
            int slot = bed.GetCurOccupantSlotIndex(___pawn);
            IntVec2 bedSize = bed.def.size;

            const float maxOffset = 0.35f,  // Random placement inward to bed
                edgeShift = 0.5f,           // Shift the bottom row inward
                maxEdgeOffset = 0.2f;       // Random placement relative to bed edges
            float minx = -maxOffset,
                maxx = maxOffset,
                minz = -maxOffset,
                maxz = maxOffset;
            bool zIsOne = bedSize.z == 1;
            bool otherFlag = slot / bedSize.x == bedSize.z - 1;
            bool flag = zIsOne || otherFlag;
            bool thirdFlag = (slot + 1) % bedSize.x == 0;
            bool fourthFlag = slot % bedSize.x == 0;
            bool fithFlag = slot / bedSize.x == 0;
            float negativeEdgeMinusMaxEdge = -edgeShift - maxEdgeOffset;
            float negativeEdgePlusMaxEdge = -edgeShift + maxEdgeOffset;
            float edgeShiftMinusMaxEdge = edgeShift - maxEdgeOffset;
            float edgePlusMaxEdge = edgeShift + maxEdgeOffset;
            if (bed.Rotation == Rot4.North)
            {
                minx = fourthFlag ? -maxEdgeOffset : -maxOffset;
                maxx = thirdFlag ? maxEdgeOffset : maxOffset;

                minz = flag ? negativeEdgeMinusMaxEdge : fithFlag ? -maxEdgeOffset : -maxOffset;
                maxz = flag ? negativeEdgePlusMaxEdge : maxOffset; maxz = -edgeShift + maxEdgeOffset;
            }
            else if (bed.Rotation == Rot4.East)
            {
                minx = flag ? negativeEdgeMinusMaxEdge : fithFlag ? -maxEdgeOffset : minx;
                maxx = flag ? negativeEdgePlusMaxEdge : maxx;

                minx = otherFlag ? negativeEdgeMinusMaxEdge : minx;
                maxx = otherFlag ? negativeEdgePlusMaxEdge : maxx;

                minz = otherFlag ? -maxEdgeOffset : minz;
                maxz = fourthFlag ? maxEdgeOffset : maxz;
            }
            else if (bed.Rotation == Rot4.South)
            {
                minx = fourthFlag ? -maxEdgeOffset : minx;

                maxx = otherFlag ? maxEdgeOffset : maxx;

                minz = flag ? edgeShiftMinusMaxEdge : minz;
                maxz = flag ? edgePlusMaxEdge : fithFlag ? maxEdgeOffset : maxz;

            }
            else // West
            {
                minx = flag ? edgeShiftMinusMaxEdge : minx;
                maxx = zIsOne ? edgePlusMaxEdge : fithFlag ? maxEdgeOffset : maxx;
                minz = thirdFlag ? -maxEdgeOffset : minz;
                maxz = fourthFlag ? maxEdgeOffset : maxz;
            }

            renderData.pos = new Vector3(Rand.RangeSeeded(minx, maxx, seed + 900), 0f, Rand.RangeSeeded(minz, maxz, seed + 1200));

            drawLoc += renderData.pos;
        }
        catch
        {
            renderDatas.Remove(___pawn);
        }
    }


    // POSTFIX
    public static void PawnRenderer_GetBodyPos_Postfix(PawnRenderer __instance, ref Vector3 __result, Vector3 drawLoc, ref bool showBody, Pawn ___pawn)
    {
        if (renderDatas.TryGetValue(___pawn, out RenderData renderData))
        {
            __result += renderData.pos;
        }
    }

    public static void PawnRenderer_BodyAngle_Postfix(PawnRenderer __instance, ref float __result, Pawn ___pawn)
    {
        if (renderDatas.TryGetValue(___pawn, out RenderData renderData))
        {
            __result += renderData.angle;
        }
    }

    public static void PawnRenderer_LayingFacing_Postfix(PawnRenderer __instance, ref Rot4 __result, Pawn ___pawn)
    {
        if (renderDatas.TryGetValue(___pawn, out RenderData renderData))
        {
            __result = renderData.rot ?? __result;
        }
    }
}
