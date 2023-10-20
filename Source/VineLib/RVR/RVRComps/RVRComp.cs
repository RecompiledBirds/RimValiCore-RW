using RVCRestructured.Defs;
using System.Collections.Generic;
using System.Linq;
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
        private List<IRenderable> defList = new List<IRenderable>();
        private List<Renderable> defListRenderable = new List<Renderable>();
        private List<RenderableDef> defListRenderableDefs = new List<RenderableDef>();

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

        public void GenGraphics()
        {
            Pawn pawn = this.parent as Pawn;

            GraphicsComp comp = pawn.TryGetComp<GraphicsComp>();
            if (comp == null)
                return;

            if (defList.NullOrEmpty() && comp.Props.renderableDefs.NullOrEmpty())
            {
                defList = new List<IRenderable>();
                foreach(RenderableDef def in comp.Props.renderableDefs)
                {
                    defList.Add(def);
                }
            }
            foreach (RenderableDef rDef in comp.Props.renderableDefs)
            {
                if (renderableIndexes.ContainsKey(rDef.defName))
                {
                    continue;
                }
                Random rand = new Random();
                int index = rand.Next(rDef.textures.Count);

                renderableIndexes[rDef.defName] = index;
                int maskIndex = rDef.textures[renderableIndexes[rDef.defName]].GetMasks(pawn).Count;
                index = rand.Next(maskIndex);
                masks.Add(rDef.defName, index);
                if (rDef.linkTexWith != null)
                {
                    if (renderableIndexes.ContainsKey(rDef.defName))
                    {
                        renderableIndexes.Add(rDef.defName, renderableIndexes[rDef.linkTexWith.defName]);
                        masks.Add(rDef.defName, masks[rDef.linkTexWith.defName]);
                        continue;
                    }



                    renderableIndexes.Add(rDef.linkTexWith.defName, index);

                    masks.Add(rDef.linkTexWith.defName, index);
                }
            }
            foreach (RaceColors colors in comp.Props.colorGenerators)
            {
                if (sets.ContainsKey(colors.name))
                    continue;

                Color c1 = colors.GeneratorToUse(pawn).colorOne.NewRandomizedColor();
                Color c2 = colors.GeneratorToUse(pawn).colorTwo.NewRandomizedColor();
                Color c3 = colors.GeneratorToUse(pawn).colorThree.NewRandomizedColor();
                sets.Add(colors.name, new TriColorSet(c1, c2, c3, true));
            }
        }
    }
}
