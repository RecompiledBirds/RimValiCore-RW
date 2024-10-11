using RVCRestructured.Defs;
using RVCRestructured.Shifter;
using UnityEngine;
using Verse;

namespace RVCRestructured;

public class RVRCP : CompProperties
{
    public RVRCP()
    {
        compClass = typeof(RVRComp);
    }
}
public class RVRComp : ThingComp
{
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {

        base.PostSpawnSetup(respawningAfterLoad);
    }

    private List<IRenderable> defList = [];
    private List<Renderable> defListRenderable = [];
    private List<RenderableDef> defListRenderableDefs = [];
    private bool generated = false;
    public List<IRenderable> RenderableDefs
    {
        get
        {
            return defList;
        }
        set
        {
            defList = value;
        }
    }

    private Dictionary<string, TriColorSet> sets = [];

    public Dictionary<string, TriColorSet> Colors
    {
        get
        {
            return sets;
        }
    }



    private Dictionary<string, int> masks = [];
    private Dictionary<string, int> renderableIndexes = [];
    //used in loading
    private List<string> lKeys = [];
    private List<TriColorSet> lSets = [];
    private List<int> lInts = [];

    public string GetTexPath(RenderableDef def)
    {
        return def.textures[renderableIndexes[def.defName]].texPath;
    }

    public void SendRenderableDefToNextTexture(RenderableDef def)
    {
        if (!renderableIndexes.ContainsKey(def.defName)) return;
        int index = renderableIndexes[def.defName];
        index++;
        if (def.textures.Count == index)
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
            index = def.textures.Count - 1;
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
        List<string> maskList = def.textures[texIndex].GetMasks(parent as Pawn);
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
        List<string> maskList = def.textures[texIndex].GetMasks(parent as Pawn);
        if (maskIndex == -1)
        {
            maskIndex = maskList.Count - 1;
        }
        masks[def.defName] = maskIndex;
    }
    public string GetMaskPath(RenderableDef def, Pawn pawn)
    {
        if (masks.ContainsKey(def.defName) && !def.textures[renderableIndexes[def.defName]].GetMasks(pawn).NullOrEmpty())
            return def.textures[renderableIndexes[def.defName]].GetMasks(pawn)[masks[def.defName]];
        return def.textures[renderableIndexes[def.defName]].texPath;
    }

    public TriColorSet this[string name]
    {
        get
        {
            if (sets.ContainsKey(name))
            {
                return sets[name];
            }
            Pawn pawn = parent as Pawn;
            RVCLog.Log($"ColorSet {name} is not on {pawn.Name.ToStringShort}!", RVCLogType.Error);
            return null;
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
                }
                else
                {
                    defListRenderable.Append(renderable);
                }
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

    public void GenGraphics()
    {
        if (generated) return;
        generated = true;
        Pawn pawn = parent as Pawn;

        GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
        if (comp == null)
            return;
        RVRGraphicsComp props = comp.Props;

        ShapeshifterComp shapeshifterComp = pawn.TryGetComp<ShapeshifterComp>();
        if (shapeshifterComp != null)
        {
            RVRGraphicsComp shifterGraphics = shapeshifterComp.GetCompProperties<RVRGraphicsComp>();
            if (!shapeshifterComp.IsParentDef())
            {

                if (shifterGraphics != null)
                    props = shifterGraphics;
                else
                    return;
            }
        }
        GenFromComp(props, pawn);
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
        if (renderableIndexes.ContainsKey(rDef.defName))
        {
            return;
        }
        bool hasLink = rDef.linkTexWith != null;
        if (hasLink && renderableIndexes.ContainsKey(rDef.linkTexWith.defName) && !renderableIndexes.ContainsKey(rDef.defName))
        {
            string linkString = rDef.linkTexWith.defName;
            renderableIndexes[rDef.defName] = renderableIndexes[linkString];
            masks[rDef.defName] = masks[linkString];
            return;
        }
        if (renderableIndexes.ContainsKey(rDef.defName)) return;
        BaseTex tex = rDef.textures.RandomElement();
        int index = rDef.textures.IndexOf(tex);
        renderableIndexes[rDef.defName] = index;
        index = tex.GetMasks(pawn).IndexOf(tex.GetMasks(pawn).RandomElement());
        masks.Add(rDef.defName, index);

        if (hasLink)
        {
            string linkString = rDef.linkTexWith.defName;
            renderableIndexes[linkString] = renderableIndexes[rDef.defName];
            masks[linkString] = renderableIndexes[rDef.defName];
            return;
        }
    }
}
