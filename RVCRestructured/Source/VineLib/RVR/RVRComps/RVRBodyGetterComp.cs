﻿using RimWorld;
using Verse;

namespace RVCRestructured;

public class RVRBodyGetterComp : CompProperties
{
    public RVRBodyGetterComp() { 
        compClass = typeof(RVRBodyGetter);
    }
    public List<BodyTypeDef> allowedBodyTypes = [];
    public List<string> modAllowedBodyTypes = [];
    private bool resolved;
    public override void ResolveReferences(ThingDef parentDef)
    {
        if (resolved) return;
        resolved = true;
        foreach (string mod in modAllowedBodyTypes)
        {
            //Try to find the mod.
            ModContentPack pack = LoadedModManager.RunningModsListForReading.Find(x => x.Name == mod || x.PackageId.ToLower() == mod.ToLower());
            //If we can't find it, skip
            if (pack == null) continue;
            //Add everything considered to be food
            allowedBodyTypes.AddRange(DefDatabase<BodyTypeDef>.AllDefsListForReading.Where(x => x.modContentPack == pack));
        }
        base.ResolveReferences(parentDef);
    }
    
}

public class RVRBodyGetter : ThingComp
{
    public RVRBodyGetterComp Props => (RVRBodyGetterComp)props;

    public void GenBody()
    {
        if (parent is not Pawn pawn) return;
        if (Props.allowedBodyTypes.Contains(pawn.story.bodyType)) return;
        pawn.story.bodyType = Props.allowedBodyTypes.RandomElement();
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        GenBody();
    }
}
