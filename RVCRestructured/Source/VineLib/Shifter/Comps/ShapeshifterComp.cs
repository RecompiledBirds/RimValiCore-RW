using RimWorld;
using System.Reflection;
using Verse;

namespace RVCRestructured.Shifter;

public class ShapeshifterComp : ThingComp
{
    private static readonly Dictionary<Type, bool> shouldNotcopy = [];

    /// <summary>
    /// Can we copy this property?
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static bool CanCopyComp(CompProperties properties)
    {
        Type type = properties.GetType();
        return shouldNotcopy.ContainsKey(type)&&shouldNotcopy[type];
    }

    /// <summary>
    /// Informs Vine any CompProperties that match the same type as this should not be copied.
    /// </summary>
    /// <param name="properties"></param>
    public static void FlagShouldNotCopyComp(CompProperties properties)
    {
        Type propType=properties.GetType();
        if (shouldNotcopy.ContainsKey(propType))return;
        shouldNotcopy.Add(propType, true);
    }


    private XenotypeDef? baseXenoTypeDef = null;
    private ThingDef? currentForm = null;
    private BodyTypeDef? mimickedBody = null;
    private HeadTypeDef? mimickedHead = null;

    public XenotypeDef BaseXenoTypeDef => baseXenoTypeDef ?? throw new NullReferenceException();

    private RaceProperties raceProperties = null!;
    public virtual RaceProperties RaceProperties
    {
        get
        {
            if (raceProperties == null)
            {
                raceProperties = CurrentForm.race;
                FieldInfo fieldInfo = typeof(RaceProperties).GetField("intelligence", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                fieldInfo.SetValue(raceProperties, Intelligence.Humanlike);
            }

            return raceProperties;
        }
    }
    private readonly List<ThingComp> comps = [];

    public List<ThingComp> Comps => comps;

    public virtual BodyTypeDef MimickedBodyType
    {
        get
        {
            if(mimickedBody == null)
            {
                Pawn pawn = (Pawn)parent;
                mimickedBody = pawn.story.bodyType;
            }
            return mimickedBody;
        }
    }

    public virtual HeadTypeDef MimickedHead
    {

        get {
            if (mimickedHead == null)
            {
                Pawn pawn = (Pawn)parent;
                mimickedHead = pawn.story.headType;
            }
            return mimickedHead; }
    }

    public virtual ThingDef CurrentForm => currentForm ??= parent.def;

    public virtual ModContentPack ContentPack => CurrentForm.modContentPack;

    /// <summary>
    /// Is the parent the same def as our parent?
    /// </summary>
    /// <returns></returns>
    public virtual bool IsParentDef() => IsDef(parent.def);

    /// <summary>
    /// Is the parent.def the given def?
    /// </summary>
    /// <param name="def"></param>
    /// <returns></returns>
    public virtual bool IsDef(ThingDef def) => def == CurrentForm;

    /// <summary>
    /// Returns the label of the currently shifted race.
    /// </summary>
    /// <returns></returns>
    public virtual string Label() => CurrentForm.label;



    /// <summary>
    /// Pull comp properties directly from the CurrentDef. This is useful in a lot of Vine's internal functions.
    /// </summary>
    /// <typeparam name="T">Type of CompProperties to retrieve</typeparam>
    /// <returns></returns>
    public virtual T GetCompProperties<T>() where T : CompProperties => CurrentForm.GetCompProperties<T>();



    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (baseXenoTypeDef == null) return;
        Pawn pawn = (Pawn)parent;
        if(ModLister.BiotechInstalled)
            baseXenoTypeDef = pawn.genes.Xenotype;           
    }

    /// <summary>
    /// Sets the current def and xenotype to that of the given pawn
    /// </summary>
    /// <param name="pawn"></param>
    public virtual void SetForm(Pawn pawn) => SetForm(pawn, null);

    /// <summary>
    /// Sets the form to the given pawns def, bodytype, headtype, and either the given xenotype, pawn's xenotype, or baseliner.
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="xenotypeDef"></param>
    public virtual void SetForm(Pawn pawn, XenotypeDef? xenotypeDef = null)
    {
        xenotypeDef ??= (pawn.genes?.Xenogenes != null ? pawn.genes.Xenotype : XenotypeDefOf.Baseliner);

        if (pawn.story != null)
        {
            mimickedBody = pawn.story.bodyType;
            mimickedHead = pawn.story.headType;
        }

        Pawn parentPawn = GetParentPawnAndSetBaseXenoType();

        RevertGenes();
        SetForm(pawn.def);

        if (!ModLister.BiotechInstalled) return;

        XenotypeDef def = xenotypeDef;
        parentPawn.genes.SetXenotype(def);
        SetGenes(def, BaseXenoTypeDef);
    }

    public virtual void SetForm(ThingDef def, XenotypeDef xenotypeDef, BodyTypeDef bodyTypeDef)
    {
        mimickedBody = bodyTypeDef;
        SetForm(def, xenotypeDef);
    }

    public virtual void SetForm(ThingDef def, XenotypeDef xenotypeDef)
    {
        Pawn parentPawn = GetParentPawnAndSetBaseXenoType();

        RevertGenes();
        SetForm(def);

        if (!ModLister.BiotechInstalled) return;

        parentPawn.genes.SetXenotype(xenotypeDef);
        SetGenes(xenotypeDef, BaseXenoTypeDef);
    }

    private Pawn GetParentPawnAndSetBaseXenoType()
    {
        Pawn parentPawn = (Pawn)parent;
        baseXenoTypeDef ??= parentPawn.genes.Xenotype;
        return parentPawn;
    }

    public virtual void SetForm(ThingDef def, BodyTypeDef? bodyTypeDef = null, bool log = true, bool generating = false)
    {
        currentForm = def;

        Pawn pawn = (Pawn)parent;
        
        if(log)
            VineLog.MSG($"{pawn.Name.ToStringShort} became {currentForm}",debugOnly:true);
        if(bodyTypeDef!=null)mimickedBody=bodyTypeDef;
        RVRComp comp = pawn.TryGetComp<RVRComp>();
        if (comp == null) return;
        RVRGraphicsComp targetGraphics = def.GetCompProperties<RVRGraphicsComp>();
        LoadCompsFromForm();
        comp.ClearAllGraphics();
        if (targetGraphics != null)
        {

            comp.GenAllDefs(targetGraphics, pawn);
            comp.GenColors(targetGraphics, pawn);
        }
        
        comp.InformGraphicsDirty();
      /*  if(!generating)
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();*/
    }

    public virtual bool FormUnstable()
    {
        Pawn pawn = (Pawn)parent;
        return HealthUtility.IsMissingSightBodyPart(pawn) || HealthUtility.TicksUntilDeathDueToBloodLoss(pawn) < 60000 || pawn.Downed || pawn.Dead;
    }

    public void RevertGenes()
    {
        if (!ModLister.BiotechInstalled) return;
        
        Pawn parentPawn = (Pawn)parent;
        XenotypeDef def = parentPawn.genes.Xenotype;
        baseXenoTypeDef ??= XenotypeDefOf.Baseliner;
        parentPawn.genes.SetXenotype(baseXenoTypeDef);
        SetGenes(baseXenoTypeDef, def);
    }

    public void RevertForm()
    {
        mimickedBody = null;
        mimickedHead = null;
        SetForm(parent.def);
        RevertGenes();
       
    }

    public void SetGenes(XenotypeDef xenotype, XenotypeDef from)
    {
        Pawn parentPawn = (Pawn)parent;
        foreach (GeneDef def in xenotype.AllGenes)
        {
            parentPawn.genes.AddGene(def, !xenotype.inheritable);
        }

        foreach(GeneDef def in from.AllGenes)
        {
            if (!parentPawn.genes.HasActiveGene(def)) continue;
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

    /// <summary>
    /// Loads comps from the current form
    /// </summary>
    public void LoadCompsFromForm()
    {
        if (IsParentDef()) return;
        if(IsDef(CurrentForm)) return;
        ClearComps();
        LoadComps(CurrentForm);
        AddCompsToParent();
    }


    private readonly Dictionary<string, bool> addedComp = [];

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

    private void LoadComps(ThingDef def)
    {
        foreach (CompProperties properties in def.comps)
        {
            if (!CanCopyComp(properties)) continue;
            
            ThingComp? thingComp = null;
            try
            {
                thingComp = (ThingComp)Activator.CreateInstance(properties.compClass);
                thingComp.parent = parent;
                comps.Add(thingComp);
                thingComp.Initialize(properties);
            }
            catch (Exception arg)
            {
                Log.Error("Could not instantiate or initialize a ThingComp: " + arg);
                
                if (thingComp != null) comps.Remove(thingComp);
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

    /// <summary>
    /// Allows a shapesifter to override the stat and change it to something else. This is connected to the GetValueUnfinalized patch.
    /// </summary>
    /// <param name="stat"></param>
    /// <returns></returns>
    public virtual float OffsetStat(StatDef stat)
    {
        float result = 0;
        Pawn pawn = (Pawn)parent;
        if (IsParentDef()) return result;
        
        result -= pawn.def.statBases.GetStatOffsetFromList(stat);
        result += CurrentForm.statBases.GetStatOffsetFromList(stat);
        return result;
    }
}
