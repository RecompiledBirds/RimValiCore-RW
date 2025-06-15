using RimWorld.Planet;
using Verse;

namespace RVCRestructured.Comps;

public class HealableGameComp : WorldComponent
{
    private readonly List<Thing> things = [];
    private readonly Dictionary<Thing, int> ticks = [];
    private readonly Dictionary<Thing, HealableMaterialCompProperties> healables = [];

    public HealableGameComp(World world) : base(world)
    {
    }

    public void RegisterThing(Thing thing, HealableMaterialCompProperties props)
    {
        healables.Add(thing, props);
        ticks.Add(thing,props.TicksBetweenHeal);
        things.Add(thing);
    }

    public void UnregisterThing(Thing thing)
    {
        healables.Remove(thing);
        ticks.Remove(thing);
        things.Remove(thing);
    }

    public override void WorldComponentTick()
    {
        int tickCount = ticks.Count;
        for(int i=0; i<tickCount; i++)
        {
            Thing thing = things[i];
            
            if (thing == null || !thing.Spawned)
                continue;

            if (ticks[thing]!=0)
            {
                ticks[thing]--;
                continue;
            }

            HealableMaterialCompProperties props = healables[thing];

            int wantedHp = thing.HitPoints + props.AmountHealed;
            thing.HitPoints = wantedHp > thing.MaxHitPoints ? thing.MaxHitPoints : wantedHp;
            ticks[thing] = props.TicksBetweenHeal;
        }
    }
}
