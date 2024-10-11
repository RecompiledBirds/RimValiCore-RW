using RimWorld;
using Verse;

namespace RVCRestructured;

public class RimworldDate : IExposable
{
    public long date = 0;
    public int day;
    public Quadrum quadrum = Quadrum.Undefined;
    public int ticks;

    /// <summary>
    /// FOR USE WITH LOADING. DO NOT USE ELSEWHERE.
    /// </summary>
    public RimworldDate() { }

    public RimworldDate(Map map)
    {
        GetCurrentDate(map);
    }

    public virtual void ExposeData()
    {
        Scribe_Values.Look(ref ticks, "ticks", ticks, true);
        Scribe_Values.Look(ref day, "Day", day, true);
        Scribe_Values.Look(ref quadrum, "Quadrum", quadrum, true);
    }

    public void GetCurrentDate(Map map)
    {
        day = GenDate.DayOfYear(Find.TickManager.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile).x);
        quadrum = GenDate.Quadrum(Find.TickManager.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile).x);
        ticks = GenTicks.TicksGame;
    }


    public override string ToString()
    {
        return day + quadrum.ToString();
    }

    public string GetFormattedDate
    {
        get
        {
            return $"{day}/{quadrum}";
        }
    }
}

public class RimworldDeathDate : RimworldDate
{
    public Pawn deadPawn;

    public RimworldDeathDate(Pawn pawn)
    {
        if (pawn != null)
        {
            deadPawn = pawn;
            GetCurrentDate(pawn.Map);
        }
    }
    /// <summary>
    /// FOR USE WITH LOADING. DO NOT USE ELSEWHERE.
    /// </summary>
    public RimworldDeathDate() { }
    public override void ExposeData()
    {
        Scribe_References.Look(ref deadPawn, "dPawn");
        base.ExposeData();
    }
}
