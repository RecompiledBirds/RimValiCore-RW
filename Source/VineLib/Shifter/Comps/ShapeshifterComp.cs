using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using Verse;

namespace RVCRestructured.Shifter
{
    public class ShapeshifterComp : ThingComp
    {
        private XenotypeDef baseXenoTypeDef;
        private ThingDef currentForm;
        private BodyTypeDef mimickedBody;
        private HeadTypeDef mimickedHead;

        public XenotypeDef BaseXenoType
        {
            get
            {

                return baseXenoTypeDef;
            }
        }

     
        private List<ThingComp> comps = new List<ThingComp>();

        public List<ThingComp> Comps
        {
            get
            {
                return comps;
            }
        }

        public BodyTypeDef MimickedBodyType
        {
            get
            {
                return mimickedBody;
            }
        }

        public HeadTypeDef MimickedHead
        {
            get { return mimickedHead; }
        }

        public virtual ThingDef CurrentForm
        {
            get
            {
                if(currentForm == null)
                {
                    currentForm = parent.def;
                }
                return currentForm;
            }
        }

        public virtual ModContentPack ContentPack
        {
            get
            {
                return CurrentForm.modContentPack;
            }
        }

        public virtual bool IsParentDef()
        {
            return IsDef(parent.def);
        }
        public virtual bool IsDef(ThingDef def)
        {
            return def == CurrentForm;
        }

        public virtual string label()
        {
            return CurrentForm.label;
        }


       

        public virtual T GetCompProperties<T>() where T : CompProperties
        {
            return CurrentForm.GetCompProperties<T>();
        }

     

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (baseXenoTypeDef == null) return;
            Pawn pawn = parent as Pawn;
            baseXenoTypeDef = pawn.genes.Xenotype;           
        }

        public void SetForm(Pawn pawn)
        {
            mimickedBody = pawn.story.bodyType;
            mimickedHead = pawn.story.headType;
            Pawn parentPawn = parent as Pawn;
            if (baseXenoTypeDef == null)
            {
                baseXenoTypeDef = parentPawn.genes.Xenotype;

            }
            RevertGenes();
            SetForm(pawn.def);
            parentPawn.genes.SetXenotype(pawn.genes.Xenotype);
            SetGenes(pawn.genes.Xenotype,baseXenoTypeDef);
        }

        public void SetForm(ThingDef def)
        {
            currentForm = def;
            Pawn pawn = parent as Pawn;
            Log.Message($"{pawn.Name.ToStringShort} became {currentForm}");
            
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            if (comp == null) return;
            RVRGraphicsComp targetGraphics=def.GetCompProperties<RVRGraphicsComp>();
            LoadCompsFromForm();
            comp.RenderableDefs.Clear();
            if (targetGraphics != null) {
              
                comp.GenAllDefs(targetGraphics,pawn);
                comp.GenColors(targetGraphics,pawn);
            }
            comp.InformGraphicsDirty();
            
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();


        }

        public virtual bool FormUnstable()
        {
            Pawn p = parent as Pawn;
            return HealthUtility.IsMissingSightBodyPart(p) || HealthUtility.TicksUntilDeathDueToBloodLoss(p) < 60000 || p.Downed || p.Dead;
        }

        public void RevertGenes()
        {
            Pawn parentPawn = parent as Pawn;
            XenotypeDef def = parentPawn.genes.Xenotype;
            parentPawn.genes.SetXenotype(baseXenoTypeDef);
            SetGenes(baseXenoTypeDef, def);
        }

        public void RevertForm()
        {
            mimickedBody = null;
            mimickedHead = null;
            RevertGenes();
            SetForm(parent.def);
           
        }

        public void SetGenes(XenotypeDef xenotype, XenotypeDef from)
        {
            Pawn parentPawn = parent as Pawn;
            foreach (GeneDef def in xenotype.AllGenes)
            {
                parentPawn.genes.AddGene(def, !xenotype.inheritable);
            }
            foreach(GeneDef def in from.AllGenes)
            {
                if (!parentPawn.genes.HasGene(def)) continue;
                Log.Message(def.defName);
                parentPawn.genes.RemoveGene(parentPawn.genes.GetGene(def));
            }
        }

        public virtual IEnumerable<BodyPartRecord> GetBodyPartRecords(HediffSet hediffSet, BodyPartHeight height, BodyPartDepth depth, BodyPartTagDef tag, BodyPartRecord partParent)
        {
            List<BodyPartRecord> body = CurrentForm.race.body.AllParts;
            foreach (BodyPartRecord entry in body)
            {
                if (hediffSet.PartIsMissing(entry)) continue;
                if ((height == BodyPartHeight.Undefined || entry.height == height) && (depth == BodyPartDepth.Undefined || entry.depth == depth) && (tag == null || entry.def.tags.Contains(tag)) && (partParent == null || entry.parent == partParent))
                {
                    yield return entry;
                }
            }
        }

        public void LoadCompsFromForm()
        {
            if (IsParentDef()) return;
            if(IsDef(CurrentForm)) return;
            ClearComps();
            LoadComps(CurrentForm);
            AddCompsToParent();
        }


        private Dictionary<string, bool> addedComp = new Dictionary<string, bool>();

        private void AddCompsToParent()
        {
            foreach(ThingComp comp in comps)
            {

                bool contained = parent.AllComps.Contains(comp);
                if(!contained)
                    parent.AllComps.Add(comp);
                addedComp.Add(comp.GetType().FullName, contained);
            }
        }
        
        private void ClearComps()
        {
            foreach (ThingComp comp in comps)
            {
                string name = comp.GetType().FullName;
                bool hasKey=addedComp.ContainsKey(name);
                bool added =hasKey && addedComp[name];
                if (added)
                {
                    parent.AllComps.Remove(comp);
                }
                if (hasKey)
                {
                    addedComp.Remove(name);
                }
            }
            comps.Clear();
        }

        public void LoadComps(ThingDef def)
        {
            foreach (CompProperties properties in def.comps)
            {
                ThingComp thingComp = null;
                try
                {
                    thingComp = (ThingComp)Activator.CreateInstance(properties.compClass);
                    thingComp.parent = this.parent;
                    comps.Add(thingComp);
                    thingComp.Initialize(properties);
                }
                catch (Exception arg)
                {
                    Log.Error("Could not instantiate or initialize a ThingComp: " + arg);
                    comps.Remove(thingComp);
                }
            }
        }

        public override void PostExposeData()
        {

            Scribe_Defs.Look(ref mimickedHead, nameof(mimickedHead));
            Scribe_Defs.Look(ref mimickedBody, nameof(mimickedBody));
            Scribe_Defs.Look(ref currentForm, nameof(currentForm));
            Scribe_Defs.Look(ref baseXenoTypeDef,nameof(baseXenoTypeDef));
            base.PostExposeData();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                LoadCompsFromForm();
            }
            if (!comps.NullOrEmpty())
            {
                foreach(ThingComp comp in comps)
                {
                    comp.PostExposeData();
                }
            }
        }

        public virtual float OffsetStat(StatDef stat)
        {
            float result = 0;
            Pawn pawn = parent as Pawn;
            if (IsParentDef()) return result;
            result -= pawn.def.statBases.GetStatOffsetFromList(stat);
            result += CurrentForm.statBases.GetStatOffsetFromList(stat);
            return result;
        }
    }
}
