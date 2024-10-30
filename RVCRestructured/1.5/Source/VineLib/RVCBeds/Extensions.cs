using RimWorld;
using Verse;

namespace RVCRestructured.RVCBeds;

internal static class Extensions
{
    /// <param name="thingDef">the given <see cref="ThingDef"/></param>
    /// <returns>if the given <paramref name="thingDef"/> has a <see cref="BedComp"/></returns>
    public static bool HasBedComp(this ThingDef thingDef) => thingDef.HasComp(typeof(BedComp));

    /// <param name="def">the <see cref="Building_Bed"/>s def, should have a <see cref="BedComp"/> for this calculation to make sense</param>
    /// <returns>the amount of sleeping spaces in a <see cref="Building_Bed"/></returns>
    internal static int GetSleepSpacesAmount(this ThingDef def) => def.size.x * def.size.z;

    /// <summary>
    ///     Calculates the position a <see cref="Pawn"/> should sleep at
    /// </summary>
    /// <param name="bed">the <see cref="Building_Bed"/> the pawn is sleeping in</param>
    /// <param name="index">the position in the <see cref="Building_Bed"/> the pawn is sleeping in</param>
    /// <returns>The position a <see cref="Pawn"/> should sleep at</returns>
    internal static IntVec3 RVC_GetSleepingSlotPos(this Building_Bed bed, int index)
    {
        IntVec3 bedCenter = bed.Position;
        Rot4 bedRot = bed.Rotation;
        return CalcSleepSpot(bed.def, index, bedCenter, bedRot);
    }

    /// <summary>
    ///     Calculates the position a <see cref="Pawn"/> should sleep at
    /// </summary>
    /// <param name="def">the given <see cref="Building_Bed"/>s <see cref="Def"/></param>
    /// <param name="index">the position in the bed, that the pawn is supposed to sleep in</param>
    /// <param name="bedCenter">the <see cref="Building_Bed"/> position</param>
    /// <param name="bedRot">the <see cref="Building_Bed"/>s rotation</param>
    /// <returns>The position a <see cref="Pawn"/> should sleep at</returns>
    internal static IntVec3 CalcSleepSpot(this ThingDef def, int index, IntVec3 bedCenter, Rot4 bedRot)
    {
        int sleepingSlotsCount = def.GetSleepSpacesAmount();
        IntVec2 bedSize = def.size;

        if (index < 0 || index >= sleepingSlotsCount)
        {
            RVCLog.Log(string.Concat(new object[]
            {
                "Tried to get sleeping slot pos with index ",
                index,
                ", but there are only ",
                sleepingSlotsCount,
                " sleeping slots available."
            }), RVCLogType.ErrorOnce);
            return bedCenter;
        }

        CellRect cellRect = GenAdj.OccupiedRect(bedCenter, bedRot, bedSize);

        if (bedRot == Rot4.North) return new IntVec3(cellRect.minX + index % bedSize.x, bedCenter.y, cellRect.minZ + index / bedSize.x);
        if (bedRot == Rot4.East) return new IntVec3(cellRect.minX + index / bedSize.x, bedCenter.y, cellRect.maxZ - index % bedSize.x);
        if (bedRot == Rot4.South) return new IntVec3(cellRect.minX + index % bedSize.x, bedCenter.y, cellRect.maxZ - index / bedSize.x);
        return new IntVec3(cellRect.maxX - index / bedSize.x, bedCenter.y, cellRect.maxZ - index % bedSize.x);
    }
}
