using RVCRestructured.Defs;
using RVCRestructured.Shifter;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace RVCRestructured
{
    public class RVRCP : CompProperties
    {
        public RVRCP()
        {
            this.compClass = typeof(RVRComp);
        }
    }
    public class RVRComp : ThingComp
    {

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        private List<IRenderable> defList = new List<IRenderable>();
        private List<Renderable> defListRenderable = new List<Renderable>();
        private List<RenderableDef> defListRenderableDefs = new List<RenderableDef>();
        private bool generated= false;
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

        private Dictionary<string, TriColorSet> sets = new Dictionary<string, TriColorSet>();
        
        public Dictionary<string,TriColorSet> Colors
        {
            get
            {
                return sets;
            }
        }

        private Dictionary<string, int> masks = new Dictionary<string, int>();
        private Dictionary<string, int> renderableIndexes = new Dictionary<string, int>();
        //used in loading
        private List<string> lKeys = new List<string>();
        private List<TriColorSet> lSets = new List<TriColorSet>();
        private List<int> lInts = new List<int>();

        public string GetTexPath(RenderableDef def)
        {
            if (renderableIndexes.ContainsKey(def.defName))
                return def.textures[renderableIndexes[def.defName]].texPath;
            return null;
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
                if (defList.NullOrEmpty()) defList = new List<IRenderable>();
                if (!defListRenderableDefs.NullOrEmpty()) defList.AddRange(defListRenderableDefs);
                if (!defListRenderable.NullOrEmpty()) defList.AddRange(defListRenderable);
            }
        }

        public void ClearAllGraphics()
        {
            generated=false;
            masks = new Dictionary<string, int>();
            renderableIndexes= new Dictionary<string, int>();
            sets = new Dictionary<string, TriColorSet>();
            defList = new List<IRenderable>();
            defListRenderable = new List<Renderable>();
            defListRenderableDefs = new List<RenderableDef>();
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

                defList = new List<IRenderable>();
                foreach (RenderableDef def in comp.renderableDefs)
                {
                    defList.Add(def);
                }
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
            graphicsDirty=true;
        }

        private void GenerateRenderableDef(RenderableDef rDef, Pawn pawn)
        {
            if (renderableIndexes.ContainsKey(rDef.defName))
            {
                return;
            }
            
            bool hasLink = rDef.linkTexWith != null;
            if (hasLink && renderableIndexes.ContainsKey(rDef.linkTexWith.defName)) {
                string linkString = rDef.linkTexWith.defName;
                renderableIndexes[rDef.defName] = renderableIndexes[linkString];
                masks[rDef.defName] = renderableIndexes[linkString];
                return;
            }

            Random rand = new Random();
            int index = rand.Next(rDef.textures.Count);
            renderableIndexes[rDef.defName] = index;
            int maskIndex = rDef.textures[renderableIndexes[rDef.defName]].GetMasks(pawn).Count;
            index = rand.Next(maskIndex);
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
}
