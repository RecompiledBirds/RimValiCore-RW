using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Comps
{
    public class HealableGameComp : WorldComponent
    {
        private List<Thing> things = new List<Thing>();
        private Dictionary<Thing, int> ticks = new Dictionary<Thing, int>();
        private Dictionary<Thing, HealableMaterialCompProperties> healables = new Dictionary<Thing, HealableMaterialCompProperties>();

        public HealableGameComp(World world) : base(world)
        {
        }

        public void RegisterThing(Thing thing, HealableMaterialCompProperties props)
        {
            healables.Add(thing, props);
            ticks.Add(thing,props.ticksBetweenHeal);
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

                int wantedHp = thing.HitPoints + props.amountHealed;
                thing.HitPoints = wantedHp > thing.MaxHitPoints ? thing.MaxHitPoints : wantedHp;
                ticks[thing] = props.ticksBetweenHeal;
            }
        }
    }
}
