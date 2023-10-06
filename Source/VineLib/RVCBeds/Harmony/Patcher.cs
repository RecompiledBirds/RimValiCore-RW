using UnityEngine;
using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace RVCRestructured.Source.VineLib.RVCBeds.Harmony
{
    [StaticConstructorOnStartup]
    public static class Patcher
    {
        private const string moduleName = "[RVCBeds]";

        static Patcher()
        {
            {
                RVCLog.Log($"{moduleName} Patching started");
                Harmony harmony = new Harmony("Fluxic.Beds2DPatch");

                harmony.Patch(AccessTools.PropertyGetter(typeof(Building_Bed), nameof(Building_Bed.SleepingSlotsCount)), prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_SleepingSlotsCount)));
                harmony.Patch(AccessTools.Method(typeof(Building_Bed), nameof(Building_Bed.GetSleepingSlotPos)), prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_GetSleepingSlotPos)));
                harmony.Patch(AccessTools.Method(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.CanPotentiallyLinkTo_Static), new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4) }), prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_CanPotentiallyLinkTo_Static)));
                harmony.Patch(AccessTools.Method(typeof(CompProperties_AssignableToPawn), nameof(CompProperties_AssignableToPawn.PostLoadSpecial)), prefix: new HarmonyMethod(typeof(Patcher), nameof(Prefix_PostLoadSpecial)));

                RVCLog.Log($"{moduleName} Patching success");
            }
        }

        /// <summary>
        ///     Calculates the amount of sleeping spaces a <see cref="Building_Bed"/> provides, if it has a <see cref="BedComp"/>
        ///     skips to vanilla function otherwise
        /// </summary>
        public static bool Prefix_SleepingSlotsCount(ref int __result, ref Building_Bed __instance)
        {
            if (!__instance.def.HasBedComp()) return true;

            __result = __instance.def.GetSleepSpacesAmount();
            return false;
        }

        /// <summary>
        ///     Calculates the amount of sleeping spaces a <see cref="Building_Bed"/> provides, if it has a <see cref="BedComp"/>
        ///     Does this in order to figure out how many pawns may use a bed
        ///     skips to vanilla function otherwise
        /// </summary>
        public static bool Prefix_PostLoadSpecial(ref CompProperties_AssignableToPawn __instance, ThingDef parent)
        {
            if (!parent.HasBedComp()) return true;

            __instance.maxAssignedPawnsCount = parent.GetSleepSpacesAmount();
            return false;
        }

        /// <summary>
        ///     Calculates the sleeping position of <see cref="Pawn"/>s trying to sleep in a <see cref="Building_Bed"/>, if it has the <see cref="BedComp"/>
        ///     Skips to the vanilla function otherwise
        /// </summary>
        public static bool Prefix_GetSleepingSlotPos(ref IntVec3 __result, ref Building_Bed __instance, int index)
        {
            if (!__instance.def.HasBedComp()) return true;

            __result = __instance.RVC_GetSleepingSlotPos(index);
            return false;
        }

        /// <summary>
        ///     Determines if a facility can link to a <see cref="Building_Bed"/> with a <see cref="BedComp"/>.
        ///     Skips to the vanilla function if the given <paramref name="myDef"/> doesn't have the <see cref="BedComp"/>.
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="facilityDef">The linking facilities def</param>
        /// <param name="facilityPos">The linking facilities position</param>
        /// <param name="facilityRot">The linking facilities rotation</param>
        /// <param name="myDef">The <see cref="Building_Bed"/>s def</param>
        /// <param name="myPos">The <see cref="Building_Bed"/>s position</param>
        /// <param name="myRot">The <see cref="Building_Bed"/>s rotation</param>
        /// <returns>if the vanilla function should execute</returns>
        public static bool Prefix_CanPotentiallyLinkTo_Static(ref bool __result, ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
        {
            if (!myDef.HasBedComp()) return true;

            CompProperties_Facility compProperties = facilityDef.GetCompProperties<CompProperties_Facility>();
            TmpVariableContainer tmp = new TmpVariableContainer(facilityDef, facilityPos, facilityRot, myDef, myPos, myRot);

            if (!PassesAdjecency(compProperties, ref __result, tmp)) return false;
            if (!PassesDistanceCheck(compProperties, ref __result, tmp)) return false;
            if (!PassesAdjacentCardinal(compProperties, ref __result, tmp)) return false;

            __result = true;
            return false;
        }

        /// <summary>
        ///     Checks if the linked facility passes Adjecency checks
        /// </summary>
        private static bool PassesAdjecency(CompProperties_Facility compProperties, ref bool __result, TmpVariableContainer tmp)
        {
            if (!compProperties.mustBePlacedAdjacent) return true;

            CellRect rect = GenAdj.OccupiedRect(tmp.myPos, tmp.myRot, tmp.myDef.size);
            CellRect rect2 = GenAdj.OccupiedRect(tmp.facilityPos, tmp.facilityRot, tmp.facilityDef.size);

            if (GenAdj.AdjacentTo8WayOrInside(rect, rect2)) return true;

            __result = false;
            return false;
        }

        /// <summary>
        ///     Checks if the linked facility passes cardinal adjecency checks
        /// </summary>
        private static bool PassesAdjacentCardinal(CompProperties_Facility compProperties, ref bool __result, TmpVariableContainer tmp)
        {
            if (!compProperties.mustBePlacedAdjacentCardinalToBedHead || !compProperties.mustBePlacedAdjacentCardinalToAndFacingBedHead) return true;

            CellRect other = GenAdj.OccupiedRect(tmp.facilityPos, tmp.facilityRot, tmp.facilityDef.size);

            int sleepingSlotsCount = tmp.myDef.GetSleepSpacesAmount();

            for (int i = 0; i < sleepingSlotsCount; i++)
            {
                IntVec3 sleepingSlotPos = tmp.myDef.CalcSleepSpot(i, tmp.myPos, tmp.myRot);
                if (sleepingSlotPos.IsAdjacentToCardinalOrInside(other) && compProperties.mustBePlacedAdjacentCardinalToAndFacingBedHead && !other.MovedBy(tmp.facilityRot.FacingCell).Contains(sleepingSlotPos))
                {
                    __result = false;
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        ///     Checks if the linked facility passes distance checks
        /// </summary>
        private static bool PassesDistanceCheck(CompProperties_Facility compProperties, ref bool __result, TmpVariableContainer tmp)
        {
            if (compProperties.mustBePlacedAdjacent || compProperties.mustBePlacedAdjacentCardinalToBedHead || compProperties.mustBePlacedAdjacentCardinalToAndFacingBedHead) return true;

            Vector3 a = GenThing.TrueCenter(tmp.myPos, tmp.myRot, tmp.myDef.size, tmp.myDef.Altitude);
            Vector3 b = GenThing.TrueCenter(tmp.facilityPos, tmp.facilityRot, tmp.facilityDef.size, tmp.facilityDef.Altitude);
            if (Vector3.Distance(a, b) <= compProperties.maxDistance) return true;

            __result = false;
            return false;
        }

        private class TmpVariableContainer
        {
            public readonly ThingDef facilityDef;
            public readonly IntVec3 facilityPos;
            public readonly Rot4 facilityRot;
            public readonly ThingDef myDef;
            public readonly IntVec3 myPos;
            public readonly Rot4 myRot;

            public TmpVariableContainer(ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef, IntVec3 myPos, Rot4 myRot)
            {
                this.facilityDef = facilityDef;
                this.facilityPos = facilityPos;
                this.facilityRot = facilityRot;
                this.myDef = myDef;
                this.myPos = myPos;
                this.myRot = myRot;
            }
        }
    }
}
