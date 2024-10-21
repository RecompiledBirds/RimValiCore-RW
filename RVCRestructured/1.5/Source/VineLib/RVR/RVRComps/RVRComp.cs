using RVCRestructured.Defs;
using RVCRestructured.Shifter;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RVCRestructured;

public class RVRCP : CompProperties
{
    public RVRCP() => compClass = typeof(RVRComp);
}

public class RVRComp : ThingComp
{
    private Dictionary<string, int> renderableIndexes = [];
    private Dictionary<string, TriColorSet> sets = [];
    private Dictionary<string, int> masks = [];

    private List<RenderableDef> defListRenderableDefs = [];
    private List<Renderable> defListRenderable = [];
    private List<IRenderable> defList = [];

    private bool generated = false;

    public List<IRenderable> RenderableDefs
    {
        get => defList;
        set => defList = value;
    }

    public Dictionary<string, TriColorSet> Colors => sets;

    //used in loading
    private List<string> lKeys = [];
    private List<TriColorSet> lSets = [];
    private List<int> lInts = [];

    public string GetTexPath(RenderableDef def)
    {
        return def.Textures[renderableIndexes[def.defName]].TexPath;
    }

    public void SendRenderableDefToNextTexture(RenderableDef def)
    {
        if (!renderableIndexes.ContainsKey(def.defName)) return;
        int index = renderableIndexes[def.defName];
        index++;
        if (def.Textures.Count == index)
        {
            index = 0;
        }
        renderableIndexes[def.defName] = index;
    }
    public void SendRenderableDefToPreviousTexture(RenderableDef def)
    {
        if (!renderableIndexes.ContainsKey(def.defName)) return;
        int index = renderableIndexes[def.defName];
        index--;
        if (index == -1)
        {
            index = def.Textures.Count - 1;
        }
        renderableIndexes[def.defName] = index;
    }

    public void SendRenderableDefToNextMask(RenderableDef def)
    {
        if (!renderableIndexes.ContainsKey(def.defName)) return;
        if (!masks.ContainsKey(def.defName)) return;
        int texIndex = renderableIndexes[def.defName];
        int maskIndex = masks[def.defName];
        maskIndex--;
        List<string> maskList = def.Textures[texIndex].GetMasks((Pawn)parent);
        if (maskIndex == maskList.Count)
        {
            maskIndex = 0;
        }
        masks[def.defName] = maskIndex;
    }
    public void SendRenderableDefToPreviousMask(RenderableDef def)
    {
        if (!renderableIndexes.ContainsKey(def.defName)) return;
        if (!masks.ContainsKey(def.defName)) return;
        int texIndex = renderableIndexes[def.defName];
        int maskIndex = masks[def.defName];
        maskIndex--;
        List<string> maskList = def.Textures[texIndex].GetMasks((Pawn)parent);
        if (maskIndex == -1)
        {
            maskIndex = maskList.Count - 1;
        }
        masks[def.defName] = maskIndex;
    }
    public string GetMaskPath(RenderableDef def, Pawn pawn)
    {
        if (masks.ContainsKey(def.defName) && !def.Textures[renderableIndexes[def.defName]].GetMasks(pawn).NullOrEmpty())
            return def.Textures[renderableIndexes[def.defName]].GetMasks(pawn)[masks[def.defName]];
        return def.Textures[renderableIndexes[def.defName]].TexPath;
    }

    public TriColorSet this[string name]
    {
        get
        {
            if (sets.TryGetValue(name, out TriColorSet set)) return set;

            Pawn pawn = (Pawn)parent;
            RVCLog.Log($"ColorSet {name} is not on {pawn.Name.ToStringShort}!", RVCLogType.Error);
            return TriColorSet.Empty;
        }
        set
        {
            sets[name] = value;
        }
    }

    public override void PostExposeData()
    {
        Scribe_Collections.Look(ref sets, "sets", LookMode.Value, LookMode.Deep, ref lKeys, ref lSets);
        Scribe_Collections.Look(ref masks, "masks", LookMode.Value, LookMode.Value, ref lKeys, ref lInts);
        Scribe_Collections.Look(ref renderableIndexes, "renderableIndexes", LookMode.Value, LookMode.Value, ref lKeys, ref lInts);

        if (Scribe.mode == LoadSaveMode.Saving)
        {
            foreach (IRenderable renderable in defList)
            {
                if (renderable is Def)
                {
                    defListRenderableDefs.Append(renderable);
                    continue;
                }
                
                defListRenderable.Append(renderable);
            }
        }

        Scribe_Collections.Look(ref defListRenderable, nameof(defListRenderable), LookMode.Deep);
        Scribe_Collections.Look(ref defListRenderableDefs, nameof(defListRenderableDefs), LookMode.Def);


        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            if (defList.NullOrEmpty()) defList = [];
            if (!defListRenderableDefs.NullOrEmpty()) defList.AddRange(defListRenderableDefs);
            if (!defListRenderable.NullOrEmpty()) defList.AddRange(defListRenderable);
        }
    }

    public void ClearAllGraphics()
    {
        generated = false;
        masks = [];
        renderableIndexes = [];
        sets = [];
        defList = [];
        defListRenderable = [];
        defListRenderableDefs = [];
    }

    public void CleanAndGenGraphics()
    {
        ClearAllGraphics();
        GenGraphics();
    }

    public void GenGraphics([CallerMemberName] string callerMemberName = "")
    {
        try
        {
            if (generated) return;
            generated = true;
            Pawn pawn = (Pawn)parent;

            if (pawn.TryGetComp<GraphicsComp>() is not GraphicsComp comp) return;

            RVRGraphicsComp props = comp.Props;

            if (pawn.TryGetComp<ShapeshifterComp>() is ShapeshifterComp shapeshifterComp)
            {
                RVRGraphicsComp shifterGraphics = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
                if (!shapeshifterComp.IsParentDef())
                {
                    if (shifterGraphics == null) return;
                
                    props = shifterGraphics;
                }
            }

            GenFromComp(props, pawn);
        }
        catch (Exception ex)
        {
            RVCLog.Log($"Hit an exception {ex} trying to generate graphics from {callerMemberName}!", RVCLogType.Error);
        }
    }

    public void GenFromComp(RVRGraphicsComp comp, Pawn pawn)
    {
        GenColors(comp, pawn);
        GenAllDefs(comp, pawn);
        InformGraphicsDirty();
    }

    public void GenColors(RVRGraphicsComp comp, Pawn pawn)
    {
        foreach (RaceColors colors in comp.colorGenerators)
        {
            if (sets.ContainsKey(colors.name))
                continue;

            Color c1 = colors.GeneratorToUse(pawn).colorOne.NewRandomizedColor();
            Color c2 = colors.GeneratorToUse(pawn).colorTwo.NewRandomizedColor();
            Color c3 = colors.GeneratorToUse(pawn).colorThree.NewRandomizedColor();
            sets.Add(colors.name, new TriColorSet(c1, c2, c3, true));
        }
    }

    public void GenAllDefs(RVRGraphicsComp comp, Pawn pawn)
    {
        defList.Clear();

        if (defList.NullOrEmpty() && !comp.renderableDefs.NullOrEmpty())
        {
            defList = [.. comp.renderableDefs];
        }

        foreach (RenderableDef rDef in comp.renderableDefs)
        {
            GenerateRenderableDef(rDef, pawn);
        }

        InformGraphicsDirty();
    }

    private bool graphicsDirty;

    public bool ShouldResetGraphics
    {
        get
        {
            if (graphicsDirty)
            {
                graphicsDirty = false;
                return true;
            }
            return false;
        }
    }
    public void InformGraphicsDirty()
    {
        graphicsDirty = true;
    }

    private void GenerateRenderableDef(RenderableDef rDef, Pawn pawn)
    {
        if (renderableIndexes.ContainsKey(rDef.defName)) return;

        bool hasLink = rDef.LinkTexWith != null;
        if (hasLink && renderableIndexes.ContainsKey(rDef.LinkTexWith!.defName))
        {
            string linkString = rDef.LinkTexWith.defName;
            renderableIndexes[rDef.defName] = renderableIndexes[linkString];
            masks[rDef.defName] = masks[linkString];
            return;
        }

        if (rDef.Textures.Count == 0)
        {
            Log.Warning($"Textures count for {rDef.defName} is 0!");
            return;
        }

        BaseTex tex = rDef.Textures.RandomElement();
        int index = rDef.Textures.IndexOf(tex);
        renderableIndexes[rDef.defName] = index;

        int maskCount = tex.GetMasks(pawn).Count;
        if (maskCount == 0)
        {
            Log.Warning($"Mask count for def: {rDef.defName}, path: {tex.TexPath}, index: ({index}) is 0!");
            return;
        }

        masks.Add(rDef.defName, Rand.Range(0, maskCount));

        if (hasLink)
        {
            string linkString = rDef.LinkTexWith!.defName;
            renderableIndexes[linkString] = renderableIndexes[rDef.defName];
            masks[linkString] = renderableIndexes[rDef.defName];
            return;
        }
    }
}
