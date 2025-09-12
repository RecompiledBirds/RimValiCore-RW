using HarmonyLib;
using UnityEngine;

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
            AccessTools.Method(typeof(PawnRenderer), "GetBodyPos"),
            postfix: new HarmonyMethod(patchType, nameof(PawnRenderer_GetBodyPos_Postfix)));

        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.BodyAngle)),
            postfix: new HarmonyMethod(patchType, nameof(PawnRenderer_BodyAngle_Postfix)));

        harmony.Patch(
            AccessTools.Method(typeof(PawnRenderer), nameof(PawnRenderer.LayingFacing)),
            postfix: new HarmonyMethod(patchType, nameof(PawnRenderer_LayingFacing_Postfix)));
    }

    // PREFIX
    public static void PawnRenderer_RenderPawnAt_Prefix(PawnRenderer __instance, Pawn ___pawn, ref Vector3 drawLoc, Rot4? rotOverride, bool neverAimWeapon)
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
            if (bed==null || !Patcher.Is2DBed(bed, out BedComp? resizedBedCompProperties))
            {
                renderDatas.Remove(___pawn);
                return;
            }

            int seed = (Find.TickManager.TicksGame + ___pawn.thingIDNumber) / 20000 + ___pawn.thingIDNumber;
            if (!renderDatas.TryGetValue(___pawn, out RenderData renderData))
            {
                renderData = new RenderData();
                // Angle
                if (resizedBedCompProperties?.Props.isPile ?? false)
                    renderData.angle = resizedBedCompProperties.Props.rotationRange.RandomInRangeSeeded(seed + 200);
                renderDatas[___pawn] = renderData;
            }


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



            // Position
            int slot = bed.GetCurOccupantSlotIndex(___pawn);
            IntVec2 bedSize = bed.def.size;

            const float MAX_OFFSET = 0.35f; // Random shift
            const float MAX_TOP_EDGE_OFFSET = -0.05f; // Max top edge outward shift
            const float MAX_BOTTOM_EDGE_OFFSET = -0.05f; // Max bottom edge outward shift
            const float MAX_SIDE_EDGE_OFFSET = -0.05f; // Max left/right edge outward shift
            float minx = -MAX_OFFSET;
            float maxx = MAX_OFFSET;
            float minz = -MAX_OFFSET;
            float maxz = MAX_OFFSET;
            bool isEdge1 = slot % bedSize.x == 0;
            bool isEdge2 = (slot + 1) % bedSize.x == 0;
            bool isEdge3 = slot / bedSize.x == 0;
            bool isEdge4 = slot / bedSize.x == bedSize.z - 1;
            bool isLeft, isRight, isTop, isBottom;

            if (bed.Rotation == Rot4.North) // Bottom row is at top
            {
                isLeft = isEdge1;
                isRight = isEdge2;
                isTop = isEdge4;
                isBottom = isEdge3;
            }
            else if (bed.Rotation == Rot4.East) // Bottom row is on right
            {
                isLeft = isEdge3;
                isRight = isEdge4;
                isTop = isEdge1;
                isBottom = isEdge2;
            }
            else if (bed.Rotation == Rot4.South) // Bottom row on bottom
            {
                isLeft = isEdge1;
                isRight = isEdge2;
                isTop = isEdge3;
                isBottom = isEdge4;
            }
            else // West - Bottom row on left
            {
                isLeft = isEdge4;
                isRight = isEdge3;
                isTop = isEdge1;
                isBottom = isEdge2;
            }

            if (bedSize.x == 1) isLeft = isRight = true; // 1xN bed
            if (bedSize.z == 1) isTop = isBottom = true; // Nx1 bed

#pragma warning disable CS0162 // Unreachable code detected
            // Left/Right
            if (isLeft && isRight) // 1xN bed
            {
                if (MAX_SIDE_EDGE_OFFSET < 0) minx = maxx = 0;
                else
                {
                    minx = -MAX_SIDE_EDGE_OFFSET;
                    maxx = MAX_SIDE_EDGE_OFFSET;
                }
            }
            else if (isLeft) // Left edge
            {
                minx = -MAX_SIDE_EDGE_OFFSET;
                if (MAX_SIDE_EDGE_OFFSET < 0) maxx = -MAX_SIDE_EDGE_OFFSET + MAX_OFFSET;
            }
            else if (isRight) // Right edge
            {
                maxx = MAX_SIDE_EDGE_OFFSET;
                if (MAX_SIDE_EDGE_OFFSET < 0) minx = MAX_SIDE_EDGE_OFFSET - MAX_OFFSET;
            }

            // Top/Bottom
            if (isTop && isBottom) // Nx1 bed
            {
                if (-MAX_BOTTOM_EDGE_OFFSET > MAX_TOP_EDGE_OFFSET) minz = maxz = 0;
                else
                {
                    minz = -MAX_BOTTOM_EDGE_OFFSET;
                    maxz = MAX_TOP_EDGE_OFFSET;
                }
            }
            else if (isTop) // Top edge
            {
                maxz = MAX_TOP_EDGE_OFFSET;
                if (MAX_TOP_EDGE_OFFSET < 0) minz = MAX_TOP_EDGE_OFFSET - MAX_OFFSET;
            }
            else if (isBottom) // Bottom edge
            {
                minz = -MAX_BOTTOM_EDGE_OFFSET;
                if (MAX_BOTTOM_EDGE_OFFSET < 0) maxz = -MAX_BOTTOM_EDGE_OFFSET + MAX_OFFSET;
            }
#pragma warning restore CS0162 // Unreachable code detected

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
