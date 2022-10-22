using RimValiCore_RW.Source;
using RVCRestructured.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Verse;
using Random = System.Random;

namespace RVCRestructured.RVR
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
        private Dictionary<string, TriColorSet> sets = new Dictionary<string, TriColorSet>();
        private Dictionary<string, int> masks = new Dictionary<string, int>();
        private Dictionary<string, int> renderableIndexes = new Dictionary<string, int>();
        //used in loading
        private List<string> lKeys = new List<string>();
        private List<TriColorSet> lSets = new List<TriColorSet>();
        private List<int> lInts = new List<int>();

        public TriColorSet this[string name]{
            get
            {
                if (!sets.ContainsKey(name))
                {
                    RVCLog.Log($"ColorSet {name} is not on {parent.def.defName}!", RVCLogType.Error);
                    return null;
                }
                return sets[name];
            }
            set
            {
                sets[name] = value;
            }
        }


        private RaceDef raceDef
        {
            get
            {
                return this.parent.def as RaceDef;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Collections.Look(ref sets, "sets", LookMode.Value, LookMode.Deep, ref lKeys, ref lSets);
            Scribe_Collections.Look(ref masks, "masks", LookMode.Value, LookMode.Value, ref lKeys, ref lInts);
            Scribe_Collections.Look(ref renderableIndexes, "renderableIndexes", LookMode.Value, LookMode.Value, ref lKeys, ref lInts);
        }

        public void GenGraphics()
        {
            Pawn pawn = this.parent as Pawn;
            foreach(RenderableDef rDef in raceDef.RaceGraphics.renderableDefs)
            {
                if (renderableIndexes.ContainsKey(rDef.defName))
                {
                    return;
                }
                if (rDef.linkWith != null)
                {
                    if (renderableIndexes.ContainsKey(rDef.defName))
                    {
                        renderableIndexes.Add(rDef.defName, renderableIndexes[rDef.linkWith.defName]);
                        masks.Add(rDef.defName, masks[rDef.linkWith.defName]);
                        continue;
                    }

                    Random rand = new System.Random();
                    int index = rand.Next(rDef.textures.Count);

                    renderableIndexes.Add(rDef.defName, index);
                    renderableIndexes.Add(rDef.linkWith.defName,index);
                    int maskIndex = rDef.textures[renderableIndexes[rDef.defName]].GetMasks(pawn).Count;
                    index=rand.Next(maskIndex);
                    masks.Add(rDef.defName, index);
                    masks.Add(rDef.linkWith.defName, index);
                }
            }
            foreach(RaceColors colors in raceDef.RaceGraphics.colorGenerators)
            {
                Color c1 = colors.GeneratorToUse(pawn).colorOne.NewRandomizedColor();
                Color c2 = colors.GeneratorToUse(pawn).colorTwo.NewRandomizedColor();
                Color c3 = colors.GeneratorToUse(pawn).colorThree.NewRandomizedColor();
                sets.Add(colors.name, new TriColorSet(c1, c2, c3, true));
            }
        }
    }
}
