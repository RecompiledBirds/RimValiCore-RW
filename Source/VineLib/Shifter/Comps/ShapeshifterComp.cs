using RimWorld;
using RVCRestructured.Defs;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter
{
    public class ShapeshifterComp : ThingComp
    {
        private XenotypeDef baseXenoTypeDef;
        private ThingDef currentForm;
        public ThingDef CurrentForm
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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (baseXenoTypeDef == null) return;
            Pawn pawn = parent as Pawn;
            baseXenoTypeDef = pawn.genes.Xenotype;
           
        }

        public void SetForm(Pawn pawn)
        {
            Pawn parentPawn = parent as Pawn;
            if (baseXenoTypeDef == null)
            {
                baseXenoTypeDef = parentPawn.genes.Xenotype;

            }
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
            if (targetGraphics != null) {
                comp.RenderableDefs.Clear();
                comp.GenAllDefs(targetGraphics,pawn);
                comp.GenColors(targetGraphics,pawn);
                return;
            }
            
        }

        public void RevertForm()
        {
            SetForm(parent.def);
            Pawn parentPawn = parent as Pawn;
            XenotypeDef def = parentPawn.genes.Xenotype;
            parentPawn.genes.SetXenotype(baseXenoTypeDef);
            SetGenes(baseXenoTypeDef, def);
        }

        public void SetGenes(XenotypeDef xenotype, XenotypeDef from)
        {
            Pawn parentPawn = parent as Pawn;
            Log.Message(xenotype.defName);
            Log.Message(from.defName);
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

        public override void PostExposeData()
        {
            Scribe_Defs.Look(ref currentForm, nameof(currentForm));
            Scribe_Defs.Look(ref baseXenoTypeDef,nameof(baseXenoTypeDef));
            base.PostExposeData();
        }

        public virtual float OffsetStat(StatDef stat)
        {
            float result = 0;
            Pawn pawn = parent as Pawn;
            if (CurrentForm == pawn.def) return result;
            result -= pawn.def.statBases.GetStatOffsetFromList(stat);
            result += CurrentForm.statBases.GetStatOffsetFromList(stat);
            return result;
        }
    }
}
