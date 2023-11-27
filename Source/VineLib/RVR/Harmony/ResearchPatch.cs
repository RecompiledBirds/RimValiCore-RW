using RVCRestructured.Shifter;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ResearchPatch
    {
        public static void ResearchPostfix(Pawn pawn, ref bool __result)
        {
            ResearchProjectDef def = Find.ResearchManager.currentProj;
            if (def == null)
                return;
            ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
            RestrictionComp comp = pawn.TryGetComp<RestrictionComp>();
            RVRRestrictionComp restrictions=null;
            if (comp == null)
            {
                if (shapeshifterComp != null)
                {
                    restrictions = shapeshifterComp.GetCompProperties<RVRRestrictionComp>();
                    __result &= !RestrictionsChecker.IsRestricted(def) || (restrictions?.restrictedResearchDefs.Contains(def) ?? false);
                    return;
                }
                __result &= !RestrictionsChecker.IsRestricted(def);
                return;
            }
            if (shapeshifterComp != null)
            {
                restrictions = shapeshifterComp.GetCompProperties<RVRRestrictionComp>();
                __result &= !RestrictionsChecker.IsRestricted(def) || (restrictions?.restrictedResearchDefs.Contains(def) ?? false);
                return;
            }
            __result &= !RestrictionsChecker.IsRestricted(def) || (restrictions?.restrictedResearchDefs.Contains(def) ?? false);
        }
    }
}
